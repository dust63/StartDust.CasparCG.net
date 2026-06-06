# REST API Addon

`StarDust.CasparCG.AspNetCore` exposes a resource-oriented HTTP and SSE layer on top of `CasparClient`.

## Setup

```csharp
builder.Services
    .AddCasparCG()
    .ConnectTo("127.0.0.1", 5250)
    .ListenOscOn(6250);

builder.Services.AddCasparCGFromConfiguration(builder.Configuration);
builder.Services.AddCasparCGRestApi();

var app = builder.Build();
// Optional warm-up: the first AMCP call also connects lazily.
await app.Services.GetRequiredService<CasparClient>().ConnectAsync();
app.MapCasparCGApi();
```

## Addon Options

`AddCasparCGRestApi(...)` accepts a `CasparRestApiOptions` block.

| Option | Default | Purpose |
| --- | --- | --- |
| `RoutePrefix` | empty | Prefixes every REST and Swagger route, for example `/api/caspar` |
| `MapAdminEndpoints` | `false` | Enables the admin routes that mutate server state |
| `EnableSse` | `true` | Maps `GET /events` for live SSE streaming |
| `EnableOpenApi` | `true` | Maps `GET /swagger/v1/swagger.json` for the generated OpenAPI document |
| `WarmUpClientsOnStartup` | `false` | Connects configured clients when the host starts |
| `MapHealthEndpoints` | `true` | Maps `GET /health` and `GET /servers/{name}/health` |

When you do not call `ConnectAsync()` at startup, the first REST request performs the AMCP connection lazily. Calling `ConnectAsync()` remains useful when you want the application to fail fast during boot.
The health route also lazy-connects the target client on first use and returns a `503` `ProblemDetails` payload when the connection cannot be established.

## Multiple Servers

You can register several named clients at startup and target them through the REST API by prefixing routes with `/servers/{name}`.

```csharp
builder.Services
    .AddCasparCG()
    .ConnectTo("127.0.0.1", 5250);

builder.Services
    .AddCasparCG("studio-a")
    .ConnectTo("10.0.0.10", 5250);

builder.Services.AddCasparCGRestApi();
```

The REST addon resolves the client from the route prefix:

```text
GET /servers/default/server/version
GET /servers/studio-a/server/version
POST /servers/studio-a/channels/1/layers/10/play
```

If no `/servers/{name}` prefix is used, the addon continues to target the default client.

`AddCasparCGFromConfiguration(...)` looks for `CasparCG:Clients` in configuration and registers each named client found there. The `default` entry remains the client used by unprefixed routes.

## Route Table

