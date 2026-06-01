# Events and state

## Consume filtered events

```csharp
await foreach (var evt in client.Events
    .ForChannel(1)
    .OfType<PlaybackClipChangedEvent>()
    .ReadAllAsync(ct))
{
    Console.WriteLine(evt.Clip);
}
```

## Read the current projection

```csharp
var snapshot = client.State.GetSnapshot();
var currentClip = snapshot.Channels[1].Layers[10].Clip;
```
