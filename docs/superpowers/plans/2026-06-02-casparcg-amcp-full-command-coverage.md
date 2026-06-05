# CasparCG AMCP Full Command Coverage Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** expose the full AMCP command surface of CasparCG through the vNext client, with a coherent family-based protocol layer, thin client helpers, and clean docs/tests.

**Architecture:** keep AMCP serialization in `StarDust.CasparCG.Protocol.Amcp`, keep `CasparClient` thin, and use family files for the new command sets so the surface stays exhaustive without turning into a switch-driven API. Prefer typed helpers where response shapes are stable, and keep raw `AmcpResponse` access for heterogeneous query commands. Use .NET 10 idioms on touched types, especially primary constructors for small scopes and records, and keep Roslyn clean: zero warnings, no analyzer debt.

**Tech Stack:** .NET 10, C#, primary constructors on new small types, `ReadOnlySpan<char>`, `IReadOnlyList<string>`, xUnit, existing AMCP transport/parser stack.

---

### Task 1: Basic and Query AMCP command catalog

**Files:**
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/AmcpCommandFormatting.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/BasicCommands.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/QueryCommands.cs`
- Modify: `src/StarDust.CasparCG.UnitTests/CasparClientCommandTests.cs`
- Modify: `src/StarDust.CasparCG.UnitTests/AmcpResponseParserTests.cs`

- [ ] **Step 1: Write the failing serialization tests**

```csharp
Assert.Equal("LOAD 1-10 AMB\r\n", transport.LastCommandText);
Assert.Equal("PAUSE 1-10\r\n", transport.LastCommandText);
Assert.Equal("CALLBG 1-10 AMB\r\n", transport.LastCommandText);
Assert.Equal("INFO CONFIG\r\n", transport.LastCommandText);
Assert.Equal("GL GC\r\n", transport.LastCommandText);
```

Cover at least one representative command from each of these groups:
- Basic: `LOAD`, `CALLBG`, `PAUSE`, `RESUME`, `CLEAR`, `CALL`, `SWAP`, `ADD`, `REMOVE`, `APPLY`, `PRINT`, `CLEAR ALL`, `SET`
- Query: `INFO`, `INFO CONFIG`, `INFO PATHS`, `CINF`, `CLS`, `FLS`, `TLS`, `GL INFO`, `GL GC`

- [ ] **Step 2: Run the focused tests to confirm the missing types/signatures**

Run: `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "CasparClientCommandTests|AmcpResponseParserTests" -v minimal`

Expected: fail at compile time or on missing serialization because the new command catalog is not implemented yet.

- [ ] **Step 3: Implement the minimal catalog and shared formatting helpers**

Use primary constructors for the new records and keep the AMCP string formatting exact:

```csharp
internal static string Channel(int channel) => $"{channel}";
internal static string ChannelLayer(int channel, int layer) => $"{channel}-{layer}";

public sealed record LoadCommand(int Channel, string Clip) : AmcpCommand;
public sealed record CallBgCommand(int Channel, int Layer, string Clip) : AmcpCommand;
public sealed record InfoConfigCommand : AmcpCommand;
public sealed record GlGcCommand : AmcpCommand;
```

Make the query family return raw `AmcpResponse` or `IReadOnlyList<string>` only when the payload is stable enough to parse safely. Do not invent extra DTOs for one-off XML/text responses.

- [ ] **Step 4: Re-run the focused tests**

Run: `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "CasparClientCommandTests|AmcpResponseParserTests" -v minimal`

Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add src/StarDust.CasparCG.Protocol.Amcp/Commands/AmcpCommandFormatting.cs src/StarDust.CasparCG.Protocol.Amcp/Commands/BasicCommands.cs src/StarDust.CasparCG.Protocol.Amcp/Commands/QueryCommands.cs src/StarDust.CasparCG.UnitTests/CasparClientCommandTests.cs src/StarDust.CasparCG.UnitTests/AmcpResponseParserTests.cs
git commit -m "feat: add basic and query amcp commands"
```

### Task 2: Client surface for basic/query helpers

**Files:**
- Modify: `src/StarDust.CasparCG/CasparClient.cs`
- Modify: `src/StarDust.CasparCG/Fluent/ChannelScope.cs`
- Modify: `src/StarDust.CasparCG/Fluent/LayerScope.cs`
- Modify: `src/StarDust.CasparCG/Fluent/PlayCommandBuilder.cs`
- Modify: `src/StarDust.CasparCG.UnitTests/CasparClientBootstrapTests.cs`
- Modify: `src/StarDust.CasparCG.UnitTests/FluentPlayCommandBuilderTests.cs`

- [ ] **Step 1: Write the failing API-shape tests**

