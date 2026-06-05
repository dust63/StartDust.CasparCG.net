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
