# Fluent API Coverage Design

Date: 2026-06-02
Branch: `design/casparcg-vnext`

## Context

`CasparClient` already exposes most of the AMCP command surface, but the Fluent API is still limited to `Channel(...).Layer(...).Play(...).WithTransition().Mix(...).WithLoop().SendAsync(...)`.

This gap makes the public API inconsistent:

- simple scenarios are fluent
- most runtime operations still require direct `CasparClient` calls
- orchestration scenarios such as delayed steps and multi-layer parallel execution have no first-class API

The goal of this design is to expand the Fluent API so it covers the maintained AMCP surface with intuitive channel/layer orchestration, while keeping global commands discoverable and keeping dangerous commands behind an explicit admin scope.

## Goals

- Expose the maintained `CasparClient` surface through a structured Fluent API.
- Keep query commands result-oriented rather than command-builder-oriented.
- Support local sequential orchestration on a layer, including `Then()` and `Wait(...)`.
- Support parallel execution only for channel/layer sequences.
- Keep global commands organized by domain scopes rather than flattening everything on `CasparClient`.
- Preserve intuitive API discovery in IDE completion.

## Non-Goals

- No server-side transaction or atomic multi-command execution.
- No event-driven waits such as `WaitUntilPlaying()` or `WaitForEvent<T>()` in this iteration.
- No parallel orchestration for global scopes such as `Data`, `Server`, `Thumbnails`, `Osc`, or `Admin`.
- No attempt to expose every possible AMCP argument combination as a single universal builder.

## Recommended Approach

Three approaches were considered:

1. Flatten more commands directly on `CasparClient`, while keeping only `Channel(...).Layer(...)` as a fluent convenience layer.
2. Introduce structured domain scopes plus local layer sequences and explicit parallel orchestration.
3. Build a universal builder abstraction for all commands and all scopes.

Approach 2 is recommended.

Reasons:

- It keeps `CasparClient` from becoming a giant flat facade.
- It aligns with the difference between global commands and channel/layer commands.
- It supports scenario-oriented workflows such as delayed steps and layer fan-out without inventing false server-side transactional semantics.
- It remains understandable and testable because sequencing and parallelism are explicit client-side orchestration concepts.

## Public API Shape

### Domain Scopes

`CasparClient` will expose additional fluent entry points:

- `Channel(int channel)`
- `Server()`
- `Data()`
- `Thumbnails()`
- `Osc()`
- `Admin()`
- `Parallel(params LayerSequenceBuilder[] sequences)` or equivalent strongly typed overloads

The existing direct async methods on `CasparClient` remain supported. The Fluent API is an additive public surface, not a replacement.

### Channel and Layer

`ChannelScope` remains the channel entry point:

- `Layer(int layer)`
- `GridAsync(CancellationToken cancellationToken)`

`LayerScope` becomes the main fluent surface for channel/layer commands.

It will expose:

- direct terminal async methods for simple actions
- specialized builders for configurable commands
- a scenario-oriented sequence entry point

Examples:

```csharp
await client.Channel(1).Layer(10).PauseAsync(ct);
await client.Channel(1).Layer(10).ClearAsync(ct);
await client.Channel(1).GridAsync(ct);
```

### Global Query Scopes

Queries remain direct and result-oriented.

Examples:

```csharp
var version = await client.Server().VersionAsync(ct);
var media = await client.Server().MediaFilesAsync(ct);
var info = await client.Server().InfoAsync(ct);
```

This design intentionally avoids turning queries into builders such as `Version().SendAsync()`.

### Admin Scope

Dangerous commands are grouped behind `Admin()`.

Examples:

```csharp
await client.Admin().RestartAsync(ct);
await client.Admin().KillAsync(ct);
await client.Admin().LockAsync(ct);
```

This keeps operational commands explicit and harder to invoke accidentally through autocomplete.

## Scope Breakdown

### `LayerScope`

`LayerScope` should expose direct methods for simple channel/layer operations:

- `PauseAsync`
- `ResumeAsync`
- `StopAsync`
- `ClearAsync`
- `CallAsync`
- `CallBgAsync`
- `SwapAsync`
- `AddAsync`
- `RemoveAsync`
- `ApplyAsync`
- `PrintAsync`
- `SetAsync`
- `CgPlayAsync`
- `CgStopAsync`
- `CgNextAsync`
- `CgRemoveAsync`
- `CgClearAsync`
- `CgInvokeAsync`
- `MixerKeyerAsync`
- `MixerInvertAsync`
- `MixerBlendAsync`
- `MixerOpacityAsync`
- `MixerBrightnessAsync`
- `MixerSaturationAsync`
- `MixerContrastAsync`
- `MixerVolumeAsync`
- `MixerCommitAsync`
- `MixerClearAsync`

`LayerScope` should expose specialized builders for configurable commands:

- `Play(string clip)`
- `LoadBg(string clip)`
- `Load(string clip)`
- `CgAdd(string template, bool playOnLoad = true, string? data = null)`
- `CgUpdate(string data)`
- `MixerChroma()`
- `MixerLevels()`
- `MixerFill()`
- `MixerClip()`
- `MixerAnchor()`
- `MixerCrop()`
- `MixerRotation()`
- `MixerPerspective()`
- `MixerGrid()`

The exact builder set should follow actual command complexity rather than artificially wrapping every direct method.

### `ServerScope`

`ServerScope` should expose:

