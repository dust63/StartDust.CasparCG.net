# AMCP Problem Details Mapping Design

## Status
Draft

## Context

The REST addon already translates transport failures and AMCP command failures into `ProblemDetails`, but the mapping is still too coarse:

- AMCP numeric error codes are not surfaced in a structured way.
- REST status codes are currently chosen with a small number of broad buckets.
- callers cannot easily inspect the original AMCP status line or command text when diagnosing failures.

The goal of this design is to make REST error responses more useful without leaking AMCP internals into every endpoint handler. We want a single mapping layer that:

- converts AMCP failures into an appropriate HTTP status code
- preserves the original AMCP status code and status line
- exposes enough structured diagnostics for clients and operators to understand what happened

This is a breaking behavior change for error payloads only, not for success responses.

## Goals

- Map AMCP failures to REST status codes by error class.
- Expose the original AMCP status code, status line, command text, and category in `ProblemDetails.extensions`.
- Keep a single, shared translation path for REST handlers.
- Preserve the existing `ProblemDetails` shape for non-AMCP errors.
- Keep the implementation small and easy to reason about.

## Non-Goals

- No redesign of successful REST payloads.
- No change to the AMCP protocol itself.
- No per-endpoint custom error mapping.
- No attempt to encode every possible AMCP semantic into HTTP.

## Proposed Design

### Translation boundary

`CasparProblemDetailsFactory` remains the only place that knows how to translate exceptions into REST error payloads.

It should accept the thrown exception and, for `AmcpCommandException`, read the failed `AmcpResponse` directly from the exception.

For AMCP command failures, the factory should populate:

- `title`
- `detail`
- `status`
- `extensions["amcpStatusCode"]`
- `extensions["amcpStatusLine"]`
- `extensions["amcpCommandText"]`
- `extensions["amcpCategory"]`

For non-AMCP failures, the factory should keep the response lightweight and only map the exception to an HTTP status and message.

### HTTP status mapping

The REST status code should represent the class of the failure, not the raw AMCP code.

Recommended mapping:

| AMCP code | AMCP meaning | REST status |
| --- | --- | --- |
| `400` | Command not understood | `400 Bad Request` |
| `401` | Invalid or missing channel | `400 Bad Request` |
| `402` | Parameter missing | `400 Bad Request` |
| `403` | Invalid parameter | `400 Bad Request` |
| `404` | File not found | `404 Not Found` |
| `500` | Internal error | `502 Bad Gateway` |
| `501` | Internal error | `502 Bad Gateway` |
| `502` | Could not read file | `502 Bad Gateway` |
| `503` | Access denied | `403 Forbidden` |
| `504` | Queue overflow | `429 Too Many Requests` |
| `600` | Not implemented | `501 Not Implemented` |

If an unknown AMCP status code appears, the fallback should be `502 Bad Gateway`.

### Payload shape

The payload should remain standard RFC 9457 `ProblemDetails`, with extra AMCP fields in `extensions`.

Example:

```json
{
  "title": "CasparCG upstream failure",
  "status": 404,
  "detail": "AMCP command failed with status code 404: 404 PLAY FAILED",
  "amcpStatusCode": 404,
  "amcpStatusLine": "404 PLAY FAILED",
  "amcpCommandText": "PLAY",
  "amcpCategory": "ClientError"
}
```

The exact JSON field names are part of the contract and should stay stable once implemented.

### Handler integration

REST handlers should continue to call a shared helper instead of duplicating error logic.

The helper should:

- catch `AmcpCommandException`
- call the factory
- return the result as `ProblemDetails`

The health route should keep its own `503 Service Unavailable` path for connection failures and may reuse the same `ProblemDetails` formatting helper for consistency.

## Error Handling

- AMCP errors with a direct HTTP analogue should use the closest HTTP status class.
- Transport failures should remain `502 Bad Gateway` or `503 Service Unavailable` depending on whether the client reached the server at all.
- Unknown exceptions should continue to default to `503 Service Unavailable` unless a transport failure makes `502` more appropriate.
- The response body should always include the AMCP diagnostic fields when the exception was produced from an AMCP response.

## Testing Strategy

Add tests that cover:

- `404` AMCP -> `404 Not Found`
- `402` AMCP -> `400 Bad Request`
- `503` AMCP -> `403 Forbidden`
- `504` AMCP -> `429 Too Many Requests`
- `600` AMCP -> `501 Not Implemented`
- `ProblemDetails.extensions` contains the AMCP metadata
- transport failures still map to the existing upstream failure status

Preferred test layers:

- unit tests for the mapping function
- one integration test that verifies the REST JSON payload includes the AMCP metadata

## Rollout Notes

- This change is breaking for error payload consumers that depend on the current coarse mapping.
- Success payloads remain unchanged.
- Existing docs should be updated to explain that AMCP failures are projected into REST `ProblemDetails` with AMCP metadata in `extensions`.