| Method | Route | Purpose | Request body |
| --- | --- | --- | --- |
| `GET` | `/server/version` | Read the CasparCG server version | none |
| `GET` | `/health` | Read and initialize the default client health | none |
| `GET` | `/server/info` | Read the `INFO` payload | none |
| `GET` | `/server/info/config` | Read the `INFO CONFIG` payload | none |
| `GET` | `/server/info/paths` | Read the `INFO PATHS` payload | none |
| `GET` | `/server/gl/info` | Read the `GL INFO` payload | none |
| `POST` | `/server/gl/gc` | Send `GL GC` | none |
| `POST` | `/server/diag` | Send `DIAG` | none |
| `GET` | `/admin/log-level` | Read the current `LOG LEVEL` | none |
| `PUT` | `/admin/log-level` | Send `LOG LEVEL <value>` | `{"value":"debug"}` |
| `POST` | `/admin/bye` | Send `BYE` | none |
| `POST` | `/admin/kill` | Send `KILL` | none |
| `GET` | `/media/files` | List media files returned by `CLS` | none |
| `GET` | `/media/files/{fileName}` | Read detailed media info returned by `CINF` | none |
| `GET` | `/fonts` | List fonts returned by `FLS` | none |
| `GET` | `/templates` | List templates returned by `TLS` | none |
| `GET` | `/data` | List data keys returned by `DATA LIST` | none |
| `GET` | `/data/{key}` | Read a data payload returned by `DATA RETRIEVE` | none |
| `PUT` | `/data/{key}` | Store a data payload with `DATA STORE` | `{"value":"hello"}` |
| `DELETE` | `/data/{key}` | Remove a data payload with `DATA REMOVE` | none |
| `POST` | `/channels/clear-all` | Send `CLEAR ALL` | none |
| `POST` | `/channels/{channel}/clear` | Send `CLEAR` for the whole channel | none |
| `POST` | `/channels/{channel}/grid` | Send `CHANNEL_GRID` | none |
| `POST` | `/channels/{channel}/layers/{layer}/load` | Send `LOAD` | `{"clip":"AMB","options":{"transition":{"kind":"mix","duration":10},"seek":12,"length":24,"filter":"hflip","clearOn404":true}}` |
| `POST` | `/channels/{channel}/layers/{layer}/play` | Send `PLAY` | `{"clip":"AMB","options":{"transition":{"kind":"slide","duration":10,"tweener":"linear","direction":"left"},"loop":false}}` |
| `POST` | `/channels/{channel}/layers/{layer}/loadbg` | Send `LOADBG` | `{"clip":"BG","options":{"transition":{"kind":"mix","duration":10},"loop":true,"seek":100,"length":200,"filter":"hflip","clearOn404":true,"autoPlay":true}}` |
| `POST` | `/channels/{channel}/layers/{layer}/pause` | Send `PAUSE` | none |
| `POST` | `/channels/{channel}/layers/{layer}/resume` | Send `RESUME` | none |
| `POST` | `/channels/{channel}/layers/{layer}/stop` | Send `STOP` | none |
| `POST` | `/channels/{channel}/layers/{layer}/clear` | Send `CLEAR` | none |
| `POST` | `/channels/{channel}/layers/{layer}/call` | Send `CALL` | `{"arguments":"SEEK 25"}` |
| `POST` | `/channels/{channel}/layers/{layer}/callbg` | Send `CALLBG` | `{"arguments":"LOOP"}` |
| `POST` | `/channels/{channel}/layers/{layer}/swap` | Send `SWAP` | `{"otherChannel":2,"otherLayer":20,"swapTransforms":true}` |
| `POST` | `/channels/{channel}/add` | Send `ADD` | `{"consumer":"FILE","arguments":"filename.mov","consumerIndex":700}` |
| `POST` | `/channels/{channel}/remove` | Send `REMOVE` | `{"arguments":"FILE filename.mov"}` or `{"consumerIndex":700}` |
| `POST` | `/channels/{channel}/apply` | Send `APPLY` | `{"layer":10,"arguments":"LOWER"}` |
| `POST` | `/channels/{channel}/print` | Send `PRINT` | none |
| `POST` | `/channels/{channel}/set` | Send `SET` | `{"key":"MODE","value":"FAST"}` |
| `POST` | `/channels/{channel}/layers/{layer}/mixer/keyer` | Send `MIXER KEYER` | `{"value":true}` |
| `POST` | `/channels/{channel}/layers/{layer}/mixer/invert` | Send `MIXER INVERT` | `{"value":false}` |
| `POST` | `/channels/{channel}/layers/{layer}/mixer/blend` | Send `MIXER BLEND` | `{"value":"ADD"}` |
| `POST` | `/channels/{channel}/layers/{layer}/mixer/opacity` | Send `MIXER OPACITY` | `{"value":0.5}` |
| `POST` | `/channels/{channel}/layers/{layer}/mixer/brightness` | Send `MIXER BRIGHTNESS` | `{"value":0.6}` |
| `POST` | `/channels/{channel}/layers/{layer}/mixer/saturation` | Send `MIXER SATURATION` | `{"value":0.7}` |
| `POST` | `/channels/{channel}/layers/{layer}/mixer/contrast` | Send `MIXER CONTRAST` | `{"value":0.8}` |
| `POST` | `/channels/{channel}/layers/{layer}/mixer/volume` | Send `MIXER VOLUME` | `{"value":0.9}` |
| `POST` | `/mixer/master-volume` | Send `MIXER MASTERVOLUME` | `{"value":0.4}` |
| `POST` | `/channels/{channel}/layers/{layer}/mixer/chroma` | Send `MIXER CHROMA` | `{"arguments":"0.1 0.2 0.3"}` |
| `POST` | `/channels/{channel}/layers/{layer}/mixer/levels` | Send `MIXER LEVELS` | `{"arguments":"0 1 1 0 1"}` |
| `POST` | `/channels/{channel}/layers/{layer}/mixer/fill` | Send `MIXER FILL` | `{"arguments":"0 0 1 1"}` |
| `POST` | `/channels/{channel}/layers/{layer}/mixer/clip` | Send `MIXER CLIP` | `{"arguments":"0 0 1 1"}` |
| `POST` | `/channels/{channel}/layers/{layer}/mixer/anchor` | Send `MIXER ANCHOR` | `{"arguments":"0.5 0.5"}` |
| `POST` | `/channels/{channel}/layers/{layer}/mixer/crop` | Send `MIXER CROP` | `{"arguments":"0 0 0 0"}` |
| `POST` | `/channels/{channel}/layers/{layer}/mixer/rotation` | Send `MIXER ROTATION` | `{"arguments":"45"}` |
| `POST` | `/channels/{channel}/layers/{layer}/mixer/perspective` | Send `MIXER PERSPECTIVE` | `{"arguments":"0 0 1 0 1 1 0 1"}` |
| `POST` | `/channels/{channel}/layers/{layer}/mixer/grid` | Send `MIXER GRID` | `{"arguments":"2 2"}` |
| `POST` | `/channels/{channel}/layers/{layer}/mixer/commit` | Send `MIXER COMMIT` | none |
| `POST` | `/channels/{channel}/layers/{layer}/mixer/clear` | Send `MIXER CLEAR` | none |
| `POST` | `/channels/{channel}/layers/{layer}/cg/add` | Send `CG ADD` | JSON or XML body |
| `POST` | `/channels/{channel}/layers/{layer}/cg/play` | Send `CG PLAY` | none |
| `POST` | `/channels/{channel}/layers/{layer}/cg/stop` | Send `CG STOP` | none |
| `POST` | `/channels/{channel}/layers/{layer}/cg/next` | Send `CG NEXT` | none |
| `POST` | `/channels/{channel}/layers/{layer}/cg/remove` | Send `CG REMOVE` | none |
| `POST` | `/channels/{channel}/layers/{layer}/cg/clear` | Send `CG CLEAR` | none |
| `POST` | `/channels/{channel}/layers/{layer}/cg/update` | Send `CG UPDATE` | JSON or XML body |
| `POST` | `/channels/{channel}/layers/{layer}/cg/invoke` | Send `CG INVOKE` | `{"method":"next()"}` |
| `POST` | `/admin/restart` | Send `RESTART` | none |
| `POST` | `/admin/locks/{channel}/acquire` | Send `LOCK ACQUIRE` | `{"phrase":"phrase"}` |
| `POST` | `/admin/locks/{channel}/release` | Send `LOCK RELEASE` | none |
| `POST` | `/admin/locks/{channel}/clear` | Send `LOCK CLEAR` | `{"overridePhrase":"override"}` |
| `GET` | `/thumbnails` | Send `THUMBNAIL LIST` | none |
| `GET` | `/thumbnails/{fileName}` | Send `THUMBNAIL RETRIEVE` | none |
| `POST` | `/thumbnails/{fileName}/generate` | Send `THUMBNAIL GENERATE` | none |
| `POST` | `/thumbnails/generate-all` | Send `THUMBNAIL GENERATE_ALL` | none |
| `GET` | `/events` | Stream live Caspar events over SSE | none |

