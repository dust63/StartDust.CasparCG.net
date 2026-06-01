# Getting started

## Register the client

```csharp
builder.Services
    .AddCasparCG()
    .ConnectTo("127.0.0.1", 5250)
    .ListenOscOn(6250);
```

## Resolve and connect

```csharp
var client = app.Services.GetRequiredService<CasparClient>();
await client.ConnectAsync(ct);
```

## Send a command

```csharp
await client.PlayAsync(1, 10, "AMB", ct);
```
