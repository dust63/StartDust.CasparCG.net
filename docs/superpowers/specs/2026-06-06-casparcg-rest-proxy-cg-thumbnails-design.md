# CasparCG REST Proxy CG and Thumbnails Slice Design

## Goal

Extend `StarDust.CasparCG.AspNetCore` with a focused second coverage slice that adds:

- CG template command routes
- Thumbnail query and generation routes
- Clear HTTP contracts for template payload translation

This slice should make the REST addon substantially more useful without attempting full AMCP parity in one step.

## Why This Slice

The current REST surface covers a small but credible core:

- server version
- media file listing
- data read and write
- playback commands
- one mixer command
- admin restart
- SSE events

Compared to the AMCP coverage already present in `StarDust.CasparCG`, the next most valuable gaps are:

1. CG template operations
2. Thumbnail listing, retrieval, and generation

These two families improve both application control and browser-friendly asset access while still reusing existing client primitives.

## Scope

### In scope

- CG command endpoints for add, play, stop, next, remove, clear, update, and invoke
- Thumbnail endpoints for list, retrieve, generate, and generate-all
- Content negotiation for CG payloads based on `Content-Type`
- JSON-to-XML template payload translation for CG requests
- Structured thumbnail list DTOs
- Binary thumbnail HTTP responses
- Unit and integration tests for translation, endpoint behavior, and emitted AMCP commands
- Documentation updates that clearly separate implemented REST routes from broader AMCP capability

### Out of scope

- Additional server query expansion such as `INFO`, `PATHS`, or diagnostics
- More media query families beyond the existing `GET /media/files`
- Multi-server routing
- Swagger or OpenAPI generation
- Generic AMCP passthrough
- New SSE event types specifically for templates or thumbnails

## Route Additions

### CG routes

Add the following routes:

- `POST /channels/{channel}/layers/{layer}/cg/add`
- `POST /channels/{channel}/layers/{layer}/cg/play`
- `POST /channels/{channel}/layers/{layer}/cg/stop`
- `POST /channels/{channel}/layers/{layer}/cg/next`
- `POST /channels/{channel}/layers/{layer}/cg/remove`
- `POST /channels/{channel}/layers/{layer}/cg/clear`
- `POST /channels/{channel}/layers/{layer}/cg/update`
- `POST /channels/{channel}/layers/{layer}/cg/invoke`

These routes intentionally target the existing layer-oriented CG surface already exposed by `CasparClient`. This avoids inventing a separate `cgLayer` resource shape that the current library does not model.

### Thumbnail routes

Add the following routes:

- `GET /thumbnails`
- `GET /thumbnails/{fileName}`
- `POST /thumbnails/{fileName}/generate`
- `POST /thumbnails/generate-all`

### Media routes

Keep the current `GET /media/files` route unchanged in this slice.

## CG Request Model

### Content negotiation

`cg/add` and `cg/update` accept two payload formats selected by `Content-Type`:

- `application/json`
- `application/xml` or `text/xml`

If the request uses any other media type, the API returns `415 Unsupported Media Type`.

### JSON contract

For `application/json`, the proxy accepts a Caspar-focused JSON envelope and converts it deterministically to the XML payload expected by AMCP template commands.

For `cg/add`, the JSON request shape is:

```json
{
  "template": "LowerThird",
  "playOnLoad": true,
  "templateData": {
    "components": [
      {
        "id": "f0",
        "data": [
          { "id": "headline", "value": "Hello" }
        ]
      }
    ]
  }
}
```

For `cg/update`, the JSON request shape is:

```json
{
  "templateData": {
    "components": [
      {
        "id": "f0",
        "data": [
          { "id": "headline", "value": "Updated" }
        ]
      }
    ]
  }
}
```

The conversion rules should be explicit and stable:

- `components[]` becomes repeated `<componentData>` elements
- `component.id` maps to the `id` attribute
- `data[]` becomes repeated `<data>` elements inside the component
- `data.id` maps to the `id` attribute
- `data.value` maps to the `value` attribute

The generated XML should match the payload shape commonly expected by CasparCG templates.

### XML contract

For `application/xml` or `text/xml`, the request body is treated as the template XML payload and sent through without semantic transformation.

For `cg/add`, XML requests still need route-independent JSON-like metadata in HTTP form. The cleanest V2 contract is:

