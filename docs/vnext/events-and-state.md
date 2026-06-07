# Events and state

`CasparClient.Events` exposes domain events from AMCP workflows and OSC monitor updates. OSC messages are published both as raw state events and, for common layer paths, typed events.

## Event types

| Event | Source |
| --- | --- |
| `OscStateChangedEvent` | Raw `/channel/{channel}/...` OSC monitor message |
| `PlaybackClipChangedEvent` | `stage/layer/{layer}/{slot}/file/name` |
| `LayerProducerChangedEvent` | `stage/layer/{layer}/{slot}/producer` |
| `LayerPausedChangedEvent` | `stage/layer/{layer}/foreground/paused` |
| `LayerProgressChangedEvent` | `stage/layer/{layer}/{slot}/file/time` |
| `LayerFramesLeftChangedEvent` | `stage/layer/{layer}/foreground/frames_left` |

`OscStateChangedEvent` is the fallback for channel-scoped OSC paths that do not have a typed event yet. This keeps the raw monitor stream observable while typed coverage grows over time.

## Consume filtered typed events

```csharp
await foreach (var evt in client.Events
    .ForChannel(1)
    .OfType<PlaybackClipChangedEvent>()
    .ReadAllAsync(ct))
{
    Console.WriteLine(evt.Clip);
}
```

## Consume raw OSC state

```csharp
await foreach (var evt in client.Events
    .ForChannel(1)
    .OfType<OscStateChangedEvent>()
    .ReadAllAsync(ct))
{
    Console.WriteLine($"{evt.Path}: {string.Join(", ", evt.Arguments)}");
}
```

## Read the current projection

```csharp
var snapshot = client.State.GetSnapshot();
var currentClip = snapshot.Channels[1].Layers[10].Clip;
var currentProducer = snapshot.Channels[1].Layers[10].Producer;
var currentPosition = snapshot.Channels[1].Layers[10].PositionSeconds;
```

The projection tracks the latest typed layer values: clip, producer, pause state, media position, media duration, and frames left. It does not project every raw `OscStateChangedEvent`.
