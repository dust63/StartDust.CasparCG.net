# CasparCG REST Client Lifecycle Design

## Status
Draft

## Context

The REST addon currently exposes a long-lived CasparCG client per configured server name and resolves it from the HTTP route context. The main open question is lifecycle management:

- should the REST addon require `ConnectAsync()` at application startup?
- should the first REST request be allowed to establish the AMCP connection lazily?
- how should server health be reported and what should happen if the client has not connected yet?
- how should the application disconnect from CasparCG when the host stops?

The desired behavior is:

- long-lived clients, one per configured server
- lazy connection by default
- optional warm-up at startup
- a per-server health route that can initiate the connection if needed
- clean disconnection at application shutdown

This design must remain compatible with SSE event streaming and OSC handling.

## Goals

- Keep each `CasparClient` alive for the full application lifetime.
- Allow the first AMCP-backed call to establish the connection automatically.
- Expose a per-server health route that can initialize the connection when it is still disconnected.
- Keep the REST addon usable with multiple named servers and the `default` server.
- Disconnect clients cleanly when the host shuts down.
- Avoid tying the client lifecycle to `HttpContext` or request scope.

## Non-Goals

- No per-request client creation.
- No connection open/close cycle for every REST request.
- No change to OSC transport semantics.
- No redesign of the REST routing surface beyond the health and lifecycle behavior.
- No persistence layer for server registrations.

## Proposed Design

### Client lifetime

Each configured CasparCG server remains a long-lived `CasparClient` registered in DI. The REST addon resolver only selects the appropriate client from route values; it does not own the socket.

The `CasparClient` is responsible for ensuring its own AMCP connection before it sends a command. If a client is still disconnected, the first AMCP request performs a lazy connect.

This means:

- REST endpoints can be called without a pre-flight startup connection.
- `ConnectAsync()` becomes an explicit warm-up operation rather than a hard requirement.
- the same model works for the default server and any named server exposed under `/servers/{name}`.

### Startup warm-up

Warm-up remains optional. The application may still connect selected clients at startup when it wants fail-fast behavior or shorter first-request latency.

The REST addon should expose configuration that allows startup warm-up to be enabled without making it mandatory.

### Health route

Add a per-server health route:

- `GET /health`
- `GET /servers/{name}/health`

The route must:

- resolve the target client using the same resolver path as the other REST routes
- return the current connection state
- initiate `ConnectAsync()` if the client has not connected yet
- report failure if the connection attempt fails

The health route is intentionally allowed to have a side effect. The goal is to make it both a status endpoint and a practical entry point for first-use initialization.

### Shutdown and disconnection

When the host shuts down, every configured client should be disconnected cleanly. This applies to both the default client and named clients.

The shutdown hook must:

- disconnect AMCP transports
- stop OSC transports if present
- avoid leaving background sockets alive after the application stops

This is especially important because the client is long-lived and because SSE/OSC can hold background resources beyond a single HTTP request.

### SSE and OSC compatibility

This lifecycle model is compatible with both SSE and OSC:

- SSE streams depend on a long-lived client and should keep reading from the same client instance.
- OSC listeners are not request-scoped and should remain independent from `HttpContext`.
- lazy AMCP connect does not break either transport because both are already tied to application lifetime, not request lifetime.

## Configuration Shape

The REST addon should expose explicit lifecycle-related options in `CasparRestApiOptions`.

Planned behavior:

- `RoutePrefix` still prefixes every REST route, including health and Swagger.
- `EnableOpenApi` still controls the Swagger JSON endpoint.
- `EnableSse` still controls the SSE route.
- a new option should control optional startup warm-up for registered clients.
- a new option should control whether lifecycle health routes are mapped.

Recommended option names:

- `WarmUpClientsOnStartup`
- `MapHealthEndpoints`

The exact property names can still be adjusted during implementation if there is a better fit with the existing options style.

## Response Shape

The health route should return a small payload that is easy to consume in checks and dashboards.

Recommended fields:

- `serverName`
- `status`
- `connected`
- `lastSuccessfulAmcpInteraction`
- `lastFailure`, when available

The status should align with the existing `ConnectionHealthStatus` model already used by the client.

If the lazy connection attempt fails, the health endpoint should return a failure response with problem details and an HTTP `503` status.

## Error Handling

- If a named server cannot be resolved, return a `404` style problem response.
- If the health route triggers a connection attempt and AMCP is unreachable, return `503 Service Unavailable`.
- If a regular REST command triggers a lazy connection and the connection fails, return the existing problem response path used by the addon today.
- If shutdown disconnection fails, swallow secondary failures only if needed to let the host stop cleanly, but keep the error observable through logs or diagnostics.

## Testing Strategy

Add tests that cover:

- lazy connection on the first AMCP command
- health route triggering connection on first use
- health route reporting an already connected client without forcing a reconnect
- startup warm-up behavior when enabled
- clean disconnection at shutdown
- named server health routing under `/servers/{name}/health`
- SSE and OSC remaining functional with long-lived clients

Preferred test layers:

- unit tests for lifecycle helpers and route mapping
- integration tests for the REST health route and lazy connect behavior
- integration tests for shutdown cleanup when practical

## Rollout Notes

- The change is breaking in behavior but not necessarily in route shape.
- Existing sample apps and docs should stop implying that `ConnectAsync()` is mandatory before `MapCasparCGApi()`.
- The REST addon documentation should clearly state that startup connection is optional and that the health route can initialize the client.

