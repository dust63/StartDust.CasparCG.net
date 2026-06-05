# CasparCG REST Proxy Addon Design

## Goal

Add a reusable ASP.NET Core addon for `StarDust.CasparCG` that exposes a modern HTTP API and SSE event stream over the existing `CasparClient`, while also enabling a lightweight standalone proxy host built on top of the same package.

## Product Shape

The design targets two related deliverables:

1. A reusable package, tentatively named `StarDust.CasparCG.AspNetCore`, for embedding CasparCG HTTP endpoints inside any ASP.NET Core application.
2. A sample proxy host that demonstrates how to expose the package as a standalone REST and SSE gateway.

The package is the primary product. The standalone proxy remains a thin composition layer over the package rather than a separate implementation surface.

## Scope

### In scope for V1

- Resource-oriented HTTP routes over the existing `CasparClient`
- Snapshot-style query endpoints for common server, media, and data reads
- Command endpoints for playback, mixer, CG, thumbnails, data, and selected admin operations
- Server-Sent Events for live event streaming
- Reusable endpoint mapping and DI registration for ASP.NET Core
- A client resolution abstraction that supports default-client behavior now and named clients later
- ProblemDetails-based error responses
- Unit and integration test coverage for route mapping, HTTP-to-client translation, and SSE streaming
- A sample proxy host with optional minimal authentication

### Out of scope for V1

- A separate production-grade standalone product with its own independent routing stack
- Full multi-server URL routing such as `/servers/{name}/...`
- WebSocket support
- A generic raw AMCP passthrough endpoint
- Opinionated persistence, authorization policy engines, or rate limiting frameworks

## Design Principles

- Reuse `CasparClient` as the only command/query execution surface
- Keep HTTP translation thin and predictable
- Expose readable routes without pretending every CasparCG concept is natural CRUD
- Prefer typed request/response DTOs over opaque payload blobs
- Keep the ASP.NET Core package host-neutral so app-specific auth and middleware stay in the host
- Design internal seams now for future multi-client support without forcing it into V1 routes

## Architecture

### Package layout

Add a new package:

- `src/StarDust.CasparCG.AspNetCore`

This package will sit above:

- `src/StarDust.CasparCG`
- `src/StarDust.CasparCG.Hosting`

It will not reimplement transport, protocol, or command-building logic. Its job is to adapt the public client library to HTTP and SSE.

### Primary responsibilities

The package provides:

- DI extensions to register REST API services and options
- Minimal API endpoint mapping extensions
- DTOs for HTTP request and response contracts
- Translators from HTTP requests to `CasparClient` calls
- Translators from client/query results to HTTP payloads
- A live SSE event bridge over the existing event stream
- Common error mapping into `ProblemDetails`

### Internal units

#### Service registration

`AddCasparCGRestApi()` registers:

- endpoint mapping dependencies
- request validators where needed
- result translators
- event bridge services
- REST API options

The package assumes the host has already called `AddCasparCG()` from the hosting package.

#### Endpoint mapping

`MapCasparCGApi()` maps the HTTP surface to Minimal API endpoints. It should be possible to mount the API at root or under a route group such as `/api/casparcg`.

#### Client resolver

Introduce a small internal abstraction for resolving the effective `CasparClient`.

In V1:

- resolve the default client only

Internally:

- carry enough structure so future named-client routing can resolve a client target from route data, options, or endpoint metadata

#### HTTP translators

These components convert:

- route/body/query data into typed command or query invocations
- returned values into stable HTTP DTOs
- transport/protocol failures into normalized HTTP errors

They should remain thin orchestration layers rather than alternate business logic.

#### SSE event bridge

An event streaming service adapts the existing client event pipeline into `text/event-stream` responses for HTTP consumers.

## HTTP Surface

The API is resource-oriented, but not strict CRUD for every CasparCG command family. Routes should stay readable and predictable while remaining close to the existing client capability model.

### Server routes

Examples:

- `GET /server/version`
- `GET /server/info`
- `GET /server/paths`
- `GET /server/config`
- `GET /server/diagnostics`

Selected admin operations may be exposed separately under:

- `POST /admin/restart`
- `POST /admin/kill`

Admin routes must be isolated from read/query routes so hosts can protect them independently.

### Channel and layer routes

Examples:

- `GET /channels/{channel}/layers/{layer}`
- `POST /channels/{channel}/layers/{layer}/play`
- `POST /channels/{channel}/layers/{layer}/loadbg`
- `POST /channels/{channel}/layers/{layer}/pause`
- `POST /channels/{channel}/layers/{layer}/resume`
- `POST /channels/{channel}/layers/{layer}/stop`
- `POST /channels/{channel}/layers/{layer}/clear`

Request bodies should carry only the parameters needed by the underlying command, such as clip name, loop flag, transition, seek, or mixer values.

### Mixer routes

Prefer dedicated endpoints per operation family instead of a single generic mixer command endpoint.

Examples:

- `POST /channels/{channel}/layers/{layer}/mixer/opacity`
- `POST /channels/{channel}/layers/{layer}/mixer/fill`
- `POST /channels/{channel}/layers/{layer}/mixer/clip`
- `POST /channels/{channel}/layers/{layer}/mixer/volume`
- `POST /channels/{channel}/layers/{layer}/mixer/clear`