```csharp
public ValueTask<AmcpResponse> InfoAsync(CancellationToken cancellationToken);
public ValueTask<AmcpResponse> InfoConfigAsync(CancellationToken cancellationToken);
public ValueTask<AmcpResponse> InfoPathsAsync(CancellationToken cancellationToken);
public ValueTask<IReadOnlyList<string>> GetMediaFilesAsync(CancellationToken cancellationToken);
```

Add tests that confirm:
- `Load`, `CallBG`, `Pause`, `Resume`, `Clear`, `Swap`, `Set`, `ClearAll` emit the exact command string
- `ChannelScope` and `LayerScope` stay fluent and still build the existing play path
- the new query helpers return the same AMCP parsing semantics as the current `GetVersionAsync` and `GetMediaFilesAsync`

- [ ] **Step 2: Run the focused tests and verify the expected failures**

Run: `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "CasparClientBootstrapTests|CasparClientCommandTests|FluentPlayCommandBuilderTests" -v minimal`

Expected: fail because the new methods do not exist yet or still serialize through incomplete command objects.

- [ ] **Step 3: Implement the thin client helpers and modernize the touched small types**

Use primary constructors on the touched scopes if they are rewritten:

```csharp
public sealed class ChannelScope(CasparClient client, int channel)
{
    public LayerScope Layer(int layer) => new(client, channel, layer);
}
```

Keep `CasparClient` thin: each method should only instantiate a command object and call `SendAsync`/`QueryAsync`. Do not add a command dispatcher or a reflection-based layer.

- [ ] **Step 4: Re-run the focused tests**

Run: `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "CasparClientBootstrapTests|CasparClientCommandTests|FluentPlayCommandBuilderTests" -v minimal`

Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add src/StarDust.CasparCG/CasparClient.cs src/StarDust.CasparCG/Fluent/ChannelScope.cs src/StarDust.CasparCG/Fluent/LayerScope.cs src/StarDust.CasparCG/Fluent/PlayCommandBuilder.cs src/StarDust.CasparCG.UnitTests/CasparClientBootstrapTests.cs src/StarDust.CasparCG.UnitTests/FluentPlayCommandBuilderTests.cs
git commit -m "feat: expand amcp client surface"
```

### Task 3: Data, template, thumbnail, mixer, OSC unsubscribe, and admin commands

**Files:**
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/DataCommands.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/TemplateCommands.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/ThumbnailCommands.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/MixerCommands.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/AdminCommands.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/OscCommands.cs`
- Modify: `src/StarDust.CasparCG/CasparClient.cs`
- Modify: `src/StarDust.CasparCG.UnitTests/OscClientTests.cs`
- Modify: `src/StarDust.CasparCG.UnitTests/CasparClientCommandTests.cs`

- [ ] **Step 1: Write the failing serialization tests for the remaining families**

```csharp
Assert.Equal("DATA STORE foo bar\r\n", transport.LastCommandText);
Assert.Equal("CG UPDATE 1-10 {xml}\r\n", transport.LastCommandText);
Assert.Equal("MIXER VOLUME 1-10 0.5\r\n", transport.LastCommandText);
Assert.Equal("THUMBNAIL GENERATE_ALL\r\n", transport.LastCommandText);
Assert.Equal("OSC UNSUBSCRIBE 5253\r\n", transport.LastCommandText);
Assert.Equal("KILL\r\n", transport.LastCommandText);
```

Cover at least one command from each of:
- Data: `DATA STORE`, `DATA RETRIEVE`, `DATA LIST`, `DATA REMOVE`
- Template / CG: `CG ADD`, `CG PLAY`, `CG STOP`, `CG NEXT`, `CG REMOVE`, `CG CLEAR`, `CG UPDATE`, `CG INVOKE`
- Mixer: `MIXER KEYER`, `MIXER INVERT`, `MIXER CHROMA`, `MIXER BLEND`, `MIXER OPACITY`, `MIXER BRIGHTNESS`, `MIXER SATURATION`, `MIXER CONTRAST`, `MIXER LEVELS`, `MIXER FILL`, `MIXER CLIP`, `MIXER ANCHOR`, `MIXER CROP`, `MIXER ROTATION`, `MIXER PERSPECTIVE`, `MIXER VOLUME`, `MIXER MASTERVOLUME`, `MIXER GRID`, `MIXER COMMIT`, `MIXER CLEAR`, `CHANNEL_GRID`
- Thumbnail: `THUMBNAIL LIST`, `THUMBNAIL RETRIEVE`, `THUMBNAIL GENERATE`, `THUMBNAIL GENERATE_ALL`
- Admin: `DIAG`, `BYE`, `KILL`, `RESTART`, `LOCK`
- OSC: `OSC UNSUBSCRIBE`

- [ ] **Step 2: Run the focused tests to confirm the missing coverage**

Run: `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "CasparClientCommandTests|OscClientTests" -v minimal`

Expected: fail or incomplete because the remaining command objects and client wrappers do not exist yet.

- [ ] **Step 3: Implement the remaining family files and client methods**

