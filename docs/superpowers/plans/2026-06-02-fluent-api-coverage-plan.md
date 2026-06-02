# Fluent API Coverage Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Expand the maintained Fluent API so it covers the supported AMCP surface with domain scopes, layer sequences, local waits, and channel/layer-only parallel orchestration.

**Architecture:** Keep `CasparClient` as the transport and execution core, and add thin fluent scopes/builders that forward to existing async methods. Sequence and parallel orchestration stay client-side and explicit: a layer sequence is an ordered list of local steps, while `Parallel(...)` only runs multiple independent layer sequences concurrently with no server-side transaction semantics.

**Tech Stack:** .NET 10, C#, xUnit, existing `CasparClient`, existing AMCP command types, existing unit test suite under `test/StarDust.CasparCG.UnitTests`.

---

## File Structure

### Runtime files to create

- `src/StarDust.CasparCG/Fluent/AdminScope.cs`
- `src/StarDust.CasparCG/Fluent/DataScope.cs`
- `src/StarDust.CasparCG/Fluent/OscScope.cs`
- `src/StarDust.CasparCG/Fluent/ServerScope.cs`
- `src/StarDust.CasparCG/Fluent/ThumbnailScope.cs`
- `src/StarDust.CasparCG/Fluent/LayerSequenceBuilder.cs`
- `src/StarDust.CasparCG/Fluent/LayerSequenceStep.cs`
- `src/StarDust.CasparCG/Fluent/ParallelSequenceBuilder.cs`
- `src/StarDust.CasparCG/Fluent/LoadBackgroundCommandBuilder.cs`

### Runtime files to modify

- `src/StarDust.CasparCG/CasparClient.cs`
- `src/StarDust.CasparCG/Fluent/ChannelScope.cs`
- `src/StarDust.CasparCG/Fluent/LayerScope.cs`
- `src/StarDust.CasparCG/Fluent/PlayCommandBuilder.cs`
- `src/StarDust.CasparCG/StarDust.CasparCG.csproj`

### Test files to create

- `test/StarDust.CasparCG.UnitTests/FluentScopeCoverageTests.cs`
- `test/StarDust.CasparCG.UnitTests/LayerSequenceBuilderTests.cs`
- `test/StarDust.CasparCG.UnitTests/ParallelSequenceBuilderTests.cs`

### Test files to modify

- `test/StarDust.CasparCG.UnitTests/FluentPlayCommandBuilderTests.cs`
- `test/StarDust.CasparCG.UnitTests/CasparClientBootstrapTests.cs`
- `test/StarDust.CasparCG.UnitTests/CasparClientCommandTests.cs`

### Documentation files to modify

- `README.md`
- `docs/vnext/fluent-api-cookbook.md`
- `CONTRIBUTING.md`

## Task 1: Add global fluent scopes on `CasparClient`

**Files:**
- Create: `src/StarDust.CasparCG/Fluent/AdminScope.cs`
- Create: `src/StarDust.CasparCG/Fluent/DataScope.cs`
- Create: `src/StarDust.CasparCG/Fluent/OscScope.cs`
- Create: `src/StarDust.CasparCG/Fluent/ServerScope.cs`
- Create: `src/StarDust.CasparCG/Fluent/ThumbnailScope.cs`
- Modify: `src/StarDust.CasparCG/CasparClient.cs`
- Modify: `src/StarDust.CasparCG/StarDust.CasparCG.csproj`
- Test: `test/StarDust.CasparCG.UnitTests/FluentScopeCoverageTests.cs`

- [ ] **Step 1: Write the failing scope-coverage tests**

Add `test/StarDust.CasparCG.UnitTests/FluentScopeCoverageTests.cs` with assertions that the new scope entry points exist and forward to the expected `CasparClient` operations. Cover at least:

```csharp
[Fact]
public async Task ServerScope_version_forwards_to_client_query()
{
    var transport = new StubAmcpTransport("201 VERSION OK\r\n2.5.0\r\n");
    var client = new CasparClient(transport);

    var version = await client.Server().VersionAsync(CancellationToken.None);

    Assert.Equal("2.5.0", version);
    Assert.Equal("VERSION SERVER\r\n", transport.SentCommands.Single());
}

[Fact]
public async Task AdminScope_restart_forwards_to_restart_command()
{
    var transport = new StubAmcpTransport("202 RESTART OK\r\n");
    var client = new CasparClient(transport);

    await client.Admin().RestartAsync(CancellationToken.None);

    Assert.Equal("RESTART\r\n", transport.SentCommands.Single());
}
```

