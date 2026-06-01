# CasparCG vNext Design

- Date: 2026-06-02
- Project: `StartDust.CasparCG.net`
- Status: Draft validated in discussion

## 1. Intent

This design defines a vNext modernization of the library around `.NET 10`, modern hosting patterns, stronger robustness, lower operational complexity, and a much simpler public API.

The core product goal is to make CasparCG control feel obvious:

- a single high-level client
- fluent registration and fluent command building
- async-first API only
- first-class AMCP, OSC, and domain event support
- native integration with `Microsoft.Extensions.DependencyInjection`, `ILogger`, and hosted applications

This is a breaking redesign. Backward compatibility is not a primary goal for the runtime architecture.

## 2. Product Goals

The vNext release must optimize for the following:

- target `.NET 10`
- make the default experience trivial for a single CasparCG server
- support multiple servers cleanly through named clients or a factory
- expose a single main entry point: `CasparClient`
- use fluent configuration for DI registration
- use fluent methods for high-level command composition
- be async-first with `CancellationToken` on all networked operations
- provide strong built-in reconnect, recovery, health, and diagnostics
- unify AMCP, OSC, and domain events in one consistent runtime model
- make future extensions for WPF and ASP.NET Core / SignalR straightforward

## 3. Non-Goals

The first vNext phase does not aim to:

- preserve binary compatibility with the legacy API
- preserve legacy internal types such as managers, low-level connection abstractions, or the current event hub as architectural anchors
- depend on Docker-based integration tests in phase 1
- ship UI-specific or web-specific dependencies in the core package

## 4. Public Product Shape

### 4.1 Main entry point

The public API is centered on a single high-level type:

```csharp
CasparClient
```

This client owns the runtime lifecycle, network orchestration, event publication, health model, and access to fluent command builders.

### 4.2 Fluent DI registration

DI configuration is fluent-first rather than property-bag-first:

```csharp
builder.Services
    .AddCasparCG()
    .ConnectTo("127.0.0.1", 5250)
    .ListenOscOn(6250)
    .WithAutoReconnect();
```

Named client setup must also be supported:

```csharp
builder.Services
    .AddCasparCG("studio-a")
    .ConnectTo("10.0.0.10", 5250)
    .ListenOscOn(6250);
```

For multi-server scenarios, the runtime must expose a factory-based access pattern:

```csharp
var client = factory.GetClient("studio-a");
```

### 4.3 Trivial mono-server usage

The easiest path must stay short:

```csharp
var client = services.GetRequiredService<CasparClient>();
await client.ConnectAsync(ct);
await client.PlayAsync(channel: 1, layer: 10, clip: "AMB", ct);
```

### 4.4 Fluent command usage

The API must also support high-level fluent composition:

```csharp
await client
    .Channel(1)
    .Layer(10)
    .Play("AMB")
    .WithTransition().Mix(12)
    .WithLoop()
    .SendAsync(ct);
```

The fluent chain must build a validated immutable command intent locally before network execution.

This allows:

- local validation before send
- easier testing
- predictable behavior
- fewer hidden side effects

## 5. Runtime Model

### 5.1 Async-first only

All networked public operations are asynchronous and take a `CancellationToken`.

The API intentionally avoids public synchronous wrappers. This reduces ambiguity, avoids blocking pitfalls, and aligns with modern `.NET` hosting and background processing patterns.

### 5.2 Single-client public abstraction

`CasparClient` is the only abstraction a normal consumer should need to understand.

Internally, the runtime may decompose into transport, protocol, parsing, health supervision, and state projection layers, but those concerns must not leak into basic usage.

### 5.3 State and event separation

The runtime exposes two complementary read models:

- an event stream for live reactions
- a state view for simple reads

Example:

```csharp
await foreach (var evt in client.Events.ReadAllAsync(ct))
{
}
```

```csharp
var snapshot = client.State.GetSnapshot();
var channel = client.State.Channel(1);
var layer = client.State.Channel(1).Layer(10);
```

The event stream answers "what is happening now". The state view answers "what do we currently know".

