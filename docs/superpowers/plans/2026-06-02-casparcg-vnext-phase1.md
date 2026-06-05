# CasparCG vNext Phase 1 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build the first production-ready vNext slice: a `.NET 10` `CasparClient` with fluent DI registration, async AMCP command execution, unified OSC/domain event streaming, minimal state projection, health/reconnect supervision, `DummyServer`-based integration tests, clear upgrade docs, and modern GitHub Actions CI.

**Architecture:** Keep the legacy code in place and add the vNext stack in parallel under new `StarDust.CasparCG.*` projects. The public API lives in `StarDust.CasparCG` and `StarDust.CasparCG.Hosting`, while AMCP, OSC, transport, and test infrastructure stay split into focused projects. Drive each capability with tests first, then add the thinnest code that makes the tests pass, and commit after every coherent slice.

**Tech Stack:** `.NET 10`, `Microsoft.Extensions.DependencyInjection`, `Microsoft.Extensions.Logging`, `System.Threading.Channels`, `System.Net.Sockets`, `xUnit`, `Moq`, GitHub Actions

---

## File Structure Map

### New production projects

- `src/StarDust.CasparCG/StarDust.CasparCG.csproj`
  Public `CasparClient`, fluent command builders, event abstractions, state snapshots, diagnostics, lifecycle notifications.
- `src/StarDust.CasparCG.Hosting/StarDust.CasparCG.Hosting.csproj`
  `AddCasparCG()` registration, named client support, factory wiring, fluent registration builder.
- `src/StarDust.CasparCG.Protocol.Amcp/StarDust.CasparCG.Protocol.Amcp.csproj`
  AMCP command objects, serialization, response parsing.
- `src/StarDust.CasparCG.Protocol.Osc/StarDust.CasparCG.Protocol.Osc.csproj`
  OSC message intake and mapping into domain events.
- `src/StarDust.CasparCG.Transport/StarDust.CasparCG.Transport.csproj`
  AMCP TCP transport, OSC UDP listener, reconnect supervisor, backpressure-safe pipelines.
- `src/StarDust.CasparCG.Testing/StarDust.CasparCG.Testing.csproj`
  `DummyServer`, scripted AMCP/OSC scenarios, reusable test helpers.

### New test projects

- `src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj`
  Unit tests for commands, DI, events, state, health, and reconnection logic.
- `src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj`
  Socket-level integration tests using `DummyServer`.

### Shared repo-level files

- `global.json`
  Pin the expected `.NET 10` SDK.
- `Directory.Build.props`
  Shared `net10.0` defaults, nullable, implicit usings, deterministic builds, package metadata defaults.
- `src/StarDust.CasparCG.net.sln`
  Keep the legacy solution, add the vNext projects without removing the old ones yet.
- `README.md`
  Rewrite quick start around the vNext happy path.
- `BREAKING_CHANGES.md`
  Explicit migration notes from the current release.
- `CONTRIBUTING.md`
  Contributor workflow, CI expectations, test matrix.
- `docs/vnext/getting-started.md`
  Onboarding for the new API.
- `docs/vnext/fluent-api-cookbook.md`
  Focused short recipes.
- `docs/vnext/events-and-state.md`
  Event stream, filtering, and state snapshot guidance.
- `docs/vnext/hosting-and-di.md`
  Named clients, host wiring, logging, health.
- `docs/vnext/testing-with-dummy-server.md`
  Integration testing guidance for contributors.
- `.github/workflows/ci.yml`
  PR and branch validation.
- `.github/workflows/package.yml`
  Packaging and publishing validation flow.
- `scripts/verify-docs.sh`
  Lightweight docs completeness check.
- `scripts/verify-workflows.sh`
  Lightweight workflow sanity check.

### Repository notes

- Do not delete the legacy projects in phase 1.
- Keep namespaces stable inside the new projects: `StarDust.CasparCG.*`.
- Prefer adding new files over mutating the legacy runtime unless a shared asset is genuinely reusable.

### Task 1: Bootstrap the vNext solution slice

**Files:**
- Create: `global.json`
- Create: `Directory.Build.props`
- Create: `src/StarDust.CasparCG/StarDust.CasparCG.csproj`
- Create: `src/StarDust.CasparCG/CasparClient.cs`
- Create: `src/StarDust.CasparCG/ICasparClientFactory.cs`
- Create: `src/StarDust.CasparCG.Hosting/StarDust.CasparCG.Hosting.csproj`
- Create: `src/StarDust.CasparCG.Hosting/ServiceCollectionExtensions.cs`
- Create: `src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj`
- Create: `src/StarDust.CasparCG.UnitTests/CasparClientBootstrapTests.cs`
- Modify: `src/StarDust.CasparCG.net.sln`

- [ ] **Step 1: Write the failing bootstrap smoke test**

```csharp
// src/StarDust.CasparCG.UnitTests/CasparClientBootstrapTests.cs
using Microsoft.Extensions.DependencyInjection;
using StarDust.CasparCG;
using StarDust.CasparCG.Hosting;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class CasparClientBootstrapTests
{
    [Fact]
    public void AddCasparCg_registers_default_client_and_factory()
    {
        var services = new ServiceCollection();

        services.AddCasparCG();

        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<CasparClient>());
        Assert.NotNull(provider.GetRequiredService<ICasparClientFactory>());
    }
}
```

- [ ] **Step 2: Run the smoke test to verify the new surface does not exist yet**

Run: `rtk dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter CasparClientBootstrapTests -v minimal`

Expected: FAIL with compile errors because `StarDust.CasparCG`, `StarDust.CasparCG.Hosting`, and the project files do not exist yet.

- [ ] **Step 3: Create the minimal project skeleton and stub types**

```json
// global.json
{
  "sdk": {
    "version": "10.0.100",
    "rollForward": "latestFeature"
  }
}
```

```xml
<!-- Directory.Build.props -->
<Project>
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <LangVersion>preview</LangVersion>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <Deterministic>true</Deterministic>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
  </PropertyGroup>
</Project>
```

```xml
<!-- src/StarDust.CasparCG/StarDust.CasparCG.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <AssemblyName>StarDust.CasparCG</AssemblyName>
    <RootNamespace>StarDust.CasparCG</RootNamespace>
  </PropertyGroup>
</Project>
```

```csharp
// src/StarDust.CasparCG/CasparClient.cs
namespace StarDust.CasparCG;

public sealed class CasparClient
{
}
```

```csharp
// src/StarDust.CasparCG/ICasparClientFactory.cs
namespace StarDust.CasparCG;

public interface ICasparClientFactory
{
    CasparClient GetClient(string name);
}
```

```xml
<!-- src/StarDust.CasparCG.Hosting/StarDust.CasparCG.Hosting.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\StarDust.CasparCG\StarDust.CasparCG.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="10.0.0" />
  </ItemGroup>
</Project>
```