- [ ] **Step 2: Run the focused tests to verify they fail**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "FluentScopeCoverageTests" -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: FAIL because the new fluent scopes do not exist yet.

- [ ] **Step 3: Implement thin global scopes**

Create scope types as primary-constructor wrappers around `CasparClient`, for example:

```csharp
namespace StarDust.CasparCG.Fluent;

public sealed class ServerScope(CasparClient client)
{
    public ValueTask<string> VersionAsync(CancellationToken cancellationToken) =>
        client.GetVersionAsync(cancellationToken);

    public ValueTask<IReadOnlyList<string>> MediaFilesAsync(CancellationToken cancellationToken) =>
        client.GetMediaFilesAsync(cancellationToken);
}
```

Add corresponding entry points on `CasparClient`:

```csharp
public ServerScope Server() => new(this);
public DataScope Data() => new(this);
public ThumbnailScope Thumbnails() => new(this);
public OscScope Osc() => new(this);
public AdminScope Admin() => new(this);
```

Keep admin methods limited to `ByeAsync`, `KillAsync`, `RestartAsync`, `LockAsync`.

- [ ] **Step 4: Run the focused tests to verify they pass**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "FluentScopeCoverageTests" -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add src/StarDust.CasparCG/CasparClient.cs src/StarDust.CasparCG/Fluent/AdminScope.cs src/StarDust.CasparCG/Fluent/DataScope.cs src/StarDust.CasparCG/Fluent/OscScope.cs src/StarDust.CasparCG/Fluent/ServerScope.cs src/StarDust.CasparCG/Fluent/ThumbnailScope.cs src/StarDust.CasparCG/StarDust.CasparCG.csproj test/StarDust.CasparCG.UnitTests/FluentScopeCoverageTests.cs
git commit -m "feat: add global fluent scopes"
```

## Task 2: Expand `ChannelScope` and `LayerScope` direct fluent coverage

**Files:**
- Modify: `src/StarDust.CasparCG/Fluent/ChannelScope.cs`
- Modify: `src/StarDust.CasparCG/Fluent/LayerScope.cs`
- Test: `test/StarDust.CasparCG.UnitTests/FluentScopeCoverageTests.cs`

- [ ] **Step 1: Extend the failing tests for channel/layer coverage**

Add tests for direct layer and channel forwarding:

```csharp
[Fact]
public async Task ChannelScope_grid_forwards_to_channel_grid()
{
    var transport = new StubAmcpTransport("202 CHANNEL_GRID OK\r\n");
    var client = new CasparClient(transport);

    await client.Channel(1).GridAsync(CancellationToken.None);

    Assert.Equal("CHANNEL_GRID 1\r\n", transport.SentCommands.Single());
}

[Fact]
public async Task LayerScope_pause_forwards_to_pause_command()
{
    var transport = new StubAmcpTransport("202 PAUSE OK\r\n");
    var client = new CasparClient(transport);

    await client.Channel(1).Layer(10).PauseAsync(CancellationToken.None);

    Assert.Equal("PAUSE 1-10\r\n", transport.SentCommands.Single());
}
```

- [ ] **Step 2: Run the focused tests to verify they fail**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "FluentScopeCoverageTests" -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: FAIL because `GridAsync` and the expanded layer methods are missing.

- [ ] **Step 3: Add direct channel/layer forwarding methods**

Update `ChannelScope`:

```csharp
public sealed class ChannelScope(CasparClient client, int channel)
{
    public LayerScope Layer(int layer) => new(client, channel, layer);
    public ValueTask GridAsync(CancellationToken cancellationToken) =>
        client.ChannelGridAsync(channel, cancellationToken);
}
```

Update `LayerScope` with direct wrappers for the simple channel/layer operations from the spec, for example:

```csharp
public ValueTask PauseAsync(CancellationToken cancellationToken) =>
    client.PauseAsync(channel, layer, cancellationToken);

public ValueTask ResumeAsync(CancellationToken cancellationToken) =>
    client.ResumeAsync(channel, layer, cancellationToken);

public ValueTask ClearAsync(CancellationToken cancellationToken) =>
    client.ClearAsync(channel, layer, cancellationToken);
