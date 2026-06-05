# Fluent API cookbook

## Play with transition and loop

```csharp
await client
    .Channel(1)
    .Layer(10)
    .Play("AMB")
    .WithTransition().Mix(12)
    .WithLoop()
    .SendAsync(ct);
```

## Direct async methods

```csharp
await client.LoadBackgroundAsync(1, 10, "AMB", ct);
await client.StopAsync(1, 10, ct);
```

## Global scopes

```csharp
var version = await client.Server().VersionAsync(ct);
var media = await client.Server().MediaFilesAsync(ct);
var paths = await client.Server().InfoPathsAsync(ct);
var fonts = await client.Server().FontFilesAsync(ct);

await client.Admin().RestartAsync(ct);
```

## Local layer orchestration

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

## Parallel layer orchestration

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

## Structure

The fluent surface, protocol primitives, and transports are all maintained under `src/StarDust.CasparCG`.
