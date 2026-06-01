# Breaking changes from the current release

## Public API redesign

- `ICasparDevice` is replaced by `CasparClient`
- synchronous public command methods are removed
- DI registration now starts with `AddCasparCG()` fluent configuration
- OSC domain notifications now flow through `client.Events`
- legacy manager-centric navigation is replaced by fluent channel/layer command scopes

## Migration example

### Before

```csharp
services.AddCasparCG();
var device = provider.GetRequiredService<ICasparDevice>();
device.Connect("127.0.0.1");
```

### After

```csharp
services.AddCasparCG()
    .ConnectTo("127.0.0.1", 5250)
    .ListenOscOn(6250);

var client = provider.GetRequiredService<CasparClient>();
await client.ConnectAsync(ct);
```