## 6. Event Model

### 6.1 Main approach

The primary event model for OSC and domain activity is stream-based, not a large public surface of `.NET` events.

The recommended public abstraction is:

- `IAsyncEnumerable<CasparEvent>` for consumption
- a unified event source exposed by `client.Events`

Example:

```csharp
await foreach (var evt in client.Events.ReadAllAsync(ct))
{
    // PlaybackClipChangedEvent, LayerPausedChangedEvent, etc.
}
```

### 6.2 Typed fluent filtering

The event source must support fluent narrowing:

```csharp
await foreach (var evt in client.Events
    .ForChannel(1)
    .ForLayer(10)
    .OfType<PlaybackClipTimeChangedEvent>()
    .ReadAllAsync(ct))
{
}
```

### 6.3 Lifecycle events

Rare lifecycle changes may still use classic `.NET` events or equivalent lightweight notification primitives, for example:

- `Connected`
- `Disconnected`
- `Reconnected`
- `Faulted`
- `StateChanged`

These remain useful because they are low-volume and easy to understand.

### 6.4 Raw access

The runtime should expose a raw stream for diagnostics and advanced consumers:

- `client.RawOscMessages`

The main product experience remains domain-oriented, not protocol-oriented.

### 6.5 Internal transport-to-event pipeline

Internally, AMCP and OSC are parsed and translated into domain events before they reach the public event stream.

The runtime should use a bounded internal pipeline, for example a `Channel<CasparEvent>`, to control backpressure and memory.

## 7. Health, Recovery, and Diagnostics

### 7.1 Built-in connection supervision

The client owns its own connection supervisor for both AMCP and OSC.

It is responsible for:

- initial connection
- failure detection
- reconnect attempts
- backoff strategy
- coordinated transport restart
- post-reconnect recovery steps

### 7.2 Health model

The runtime must expose an explicit health state, for example:

- `Disconnected`
- `Connecting`
- `Connected`
- `Degraded`
- `Reconnecting`
- `Faulted`
- `Stopping`

This state must be directly observable by consumers.

### 7.3 Recovery model

After reconnect, the runtime should restore its own runtime capabilities, including:

- reopening transports
- restoring internal listeners
- reactivating parsing pipelines
- rebuilding minimal live state when possible
- publishing recovery lifecycle notifications

### 7.4 Heartbeat and silence detection

The runtime must detect:

- AMCP response timeouts
- unexpected OSC silence
- half-connected states
- slow or stalled server behavior

### 7.5 Diagnostics surface

The runtime should expose a diagnostics snapshot such as:

- last successful AMCP interaction
- last OSC message received
- last failure
- reconnect count
- current degradation reason
- uptime or connection duration

### 7.6 Logging

The design uses `ILogger` rather than custom logging infrastructure.

The following should be loggable:

- connection transitions
- reconnect attempts
- timeouts
- dropped events
- parsing failures
- saturation signals
- recovery completion

## 8. Memory and Concurrency Strategy

The vNext runtime must explicitly avoid unbounded in-memory growth.

### 8.1 Bounded buffering

Live event transport uses bounded buffering. Policies may vary by stream, but must be explicit and testable.

For some high-frequency event categories, the runtime may prefer "latest wins" semantics instead of preserving all intermediate values.

### 8.2 No hidden thread chaos

The runtime owns its background loops, cancellation boundaries, and event publication mechanics.

Consumers should not have to manage hidden event threads or manually orchestrate low-level concurrency concerns.

### 8.3 Low-allocation posture

The implementation should prefer:

- stable runtime loops
- bounded queues
- predictable object lifetimes
- minimizing repeated parsing allocations where practical

The first implementation does not need micro-optimized code everywhere, but the architecture must not force high allocation churn.

## 9. Extensibility for WPF and ASP.NET Core

The chosen event model is intentionally extension-friendly.

### 9.1 WPF

A WPF extension can consume `IAsyncEnumerable<CasparEvent>` and project it into:

- `INotifyPropertyChanged`
- `ObservableCollection`
- dispatcher-aware view model updates