```

Cover the remaining simple wrappers in the same style.

- [ ] **Step 4: Run the focused tests to verify they pass**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "FluentScopeCoverageTests" -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add src/StarDust.CasparCG/Fluent/ChannelScope.cs src/StarDust.CasparCG/Fluent/LayerScope.cs test/StarDust.CasparCG.UnitTests/FluentScopeCoverageTests.cs
git commit -m "feat: expand direct channel and layer fluent coverage"
```

## Task 3: Add `LoadBg` fluent builder and align existing play builder patterns

**Files:**
- Create: `src/StarDust.CasparCG/Fluent/LoadBackgroundCommandBuilder.cs`
- Modify: `src/StarDust.CasparCG/Fluent/LayerScope.cs`
- Modify: `src/StarDust.CasparCG/Fluent/PlayCommandBuilder.cs`
- Modify: `test/StarDust.CasparCG.UnitTests/FluentPlayCommandBuilderTests.cs`

- [ ] **Step 1: Write the failing builder tests**

Extend `FluentPlayCommandBuilderTests.cs` with scenarios such as:

```csharp
[Fact]
public async Task LoadBg_builder_serializes_loop_and_auto_play()
{
    var transport = new StubAmcpTransport("202 LOADBG OK\r\n");
    var client = new CasparClient(transport);

    await client.Channel(1).Layer(10).LoadBg("AMB").Loop().AutoPlay().SendAsync(CancellationToken.None);

    Assert.Equal("LOADBG 1-10 AMB LOOP AUTO\r\n", transport.SentCommands.Single());
}
```

- [ ] **Step 2: Run the focused tests to verify they fail**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "FluentPlayCommandBuilderTests" -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: FAIL because `LoadBg(...)` and the corresponding builder do not exist.

- [ ] **Step 3: Add the new builder**

Create `LoadBackgroundCommandBuilder.cs` with only the options that map to the maintained command surface:

```csharp
public sealed class LoadBackgroundCommandBuilder(CasparClient client, int channel, int layer, string clip)
{
    private bool _loop;
    private bool _autoPlay;

    public LoadBackgroundCommandBuilder Loop()
    {
        _loop = true;
        return this;
    }

    public LoadBackgroundCommandBuilder AutoPlay()
    {
        _autoPlay = true;
        return this;
    }

    public ValueTask SendAsync(CancellationToken cancellationToken) =>
        client.SendAsync(BuildCommand(), cancellationToken);
}
```

Expose it from `LayerScope`:

```csharp
public LoadBackgroundCommandBuilder LoadBg(string clip) => new(client, channel, layer, clip);
```

Keep `PlayCommandBuilder` focused on `PLAY` options and avoid overloading it with sequence concerns.

- [ ] **Step 4: Run the builder tests to verify they pass**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "FluentPlayCommandBuilderTests" -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add src/StarDust.CasparCG/Fluent/LayerScope.cs src/StarDust.CasparCG/Fluent/LoadBackgroundCommandBuilder.cs src/StarDust.CasparCG/Fluent/PlayCommandBuilder.cs test/StarDust.CasparCG.UnitTests/FluentPlayCommandBuilderTests.cs
git commit -m "feat: add loadbg fluent builder"
```

## Task 4: Add local layer sequence orchestration with `Then()` and `Wait(...)`

**Files:**
- Create: `src/StarDust.CasparCG/Fluent/LayerSequenceBuilder.cs`
- Create: `src/StarDust.CasparCG/Fluent/LayerSequenceStep.cs`
- Modify: `src/StarDust.CasparCG/Fluent/LayerScope.cs`
- Test: `test/StarDust.CasparCG.UnitTests/LayerSequenceBuilderTests.cs`

- [ ] **Step 1: Write the failing sequence tests**

Create `LayerSequenceBuilderTests.cs` with at least:

```csharp
[Fact]
public async Task Sequence_executes_commands_in_order_with_local_wait()
{
    var transport = new StubAmcpTransport("202 LOADBG OK\r\n", "202 PLAY OK\r\n");
    var client = new CasparClient(transport);

    await client.Channel(1).Layer(10).Sequence()
        .LoadBg("AMB").Loop()
        .Then()
        .Wait(1)
        .Then()
        .Play("AMB")
        .SendAsync(CancellationToken.None);

    Assert.Equal(
        new[] { "LOADBG 1-10 AMB LOOP\r\n", "PLAY 1-10 AMB\r\n" },
        transport.SentCommands);
}
```

Also add tests for:

- `Wait(TimeSpan)`
- negative wait validation
- stop-on-error behavior

- [ ] **Step 2: Run the focused tests to verify they fail**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "LayerSequenceBuilderTests" -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: FAIL because `Sequence()` and the step model do not exist.

- [ ] **Step 3: Implement the sequence builder**

Create a small step model:

```csharp
internal abstract record LayerSequenceStep;
internal sealed record DelayStep(TimeSpan Delay) : LayerSequenceStep;
internal sealed record CommandStep(Func<CancellationToken, ValueTask> SendAsync) : LayerSequenceStep;
```

Implement `LayerSequenceBuilder` so that:

- `Then()` marks the next step boundary
- `Wait(TimeSpan)` and `Wait(int)` append delay steps
- `Play(...)`, `LoadBg(...)`, `Clear()`, `Pause()` append command steps
- `SendAsync(...)` iterates the steps in order

Expose it from `LayerScope`:

```csharp
public LayerSequenceBuilder Sequence() => new(client, channel, layer);
```

Keep the sequence implementation local and explicit; do not add server-side scheduling semantics.

- [ ] **Step 4: Run the focused tests to verify they pass**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "LayerSequenceBuilderTests" -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add src/StarDust.CasparCG/Fluent/LayerScope.cs src/StarDust.CasparCG/Fluent/LayerSequenceBuilder.cs src/StarDust.CasparCG/Fluent/LayerSequenceStep.cs test/StarDust.CasparCG.UnitTests/LayerSequenceBuilderTests.cs
git commit -m "feat: add layer sequence orchestration"
```

