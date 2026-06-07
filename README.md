# StarDust.CasparCG.net

`StartDust.CasparCG.net` is now centered on a vNext API built around a single `CasparClient`, fluent DI registration, async-only command execution, unified event streaming, and testable transports.

The active solution now contains only the maintained vNext projects under `src/StarDust.CasparCG*`. Legacy runtime, legacy tests, and legacy demos have been removed from the repository.

## Quick start

```csharp
builder.Services
    .AddCasparCG()
    .ConnectTo("127.0.0.1", 5250)
    .ListenOscOn(6250);

var client = app.Services.GetRequiredService<CasparClient>();
// Optional warm-up: the first AMCP call also connects lazily.
await client.ConnectAsync(ct);
await client.PlayAsync(1, 10, "AMB", ct);
```

## Fluent commands

```csharp
await client
    .Channel(1)
    .Layer(10)
    .Play("AMB")
    .WithTransition().Mix(12)
    .WithLoop()
    .SendAsync(ct);
```

```csharp
var version = await client.Server().VersionAsync(ct);
await client.Admin().RestartAsync(ct);
```

```csharp
await client
    .Channel(1)
    .Layer(10)
    .Sequence()
    .LoadBg("AMB").Loop().Then()
    .Wait(500).Then()
    .Play("AMB")
    .SendAsync(ct);
```

```csharp
await client.Parallel(
    client.Channel(1).Layer(10).Sequence()
        .LoadBg("AMB").Loop().Then()
        .Play("AMB"),
    client.Channel(2).Layer(20).Sequence()
        .Wait(250).Then()
        .Play("SAMPLE-1"))
    .SendAsync(ct);
```

## Events and state

```csharp
await foreach (var evt in client.Events.ForChannel(1).ReadAllAsync(ct))
{
    Console.WriteLine(evt);
}

var snapshot = client.State.GetSnapshot();
```

OSC monitor messages are available as raw `OscStateChangedEvent` values and selected typed events:

| Event | Meaning |
| --- | --- |
| `OscStateChangedEvent` | Raw channel-scoped OSC monitor update |
| `PlaybackClipChangedEvent` | Foreground or background clip name changed |
| `LayerProducerChangedEvent` | Foreground or background producer name changed |
| `LayerPausedChangedEvent` | Foreground pause state changed |
| `LayerProgressChangedEvent` | Foreground or background media progress changed |
| `LayerFramesLeftChangedEvent` | Foreground autoplay frames-left changed |

## Repository layout

- `src/StarDust.CasparCG`: public client API, fluent commands, AMCP/OSC protocol primitives, and transport implementations
- `src/StarDust.CasparCG.Hosting`: `AddCasparCG` registration and named clients
- `test/StarDust.CasparCG.Testing`: `DummyServer` helpers for integration tests
- `test/StarDust.CasparCG.UnitTests`: unit coverage for commands, events, parsing, and hosting
- `test/StarDust.CasparCG.IntegrationTests`: transport and client integration coverage

## AMCP coverage

| Family | Coverage |
| --- | --- |
| Basic playback | `PLAY`, `LOADBG`, `LOAD`, `STOP`, `PAUSE`, `RESUME`, `CLEAR`, `CALL`, `CALLBG`, `SWAP`, `ADD`, `REMOVE`, `APPLY`, `PRINT`, `CLEAR ALL`, `SET` |
| Query | `VERSION`, `INFO`, `INFO CONFIG`, `INFO PATHS`, `CINF`, `CLS`, `FLS`, `TLS`, `GL INFO`, `GL GC` |
| Data | `DATA STORE`, `DATA RETRIEVE`, `DATA LIST`, `DATA REMOVE` |
| Template / CG | `CG ADD`, `CG PLAY`, `CG STOP`, `CG NEXT`, `CG REMOVE`, `CG CLEAR`, `CG UPDATE`, `CG INVOKE` |
| Thumbnail | `THUMBNAIL LIST`, `THUMBNAIL RETRIEVE`, `THUMBNAIL GENERATE`, `THUMBNAIL GENERATE_ALL` |
| Mixer | `MIXER KEYER`, `MIXER INVERT`, `MIXER CHROMA`, `MIXER BLEND`, `MIXER OPACITY`, `MIXER BRIGHTNESS`, `MIXER SATURATION`, `MIXER CONTRAST`, `MIXER LEVELS`, `MIXER FILL`, `MIXER CLIP`, `MIXER ANCHOR`, `MIXER CROP`, `MIXER ROTATION`, `MIXER PERSPECTIVE`, `MIXER VOLUME`, `MIXER MASTERVOLUME`, `MIXER GRID`, `MIXER COMMIT`, `MIXER CLEAR`, `CHANNEL_GRID` |
| Runtime / admin | `OSC SUBSCRIBE`, `OSC UNSUBSCRIBE`, `DIAG`, `BYE`, `KILL`, `RESTART`, `LOCK` |