### 9.2 ASP.NET Core / SignalR

A hosted web extension can consume the same event stream and republish to SignalR hubs from a background service.

This allows the core package to remain clean while enabling UI and web adapters later.

## 10. Internal Package Layout

The internal architecture should be modular even though the public API is simple.

Recommended package split:

- `StarDust.CasparCG`
- `StarDust.CasparCG.Hosting`
- `StarDust.CasparCG.Protocol.Amcp`
- `StarDust.CasparCG.Protocol.Osc`
- `StarDust.CasparCG.Transport`
- `StarDust.CasparCG.Testing`

Expected responsibilities:

- `StarDust.CasparCG`: public client, commands, events, state, abstractions
- `StarDust.CasparCG.Hosting`: DI registration, named clients, hosting integration
- `StarDust.CasparCG.Protocol.Amcp`: command building and response parsing
- `StarDust.CasparCG.Protocol.Osc`: message intake and mapping to domain events
- `StarDust.CasparCG.Transport`: AMCP/OSC transport lifecycle and low-level I/O
- `StarDust.CasparCG.Testing`: fake transports, scripted scenarios, `DummyServer`

Consumers should ideally depend only on:

- `StarDust.CasparCG`
- optionally `StarDust.CasparCG.Hosting`

## 11. Testing Strategy

### 11.1 Test pyramid

Phase 1 validation is based on:

- unit tests
- integration tests with an official `DummyServer`
- scenario tests for reconnect, health, and recovery

### 11.2 Unit tests

Unit tests cover:

- fluent builders
- local validation
- AMCP parsing
- OSC parsing and mapping
- state projection
- health transitions

### 11.3 DummyServer

The repository will include an official `DummyServer` in phase 1.

Its role is to simulate:

- AMCP responses
- OSC emission
- malformed or partial responses
- timeouts
- disconnects
- reconnect windows
- scripted recovery scenarios

It should be scenarized rather than a passive mock.

### 11.4 No Docker requirement in phase 1

The design deliberately avoids making Docker-based real-server integration a prerequisite for phase 1.

That path remains open for a later phase, but the first vNext implementation will rely on deterministic in-repo integration infrastructure.

## 12. Migration Strategy

The legacy codebase remains useful as:

- protocol knowledge
- OSC mapping reference
- behavioral reference
- regression insight
- source material for tests

It is not treated as an architectural constraint.

### 12.1 Delivery model

The recommended migration path is:

- build vNext in parallel
- avoid forcing legacy managers and connection abstractions into the new runtime core
- provide documentation-based migration instead of compatibility wrappers first

### 12.2 Consumer migration support

The project should provide:

- a dedicated breaking changes document from the current version to vNext
- old-to-new API mapping guidance
- before/after examples
- mono-server usage examples
- multi-server usage examples
- event consumption examples
- hosting examples

### 12.3 Breaking changes documentation

The vNext release must document breaking changes explicitly rather than expecting users to infer them from examples.

The documentation set should include:

- what changed
- why it changed
- what legacy API shape it replaces
- how to migrate to the new shape
- whether a compatibility shim exists or not

This should be written for users upgrading from the current public release, not only for new users discovering the library.

Recommended deliverables:

- a `BREAKING_CHANGES.md` file at repository root or an equivalent prominently linked document
- a migration guide with before/after code samples
- release notes that clearly call out the redesign scope
- updated README quick starts that use only the vNext API

## 13. Recommended First Implementation Scope

The first implementation plan should focus on the smallest coherent vertical slice that validates the architecture:

- `CasparClient`
- fluent registration
- mono-server connection flow
- a first set of fluent playback commands
- unified event stream
- minimal state projection
- health model
- reconnect supervision
- `DummyServer` support
- core unit and scenario integration tests

This keeps the first slice large enough to prove the design, but small enough to execute without dragging the full legacy surface into phase 1.

## 14. Open Architectural Rule

Whenever there is a trade-off between:

- preserving historical API shapes
- exposing low-level protocol details
- or keeping the new API obvious and host-friendly

the design prefers the simpler vNext API.
