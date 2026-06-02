# StarDust.CasparCG.net

`StartDust.CasparCG.net` is evolving toward a vNext API centered on a single `CasparClient`, fluent DI registration, async-only command execution, unified event streaming, and testable transports.

The legacy projects remain in the repository for compatibility and migration work. The new phase-one vNext slice lives under `src/StarDust.CasparCG*`.

## Quick start

```csharp
builder.Services
    .AddCasparCG()
    .ConnectTo("127.0.0.1", 5250)
    .ListenOscOn(6250);

var client = app.Services.GetRequiredService<CasparClient>();
await client.ConnectAsync(ct);
await client.PlayAsync(1, 10, "AMB", ct);
```

## Fluent commands

```csharp
await client
    .Channel(1)
    .Layer(10)
    .Play("AMB")
    .WithTransition().Mix(12)
    .WithLoop()
    .SendAsync(ct);
```

## Events and state

```csharp
await foreach (var evt in client.Events.ForChannel(1).ReadAllAsync(ct))
{
    Console.WriteLine(evt);
}

var snapshot = client.State.GetSnapshot();
```

## Repository layout

- `src/StarDust.CasparCG`: public client API and fluent command surface
- `src/StarDust.CasparCG.Hosting`: `AddCasparCG` registration and named clients
- `src/StarDust.CasparCG.Protocol.*`: protocol-specific AMCP and OSC primitives
- `src/StarDust.CasparCG.Transport`: transport abstractions and TCP transport
- `src/StarDust.CasparCG.Testing`: `DummyServer` helpers for integration tests

## Additional guides

- [Breaking changes](BREAKING_CHANGES.md)
- [Contributing](CONTRIBUTING.md)
- [Getting started](docs/vnext/getting-started.md)
- [Fluent API cookbook](docs/vnext/fluent-api-cookbook.md)
- [Events and state](docs/vnext/events-and-state.md)
- [Hosting and DI](docs/vnext/hosting-and-di.md)
- [Testing with DummyServer](docs/vnext/testing-with-dummy-server.md)
- [AMCP Protocol specification](https://casparcg.com/docs/wiki/protocols/amcp-protocol)
- [OSC Protocol specification](https://casparcg.com/docs/wiki/protocols/osc-protocol)