```csharp
// src/StarDust.CasparCG.Hosting/ServiceCollectionExtensions.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StarDust.CasparCG;

namespace StarDust.CasparCG.Hosting;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCasparCG(this IServiceCollection services)
    {
        services.TryAddSingleton<CasparClient>();
        services.TryAddSingleton<ICasparClientFactory, DefaultCasparClientFactory>();
        return services;
    }

    private sealed class DefaultCasparClientFactory : ICasparClientFactory
    {
        private readonly CasparClient _client;

        public DefaultCasparClientFactory(CasparClient client) => _client = client;

        public CasparClient GetClient(string name) => _client;
    }
}
```

```xml
<!-- src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\StarDust.CasparCG\StarDust.CasparCG.csproj" />
    <ProjectReference Include="..\StarDust.CasparCG.Hosting\StarDust.CasparCG.Hosting.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.1" />
    <PackageReference Include="xunit" Version="2.9.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2">
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>
</Project>
```

- [ ] **Step 4: Add the new projects to the solution and rerun the smoke test**

Run:

```bash
rtk dotnet sln src/StarDust.CasparCG.net.sln add src/StarDust.CasparCG/StarDust.CasparCG.csproj
rtk dotnet sln src/StarDust.CasparCG.net.sln add src/StarDust.CasparCG.Hosting/StarDust.CasparCG.Hosting.csproj
rtk dotnet sln src/StarDust.CasparCG.net.sln add src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj
rtk dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter CasparClientBootstrapTests -v minimal
```

Expected: PASS with one test executed.

- [ ] **Step 5: Commit the bootstrap slice**

```bash
rtk git add global.json Directory.Build.props src/StarDust.CasparCG src/StarDust.CasparCG.Hosting src/StarDust.CasparCG.UnitTests src/StarDust.CasparCG.net.sln
rtk git commit -m "feat: bootstrap CasparCG vNext projects"
```

### Task 2: Implement async AMCP short-path commands

**Files:**
- Create: `src/StarDust.CasparCG.Protocol.Amcp/StarDust.CasparCG.Protocol.Amcp.csproj`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/AmcpCommand.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/PlayCommand.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/LoadBackgroundCommand.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/StopCommand.cs`
- Create: `src/StarDust.CasparCG.Transport/StarDust.CasparCG.Transport.csproj`
- Create: `src/StarDust.CasparCG.Transport/IAmcpTransport.cs`
- Modify: `src/StarDust.CasparCG/CasparClient.cs`
- Create: `src/StarDust.CasparCG.UnitTests/CasparClientCommandTests.cs`

- [ ] **Step 1: Write failing tests for `ConnectAsync`, `PlayAsync`, `LoadBackgroundAsync`, and `StopAsync`**

```csharp
// src/StarDust.CasparCG.UnitTests/CasparClientCommandTests.cs
using StarDust.CasparCG;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class CasparClientCommandTests
{
    [Fact]
    public async Task PlayAsync_serializes_expected_amcp_command()
    {
        var transport = new RecordingAmcpTransport();
        var client = new CasparClient(transport);

        await client.PlayAsync(1, 10, "AMB", CancellationToken.None);

        Assert.Equal("PLAY 1-10 AMB\r\n", transport.LastCommandText);
    }

    [Fact]
    public async Task LoadBackgroundAsync_serializes_expected_amcp_command()
    {
        var transport = new RecordingAmcpTransport();
        var client = new CasparClient(transport);

        await client.LoadBackgroundAsync(1, 10, "AMB", CancellationToken.None);

        Assert.Equal("LOADBG 1-10 AMB\r\n", transport.LastCommandText);
    }

    [Fact]
    public async Task StopAsync_serializes_expected_amcp_command()
    {
        var transport = new RecordingAmcpTransport();
        var client = new CasparClient(transport);

        await client.StopAsync(1, 10, CancellationToken.None);

        Assert.Equal("STOP 1-10\r\n", transport.LastCommandText);
    }

    private sealed class RecordingAmcpTransport : IAmcpTransport
    {
        public string? LastCommandText { get; private set; }

        public ValueTask ConnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
        public ValueTask DisconnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
        public ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken)
        {
            LastCommandText = commandText;
            return ValueTask.FromResult("202 PLAY OK");
        }
    }
}
```

- [ ] **Step 2: Run the command tests to confirm the runtime API is missing**

Run: `rtk dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter CasparClientCommandTests -v minimal`

Expected: FAIL with constructor and method-not-found compile errors on `CasparClient` and `IAmcpTransport`.

- [ ] **Step 3: Add the minimal transport and AMCP command implementation**

```xml
<!-- src/StarDust.CasparCG.Protocol.Amcp/StarDust.CasparCG.Protocol.Amcp.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <AssemblyName>StarDust.CasparCG.Protocol.Amcp</AssemblyName>
    <RootNamespace>StarDust.CasparCG.Protocol.Amcp</RootNamespace>
  </PropertyGroup>
</Project>
```

```xml
<!-- src/StarDust.CasparCG.Transport/StarDust.CasparCG.Transport.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="10.0.0" />
  </ItemGroup>
</Project>
```

```csharp
// src/StarDust.CasparCG.Transport/IAmcpTransport.cs
namespace StarDust.CasparCG.Transport;

public interface IAmcpTransport
{
    ValueTask ConnectAsync(CancellationToken cancellationToken);
    ValueTask DisconnectAsync(CancellationToken cancellationToken);
    ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken);
}
```

```csharp
// src/StarDust.CasparCG.Protocol.Amcp/AmcpCommand.cs
namespace StarDust.CasparCG.Protocol.Amcp;

public abstract record AmcpCommand
{
    public abstract string Serialize();
    protected static string Address(int channel, int layer) => $"{channel}-{layer}";
}
```

```csharp
// src/StarDust.CasparCG.Protocol.Amcp/Commands/PlayCommand.cs
namespace StarDust.CasparCG.Protocol.Amcp.Commands;

public sealed record PlayCommand(int Channel, int Layer, string Clip) : AmcpCommand
{
    public override string Serialize() => $"PLAY {Address(Channel, Layer)} {Clip}\r\n";
}
```

```csharp
// src/StarDust.CasparCG.Protocol.Amcp/Commands/LoadBackgroundCommand.cs
namespace StarDust.CasparCG.Protocol.Amcp.Commands;

public sealed record LoadBackgroundCommand(int Channel, int Layer, string Clip) : AmcpCommand
{
    public override string Serialize() => $"LOADBG {Address(Channel, Layer)} {Clip}\r\n";
}
```

```csharp
// src/StarDust.CasparCG.Protocol.Amcp/Commands/StopCommand.cs
namespace StarDust.CasparCG.Protocol.Amcp.Commands;

public sealed record StopCommand(int Channel, int Layer) : AmcpCommand
{
    public override string Serialize() => $"STOP {Address(Channel, Layer)}\r\n";
}
```

```csharp
// src/StarDust.CasparCG/CasparClient.cs
using StarDust.CasparCG.Protocol.Amcp.Commands;
using StarDust.CasparCG.Transport;

namespace StarDust.CasparCG;

public sealed class CasparClient
{
    private readonly IAmcpTransport _transport;

