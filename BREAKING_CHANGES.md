# Breaking changes from the current release

## Public API redesign

- `ICasparDevice` is replaced by `CasparClient`
- synchronous public command methods are removed
- DI registration now starts with `AddCasparCG()` fluent configuration
- OSC domain notifications now flow through `client.Events`
- legacy manager-centric navigation is replaced by fluent channel/layer command scopes

## Active solution cleanup

- the active `src/StarDust.CasparCG.net.sln` solution no longer includes the legacy `StarDust.CasparCG.net.*`, `StartDust.CasparCG.net.*`, or demo projects
- the legacy runtime, tests, and demos have been removed from the repository
- AMCP, OSC, and transport code now live inside `src/StarDust.CasparCG` instead of separate runtime projects

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