Every route in the table is also available under `/servers/{name}/...` so a single REST host can target multiple named CasparCG clients. For example:

```text
GET /servers/studio-a/server/version
POST /servers/studio-b/channels/1/layers/10/play
```

That includes the per-server health route:

```text
GET /servers/studio-a/health
```

## Request Contracts

### Play

```json
{
  "clip": "AMB",
  "options": {
    "transition": {
      "kind": "mix",
      "duration": 10,
      "tweener": "linear"
    },
    "loop": false,
    "seek": 12,
    "length": 24,
    "filter": "hflip",
    "clearOn404": true
  }
}
```

`options.transition` is typed and serializes to the AMCP transition tail. Supported transition kinds include `cut`, `mix`, `push`, `slide`, `wipe`, `fadeCut`, `cutFade`, `vFade`, and `sting`.

`loop`, `seek`, `length`, `filter`, and `clearOn404` map directly to the corresponding AMCP playback options.

### Load

```json
{
  "clip": "AMB",
  "options": {
    "transition": {
      "kind": "slide",
      "duration": 10,
      "tweener": "linear",
      "direction": "left"
    },
    "seek": 12,
    "length": 24,
    "filter": "hflip",
    "clearOn404": true
  }
}
```

### Load background

```json
{
  "clip": "BG",
  "options": {
    "transition": {
      "kind": "mix",
      "duration": 10
    },
    "loop": true,
    "seek": 100,
    "length": 200,
    "filter": "hflip",
    "clearOn404": true,
    "autoPlay": true
  }
}
```

