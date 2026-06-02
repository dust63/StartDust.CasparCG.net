# CasparCG Structure Cleanup Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** remove obsolete legacy projects from the repository and merge the active runtime split into a single `StarDust.CasparCG` project.

**Architecture:** keep `StarDust.CasparCG` as the only runtime assembly, move AMCP/OSC/transport source files into internal folders under that project, and preserve public namespaces to avoid unnecessary API breakage. Validate project-by-project because solution-level restore currently fails in this environment without actionable Roslyn diagnostics.

**Tech Stack:** .NET 10, C#, SDK-style projects, xUnit, `dotnet build`, `dotnet test`

---

### Task 1: Merge `Protocol.Amcp` into `StarDust.CasparCG`

**Files:**
- Move: `src/StarDust.CasparCG.Protocol.Amcp/AmcpCommand.cs`
- Move: `src/StarDust.CasparCG.Protocol.Amcp/AmcpCommandException.cs`
- Move: `src/StarDust.CasparCG.Protocol.Amcp/AmcpResponse.cs`
- Move: `src/StarDust.CasparCG.Protocol.Amcp/AmcpResponseParser.cs`
- Move: `src/StarDust.CasparCG.Protocol.Amcp/AmcpStatusCategory.cs`
- Move: `src/StarDust.CasparCG.Protocol.Amcp/Commands/*.cs`
- Modify: `src/StarDust.CasparCG/StarDust.CasparCG.csproj`
- Modify: `src/StarDust.CasparCG/CasparClient.cs`
- Modify: `src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj`

- [ ] **Step 1: Write the failing reference test**

Add one assertion in `src/StarDust.CasparCG.UnitTests/CasparClientCommandTests.cs` that still references an AMCP command type through the existing namespace:

```csharp
[Fact]
public void Amcp_namespace_remains_available_after_merge()
{
    var command = new StarDust.CasparCG.Protocol.Amcp.Commands.VersionCommand("SERVER");

    Assert.Equal("VERSION SERVER\r\n", command.Serialize());
}
```

- [ ] **Step 2: Run the focused AMCP unit tests**

Run: `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "CasparClientCommandTests|AmcpResponseParserTests" -v minimal`

Expected: PASS before the move, proving the baseline is green.

- [ ] **Step 3: Move the AMCP source files into `StarDust.CasparCG`**

Create these target folders if needed:

```text
src/StarDust.CasparCG/Protocol/Amcp/
src/StarDust.CasparCG/Protocol/Amcp/Commands/
```

Move the files listed in the task file list into those folders without changing their namespaces.

- [ ] **Step 4: Remove the project reference and compile ownership**

Update `src/StarDust.CasparCG/StarDust.CasparCG.csproj` so it no longer references `..\StarDust.CasparCG.Protocol.Amcp\StarDust.CasparCG.Protocol.Amcp.csproj`.

The project reference block should become:

```xml
<ItemGroup>
  <ProjectReference Include="..\StarDust.CasparCG.Protocol.Osc\StarDust.CasparCG.Protocol.Osc.csproj" />
  <ProjectReference Include="..\StarDust.CasparCG.Transport\StarDust.CasparCG.Transport.csproj" />
  <None Include="..\..\README.md" Pack="true" PackagePath="" />
</ItemGroup>
```

- [ ] **Step 5: Update test project references if needed**

If `src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj` still references `StarDust.CasparCG.Protocol.Amcp`, remove that project reference and rely on `StarDust.CasparCG` instead.

- [ ] **Step 6: Re-run the focused AMCP tests**

Run: `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "CasparClientCommandTests|AmcpResponseParserTests" -v minimal`

Expected: PASS.

- [ ] **Step 7: Commit**

```bash
git add src/StarDust.CasparCG/Protocol/Amcp src/StarDust.CasparCG/StarDust.CasparCG.csproj src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj src/StarDust.CasparCG.UnitTests/CasparClientCommandTests.cs
git commit -m "refactor: merge amcp protocol into runtime project"
```

### Task 2: Merge `Protocol.Osc` into `StarDust.CasparCG`