- `VersionAsync`
- `InfoAsync`
- `InfoConfigAsync`
- `InfoPathsAsync`
- `MediaFilesAsync`
- `MediaInfoAsync(string clip)` for `CINF`
- `FileListAsync`
- `TemplateListAsync`
- `GlInfoAsync`
- `GlGcAsync`
- `DiagAsync`

### `DataScope`

`DataScope` should expose:

- `StoreAsync`
- `RetrieveAsync`
- `ListAsync`
- `RemoveAsync`

### `ThumbnailScope`

`Thumbnails()` should expose:

- `ListAsync`
- `RetrieveAsync`
- `GenerateAsync`
- `GenerateAllAsync`

### `OscScope`

`Osc()` should expose:

- `StartAsync`
- `StopAsync`
- `SubscribeAsync`
- `UnsubscribeAsync`

### `AdminScope`

`Admin()` should expose:

- `ByeAsync`
- `KillAsync`
- `RestartAsync`
- `LockAsync`

## Layer Sequence Design

### Rationale

Layer orchestration must support intuitive scenario composition such as:

- `LoadBg(...).Loop()`
- wait locally for a duration
- then send a second command such as `Play(...)`

This is not a single AMCP command. It is a client-side orchestration pipeline.

### Public Shape

`LayerScope` exposes `Sequence()`.

Examples:

```csharp
await client
    .Channel(1)
    .Layer(10)
    .Sequence()
    .LoadBg("AMB").Loop()
    .Then()
    .Wait(TimeSpan.FromSeconds(10))
    .Then()
    .Play("AMB")
    .SendAsync(ct);
```

```csharp
await client
    .Channel(1)
    .Layer(10)
    .Sequence()
    .Clear()
    .Then()
    .Wait(1_000)
    .Then()
    .CgAdd("LOWER_THIRD", playOnLoad: true, data: payload)
    .SendAsync(ct);
```

### Wait Semantics

`Wait(...)` is strictly local to the client.

Supported overloads:

- `Wait(TimeSpan delay)`
- `Wait(int milliseconds)`

The `int` overload is defined explicitly as milliseconds.

Not included in this iteration:

- `Wait(double seconds)`
- event-driven waits
- server-clock scheduling

### Then Semantics

`Then()` defines an explicit step boundary.

It does not:

- imply parallelism
- imply batching
- imply transactionality

It only separates one local step from the next step in an ordered sequence.

### Execution Rules

`SendAsync(CancellationToken cancellationToken)` on a layer sequence:

1. executes steps in order
2. sends one AMCP command step at a time
3. waits for each AMCP response before advancing
4. performs local delay for `Wait(...)` steps
5. stops immediately when a command fails

There is no rollback if a later step fails.

## Parallel Layer Execution

### Scope Limitation

Parallelism is supported only for channel/layer sequences.

It is explicitly not supported for:

- `Server()`
- `Data()`
- `Thumbnails()`
- `Osc()`
- `Admin()`

### Public Shape

Examples:

```csharp
await client.Parallel(
    client.Channel(1).Layer(10).Sequence()
        .LoadBg("AMB").Loop()
        .Then()
        .Play("AMB"),
    client.Channel(2).Layer(20).Sequence()
        .Wait(500)
        .Then()
        .Play("SAMPLE-1"))
    .SendAsync(ct);
```

The exact type names can change during implementation, but the user-facing model is:

- build independent layer sequences
- submit them to a client-level parallel orchestrator
- call one `SendAsync`

### Execution Rules

- each sequence remains sequential internally
- the parallel orchestrator starts all sequences concurrently
- implementation uses client-side task orchestration, such as `Task.WhenAll`
- if one sequence fails, the overall operation fails
- already-started sequences are not rolled back
- cancellation prevents future steps and stops local waits where possible

## Error Handling

### Sequence Errors

- AMCP command failure throws immediately
- the current sequence stops
- subsequent sequence steps are not executed

### Parallel Errors

- any sequence failure fails the aggregate operation
- no rollback is attempted
- exception aggregation behavior should be explicit and documented in XML comments

### Validation Errors

Builder validation should fail early where practical:

- missing clip name
- invalid duration such as negative wait
- invalid null arguments where not allowed

## Testing Strategy

Tests should be added before implementation changes where possible.

Coverage should include:

- fluent scope discovery and direct method forwarding
- query scopes returning the expected result types
- admin scope command forwarding
- layer sequence ordering
- `Wait(TimeSpan)` and `Wait(int)` local delay registration
- `Then()` boundary handling
- sequence stop-on-error behavior
- parallel sequence fan-out with independent ordering preserved per sequence
- cancellation behavior for local waits and subsequent steps

Testing should use existing dummy transport and `DummyServer` patterns rather than introducing new infrastructure.

## Compatibility and Migration

- Existing `CasparClient` direct methods remain available.
- Existing `Channel(...).Layer(...).Play(...).WithTransition().Mix(...).WithLoop().SendAsync(...)` remains supported.
- The new Fluent API expands discoverability and orchestration but does not remove the existing direct command surface.

## Implementation Constraints

- Keep the Fluent API intuitive first, but never hide real execution semantics.
- Avoid a single universal mega-builder.
- Keep command builders focused and cohesive.
- Use modern .NET 10 patterns where they improve clarity.
- Keep Roslyn/analyzer output clean.
- Document new fluent entry points and scenario orchestration in user-facing docs.
