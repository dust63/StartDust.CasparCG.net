# REST API Addon

`StarDust.CasparCG.AspNetCore` exposes a resource-oriented HTTP and SSE layer on top of `CasparClient`.

## Setup

```csharp
builder.Services
    .AddCasparCG()
    .ConnectTo("127.0.0.1", 5250)
    .ListenOscOn(6250);

builder.Services.AddCasparCGRestApi();

var app = builder.Build();
await app.Services.GetRequiredService<CasparClient>().ConnectAsync();
app.MapCasparCGApi();
```

## Route Table

| Method | Route | Purpose | Request body |
| --- | --- | --- | --- |
| `GET` | `/server/version` | Read the CasparCG server version | none |
| `GET` | `/media/files` | List media files returned by `CLS` | none |
| `GET` | `/data/{key}` | Read a data payload returned by `DATA RETRIEVE` | none |
| `PUT` | `/data/{key}` | Store a data payload with `DATA STORE` | `{"value":"hello"}` |
| `POST` | `/channels/{channel}/layers/{layer}/play` | Send `PLAY` | `{"clip":"AMB","loop":false,"transitionDuration":null}` |
| `POST` | `/channels/{channel}/layers/{layer}/loadbg` | Send `LOADBG` | `{"clip":"BG","loop":true,"autoPlay":true}` |
| `POST` | `/channels/{channel}/layers/{layer}/pause` | Send `PAUSE` | none |
| `POST` | `/channels/{channel}/layers/{layer}/resume` | Send `RESUME` | none |
| `POST` | `/channels/{channel}/layers/{layer}/stop` | Send `STOP` | none |
| `POST` | `/channels/{channel}/layers/{layer}/mixer/opacity` | Send `MIXER OPACITY` | `{"value":0.5}` |
| `POST` | `/channels/{channel}/layers/{layer}/cg/add` | Send `CG ADD` | JSON or XML body |
| `POST` | `/channels/{channel}/layers/{layer}/cg/play` | Send `CG PLAY` | none |
| `POST` | `/channels/{channel}/layers/{layer}/cg/stop` | Send `CG STOP` | none |
| `POST` | `/channels/{channel}/layers/{layer}/cg/next` | Send `CG NEXT` | none |
| `POST` | `/channels/{channel}/layers/{layer}/cg/remove` | Send `CG REMOVE` | none |
| `POST` | `/channels/{channel}/layers/{layer}/cg/clear` | Send `CG CLEAR` | none |
| `POST` | `/channels/{channel}/layers/{layer}/cg/update` | Send `CG UPDATE` | JSON or XML body |
| `POST` | `/channels/{channel}/layers/{layer}/cg/invoke` | Send `CG INVOKE` | `{"method":"next()"}` |
| `POST` | `/admin/restart` | Send `RESTART` | none |
| `GET` | `/thumbnails` | Send `THUMBNAIL LIST` | none |
| `GET` | `/thumbnails/{fileName}` | Send `THUMBNAIL RETRIEVE` | none |
| `POST` | `/thumbnails/{fileName}/generate` | Send `THUMBNAIL GENERATE` | none |
| `POST` | `/thumbnails/generate-all` | Send `THUMBNAIL GENERATE_ALL` | none |
| `GET` | `/events` | Stream live Caspar events over SSE | none |

## Request Contracts

### Play

```json
{
  "clip": "AMB",
  "loop": false,
  "transitionDuration": null
}
```

If `transitionDuration` is set, the REST addon sends `PLAY ... MIX <duration>`.

### Load background

```json
{
  "clip": "BG",
  "loop": true,
  "autoPlay": true
}
```

This maps to `LOADBG ... LOOP AUTO` when both flags are enabled.

### Mixer opacity

```json
{
  "value": 0.5
}
```

### CG add with JSON

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

The addon translates the JSON envelope into CasparCG-compatible `<templateData>` XML before sending `CG ADD`.

### CG add or update with XML

For XML payloads, post the raw `<templateData>` body with `Content-Type: application/xml` or `text/xml`.

`CG ADD` requires `template` and optional `playOnLoad` query parameters when the body is XML:

```text
POST /channels/1/layers/10/cg/add?template=LowerThird&playOnLoad=true
Content-Type: application/xml
```

```xml
<templateData><componentData id="f0"><data id="headline" value="Hello" /></componentData></templateData>
```

### CG invoke

```json
{
  "method": "next()"
}
```

### Data store

```json
{
  "value": "hello"
}
```

## Response Contracts

### `GET /server/version`

```json
{
  "version": "2.4.0"
}
```

### `GET /media/files`

```json
[
  {
    "name": "AMB",
    "kind": "Movie",
    "sizeBytes": 42,
    "lastModified": "2024-01-01T12:00:00+00:00",
    "frameCount": 240,
    "frameRateOrDuration": "1/25"
  }
]
```

### `GET /data/{key}`

```json
{
  "key": "title",
  "lines": ["hello"]
}
```

### `GET /thumbnails`

```json
[
  {
    "name": "AMB",
    "sizeBytes": 42,
    "lastModified": "2024-01-01T12:00:00+00:00"
  }
]
```

### `GET /thumbnails/{fileName}`

Returns the decoded thumbnail bytes with `Content-Type: application/octet-stream`.

## SSE

`GET /events` returns `text/event-stream`.

Current normalized event names:

| SSE event | Payload source |
| --- | --- |
| `playbackClipChanged` | `PlaybackClipChangedEvent` |

Example frame:

```text
event: playbackClipChanged
data: {"type":"playbackClipChanged","timestamp":"2026-06-06T09:45:00+00:00","target":"default","payload":{"clientName":"default","channel":1,"layer":10,"clip":"AMB"}}
```

## Error Model

The addon returns `ProblemDetails` payloads.

Current status mapping:

| Status | Meaning |
| --- | --- |
| `502` | CasparCG upstream failure, transport I/O issue, or failed AMCP command |
| `503` | CasparCG client unavailable or other runtime failure |

## Current Limits

- No OpenAPI or Swagger integration yet
- No WebSocket endpoint
- No multi-server route prefix like `/servers/{name}/...`
- No raw AMCP passthrough route
- Thumbnail retrieval currently returns `application/octet-stream` without image type sniffing