## REST and SSE proxy

Use `StarDust.CasparCG.AspNetCore` to expose a modern HTTP and SSE layer over the client:

```csharp
builder.Services
    .AddCasparCG()
    .ConnectTo("127.0.0.1", 5250)
    .ListenOscOn(6250);

builder.Services.AddCasparCGRestApi();

var app = builder.Build();
// Optional warm-up: the first AMCP call also connects lazily.
await app.Services.GetRequiredService<CasparClient>().ConnectAsync();
app.MapCasparCGApi();
```

Current route surface:

| Method | Route | Purpose |
| --- | --- | --- |
| `GET` | `/server/version` | Read the server version |
| `GET` | `/media/files` | List media files |
| `GET` | `/data/{key}` | Read a data payload |
| `PUT` | `/data/{key}` | Store a data payload |
| `POST` | `/channels/{channel}/layers/{layer}/play` | Trigger playback |
| `POST` | `/channels/{channel}/layers/{layer}/loadbg` | Load a background clip |
| `POST` | `/channels/{channel}/layers/{layer}/pause` | Pause playback |
| `POST` | `/channels/{channel}/layers/{layer}/resume` | Resume playback |
| `POST` | `/channels/{channel}/layers/{layer}/stop` | Stop playback |
| `POST` | `/channels/{channel}/layers/{layer}/mixer/opacity` | Change layer opacity |
| `POST` | `/channels/{channel}/layers/{layer}/cg/add` | Add a CG template |
| `POST` | `/channels/{channel}/layers/{layer}/cg/play` | Play the active CG template |
| `POST` | `/channels/{channel}/layers/{layer}/cg/stop` | Stop the active CG template |
| `POST` | `/channels/{channel}/layers/{layer}/cg/next` | Advance the active CG template |
| `POST` | `/channels/{channel}/layers/{layer}/cg/remove` | Remove the active CG template |
| `POST` | `/channels/{channel}/layers/{layer}/cg/clear` | Clear all CG templates on the layer |
| `POST` | `/channels/{channel}/layers/{layer}/cg/update` | Update the active CG template data |
| `POST` | `/channels/{channel}/layers/{layer}/cg/invoke` | Invoke a CG template method |
| `POST` | `/admin/restart` | Restart the server |
| `GET` | `/thumbnails` | List generated thumbnails |
| `GET` | `/thumbnails/{fileName}` | Retrieve one thumbnail payload |
| `POST` | `/thumbnails/{fileName}/generate` | Generate one thumbnail |
| `POST` | `/thumbnails/generate-all` | Generate all thumbnails |
| `GET` | `/events` | Stream SSE events |

Detailed request and response examples live in [REST API addon](docs/vnext/rest-api-addon.md).

## Additional guides

- [Breaking changes](BREAKING_CHANGES.md)
- [Contributing](CONTRIBUTING.md)
- [Getting started](docs/vnext/getting-started.md)
- [Fluent API cookbook](docs/vnext/fluent-api-cookbook.md)
- [Events and state](docs/vnext/events-and-state.md)
- [Hosting and DI](docs/vnext/hosting-and-di.md)
- [REST API addon](docs/vnext/rest-api-addon.md)
- [Testing with DummyServer](docs/vnext/testing-with-dummy-server.md)
- [AMCP Protocol specification](https://casparcg.com/docs/wiki/protocols/amcp-protocol)
- [OSC Protocol specification](https://casparcg.com/docs/wiki/protocols/osc-protocol)