    public CasparClient(IAmcpTransport transport) => _transport = transport;

    public ValueTask ConnectAsync(CancellationToken cancellationToken) => _transport.ConnectAsync(cancellationToken);

    public async ValueTask PlayAsync(int channel, int layer, string clip, CancellationToken cancellationToken)
        => _ = await _transport.SendAsync(new PlayCommand(channel, layer, clip).Serialize(), cancellationToken);

    public async ValueTask LoadBackgroundAsync(int channel, int layer, string clip, CancellationToken cancellationToken)
        => _ = await _transport.SendAsync(new LoadBackgroundCommand(channel, layer, clip).Serialize(), cancellationToken);

    public async ValueTask StopAsync(int channel, int layer, CancellationToken cancellationToken)
        => _ = await _transport.SendAsync(new StopCommand(channel, layer).Serialize(), cancellationToken);
}
```

- [ ] **Step 4: Wire project references and rerun the command tests**

Run:

```bash
rtk dotnet add src/StarDust.CasparCG/StarDust.CasparCG.csproj reference src/StarDust.CasparCG.Protocol.Amcp/StarDust.CasparCG.Protocol.Amcp.csproj
rtk dotnet add src/StarDust.CasparCG/StarDust.CasparCG.csproj reference src/StarDust.CasparCG.Transport/StarDust.CasparCG.Transport.csproj
rtk dotnet add src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj reference src/StarDust.CasparCG.Transport/StarDust.CasparCG.Transport.csproj
rtk dotnet sln src/StarDust.CasparCG.net.sln add src/StarDust.CasparCG.Protocol.Amcp/StarDust.CasparCG.Protocol.Amcp.csproj
rtk dotnet sln src/StarDust.CasparCG.net.sln add src/StarDust.CasparCG.Transport/StarDust.CasparCG.Transport.csproj
rtk dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter CasparClientCommandTests -v minimal
```

Expected: PASS with three tests executed.

- [ ] **Step 5: Commit the AMCP short-path slice**

```bash
rtk git add src/StarDust.CasparCG src/StarDust.CasparCG.Protocol.Amcp src/StarDust.CasparCG.Transport src/StarDust.CasparCG.UnitTests src/StarDust.CasparCG.net.sln
rtk git commit -m "feat: add async AMCP short-path commands"
```

### Task 3: Implement the fluent command API

**Files:**
- Create: `src/StarDust.CasparCG/Fluent/ChannelScope.cs`
- Create: `src/StarDust.CasparCG/Fluent/LayerScope.cs`
- Create: `src/StarDust.CasparCG/Fluent/PlayCommandBuilder.cs`
- Modify: `src/StarDust.CasparCG/CasparClient.cs`
- Modify: `src/StarDust.CasparCG.Protocol.Amcp/Commands/PlayCommand.cs`
- Create: `src/StarDust.CasparCG.UnitTests/FluentPlayCommandBuilderTests.cs`

- [ ] **Step 1: Write failing tests for the fluent playback chain**

```csharp
// src/StarDust.CasparCG.UnitTests/FluentPlayCommandBuilderTests.cs
using StarDust.CasparCG;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class FluentPlayCommandBuilderTests
{
    [Fact]
    public async Task Fluent_play_builder_serializes_transition_and_loop()
    {
        var transport = new RecordingAmcpTransport();
        var client = new CasparClient(transport);

        await client
            .Channel(1)
            .Layer(10)
            .Play("AMB")
            .WithTransition().Mix(12)
            .WithLoop()
            .SendAsync(CancellationToken.None);

        Assert.Equal("PLAY 1-10 AMB MIX 12 LOOP\r\n", transport.LastCommandText);
    }

    private sealed class RecordingAmcpTransport : IAmcpTransport
    {
        public string? LastCommandText { get; private set; }
        public ValueTask ConnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
        public ValueTask DisconnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
        public ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken)
        {
            LastCommandText = commandText;
            return ValueTask.FromResult("202 PLAY OK");
        }
    }
}
```

- [ ] **Step 2: Run the fluent tests to verify the chain API is not implemented**

Run: `rtk dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter FluentPlayCommandBuilderTests -v minimal`

Expected: FAIL with compile errors for `Channel`, `Layer`, `Play`, `WithTransition`, `Mix`, `WithLoop`, and `SendAsync`.

- [ ] **Step 3: Implement the smallest fluent builder chain that satisfies the test**

```csharp
// src/StarDust.CasparCG/Fluent/ChannelScope.cs
namespace StarDust.CasparCG.Fluent;

public sealed class ChannelScope
{
    private readonly CasparClient _client;
    private readonly int _channel;

    public ChannelScope(CasparClient client, int channel)
    {
        _client = client;
        _channel = channel;
    }

    public LayerScope Layer(int layer) => new(_client, _channel, layer);
}
```

```csharp
// src/StarDust.CasparCG/Fluent/LayerScope.cs
namespace StarDust.CasparCG.Fluent;

public sealed class LayerScope
{
    private readonly CasparClient _client;
    private readonly int _channel;
    private readonly int _layer;

    public LayerScope(CasparClient client, int channel, int layer)
    {
        _client = client;
        _channel = channel;
        _layer = layer;
    }

    public PlayCommandBuilder Play(string clip) => new(_client, _channel, _layer, clip);
}
```

```csharp
// src/StarDust.CasparCG/Fluent/PlayCommandBuilder.cs
using StarDust.CasparCG.Protocol.Amcp.Commands;

namespace StarDust.CasparCG.Fluent;

public sealed class PlayCommandBuilder
{
    private readonly CasparClient _client;
    private readonly int _channel;
    private readonly int _layer;
    private readonly string _clip;
    private string? _transitionKind;
    private int? _transitionDuration;
    private bool _loop;

    public PlayCommandBuilder(CasparClient client, int channel, int layer, string clip)
    {
        _client = client;
        _channel = channel;
        _layer = layer;
        _clip = clip;
    }

    public PlayCommandBuilder WithTransition() => this;

    public PlayCommandBuilder Mix(int duration)
    {
        _transitionKind = "MIX";
        _transitionDuration = duration;
        return this;
    }

    public PlayCommandBuilder WithLoop()
    {
        _loop = true;
        return this;
    }

    public ValueTask SendAsync(CancellationToken cancellationToken)
        => _client.SendAsync(new PlayCommand(_channel, _layer, _clip, _transitionKind, _transitionDuration, _loop), cancellationToken);
}
```

```csharp
// src/StarDust.CasparCG.Protocol.Amcp/Commands/PlayCommand.cs
namespace StarDust.CasparCG.Protocol.Amcp.Commands;