- `template` and `playOnLoad` remain query parameters for XML requests
- the request body contains the XML template data

Example:

- `POST /channels/1/layers/10/cg/add?template=LowerThird&playOnLoad=true`
- body: raw XML

For `cg/update`, only the XML body is needed.

This keeps XML support unambiguous without requiring mixed JSON and XML in one request body.

### Invoke contract

`cg/invoke` accepts a small JSON body:

```json
{
  "method": "next()"
}
```

## Thumbnail Response Model

### `GET /thumbnails`

Return a structured JSON list instead of raw AMCP text.

Each item should expose:

- `name`
- `sizeBytes` when present
- `lastModified` when present
- any additional thumbnail-list metadata that can be parsed reliably from the current response format

The route should not expose a fallback raw string field in this slice. If the current parser cannot extract a field reliably, omit that field rather than leaking unstable raw formatting into the public API.

### `GET /thumbnails/{fileName}`

Return the thumbnail as a binary HTTP response suitable for browsers and dashboards.

Expected behavior:

- decode the AMCP thumbnail payload into bytes
- return `200 OK` with an image media type
- prefer a precise image content type if it can be inferred reliably
- otherwise use a safe fallback such as `application/octet-stream`

Errors retrieving or decoding the thumbnail should be translated into `ProblemDetails`.

### Generation routes

- `POST /thumbnails/{fileName}/generate` triggers thumbnail generation for a single file
- `POST /thumbnails/generate-all` triggers generation for all files

Both routes return success with no body content beyond standard HTTP semantics already used by the existing command endpoints.

## Internal Architecture

### Endpoint mapping

Extend `MapCasparCGApi()` with a CG route group and a thumbnail route group. These additions should follow the existing pattern used for query and command routes.

### CG payload translator

Add a focused translator component responsible for:

- inspecting `Content-Type`
- reading JSON or XML request bodies
- validating required metadata
- converting supported JSON payloads into XML

This translator should live outside the endpoint method bodies so it can be unit tested directly.

### CG command handlers

Add handlers that map the translated input to the existing `CasparClient` methods:

- `CgAddAsync`
- `CgPlayAsync`
- `CgStopAsync`
- `CgNextAsync`
- `CgRemoveAsync`
- `CgClearAsync`
- `CgUpdateAsync`
- `CgInvokeAsync`

These handlers should stay thin and should not duplicate AMCP formatting logic that already exists in the core client.

### Thumbnail query handlers

Add handlers that:

- map thumbnail list responses into structured DTOs
- retrieve and decode thumbnail content into an HTTP file response
- trigger thumbnail generation commands

### Error mapping

Extend the existing error handling behavior with:

- `400 Bad Request` for invalid JSON structure, invalid XML payloads, or missing required query values
- `415 Unsupported Media Type` for unsupported CG request media types
- existing `502` and `503` behavior for CasparCG or transport failures

## Testing Strategy

### Unit tests

Add unit coverage for:

- JSON-to-XML template translation
- XML request validation
- media type selection for CG endpoints
- thumbnail list parsing into DTOs

Tests should follow AAA structure and keep each case narrow and explicit.

### Integration tests

Add integration coverage using the existing test host and dummy server for:

- each new CG route emitting the correct AMCP command
- `cg/add` using JSON payload translation
- `cg/add` using XML payload passthrough
- `cg/update` using JSON payload translation
- `cg/invoke` sending the expected method
- `GET /thumbnails` returning structured JSON
- `GET /thumbnails/{fileName}` returning binary content with an appropriate content type
- thumbnail generation routes emitting the expected AMCP commands

## Documentation Updates

Update the REST documentation so the route table is explicit about what is implemented now.

The REST docs should:

- list the newly added CG and thumbnail routes
- explain the dual `Content-Type` contract for `cg/add` and `cg/update`
- provide one JSON example and one XML example
- explain that REST coverage is still partial compared to the full AMCP support of the library
- separate current REST implementation from future AMCP-aligned expansion where useful

## Implementation Boundaries

This slice should be treated as complete when:

- the new routes are implemented
- payload translation behavior is deterministic and tested
- thumbnail retrieval works as a browser-friendly binary response
- docs accurately describe current route coverage

This slice should not expand into wider server, admin, mixer, or generic media coverage during implementation.