**Files:**
- Move: `src/StarDust.CasparCG.Protocol.Osc/OscMessage.cs`
- Move: `src/StarDust.CasparCG.Protocol.Osc/OscPacketParser.cs`
- Modify: `src/StarDust.CasparCG/StarDust.CasparCG.csproj`
- Modify: `src/StarDust.CasparCG/CasparClient.cs`
- Modify: `src/StarDust.CasparCG.UnitTests/OscMessageParserTests.cs`
- Modify: `src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj`

- [ ] **Step 1: Write the failing namespace-preservation test**

Add one assertion in `src/StarDust.CasparCG.UnitTests/OscMessageParserTests.cs`:

```csharp
[Fact]
public void Osc_namespace_remains_available_after_merge()
{
    var packets = StarDust.CasparCG.Protocol.Osc.OscPacketParser.ParseMessages(BuildOscMessage("/test", ",s", "ok"));

    Assert.Single(packets);
}
```

- [ ] **Step 2: Run the focused OSC tests**

Run: `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "OscMessageParserTests|OscClientTests" -v minimal`

Expected: PASS before the move.

- [ ] **Step 3: Move the OSC source files into `StarDust.CasparCG`**

Create:

```text
src/StarDust.CasparCG/Protocol/Osc/
```

Move the two OSC source files into that folder without changing namespaces.

- [ ] **Step 4: Remove the `Protocol.Osc` project reference**

Update `src/StarDust.CasparCG/StarDust.CasparCG.csproj` so it no longer references `..\StarDust.CasparCG.Protocol.Osc\StarDust.CasparCG.Protocol.Osc.csproj`.

- [ ] **Step 5: Update unit test project references if needed**

Remove any explicit `StarDust.CasparCG.Protocol.Osc` project reference from `src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj` if it still exists.

- [ ] **Step 6: Re-run the focused OSC tests**

Run: `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "OscMessageParserTests|OscClientTests" -v minimal`

Expected: PASS.

- [ ] **Step 7: Commit**

```bash
git add src/StarDust.CasparCG/Protocol/Osc src/StarDust.CasparCG/StarDust.CasparCG.csproj src/StarDust.CasparCG.UnitTests/OscMessageParserTests.cs src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj
git commit -m "refactor: merge osc protocol into runtime project"
```

### Task 3: Merge `Transport` into `StarDust.CasparCG`

**Files:**
- Move: `src/StarDust.CasparCG.Transport/IOscTransport.cs`
- Move: `src/StarDust.CasparCG.Transport/IAmcpTransport.cs`
- Move: `src/StarDust.CasparCG.Transport/TcpAmcpTransport.cs`
- Move: `src/StarDust.CasparCG.Transport/UdpOscTransport.cs`
- Modify: `src/StarDust.CasparCG/StarDust.CasparCG.csproj`
- Modify: `src/StarDust.CasparCG.Hosting/StarDust.CasparCG.Hosting.csproj`
- Modify: `src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj`
- Modify: `src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj`

- [ ] **Step 1: Write the failing transport ownership test**

Add one assertion in `src/StarDust.CasparCG.UnitTests/CasparClientBootstrapTests.cs`:

```csharp
[Fact]
public void Transport_namespace_remains_available_after_merge()
{
    Assert.Equal(typeof(StarDust.CasparCG.Transport.TcpAmcpTransport).Namespace, "StarDust.CasparCG.Transport");
}
```

- [ ] **Step 2: Run the focused transport-related tests**

Run: `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "CasparClientBootstrapTests|OscClientTests|CasparClientCommandTests" -v minimal`

Expected: PASS before the move.

- [ ] **Step 3: Move the transport source files into `StarDust.CasparCG`**

Create:

```text
src/StarDust.CasparCG/Transport/
```

Move the transport files into that folder without changing namespaces.

- [ ] **Step 4: Remove the `Transport` project reference from dependent projects**

Update:

- `src/StarDust.CasparCG/StarDust.CasparCG.csproj`
- `src/StarDust.CasparCG.Hosting/StarDust.CasparCG.Hosting.csproj`
- `src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj`
- `src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj`