`LOAD`, `PLAY`, and `LOADBG` all use the typed `options.transition` object rather than a raw `additionalParameters` string.

### Channel consumer commands

`ADD` adds a consumer to the channel:

```json
{
  "consumer": "FILE",
  "arguments": "filename.mov",
  "consumerIndex": 700
}
```

`REMOVE` accepts either a consumer index override or the raw consumer arguments:

```json
{
  "consumerIndex": 700
}
```

```json
{
  "arguments": "FILE filename.mov"
}
```

`APPLY` can target an optional layer and passes a raw argument tail:

```json
{
  "layer": 10,
  "arguments": "LOWER"
}
```

`PRINT` has no body.

`SET` uses the same simple key/value shape as the AMCP command:

```json
{
  "key": "MODE",
  "value": "FAST"
}
```

### Swap

```json
{
  "otherChannel": 2,
  "otherLayer": 20,
  "swapTransforms": true
}
```

### Set

```json
{
  "key": "MODE",
  "value": "FAST"
}
```

### Mixer opacity

```json
{
  "value": 0.5
}
```

The same single-value shape is used by:

- `mixer/keyer`
- `mixer/invert`
- `mixer/blend`
- `mixer/brightness`
- `mixer/saturation`
- `mixer/contrast`
- `mixer/volume`
- `/mixer/master-volume`

### Free-form mixer commands

Routes such as `mixer/chroma`, `mixer/levels`, `mixer/fill`, `mixer/clip`, `mixer/anchor`, `mixer/crop`, `mixer/rotation`, `mixer/perspective`, and `mixer/grid` accept:

```json
{
  "arguments": "0 0 1 1"
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

### Log level

```json
{
  "value": "debug"
}
```

### Lock acquire

```json
{
  "phrase": "phrase"
}
```

### Lock clear

```json
{
  "overridePhrase": "override"
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

### `GET /media/files/{fileName}`

Returns the parsed `CINF` payload, including preserved extra properties.

### `GET /fonts`

Returns the parsed `FLS` payload.

### `GET /templates`

Returns the parsed `TLS` payload.

### `GET /server/info*`

`/server/info`, `/server/info/config`, `/server/info/paths`, and `/server/gl/info` return the parsed `QueryDataMap` payload with:

- flattened `values`
- original `lines`
- raw `raw` response text

### `GET /data`

Returns the `DATA LIST` payload lines as a JSON array of strings.

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

For AMCP command failures, the REST status follows the failure class and the original AMCP diagnostics are preserved in `ProblemDetails.extensions`:

| Extension | Meaning |
| --- | --- |
| `amcpStatusCode` | Raw AMCP numeric status code |
| `amcpStatusLine` | Full AMCP status line |
| `amcpCommandText` | AMCP command text reported by the server |
| `amcpCategory` | AMCP category, for example `ClientError` or `ServerError` |

Current status mapping:

| AMCP / REST | Meaning |
| --- | --- |
| `400` -> `400` | AMCP command not understood, invalid channel, parameter missing, or invalid parameter |
| `404` -> `404` | AMCP file not found |
| `500` / `501` / `502` -> `502` | AMCP server-side failure or file read issue |
| `503` -> `403` | AMCP access denied |
| `504` -> `429` | AMCP queue overflow |
| `600` -> `501` | AMCP not implemented |
| transport I/O -> `502` | Upstream connection failed while talking to CasparCG |
| client unavailable -> `503` | Client could not be connected or was stopped |

## Current Limits

- No WebSocket endpoint
- Thumbnail retrieval currently returns `application/octet-stream` without image type sniffing