This keeps request contracts explicit and easier to validate.

### CG routes

Examples:

- `POST /channels/{channel}/layers/{layer}/cg/{cgLayer}/add`
- `POST /channels/{channel}/layers/{layer}/cg/{cgLayer}/play`
- `POST /channels/{channel}/layers/{layer}/cg/{cgLayer}/stop`
- `POST /channels/{channel}/layers/{layer}/cg/{cgLayer}/next`
- `DELETE /channels/{channel}/layers/{layer}/cg/{cgLayer}`

### Data routes

Examples:

- `GET /data`
- `GET /data/{key}`
- `PUT /data/{key}`
- `DELETE /data/{key}`

The V1 design should return typed payloads where possible instead of leaking AMCP response formatting.

### Media and thumbnail routes

Examples:

- `GET /media/files`
- `GET /media/templates`
- `GET /media/fonts`
- `GET /media/{clip}/info`
- `GET /thumbnails`
- `GET /thumbnails/{clip}`
- `POST /thumbnails/{clip}/generate`

### Future extensibility

The route design must leave room for later evolution to:

- `/servers/{serverName}/server/version`
- `/servers/{serverName}/channels/{channel}/layers/{layer}/play`

This future shape should not require rewriting the endpoint mapping architecture.

## SSE Event Streaming

### Route

- `GET /events`

### Transport choice

V1 uses Server-Sent Events rather than WebSockets because the primary need is server-to-client event delivery for dashboards, web apps, and tooling.

### Event envelope

Each SSE message should provide a stable envelope containing:

- event type
- timestamp
- logical target identifier
- payload

The logical target identifier can be a default placeholder in V1 and evolve later for named clients.

### Event model

V1 should stream:

- useful raw or near-raw CasparCG events already represented by the client
- a normalized event name for consumers

The package should avoid inventing a large parallel event taxonomy in V1. It should normalize only enough to keep the HTTP contract stable and comprehensible.

### Relationship to snapshots

REST endpoints remain the way to fetch current snapshots on demand. SSE complements them for live monitoring and UI refresh triggers.

## Error Handling

All failures should be translated into consistent `ProblemDetails` responses.

### Status guidance

- `400 Bad Request` for invalid route/query/body input
- `404 Not Found` when a resource-style lookup clearly has no result
- `409 Conflict` where command semantics indicate a meaningful command-state conflict
- `502 Bad Gateway` for CasparCG protocol or upstream execution failures that are not HTTP caller mistakes
- `503 Service Unavailable` when the proxy cannot reach or maintain connection to the target server

### Error payload

Problem responses should include:

- title
- status
- detail
- a stable error code where useful

Avoid leaking raw internal exception details by default.

## Authentication and Host Integration

The package itself remains authentication-neutral.

That means:

- no built-in mandatory auth scheme in the reusable addon
- no package-level assumption about API keys, JWT, cookies, or reverse proxy auth

For the sample proxy host:

- include optional minimal auth configuration, such as API key or bearer setup
- keep this clearly sample-level rather than core-package behavior

This preserves reusability while still demonstrating secure deployment patterns.

## Configuration

V1 configuration should remain minimal.

Expected options include:

- route prefix
- whether admin endpoints are mapped
- whether SSE is enabled
- optional serialization or API behavior toggles only if clearly necessary

Do not introduce a large configuration object without a concrete use.

## Testing Strategy

### Unit tests

Cover:

- route mapping expectations
- DTO validation
- request-to-command translation
- result-to-response translation
- error mapping

### Integration tests

Use ASP.NET Core test hosting plus existing `DummyServer` facilities to verify:

- representative query endpoints
- representative command endpoints
- failure translation
- SSE connection and event delivery behavior

### Sample smoke coverage

The sample proxy host should have lightweight verification that it starts, maps endpoints, and can execute a small happy-path request against a test server.

## Sample Proxy Host

Provide a sample application that demonstrates:

- `AddCasparCG()`
- `AddCasparCGRestApi()`
- `MapCasparCGApi()`
- optional auth setup
- route prefixing if desired

The sample should be positioned as a reference host, not as an alternate implementation.

## Packaging and Adoption

Intended consumer experience:

```csharp
builder.Services
    .AddCasparCG()
    .ConnectTo("127.0.0.1", 5250)
    .ListenOscOn(6250);

builder.Services.AddCasparCGRestApi();

var app = builder.Build();
app.MapCasparCGApi();
```

This should feel like a natural extension of the existing hosting API.

## Non-Goals

V1 is not trying to:

- redesign CasparCG semantics into perfect REST purity
- provide every possible orchestration abstraction
- replace direct use of `CasparClient` for advanced application code
- solve long-term multi-tenant API gateway concerns

## Success Criteria

The design succeeds if:

- an ASP.NET Core host can expose readable HTTP routes for major CasparCG operations with minimal setup
- the package reuses the existing client model rather than duplicating protocol logic
- consumers can both execute commands and retrieve data through modern HTTP contracts
- live event observation is available through SSE
- the path to a future named-client, multi-server surface remains open without major architectural rewrite
