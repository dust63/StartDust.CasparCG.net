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
