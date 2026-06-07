# OSC Event Coverage With REST SSE

## Goal

Expand OSC event coverage so the client no longer only recognizes active clip name changes. The implementation should preserve raw CasparCG OSC monitor updates, expose useful typed domain events, stream those events through the existing REST SSE endpoint, and document the .NET and SSE contracts.

## Current State

CasparCG server serializes its `monitor::state` as OSC bundles under `/channel/{channel}/...`. A channel frame can include stage, layer, foreground and background producer state, mixer state, output state, format, and framerate. Individual producers add fields such as `file/name`, `file/path`, `file/time`, `file/clip`, `loop`, `producer`, `paused`, and `frames_left`.

StartDust currently parses OSC bundles and maps only:

- `/channel/{channel}/stage/layer/{layer}/background/file/name`
- `/channel/{channel}/stage/layer/{layer}/foreground/file/name`

Those addresses become `PlaybackClipChangedEvent`. The current parser handles `string`, `int32`, `float`, and OSC boolean tags, but CasparCG can emit `int64` and `double`. If an unsupported tag appears in a bundle, the current client can discard the whole bundle before mappable messages are published.

The ASP.NET Core addon already exposes `GET /events` as Server-Sent Events over `client.Events`. The SSE layer does not need a new transport, but it needs stable event names for the new event types and documentation for their payloads.

## Approach

Use a layered event model:

1. Parse CasparCG OSC messages broadly enough that normal monitor bundles do not fail on common numeric types.
2. Publish a generic `OscStateChangedEvent` for every parsed OSC monitor message that targets a CasparCG channel.
3. Publish selected typed events for high-value state changes.
4. Stream both raw and typed events through the existing REST SSE endpoint using stable names.

This gives complete raw visibility while keeping typed events focused and practical. It avoids trying to model the entire CasparCG monitor tree in one step.

## Core OSC Parsing

`OscPacketParser` should add support for:

- `h` as `long` / `Int64`
- `d` as `double`

The parser should continue to support bundles and existing tags. Unknown tags should not prevent all known messages in a bundle from being delivered. The implementation can either skip unsupported messages when their payload length is knowable or isolate per-message parse failures inside bundle parsing. The important behavior is that one unsupported OSC message does not drop unrelated `file/name`, `producer`, or progress messages from the same bundle.

Malformed packets can still be ignored at the packet level, matching current behavior.

## Event Model

Add a generic event:

```csharp
public sealed record OscStateChangedEvent(
    string ClientName,
    int Channel,
    string Path,
    IReadOnlyList<object?> Arguments)
    : CasparEvent(ClientName, Channel, Layer: 0);
```

`Path` is the address after `/channel/{channel}/`, for example:

- `stage/layer/10/foreground/file/name`
- `stage/layer/10/foreground/file/time`
- `format`
- `framerate`

`Layer` remains `0` for raw channel-level OSC events because the current base event shape requires a layer. Typed layer events should use the actual layer.

Keep the existing event:

- `PlaybackClipChangedEvent`

Add typed layer events:

```csharp
public sealed record LayerProducerChangedEvent(
    string ClientName,
    int Channel,
    int Layer,
    string Slot,
    string Producer)
    : CasparEvent(ClientName, Channel, Layer);

public sealed record LayerPausedChangedEvent(
    string ClientName,
    int Channel,
    int Layer,
    bool Paused)
    : CasparEvent(ClientName, Channel, Layer);

public sealed record LayerProgressChangedEvent(
    string ClientName,
    int Channel,
    int Layer,
    string Slot,
    double PositionSeconds,
    double DurationSeconds)
    : CasparEvent(ClientName, Channel, Layer);

public sealed record LayerFramesLeftChangedEvent(
    string ClientName,
    int Channel,
    int Layer,
    long FramesLeft)
    : CasparEvent(ClientName, Channel, Layer);
```

