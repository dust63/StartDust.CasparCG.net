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

To expose multiple configured clients through the REST addon, register them by name and call routes under `/servers/{name}/...`. For example, `AddCasparCG("studio-a")` can be addressed with `GET /servers/studio-a/server/version`.

You can also load named clients from configuration:

```json
{
  "CasparCG": {
    "Clients": {
      "default": {
        "AmcpHost": "127.0.0.1",
        "AmcpPort": 5250,
        "OscPort": 6250
      },
      "studio-a": {
        "AmcpHost": "10.0.0.10",
        "AmcpPort": 5251,
        "OscPort": 6251
      }
    }
  }
}
```

Then call `services.AddCasparCGFromConfiguration(configuration);` before `AddCasparCGRestApi()`.

See [REST API addon](rest-api-addon.md) for the full route table, request payloads, SSE format, and current limits.