## Task 5: Add client-level parallel orchestration for layer sequences

**Files:**
- Create: `src/StarDust.CasparCG/Fluent/ParallelSequenceBuilder.cs`
- Modify: `src/StarDust.CasparCG/CasparClient.cs`
- Test: `test/StarDust.CasparCG.UnitTests/ParallelSequenceBuilderTests.cs`

- [ ] **Step 1: Write the failing parallel tests**

Create `ParallelSequenceBuilderTests.cs` with coverage such as:

```csharp
[Fact]
public async Task Parallel_runs_multiple_layer_sequences()
{
    var transport = new StubAmcpTransport(
        "202 PLAY OK\r\n",
        "202 PLAY OK\r\n");
    var client = new CasparClient(transport);

    await client.Parallel(
        client.Channel(1).Layer(10).Sequence().Play("AMB"),
        client.Channel(2).Layer(20).Sequence().Play("SAMPLE-1"))
        .SendAsync(CancellationToken.None);

    Assert.Contains("PLAY 1-10 AMB\r\n", transport.SentCommands);
    Assert.Contains("PLAY 2-20 SAMPLE-1\r\n", transport.SentCommands);
}
```

Also add a test that confirms failure in one sequence fails the aggregate call.

- [ ] **Step 2: Run the focused tests to verify they fail**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "ParallelSequenceBuilderTests" -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: FAIL because `Parallel(...)` does not exist.

- [ ] **Step 3: Implement `ParallelSequenceBuilder`**

Expose a client entry point:

```csharp
public ParallelSequenceBuilder Parallel(params LayerSequenceBuilder[] sequences) =>
    new(sequences);
```

Implement `ParallelSequenceBuilder.SendAsync(...)` using `Task.WhenAll` over the provided sequences. Keep it intentionally limited to `LayerSequenceBuilder` inputs.