`Slot` should be `foreground` or `background` for slot-specific events. `LayerPausedChangedEvent` maps only foreground pause state because CasparCG exposes `paused` on the foreground state in the current server code.

## OSC Mapping

`DefaultOscMessageMapper` should map every channel-scoped message to `OscStateChangedEvent`.

It should additionally map:

| OSC path pattern | Typed event |
| --- | --- |
| `stage/layer/{layer}/{slot}/file/name` | `PlaybackClipChangedEvent` |
| `stage/layer/{layer}/{slot}/producer` | `LayerProducerChangedEvent` |
| `stage/layer/{layer}/foreground/paused` | `LayerPausedChangedEvent` |
| `stage/layer/{layer}/{slot}/file/time` with two numeric args | `LayerProgressChangedEvent` |
| `stage/layer/{layer}/foreground/frames_left` | `LayerFramesLeftChangedEvent` |

The mapper should not create typed events unless the address and argument types match exactly enough to avoid misleading payloads. The raw event still preserves unmatched messages.

Because one OSC message can produce both a raw event and a typed event, the mapper interface should evolve from returning a single optional event to returning zero or more events. Existing custom mapper compatibility can be preserved by adding an adapter or a default interface method if practical.

## State Projection

`CasparStateStore` should continue to support the existing clip snapshot. It can be extended conservatively:

- keep `LayerStateSnapshot.Clip`
- add optional `Producer`, `Paused`, `PositionSeconds`, `DurationSeconds`, and `FramesLeft`

The state store should apply typed layer events. It does not need to project every `OscStateChangedEvent` in this phase.

## REST SSE Contract

`GET /events` remains the only SSE endpoint. It continues to stream events from `client.Events`.

Add stable SSE names:

| .NET event | SSE event |
| --- | --- |
| `OscStateChangedEvent` | `oscStateChanged` |
| `PlaybackClipChangedEvent` | `playbackClipChanged` |
| `LayerProducerChangedEvent` | `layerProducerChanged` |
| `LayerPausedChangedEvent` | `layerPausedChanged` |
| `LayerProgressChangedEvent` | `layerProgressChanged` |
| `LayerFramesLeftChangedEvent` | `layerFramesLeftChanged` |

The SSE envelope remains unchanged:

```json
{
  "type": "oscStateChanged",
  "timestamp": "2026-06-07T12:00:00+00:00",
  "target": "default",
  "payload": {}
}
```

This preserves compatibility for existing SSE consumers while adding new event names.

## Documentation

Update:

- `docs/vnext/events-and-state.md`
- `docs/vnext/rest-api-addon.md`
- `README.md`

The docs should describe:

- raw OSC fallback through `OscStateChangedEvent`
- typed layer events
- SSE event names
- example SSE frames for raw and typed events
- current limits: not every CasparCG monitor key has a typed event yet

## Testing

Add unit tests for:

- OSC parser support for `h` and `d`
- bundle parsing where a supported message still publishes when another message is unsupported or ignored
- `OscStateChangedEvent` mapping for channel-scoped messages
- typed mapping for producer, paused, progress, frames-left, and clip events
- state projection updates from typed events

Add integration or ASP.NET Core tests for:

- SSE `event: oscStateChanged`
- SSE event names for each new typed event
- JSON envelope shape remains `type`, `timestamp`, `target`, and `payload`

Run the existing .NET test suite after implementation.

## Non-Goals

- Do not add a new REST endpoint for OSC events.
- Do not model every CasparCG monitor key as a typed event in this phase.
- Do not change the existing `GET /events` envelope shape.
- Do not remove `PlaybackClipChangedEvent` or change its SSE event name.

## Compatibility Decision

Changing `IOscMessageMapper` from a single-event mapper to a multi-event mapper is useful for raw-plus-typed publishing. The implementation should minimize public API breakage. If a source-compatible default interface method is not suitable for the target framework, add a new internal adapter and keep the current public interface behavior for custom mappers.