public sealed record PlayCommand(
    int Channel,
    int Layer,
    string Clip,
    string? TransitionKind = null,
    int? TransitionDuration = null,
    bool Loop = false) : AmcpCommand
{
    public override string Serialize()
    {
        var parts = new List<string> { "PLAY", Address(Channel, Layer), Clip };
        if (TransitionKind is not null && TransitionDuration is not null)
            parts.AddRange(new[] { TransitionKind, TransitionDuration.Value.ToString() });
        if (Loop)
            parts.Add("LOOP");
        return string.Join(' ', parts) + "\r\n";
    }
}
```

```csharp
// src/StarDust.CasparCG/CasparClient.cs
using StarDust.CasparCG.Fluent;
using StarDust.CasparCG.Protocol.Amcp;

public ChannelScope Channel(int channel) => new(this, channel);

internal async ValueTask SendAsync(AmcpCommand command, CancellationToken cancellationToken)
    => _ = await _transport.SendAsync(command.Serialize(), cancellationToken);
```

- [ ] **Step 4: Run the fluent tests and the existing command tests**

Run: `rtk dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter "FluentPlayCommandBuilderTests|CasparClientCommandTests" -v minimal`

Expected: PASS with all selected tests green.

- [ ] **Step 5: Commit the fluent API slice**

```bash
rtk git add src/StarDust.CasparCG src/StarDust.CasparCG.Protocol.Amcp src/StarDust.CasparCG.UnitTests
rtk git commit -m "feat: add fluent playback command API"
```

### Task 4: Add fluent DI registration and named clients

**Files:**
- Create: `src/StarDust.CasparCG/CasparClientOptions.cs`
- Create: `src/StarDust.CasparCG.Hosting/CasparClientRegistrationBuilder.cs`
- Create: `src/StarDust.CasparCG.Hosting/NoOpAmcpTransport.cs`
- Modify: `src/StarDust.CasparCG.Hosting/ServiceCollectionExtensions.cs`
- Create: `src/StarDust.CasparCG.Hosting/NamedCasparClientFactory.cs`
- Create: `src/StarDust.CasparCG.UnitTests/HostingRegistrationTests.cs`

- [ ] **Step 1: Write failing tests for default and named client registration**

```csharp
// src/StarDust.CasparCG.UnitTests/HostingRegistrationTests.cs
using Microsoft.Extensions.DependencyInjection;
using StarDust.CasparCG;
using StarDust.CasparCG.Hosting;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class HostingRegistrationTests
{
    [Fact]
    public void AddCasparCg_default_registration_builds_default_client()
    {
        var services = new ServiceCollection();

        services.AddCasparCG()
            .ConnectTo("127.0.0.1", 5250)
            .ListenOscOn(6250);

        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<CasparClient>());
    }

    [Fact]
    public void AddCasparCg_named_registration_is_resolved_from_factory()
    {
        var services = new ServiceCollection();

        services.AddCasparCG("studio-a")
            .ConnectTo("10.0.0.10", 5250)
            .ListenOscOn(6250);

        using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<ICasparClientFactory>();

        Assert.NotNull(factory.GetClient("studio-a"));
    }
}
```

- [ ] **Step 2: Run the hosting tests to verify the fluent registration API is missing**

Run: `rtk dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter HostingRegistrationTests -v minimal`

Expected: FAIL with compile errors for the fluent registration methods and named factory behavior.

- [ ] **Step 3: Implement fluent registration options and named client resolution**

```csharp
// src/StarDust.CasparCG/CasparClientOptions.cs
namespace StarDust.CasparCG;

public sealed class CasparClientOptions
{
    public string Name { get; init; } = "default";
    public string AmcpHost { get; set; } = "127.0.0.1";
    public int AmcpPort { get; set; } = 5250;
    public int OscPort { get; set; } = 6250;
    public bool AutoReconnect { get; set; } = true;
}
```

```csharp
// src/StarDust.CasparCG.Hosting/CasparClientRegistrationBuilder.cs
using Microsoft.Extensions.DependencyInjection;
using StarDust.CasparCG;

namespace StarDust.CasparCG.Hosting;

public sealed class CasparClientRegistrationBuilder
{
    internal CasparClientRegistrationBuilder(IServiceCollection services, CasparClientOptions options)
    {
        Services = services;
        Options = options;
    }

    internal IServiceCollection Services { get; }
    internal CasparClientOptions Options { get; }

    public CasparClientRegistrationBuilder ConnectTo(string host, int port)
    {
        Options.AmcpHost = host;
        Options.AmcpPort = port;
        return this;
    }

    public CasparClientRegistrationBuilder ListenOscOn(int port)
    {
        Options.OscPort = port;
        return this;
    }

    public CasparClientRegistrationBuilder WithAutoReconnect(bool enabled = true)
    {
        Options.AutoReconnect = enabled;
        return this;
    }
}
```

```csharp
// src/StarDust.CasparCG.Hosting/NamedCasparClientFactory.cs
using StarDust.CasparCG;

namespace StarDust.CasparCG.Hosting;

internal sealed class NamedCasparClientFactory : ICasparClientFactory
{
    private readonly IReadOnlyDictionary<string, CasparClient> _clients;

    public NamedCasparClientFactory(IEnumerable<CasparClientOptions> options)
    {
        _clients = options.ToDictionary(
            x => x.Name,
            _ => new CasparClient(new NoOpAmcpTransport()));
    }

    public CasparClient GetClient(string name) => _clients[name];
}
```

```csharp
// src/StarDust.CasparCG.Hosting/NoOpAmcpTransport.cs
using StarDust.CasparCG.Transport;

namespace StarDust.CasparCG.Hosting;

internal sealed class NoOpAmcpTransport : IAmcpTransport
{
    public ValueTask ConnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
    public ValueTask DisconnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
    public ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken) => ValueTask.FromResult(string.Empty);
}
```

```csharp
// src/StarDust.CasparCG.Hosting/ServiceCollectionExtensions.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StarDust.CasparCG;

namespace StarDust.CasparCG.Hosting;