- [ ] **Step 4: Run the focused tests to verify they pass**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "ParallelSequenceBuilderTests" -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add src/StarDust.CasparCG/CasparClient.cs src/StarDust.CasparCG/Fluent/ParallelSequenceBuilder.cs test/StarDust.CasparCG.UnitTests/ParallelSequenceBuilderTests.cs
git commit -m "feat: add parallel layer sequence orchestration"
```

## Task 6: Expose the remaining domain methods needed for the spec

**Files:**
- Modify: `src/StarDust.CasparCG/Fluent/LayerScope.cs`
- Modify: `src/StarDust.CasparCG/Fluent/ServerScope.cs`
- Modify: `src/StarDust.CasparCG/Fluent/DataScope.cs`
- Modify: `src/StarDust.CasparCG/Fluent/ThumbnailScope.cs`
- Modify: `src/StarDust.CasparCG/Fluent/OscScope.cs`
- Modify: `src/StarDust.CasparCG/Fluent/AdminScope.cs`
- Test: `test/StarDust.CasparCG.UnitTests/FluentScopeCoverageTests.cs`

- [ ] **Step 1: Extend the failing scope tests for the remaining commands**

Add focused forwarding tests for:

- `Data().StoreAsync(...)`
- `Thumbnails().GenerateAsync(...)`
- `Osc().SubscribeAsync(...)`
- `Server().InfoPathsAsync(...)`
- `Layer(10).MixerOpacityAsync(...)`

Use the existing `StubAmcpTransport` pattern and assert command text.

- [ ] **Step 2: Run the focused tests to verify they fail**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "FluentScopeCoverageTests" -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: FAIL for the newly added assertions.

- [ ] **Step 3: Add the missing wrappers**

Implement the remaining thin fluent wrappers required by the spec. Keep them minimal and forward-only. Do not add extra abstractions for commands that do not need builders.

- [ ] **Step 4: Run the focused tests to verify they pass**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "FluentScopeCoverageTests" -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add src/StarDust.CasparCG/Fluent/LayerScope.cs src/StarDust.CasparCG/Fluent/ServerScope.cs src/StarDust.CasparCG/Fluent/DataScope.cs src/StarDust.CasparCG/Fluent/ThumbnailScope.cs src/StarDust.CasparCG/Fluent/OscScope.cs src/StarDust.CasparCG/Fluent/AdminScope.cs test/StarDust.CasparCG.UnitTests/FluentScopeCoverageTests.cs
git commit -m "feat: complete fluent scope coverage"
```

## Task 7: Update the user-facing docs for the new fluent API

**Files:**
- Modify: `README.md`
- Modify: `docs/vnext/fluent-api-cookbook.md`
- Modify: `CONTRIBUTING.md`

- [ ] **Step 1: Add failing doc verification context**

Review the existing fluent examples and identify the sections that must be updated:

- README Fluent commands section
- `docs/vnext/fluent-api-cookbook.md`
- any contributor note that references the old limited fluent surface

- [ ] **Step 2: Update the documentation**

Add examples for:

- global scopes such as `Server()` and `Admin()`
- `Sequence().Then().Wait(...)`
- `Parallel(...)`

Use concrete examples:

```csharp
await client.Server().VersionAsync(ct);
await client.Admin().RestartAsync(ct);
await client.Channel(1).Layer(10).Sequence().LoadBg("AMB").Loop().Then().Wait(500).Then().Play("AMB").SendAsync(ct);
```

- [ ] **Step 3: Run doc verification**

Run: `./scripts/verify-docs.sh`

Expected: PASS.

- [ ] **Step 4: Commit**

```bash
git add README.md docs/vnext/fluent-api-cookbook.md CONTRIBUTING.md
git commit -m "docs: cover expanded fluent api"
```

## Task 8: Full verification and branch-ready cleanup

**Files:**
- Verify only

- [ ] **Step 1: Run the full unit test suite**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: PASS.

- [ ] **Step 2: Run the integration test suite**

Run: `dotnet test test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj -c Release -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: PASS.

- [ ] **Step 3: Run build and lint**

Run: `make build`

Expected: PASS with `0 Warning(s), 0 Error(s)`.

Run: `make lint`

Expected: PASS.

- [ ] **Step 4: Check whitespace and git state**

Run: `git diff --check`

Expected: no output.

Run: `git status --short`

Expected: clean worktree.

- [ ] **Step 5: Final commit if verification required follow-up edits**

```bash
git add -A
git commit -m "chore: finalize fluent api coverage"
```

Only do this step if verification required real code or doc changes after the earlier commits.

## Self-Review

Spec coverage check:

- Domain scopes: covered by Tasks 1 and 6.
- Query-style fluent access: covered by Tasks 1 and 6.
- Admin isolation: covered by Task 1.
- Expanded direct channel/layer coverage: covered by Task 2.
- Configurable layer builders: covered by Task 3.
- Layer sequences with `Then()` and local `Wait(...)`: covered by Task 4.
- Parallel layer orchestration only: covered by Task 5.
- Docs and discoverability: covered by Task 7.
- Final validation: covered by Task 8.

Placeholder scan:

- No `TODO`, `TBD`, or “similar to previous task” placeholders remain.

Type consistency:

- The plan consistently uses `ServerScope`, `DataScope`, `ThumbnailScope`, `OscScope`, `AdminScope`, `LayerSequenceBuilder`, and `ParallelSequenceBuilder`.
- Query methods consistently end with `Async`.
- The orchestration surface consistently uses `Sequence()`, `Then()`, `Wait(...)`, and `SendAsync(...)`.
