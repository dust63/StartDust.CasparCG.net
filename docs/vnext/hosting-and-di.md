# Hosting and DI

## Default client

```csharp
services.AddCasparCG()
    .ConnectTo("127.0.0.1", 5250)
    .ListenOscOn(6250);
```

## Named client

```csharp
services.AddCasparCG("studio-a")
    .ConnectTo("10.0.0.10", 5250)
    .ListenOscOn(6250);

var client = provider.GetRequiredService<ICasparClientFactory>().GetClient("studio-a");
```

## Auto reconnect flag

```csharp
services.AddCasparCG()
    .ConnectTo("127.0.0.1", 5250)
    .WithAutoReconnect();
```

## REST addon

```csharp
builder.Services
    .AddCasparCG()
    .ConnectTo("127.0.0.1", 5250)
    .ListenOscOn(6250);

builder.Services.AddCasparCGRestApi();

var app = builder.Build();
await app.Services.GetRequiredService<CasparClient>().ConnectAsync();
app.MapCasparCGApi();
```

The addon exposes resource-oriented HTTP routes such as `GET /server/version`, `POST /channels/{channel}/layers/{layer}/play`, `POST /channels/{channel}/layers/{layer}/cg/add`, `GET /thumbnails`, and `GET /events` for SSE streaming.

See [REST API addon](rest-api-addon.md) for the full route table, request payloads, SSE format, and current limits.