public static class ServiceCollectionExtensions
{
    public static CasparClientRegistrationBuilder AddCasparCG(this IServiceCollection services, string name = "default")
    {
        var options = new CasparClientOptions { Name = name };
        services.AddSingleton(options);
        services.TryAddSingleton<ICasparClientFactory, NamedCasparClientFactory>();
        services.TryAddSingleton(sp => sp.GetRequiredService<ICasparClientFactory>().GetClient("default"));
        return new CasparClientRegistrationBuilder(services, options);
    }
}
```

- [ ] **Step 4: Run the hosting tests and keep the registration map honest**

Run: `rtk dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter HostingRegistrationTests -v minimal`

Expected: PASS with both tests green.

- [ ] **Step 5: Commit the hosting slice**

```bash
rtk git add src/StarDust.CasparCG src/StarDust.CasparCG.Hosting src/StarDust.CasparCG.UnitTests
rtk git commit -m "feat: add fluent DI registration and named clients"
```

### Task 5: Add OSC event streaming and minimal state projection

**Files:**
- Create: `src/StarDust.CasparCG.Protocol.Osc/StarDust.CasparCG.Protocol.Osc.csproj`
- Create: `src/StarDust.CasparCG/Events/CasparEvent.cs`
- Create: `src/StarDust.CasparCG/Events/PlaybackClipChangedEvent.cs`
- Create: `src/StarDust.CasparCG/Events/CasparEventStream.cs`
- Create: `src/StarDust.CasparCG/State/CasparStateSnapshot.cs`
- Create: `src/StarDust.CasparCG/State/CasparStateStore.cs`
- Create: `src/StarDust.CasparCG.Protocol.Osc/IOscMessageMapper.cs`
- Modify: `src/StarDust.CasparCG/CasparClient.cs`
- Create: `src/StarDust.CasparCG.UnitTests/EventStreamAndStateTests.cs`

- [ ] **Step 1: Write failing tests for event streaming, filtering, and state updates**

```csharp
// src/StarDust.CasparCG.UnitTests/EventStreamAndStateTests.cs
using System.Threading.Channels;
using StarDust.CasparCG.Events;
using StarDust.CasparCG.State;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class EventStreamAndStateTests
{
    [Fact]
    public async Task Filtered_stream_returns_only_matching_events()
    {
        var source = Channel.CreateBounded<CasparEvent>(8);
        var stream = new CasparEventStream(source.Reader);

        await source.Writer.WriteAsync(new PlaybackClipChangedEvent("studio-a", 1, 10, "AMB"));
        await source.Writer.WriteAsync(new PlaybackClipChangedEvent("studio-a", 2, 5, "OTHER"));
        source.Writer.Complete();

        var result = new List<PlaybackClipChangedEvent>();
        await foreach (var evt in stream.ForChannel(1).OfType<PlaybackClipChangedEvent>().ReadAllAsync(CancellationToken.None))
            result.Add(evt);

        Assert.Single(result);
        Assert.Equal("AMB", result[0].Clip);
    }

    [Fact]
    public void State_store_tracks_last_clip_per_layer()
    {
        var state = new CasparStateStore();

        state.Apply(new PlaybackClipChangedEvent("studio-a", 1, 10, "AMB"));

        Assert.Equal("AMB", state.GetSnapshot().Channels[1].Layers[10].Clip);
    }
}
```

- [ ] **Step 2: Run the event/state tests and confirm the new read models do not exist**

Run: `rtk dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter EventStreamAndStateTests -v minimal`

Expected: FAIL with compile errors for `CasparEvent`, `CasparEventStream`, and `CasparStateStore`.

- [ ] **Step 3: Implement the smallest event and state surface**

```csharp
// src/StarDust.CasparCG/Events/CasparEvent.cs
namespace StarDust.CasparCG.Events;

public abstract record CasparEvent(string ClientName, int Channel, int Layer);
```

```csharp
// src/StarDust.CasparCG/Events/PlaybackClipChangedEvent.cs
namespace StarDust.CasparCG.Events;

public sealed record PlaybackClipChangedEvent(string ClientName, int Channel, int Layer, string Clip)
    : CasparEvent(ClientName, Channel, Layer);
```

```csharp
// src/StarDust.CasparCG/Events/CasparEventStream.cs
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace StarDust.CasparCG.Events;

public sealed class CasparEventStream
{
    private readonly ChannelReader<CasparEvent> _reader;
    private readonly Func<CasparEvent, bool> _predicate;

    public CasparEventStream(ChannelReader<CasparEvent> reader, Func<CasparEvent, bool>? predicate = null)
    {
        _reader = reader;
        _predicate = predicate ?? (_ => true);
    }

    public CasparEventStream ForChannel(int channel) => new(_reader, evt => _predicate(evt) && evt.Channel == channel);
    public CasparEventStream ForLayer(int layer) => new(_reader, evt => _predicate(evt) && evt.Layer == layer);

    public TypedCasparEventStream<TEvent> OfType<TEvent>() where TEvent : CasparEvent
        => new(_reader, evt => _predicate(evt) && evt is TEvent);

    public async IAsyncEnumerable<CasparEvent> ReadAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var evt in _reader.ReadAllAsync(cancellationToken))
            if (_predicate(evt))
                yield return evt;
    }
}

public sealed class TypedCasparEventStream<TEvent> where TEvent : CasparEvent
{
    private readonly ChannelReader<CasparEvent> _reader;
    private readonly Func<CasparEvent, bool> _predicate;

    public TypedCasparEventStream(ChannelReader<CasparEvent> reader, Func<CasparEvent, bool> predicate)
    {
        _reader = reader;
        _predicate = predicate;
    }

    public async IAsyncEnumerable<TEvent> ReadAllAsync([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var evt in _reader.ReadAllAsync(cancellationToken))
            if (_predicate(evt))
                yield return (TEvent)evt;
    }
}
```

```csharp
// src/StarDust.CasparCG.Protocol.Osc/IOscMessageMapper.cs
using StarDust.CasparCG.Events;

namespace StarDust.CasparCG.Protocol.Osc;

public interface IOscMessageMapper
{
    bool TryMap(string address, IReadOnlyList<object?> arguments, out CasparEvent? evt);
}
```

```csharp
// src/StarDust.CasparCG/State/CasparStateSnapshot.cs
namespace StarDust.CasparCG.State;

public sealed record CasparStateSnapshot(IReadOnlyDictionary<int, ChannelStateSnapshot> Channels);
public sealed record ChannelStateSnapshot(IReadOnlyDictionary<int, LayerStateSnapshot> Layers);
public sealed record LayerStateSnapshot(string Clip);
```

```csharp
// src/StarDust.CasparCG/State/CasparStateStore.cs
using StarDust.CasparCG.Events;

namespace StarDust.CasparCG.State;

public sealed class CasparStateStore
{
    private readonly Dictionary<int, Dictionary<int, LayerStateSnapshot>> _channels = new();

    public void Apply(CasparEvent evt)
    {
        if (evt is PlaybackClipChangedEvent clipChanged)
        {
            var layers = _channels.TryGetValue(clipChanged.Channel, out var existing)
                ? existing
                : _channels[clipChanged.Channel] = new Dictionary<int, LayerStateSnapshot>();
            layers[clipChanged.Layer] = new LayerStateSnapshot(clipChanged.Clip);
        }
    }

    public CasparStateSnapshot GetSnapshot()
        => new(_channels.ToDictionary(x => x.Key, x => new ChannelStateSnapshot(x.Value)));
}
```

- [ ] **Step 4: Expose `client.Events` and `client.State`, then rerun the tests**

```csharp
// src/StarDust.CasparCG/CasparClient.cs
using System.Threading.Channels;
using StarDust.CasparCG.Events;
using StarDust.CasparCG.State;

private readonly Channel<CasparEvent> _eventChannel = Channel.CreateBounded<CasparEvent>(256);
private readonly CasparStateStore _state = new();

public CasparEventStream Events => new(_eventChannel.Reader);
public CasparStateStore State => _state;