Use primary constructors for the new records and keep the command text exact. Add public client wrappers only where the command is part of the supported surface:

```csharp
public ValueTask OscUnsubscribeAsync(int port, CancellationToken cancellationToken);
public ValueTask<AmcpResponse> DataStoreAsync(string key, string value, CancellationToken cancellationToken);
public ValueTask<AmcpResponse> CgUpdateAsync(int channel, int layer, string data, CancellationToken cancellationToken);
public ValueTask<AmcpResponse> KillAsync(CancellationToken cancellationToken);
```

Do not add bespoke result types unless the response shape is stable and repeatedly useful. Raw `AmcpResponse` is acceptable for XML/text-heavy commands.

- [ ] **Step 4: Re-run the focused tests**

Run: `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "CasparClientCommandTests|OscClientTests" -v minimal`

Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add src/StarDust.CasparCG.Protocol.Amcp/Commands/DataCommands.cs src/StarDust.CasparCG.Protocol.Amcp/Commands/TemplateCommands.cs src/StarDust.CasparCG.Protocol.Amcp/Commands/ThumbnailCommands.cs src/StarDust.CasparCG.Protocol.Amcp/Commands/MixerCommands.cs src/StarDust.CasparCG.Protocol.Amcp/Commands/AdminCommands.cs src/StarDust.CasparCG.Protocol.Amcp/Commands/OscCommands.cs src/StarDust.CasparCG/CasparClient.cs src/StarDust.CasparCG.UnitTests/OscClientTests.cs src/StarDust.CasparCG.UnitTests/CasparClientCommandTests.cs
git commit -m "feat: cover remaining amcp command families"
```

### Task 4: Documentation, Roslyn cleanliness, and final validation

**Files:**
- Modify: `README.md`
- Modify: `src/StarDust.CasparCG.Protocol.Amcp/Commands/AmcpCommandFormatting.cs`
- Modify: `src/StarDust.CasparCG.Protocol.Amcp/Commands/BasicCommands.cs`
- Modify: `src/StarDust.CasparCG.Protocol.Amcp/Commands/QueryCommands.cs`
- Modify: `src/StarDust.CasparCG.Protocol.Amcp/Commands/DataCommands.cs`
- Modify: `src/StarDust.CasparCG.Protocol.Amcp/Commands/TemplateCommands.cs`
- Modify: `src/StarDust.CasparCG.Protocol.Amcp/Commands/ThumbnailCommands.cs`
- Modify: `src/StarDust.CasparCG.Protocol.Amcp/Commands/MixerCommands.cs`
- Modify: `src/StarDust.CasparCG.Protocol.Amcp/Commands/AdminCommands.cs`
- Modify: `src/StarDust.CasparCG.Protocol.Amcp/Commands/OscCommands.cs`
- Modify: `src/StarDust.CasparCG/CasparClient.cs`
- Modify: `src/StarDust.CasparCG/Fluent/ChannelScope.cs`
- Modify: `src/StarDust.CasparCG/Fluent/LayerScope.cs`
- Modify: `src/StarDust.CasparCG/Fluent/PlayCommandBuilder.cs`

- [ ] **Step 1: Add or refresh XML docs on every new public command and client method**

Document the purpose of each new AMCP command object and every new public client wrapper. Keep summaries short and literal so generated XML docs stay useful in IntelliSense.

- [ ] **Step 2: Update the README with the supported AMCP families**

Add a concise command-family table to `README.md` so the public surface is discoverable without reading the source tree.

- [ ] **Step 3: Run the zero-warning build**

Run:

```bash
dotnet build src/StarDust.CasparCG.net.sln -c Release -p:TreatWarningsAsErrors=true -v minimal
```

Expected: `0 Warning(s), 0 Error(s)`.

- [ ] **Step 4: Run the full test matrix**

Run:

```bash
dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release -v minimal
dotnet test src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj -c Release -v minimal
dotnet build "src/Demo/Demo AMCP/Demo.AMCP.netcore/Demo.AMCP.netcore.csproj" -c Release -p:BuildInParallel=false -p:MSBuildEnableWorkloadResolver=false -v minimal
```

Expected: all green, no Roslyn warnings.

- [ ] **Step 5: Commit**

```bash
git add README.md src/StarDust.CasparCG.Protocol.Amcp/Commands src/StarDust.CasparCG/CasparClient.cs src/StarDust.CasparCG/Fluent/ChannelScope.cs src/StarDust.CasparCG/Fluent/LayerScope.cs src/StarDust.CasparCG/Fluent/PlayCommandBuilder.cs
git commit -m "docs: finalize amcp coverage and docs"
```

## Execution Notes

- Keep the same branch.
- Prefer .NET 10 primary constructors for new small types and records.
- Do not introduce warning suppression to force a clean build.
- Preserve the current OSC probe behavior; this work extends AMCP only and must not regress the existing OSC path.