to stop referencing `..\StarDust.CasparCG.Transport\StarDust.CasparCG.Transport.csproj` and reference `StarDust.CasparCG` only where appropriate.

- [ ] **Step 5: Re-run focused runtime tests**

Run:

```bash
dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "CasparClientBootstrapTests|OscClientTests|CasparClientCommandTests" -v minimal
dotnet test src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj -c Release -v minimal
```

Expected: PASS.

- [ ] **Step 6: Commit**

```bash
git add src/StarDust.CasparCG/Transport src/StarDust.CasparCG/StarDust.CasparCG.csproj src/StarDust.CasparCG.Hosting/StarDust.CasparCG.Hosting.csproj src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj src/StarDust.CasparCG.UnitTests/CasparClientBootstrapTests.cs
git commit -m "refactor: merge transports into runtime project"
```

### Task 4: Remove merged runtime projects from the solution and filesystem

**Files:**
- Modify: `src/StarDust.CasparCG.net.sln`
- Delete: `src/StarDust.CasparCG.Protocol.Amcp/StarDust.CasparCG.Protocol.Amcp.csproj`
- Delete: `src/StarDust.CasparCG.Protocol.Osc/StarDust.CasparCG.Protocol.Osc.csproj`
- Delete: `src/StarDust.CasparCG.Transport/StarDust.CasparCG.Transport.csproj`
- Delete remaining now-empty project folders as appropriate

- [ ] **Step 1: Verify the runtime code already builds from `StarDust.CasparCG`**

Run:

```bash
dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release -v minimal
dotnet test src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj -c Release -v minimal
```

Expected: PASS before removing the old project containers.

- [ ] **Step 2: Remove the three merged projects from the solution**

Run:

```bash
dotnet sln src/StarDust.CasparCG.net.sln remove src/StarDust.CasparCG.Protocol.Amcp/StarDust.CasparCG.Protocol.Amcp.csproj
dotnet sln src/StarDust.CasparCG.net.sln remove src/StarDust.CasparCG.Protocol.Osc/StarDust.CasparCG.Protocol.Osc.csproj
dotnet sln src/StarDust.CasparCG.net.sln remove src/StarDust.CasparCG.Transport/StarDust.CasparCG.Transport.csproj
```

- [ ] **Step 3: Delete the obsolete merged project containers**

Delete the three `.csproj` files and any leftover empty project folders that no longer contain source files after the move.

- [ ] **Step 4: Verify the solution project list**

Run: `dotnet sln src/StarDust.CasparCG.net.sln list`

Expected list:

```text
StarDust.CasparCG.Hosting/StarDust.CasparCG.Hosting.csproj
StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj
StarDust.CasparCG.Testing/StarDust.CasparCG.Testing.csproj
StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj
StarDust.CasparCG/StarDust.CasparCG.csproj
```

- [ ] **Step 5: Commit**

```bash
git add src/StarDust.CasparCG.net.sln src/StarDust.CasparCG.Protocol.Amcp src/StarDust.CasparCG.Protocol.Osc src/StarDust.CasparCG.Transport
git commit -m "chore: remove merged runtime projects"
```

### Task 5: Delete obsolete legacy projects from the repository

**Files:**
- Delete: `src/StarDust.CasparCG.net.Connection/`
- Delete: `src/StarDust.CasparCG.net.Microsoft.DependencyInjection/`
- Delete: `src/StarDust.CasparCG.net.Models/`
- Delete: `src/StarDust.CasparCG.net.OSC/`
- Delete: `src/StarDust.CasparCG.net.OSC.EventHub/`
- Delete: `src/StarDust.CasparCg.net.AmcpProtocol/`
- Delete: `src/StarDust.CasparCg.net.Device/`
- Delete: `src/StartDust.CasparCG.net.Crosscutting/`
- Delete: `src/StartDust.CasparCG.net.OSC.UnitTest/`
- Delete: `src/StartDust.CasparCG.net.UnitTest/`
- Modify any remaining docs/demos/tests that still reference them

- [ ] **Step 1: Search for remaining references to legacy project names**

Run:

```bash
grep -RIn "StarDust.CasparCG.net.Connection\|StarDust.CasparCG.net.Models\|StarDust.CasparCG.net.OSC\|StarDust.CasparCg.net.AmcpProtocol\|StarDust.CasparCg.net.Device\|StartDust.CasparCG.net.Crosscutting" . | sed -n '1,200p'
```

Expected: a small list of stale references to clean up.

- [ ] **Step 2: Remove or update stale references**

Clean any README, docs, scripts, or demo references that still point to deleted legacy project paths.

- [ ] **Step 3: Delete the legacy project directories**

Delete the directories listed in the task file list once references are clean.

- [ ] **Step 4: Verify the repo no longer contains the legacy project files**

Run:

```bash
find src -maxdepth 2 -name '*.csproj' | sort
```

Expected: only the reduced active vNext set plus demos that are intentionally still kept.

- [ ] **Step 5: Commit**

```bash
git add src README.md BREAKING_CHANGES.md CONTRIBUTING.md docs
git commit -m "chore: remove obsolete legacy projects"
```

### Task 6: Update documentation and run final validation

**Files:**
- Modify: `README.md`
- Modify: `BREAKING_CHANGES.md`
- Modify: `CONTRIBUTING.md`
- Modify: `docs/vnext/getting-started.md`
- Modify: `docs/vnext/fluent-api-cookbook.md`

- [ ] **Step 1: Update repository structure documentation**

Update `README.md` so the repository layout and active solution description match the new single-runtime-project structure.

The repository layout section should describe:

```markdown
- `src/StarDust.CasparCG`: public client API, AMCP, OSC, and transport internals
- `src/StarDust.CasparCG.Hosting`: DI and named client registration
- `src/StarDust.CasparCG.Testing`: `DummyServer` helpers
- `src/StarDust.CasparCG.UnitTests`: unit coverage
- `src/StarDust.CasparCG.IntegrationTests`: transport and integration coverage
```

- [ ] **Step 2: Update contributor-facing cleanup notes**

Add a short note in `BREAKING_CHANGES.md` and `CONTRIBUTING.md` that the runtime has been collapsed into `StarDust.CasparCG` and legacy project paths have been removed from the repo.

- [ ] **Step 3: Refresh the vNext guides if they mention removed projects**

Update `docs/vnext/getting-started.md` and `docs/vnext/fluent-api-cookbook.md` only where they mention old project structure or old runtime project names.

- [ ] **Step 4: Run final validation**

Run:

```bash
dotnet build src/StarDust.CasparCG/StarDust.CasparCG.csproj -c Release -p:TreatWarningsAsErrors=true -v minimal
dotnet build src/StarDust.CasparCG.Hosting/StarDust.CasparCG.Hosting.csproj -c Release -p:TreatWarningsAsErrors=true -v minimal
dotnet build src/StarDust.CasparCG.Testing/StarDust.CasparCG.Testing.csproj -c Release -p:TreatWarningsAsErrors=true -v minimal
dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release -v minimal
dotnet test src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj -c Release -v minimal
dotnet build "src/Demo/Demo AMCP/Demo.AMCP.netcore/Demo.AMCP.netcore.csproj" -c Release -p:BuildInParallel=false -p:MSBuildEnableWorkloadResolver=false -v minimal
```

Expected:

- build/test commands for active projects pass,
- if `dotnet build src/StarDust.CasparCG.net.sln ...` still fails with the known silent restore-graph issue, record it in the final report as an environment/tooling limitation.

- [ ] **Step 5: Commit**

```bash
git add README.md BREAKING_CHANGES.md CONTRIBUTING.md docs/vnext
git commit -m "docs: reflect simplified runtime structure"
```

## Self-Review

- Spec coverage: this plan covers the two approved outcomes from the spec: deleting obsolete legacy projects and collapsing `Transport`, `Protocol.Amcp`, and `Protocol.Osc` into `StarDust.CasparCG`.
- Placeholder scan: each task contains exact files, commands, and expected outcomes. No TBD/TODO placeholders remain.
- Type consistency: the plan keeps namespaces stable and uses the existing runtime type names already present in the repository.