internal ValueTask PublishAsync(CasparEvent evt, CancellationToken cancellationToken)
{
    _state.Apply(evt);
    return _eventChannel.Writer.WriteAsync(evt, cancellationToken);
}
```

Run:

```bash
rtk dotnet add src/StarDust.CasparCG/StarDust.CasparCG.csproj reference src/StarDust.CasparCG.Protocol.Osc/StarDust.CasparCG.Protocol.Osc.csproj
rtk dotnet sln src/StarDust.CasparCG.net.sln add src/StarDust.CasparCG.Protocol.Osc/StarDust.CasparCG.Protocol.Osc.csproj
rtk dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter EventStreamAndStateTests -v minimal
```

Expected: PASS with event filtering and state snapshot assertions green.

- [ ] **Step 5: Commit the event/state slice**

```bash
rtk git add src/StarDust.CasparCG src/StarDust.CasparCG.Protocol.Osc src/StarDust.CasparCG.UnitTests src/StarDust.CasparCG.net.sln
rtk git commit -m "feat: add event stream and state projection"
```

### Task 6: Implement health, diagnostics, and reconnect supervision

**Files:**
- Create: `src/StarDust.CasparCG/Health/ConnectionHealthStatus.cs`
- Create: `src/StarDust.CasparCG/Diagnostics/ClientDiagnostics.cs`
- Modify: `src/StarDust.CasparCG/CasparClient.cs`
- Create: `src/StarDust.CasparCG.UnitTests/HealthAndReconnectTests.cs`

- [ ] **Step 1: Write failing tests for health transitions and reconnect accounting**

```csharp
// src/StarDust.CasparCG.UnitTests/HealthAndReconnectTests.cs
using StarDust.CasparCG;
using StarDust.CasparCG.Health;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class HealthAndReconnectTests
{
    [Fact]
    public async Task ConnectAsync_sets_health_to_connected()
    {
        var transport = new ToggleTransport();
        var client = new CasparClient(transport);

        await client.ConnectAsync(CancellationToken.None);

        Assert.Equal(ConnectionHealthStatus.Connected, client.HealthStatus);
    }

    [Fact]
    public async Task Failed_send_records_reconnect_attempt_and_fault_reason()
    {
        var transport = new ToggleTransport(failSend: true);
        var client = new CasparClient(transport);

        await Assert.ThrowsAsync<InvalidOperationException>(() => client.PlayAsync(1, 10, "AMB", CancellationToken.None).AsTask());

        Assert.Equal(ConnectionHealthStatus.Faulted, client.HealthStatus);
        Assert.Equal(1, client.Diagnostics.ReconnectCount);
        Assert.NotNull(client.Diagnostics.LastFailure);
    }

    private sealed class ToggleTransport : IAmcpTransport
    {
        private readonly bool _failSend;
        public ToggleTransport(bool failSend = false) => _failSend = failSend;
        public ValueTask ConnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
        public ValueTask DisconnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
        public ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken)
            => _failSend ? ValueTask.FromException<string>(new InvalidOperationException("send failed")) : ValueTask.FromResult("202 OK");
    }
}
```

- [ ] **Step 2: Run the health tests to verify health and diagnostics are absent**

Run: `rtk dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter HealthAndReconnectTests -v minimal`

Expected: FAIL with compile errors for `HealthStatus`, `Diagnostics`, and reconnect handling.

- [ ] **Step 3: Implement the minimal health model and fault accounting**

```csharp
// src/StarDust.CasparCG/Health/ConnectionHealthStatus.cs
namespace StarDust.CasparCG.Health;

public enum ConnectionHealthStatus
{
    Disconnected,
    Connecting,
    Connected,
    Degraded,
    Reconnecting,
    Faulted,
    Stopping
}
```

```csharp
// src/StarDust.CasparCG/Diagnostics/ClientDiagnostics.cs
namespace StarDust.CasparCG.Diagnostics;

public sealed class ClientDiagnostics
{
    public DateTimeOffset? LastSuccessfulAmcpInteraction { get; set; }
    public Exception? LastFailure { get; set; }
    public int ReconnectCount { get; set; }
}
```

```csharp
// src/StarDust.CasparCG/CasparClient.cs
using StarDust.CasparCG.Diagnostics;
using StarDust.CasparCG.Health;

public ConnectionHealthStatus HealthStatus { get; private set; } = ConnectionHealthStatus.Disconnected;
public ClientDiagnostics Diagnostics { get; } = new();

public async ValueTask ConnectAsync(CancellationToken cancellationToken)
{
    HealthStatus = ConnectionHealthStatus.Connecting;
    await _transport.ConnectAsync(cancellationToken);
    HealthStatus = ConnectionHealthStatus.Connected;
    Diagnostics.LastSuccessfulAmcpInteraction = DateTimeOffset.UtcNow;
}

internal async ValueTask SendAsync(AmcpCommand command, CancellationToken cancellationToken)
{
    try
    {
        await _transport.SendAsync(command.Serialize(), cancellationToken);
        Diagnostics.LastSuccessfulAmcpInteraction = DateTimeOffset.UtcNow;
    }
    catch (Exception ex)
    {
        HealthStatus = ConnectionHealthStatus.Faulted;
        Diagnostics.LastFailure = ex;
        Diagnostics.ReconnectCount++;
        throw;
    }
}
```

- [ ] **Step 4: Run the health tests and then the full unit test suite**

Run:

```bash
rtk dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter HealthAndReconnectTests -v minimal
rtk dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -v minimal
```

Expected: PASS. The dedicated health tests and the full unit test project should both be green.

- [ ] **Step 5: Commit the health/reconnect slice**

```bash
rtk git add src/StarDust.CasparCG src/StarDust.CasparCG.Transport src/StarDust.CasparCG.UnitTests
rtk git commit -m "feat: add health model and reconnect diagnostics"
```

### Task 7: Add `DummyServer` and socket-level integration tests

**Files:**
- Create: `src/StarDust.CasparCG.Testing/StarDust.CasparCG.Testing.csproj`
- Create: `src/StarDust.CasparCG.Testing/DummyServer/DummyCasparServer.cs`
- Create: `src/StarDust.CasparCG.Testing/DummyServer/DummyScenario.cs`
- Create: `src/StarDust.CasparCG.Transport/TcpAmcpTransport.cs`
- Create: `src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj`
- Create: `src/StarDust.CasparCG.IntegrationTests/CasparClientDummyServerTests.cs`
- Modify: `src/StarDust.CasparCG.net.sln`

- [ ] **Step 1: Write a failing integration test that drives `CasparClient` against a `DummyServer`**

```csharp
// src/StarDust.CasparCG.IntegrationTests/CasparClientDummyServerTests.cs
using StarDust.CasparCG;
using StarDust.CasparCG.Testing.DummyServer;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.IntegrationTests;

public class CasparClientDummyServerTests
{
    [Fact]
    public async Task PlayAsync_sends_real_tcp_command_to_dummy_server()
    {
        await using var server = await DummyCasparServer.StartAsync(
            DummyScenario.Empty().WithAmcpReply("PLAY 1-10 AMB", "202 PLAY OK\r\n"),
            CancellationToken.None);

        var client = new CasparClient(new TcpAmcpTransport("127.0.0.1", server.AmcpPort));
        await client.ConnectAsync(CancellationToken.None);
        await client.PlayAsync(1, 10, "AMB", CancellationToken.None);

        Assert.Contains("PLAY 1-10 AMB", server.ReceivedCommands);
    }
}
```

- [ ] **Step 2: Run the integration test to confirm the socket infrastructure is missing**

Run: `rtk dotnet test src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --filter CasparClientDummyServerTests -v minimal`

Expected: FAIL with missing project/type errors for `DummyCasparServer`, `DummyScenario`, and `TcpAmcpTransport`.

- [ ] **Step 3: Implement the minimal scripted `DummyServer` and integration test projects**

```xml
<!-- src/StarDust.CasparCG.Testing/StarDust.CasparCG.Testing.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <ItemGroup>
    <ProjectReference Include="..\StarDust.CasparCG\StarDust.CasparCG.csproj" />
    <ProjectReference Include="..\StarDust.CasparCG.Transport\StarDust.CasparCG.Transport.csproj" />
  </ItemGroup>
</Project>
```

```csharp
// src/StarDust.CasparCG.Testing/DummyServer/DummyScenario.cs
namespace StarDust.CasparCG.Testing.DummyServer;

public sealed class DummyScenario
{
    private readonly Dictionary<string, string> _amcpReplies = new(StringComparer.OrdinalIgnoreCase);

    public static DummyScenario Empty() => new();

    public DummyScenario WithAmcpReply(string command, string reply)
    {
        _amcpReplies[command] = reply;
        return this;
    }

    internal bool TryGetReply(string command, out string reply) => _amcpReplies.TryGetValue(command, out reply!);
}
```

```csharp
// src/StarDust.CasparCG.Transport/TcpAmcpTransport.cs
using System.Net.Sockets;
using System.Text;

namespace StarDust.CasparCG.Transport;

public sealed class TcpAmcpTransport : IAmcpTransport, IAsyncDisposable
{
    private readonly string _host;
    private readonly int _port;
    private TcpClient? _client;
    private NetworkStream? _stream;

    public TcpAmcpTransport(string host, int port)
    {
        _host = host;
        _port = port;
    }

    public async ValueTask ConnectAsync(CancellationToken cancellationToken)
    {
        _client = new TcpClient();
        await _client.ConnectAsync(_host, _port, cancellationToken);
        _stream = _client.GetStream();
    }

    public ValueTask DisconnectAsync(CancellationToken cancellationToken)
    {
        _stream?.Dispose();
        _client?.Dispose();
        return ValueTask.CompletedTask;
    }

    public async ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(_stream);
        var bytes = Encoding.UTF8.GetBytes(commandText);
        await _stream.WriteAsync(bytes, cancellationToken);
        return "202 OK";
    }

    public ValueTask DisposeAsync() => DisconnectAsync(CancellationToken.None);
}
```

```csharp
// src/StarDust.CasparCG.Testing/DummyServer/DummyCasparServer.cs
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace StarDust.CasparCG.Testing.DummyServer;

public sealed class DummyCasparServer : IAsyncDisposable
{
    private readonly TcpListener _listener;
    private readonly DummyScenario _scenario;
    private readonly List<string> _receivedCommands = new();

    private DummyCasparServer(TcpListener listener, DummyScenario scenario)
    {
        _listener = listener;
        _scenario = scenario;
    }

    public int AmcpPort => ((IPEndPoint)_listener.LocalEndpoint).Port;
    public int OscPort => 6250;
    public IReadOnlyList<string> ReceivedCommands => _receivedCommands;

    public static async ValueTask<DummyCasparServer> StartAsync(DummyScenario scenario, CancellationToken cancellationToken)
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var server = new DummyCasparServer(listener, scenario);
        _ = server.AcceptLoopAsync(cancellationToken);
        await ValueTask.CompletedTask;
        return server;
    }

    private async Task AcceptLoopAsync(CancellationToken cancellationToken)
    {
        using var client = await _listener.AcceptTcpClientAsync(cancellationToken);
        using var stream = client.GetStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);
        using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (line is null)
                break;

            _receivedCommands.Add(line);

            if (_scenario.TryGetReply(line, out var reply))
                await writer.WriteAsync(reply);
        }
    }

    public ValueTask DisposeAsync()
    {
        _listener.Stop();
        return ValueTask.CompletedTask;
    }
}
```

```xml
<!-- src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\StarDust.CasparCG\StarDust.CasparCG.csproj" />
    <ProjectReference Include="..\StarDust.CasparCG.Testing\StarDust.CasparCG.Testing.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.1" />
    <PackageReference Include="xunit" Version="2.9.2" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.8.2">
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>
</Project>
```

- [ ] **Step 4: Wire the projects into the solution and make the integration test pass**

Run:

```bash
rtk dotnet sln src/StarDust.CasparCG.net.sln add src/StarDust.CasparCG.Testing/StarDust.CasparCG.Testing.csproj
rtk dotnet sln src/StarDust.CasparCG.net.sln add src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj
rtk dotnet test src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --filter CasparClientDummyServerTests -v minimal
```

Expected: PASS with the test proving that `CasparClient` can talk to a real loopback TCP server.

- [ ] **Step 5: Commit the integration test slice**

```bash
rtk git add src/StarDust.CasparCG.Testing src/StarDust.CasparCG.IntegrationTests src/StarDust.CasparCG.net.sln
rtk git commit -m "test: add DummyServer integration coverage"
```

### Task 8: Rewrite developer docs and the breaking-changes guide

**Files:**
- Modify: `README.md`
- Create: `BREAKING_CHANGES.md`
- Create: `CONTRIBUTING.md`
- Create: `docs/vnext/getting-started.md`
- Create: `docs/vnext/fluent-api-cookbook.md`
- Create: `docs/vnext/events-and-state.md`
- Create: `docs/vnext/hosting-and-di.md`
- Create: `docs/vnext/testing-with-dummy-server.md`
- Create: `scripts/verify-docs.sh`

- [ ] **Step 1: Write a failing docs completeness check**

```bash
#!/usr/bin/env bash
set -euo pipefail

required_files=(
  README.md
  BREAKING_CHANGES.md
  CONTRIBUTING.md
  docs/vnext/getting-started.md
  docs/vnext/fluent-api-cookbook.md
  docs/vnext/events-and-state.md
  docs/vnext/hosting-and-di.md
  docs/vnext/testing-with-dummy-server.md
)

for file in "${required_files[@]}"; do
  [[ -f "$file" ]] || { echo "missing $file"; exit 1; }
done

grep -q "Breaking changes from the current release" BREAKING_CHANGES.md
grep -q "AddCasparCG" README.md
grep -q "DummyServer" docs/vnext/testing-with-dummy-server.md
```

- [ ] **Step 2: Run the docs check to verify the required files do not exist yet**

Run:

```bash
chmod +x scripts/verify-docs.sh
rtk ./scripts/verify-docs.sh
```

Expected: FAIL because the vNext docs files and `BREAKING_CHANGES.md` do not exist yet.

- [ ] **Step 3: Write the docs set with concrete vNext examples**

````markdown
<!-- BREAKING_CHANGES.md -->
# Breaking changes from the current release

## Public API redesign

- `ICasparDevice` is replaced by `CasparClient`
- synchronous public command methods are removed
- DI registration now starts with `AddCasparCG()` fluent configuration
- OSC domain notifications now flow through `client.Events`
- legacy manager-centric navigation is replaced by fluent channel/layer command scopes

## Migration example

### Before

```csharp
services.AddCasparCG();
var device = provider.GetRequiredService<ICasparDevice>();
device.Connect("127.0.0.1");
```

### After

```csharp
services.AddCasparCG().ConnectTo("127.0.0.1", 5250).ListenOscOn(6250);
var client = provider.GetRequiredService<CasparClient>();
await client.ConnectAsync(ct);
```
````

````markdown
<!-- docs/vnext/getting-started.md -->
# Getting started

```csharp
builder.Services
    .AddCasparCG()
    .ConnectTo("127.0.0.1", 5250)
    .ListenOscOn(6250);

var client = app.Services.GetRequiredService<CasparClient>();
await client.ConnectAsync(ct);
await client.PlayAsync(1, 10, "AMB", ct);
```
````

````markdown
<!-- docs/vnext/events-and-state.md -->
# Events and state

```csharp
await foreach (var evt in client.Events.ForChannel(1).ReadAllAsync(ct))
{
    Console.WriteLine(evt);
}

var snapshot = client.State.GetSnapshot();
```
````
```

- [ ] **Step 4: Run the docs completeness check again**

Run: `rtk ./scripts/verify-docs.sh`

Expected: PASS. Follow that immediately with `rtk git diff --check` and expect no whitespace errors.

- [ ] **Step 5: Commit the docs slice**

```bash
rtk git add README.md BREAKING_CHANGES.md CONTRIBUTING.md docs/vnext scripts/verify-docs.sh
rtk git commit -m "docs: add vNext migration and developer guides"
```

### Task 9: Add modern GitHub Actions CI

**Files:**
- Create: `.github/workflows/ci.yml`
- Create: `.github/workflows/package.yml`
- Create: `scripts/verify-workflows.sh`
- Modify: `CONTRIBUTING.md`

- [ ] **Step 1: Write a failing workflow sanity check**

```bash
#!/usr/bin/env bash
set -euo pipefail

grep -q "actions/checkout@v4" .github/workflows/ci.yml
grep -q "actions/setup-dotnet@v4" .github/workflows/ci.yml
grep -q "dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj" .github/workflows/ci.yml
grep -q "dotnet test src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj" .github/workflows/ci.yml
grep -q "permissions:" .github/workflows/ci.yml
grep -q "concurrency:" .github/workflows/ci.yml
grep -q "upload-artifact" .github/workflows/ci.yml
grep -q "workflow_dispatch:" .github/workflows/package.yml
```

- [ ] **Step 2: Run the workflow check to verify the new CI does not exist yet**

Run:

```bash
chmod +x scripts/verify-workflows.sh
rtk ./scripts/verify-workflows.sh
```

Expected: FAIL because the workflow files do not exist yet.

- [ ] **Step 3: Add `ci.yml`, `package.yml`, and contributor notes**

```yaml
# .github/workflows/ci.yml
name: ci

on:
  pull_request:
  push:
    branches: [ master, main, design/**, feature/** ]

permissions:
  contents: read

concurrency:
  group: ci-${{ github.ref }}
  cancel-in-progress: true

jobs:
  build-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 10.0.x
          cache: true
      - run: dotnet restore src/StarDust.CasparCG.net.sln
      - run: dotnet build src/StarDust.CasparCG.net.sln --configuration Release --no-restore
      - run: dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --configuration Release --no-build --logger trx
      - run: dotnet test src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --configuration Release --no-build --logger trx
      - uses: actions/upload-artifact@v4
        if: always()
        with:
          name: test-results
          path: "**/*.trx"
```

```yaml
# .github/workflows/package.yml
name: package

on:
  workflow_dispatch:
  push:
    tags:
      - "v*"

permissions:
  contents: read
  packages: write

jobs:
  pack:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 10.0.x
          cache: true
      - run: dotnet pack src/StarDust.CasparCG/StarDust.CasparCG.csproj --configuration Release -o artifacts/packages
      - uses: actions/upload-artifact@v4
        with:
          name: packages
          path: artifacts/packages
```

```markdown
<!-- CONTRIBUTING.md -->
# Contributing

## Validation

- run `rtk dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj`
- run `rtk dotnet test src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj`
- run `rtk ./scripts/verify-docs.sh`
- run `rtk ./scripts/verify-workflows.sh`
```

- [ ] **Step 4: Run the workflow sanity check and a local build**

Run:

```bash
rtk ./scripts/verify-workflows.sh
rtk dotnet build src/StarDust.CasparCG.net.sln --configuration Release
```

Expected: PASS. The workflow script should succeed and the solution should still build locally.

- [ ] **Step 5: Commit the CI slice**

```bash
rtk git add .github/workflows CONTRIBUTING.md scripts/verify-workflows.sh
rtk git commit -m "ci: add modern GitHub Actions workflows"
```

### Task 10: Final verification and release-readiness pass

**Files:**
- Modify: `README.md`
- Modify: `BREAKING_CHANGES.md`
- Modify: `CONTRIBUTING.md`
- Modify: `docs/vnext/*.md` as needed after verification feedback

- [ ] **Step 1: Run the full verification matrix**

Run:

```bash
rtk dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --configuration Release
rtk dotnet test src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --configuration Release
rtk ./scripts/verify-docs.sh
rtk ./scripts/verify-workflows.sh
rtk dotnet pack src/StarDust.CasparCG/StarDust.CasparCG.csproj --configuration Release -o artifacts/packages
```

Expected: PASS end-to-end. Treat any flaky integration behavior as a bug in the `DummyServer` harness or reconnect logic and fix it before continuing.

- [ ] **Step 2: Add any final doc corrections uncovered by verification**

````markdown
<!-- Example README quick-start section -->
## Quick start

```csharp
builder.Services
    .AddCasparCG()
    .ConnectTo("127.0.0.1", 5250)
    .ListenOscOn(6250);
```
````

- [ ] **Step 3: Re-run the focused checks after doc edits**

Run:

```bash
rtk ./scripts/verify-docs.sh
rtk dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --configuration Release
```

Expected: PASS.

- [ ] **Step 4: Review the branch diff for accidental legacy churn**

Run: `rtk git diff --stat master...HEAD`

Expected: The diff should show the new vNext projects, tests, docs, scripts, and workflows. If unrelated legacy files moved unexpectedly, stop and clean them up before release prep.

- [ ] **Step 5: Commit the final verification fixes**

```bash
rtk git add README.md BREAKING_CHANGES.md CONTRIBUTING.md docs/vnext scripts .github/workflows
rtk git commit -m "chore: finalize vNext phase 1 verification"
```
