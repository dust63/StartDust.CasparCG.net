# CasparCG vNext Protocol Runtime Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Replace the placeholder vNext runtime with a real CasparCG protocol stack covering AMCP request/response semantics, OSC UDP intake, broad command-family coverage, local-server integration, and retirement of the legacy projects.

**Architecture:** Rebuild the runtime around a typed AMCP core (`AmcpCommand`, `AmcpResponse`, parser, exception model) and a separate OSC UDP listener/mapping pipeline. Keep `StarDust.CasparCG` as the single public entry point, move protocol and transport details into focused projects, and remove the legacy projects from the active solution once the vNext stack is functionally complete.

**Tech Stack:** `.NET 10`, `System.Net.Sockets`, `System.Threading.Channels`, `Microsoft.Extensions.DependencyInjection`, `xUnit`, `Moq`

---

## File Structure Map

### New or heavily reworked production files

- `src/StarDust.CasparCG.Protocol.Amcp/AmcpResponse.cs`
  Typed AMCP response model preserving status code, status line, payload lines, and raw block.
- `src/StarDust.CasparCG.Protocol.Amcp/AmcpStatusCategory.cs`
  Categorization of `1xx`, `2xx`, `4xx`, `5xx`.
- `src/StarDust.CasparCG.Protocol.Amcp/AmcpCommandException.cs`
  Exception carrying a failed `AmcpResponse`.
- `src/StarDust.CasparCG.Protocol.Amcp/AmcpResponseParser.cs`
  Deterministic parser for header-only, single-line, and multi-line AMCP responses.
- `src/StarDust.CasparCG.Protocol.Amcp/AmcpCommandRegistry.cs`
  Command text registry extracted from legacy `AMCPCommand`.
- `src/StarDust.CasparCG.Protocol.Amcp/Commands/*.cs`
  Command definitions for playout, query, CG, mixer, and data families.
- `src/StarDust.CasparCG.Transport/IAmcpTransport.cs`
  Updated to return `AmcpResponse`.
- `src/StarDust.CasparCG.Transport/TcpAmcpTransport.cs`
  Serialized TCP transport that writes commands and parses full responses.
- `src/StarDust.CasparCG.Transport/IOscTransport.cs`
  UDP listener contract.
- `src/StarDust.CasparCG.Transport/UdpOscListener.cs`
  UDP OSC intake implementation.
- `src/StarDust.CasparCG.Protocol.Osc/OscMessage.cs`
  Parsed OSC address and arguments.
- `src/StarDust.CasparCG.Protocol.Osc/OscPacketParser.cs`
  Minimal OSC datagram parser.
- `src/StarDust.CasparCG.Protocol.Osc/DefaultOscMessageMapper.cs`
  Address-pattern mapper from raw OSC into domain events.
- `src/StarDust.CasparCG/CasparClient.cs`
  Updated low-level send API, high-level families, exception semantics, and OSC hookup.
- `src/StarDust.CasparCG/Query/CasparQueryClient.cs`
  Query-oriented public surface.
- `src/StarDust.CasparCG/Cg/CasparCgClient.cs`
  CG command surface.
- `src/StarDust.CasparCG/Mixer/CasparMixerClient.cs`
  Mixer command surface.
- `src/StarDust.CasparCG/Data/CasparDataClient.cs`
  Data command surface.
- `src/StarDust.CasparCG/Events/*.cs`
  Expanded domain event set.
- `src/StarDust.CasparCG/State/CasparStateStore.cs`
  Expanded projection updates from OSC events.

### Testing and integration files

- `src/StarDust.CasparCG.UnitTests/AmcpResponseParserTests.cs`
  Pure parser tests for `100/101/200/201/202/4xx/5xx`.
- `src/StarDust.CasparCG.UnitTests/AmcpClientErrorHandlingTests.cs`
  Verifies low-level response return and high-level exception behavior.
- `src/StarDust.CasparCG.UnitTests/QueryCommandTests.cs`
  Query-family serialization and result handling.
- `src/StarDust.CasparCG.UnitTests/CgCommandTests.cs`
  CG-family coverage.
- `src/StarDust.CasparCG.UnitTests/MixerCommandTests.cs`
  Mixer-family coverage.
- `src/StarDust.CasparCG.UnitTests/DataCommandTests.cs`
  Data-family coverage.
- `src/StarDust.CasparCG.UnitTests/OscMessageMapperTests.cs`
  Address-to-event mapping tests.
- `src/StarDust.CasparCG.Testing/DummyServer/DummyOscEmitter.cs`
  UDP helper for OSC integration tests.
- `src/StarDust.CasparCG.IntegrationTests/CasparClientDummyServerTests.cs`
  Expanded AMCP integration suite on the dummy server.
- `src/StarDust.CasparCG.IntegrationTests/CasparClientLocalServerTests.cs`
  Optional tests against the real local CasparCG server on `127.0.0.1:5250`.
- `src/StarDust.CasparCG.IntegrationTests/OscLocalServerTests.cs`
  Optional OSC tests against the local server on `6250` / predefined client `5253`.

### Repository-level files

- `src/StarDust.CasparCG.net.sln`
  Remove legacy projects, keep only vNext projects and tests.
- `README.md`
  Update examples to cover new response semantics and command families.
- `BREAKING_CHANGES.md`
  Add explicit note about legacy project removal from the active solution.
- `CONTRIBUTING.md`
  Update validation commands for local-server tests.
- `.github/workflows/ci.yml`
  Keep CI on unit tests, dummy integration, docs, and workflows only.
- `.github/workflows/package.yml`
  Package the vNext projects.
- `scripts/verify-docs.sh`
  Include new protocol docs if added.
- `scripts/verify-workflows.sh`
  Keep workflow expectations aligned.

### Repository notes

- Do not mutate legacy projects except when reading from them or deleting them from the active solution/repository.
- The legacy AMCP and OSC implementations are reference material only; no new behavior belongs there.
- Keep local-server integration tests opt-in so CI remains deterministic.

### Task 1: Rebuild the AMCP response model and parser

**Files:**
- Create: `src/StarDust.CasparCG.Protocol.Amcp/AmcpStatusCategory.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/AmcpResponse.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/AmcpCommandException.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/AmcpResponseParser.cs`
- Create: `src/StarDust.CasparCG.UnitTests/AmcpResponseParserTests.cs`

- [ ] **Step 1: Write the failing parser tests**

```csharp
// src/StarDust.CasparCG.UnitTests/AmcpResponseParserTests.cs
using StarDust.CasparCG.Protocol.Amcp;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class AmcpResponseParserTests
{
    [Fact]
    public void Parse_202_response_has_success_without_payload()
    {
        var response = AmcpResponseParser.Parse("202 PLAY OK\r\n");

        Assert.Equal(202, response.StatusCode);
        Assert.Equal(AmcpStatusCategory.Success, response.Category);
        Assert.True(response.IsSuccess);
        Assert.Equal("PLAY", response.CommandText);
        Assert.Empty(response.Lines);
    }

    [Fact]
    public void Parse_201_response_has_single_payload_line()
    {
        var response = AmcpResponseParser.Parse("201 VERSION OK\r\n2.4.1 Stable\r\n");

        Assert.Equal(201, response.StatusCode);
        Assert.Single(response.Lines);
        Assert.Equal("2.4.1 Stable", response.Lines[0]);
    }

    [Fact]
    public void Parse_200_response_has_multiline_payload()
    {
        var response = AmcpResponseParser.Parse("200 CLS OK\r\n\"AMB\" MOVIE 42 20240101120000 240 1/25\r\n\r\n");

        Assert.Equal(200, response.StatusCode);
        Assert.Single(response.Lines);
        Assert.Equal("\"AMB\" MOVIE 42 20240101120000 240 1/25", response.Lines[0]);
    }

    [Fact]
    public void Parse_404_response_preserves_error_status()
    {
        var response = AmcpResponseParser.Parse("404 PLAY FAILED\r\n");

        Assert.Equal(404, response.StatusCode);
        Assert.Equal(AmcpStatusCategory.ClientError, response.Category);
        Assert.False(response.IsSuccess);
        Assert.Equal("PLAY FAILED", response.CommandText);
    }
}
```

- [ ] **Step 2: Run the parser tests to verify the model does not exist**

Run: `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter AmcpResponseParserTests -v minimal`

Expected: FAIL with missing types for `AmcpResponseParser`, `AmcpResponse`, and `AmcpStatusCategory`.

- [ ] **Step 3: Implement the minimal response model and parser**

```csharp
// src/StarDust.CasparCG.Protocol.Amcp/AmcpStatusCategory.cs
namespace StarDust.CasparCG.Protocol.Amcp;

public enum AmcpStatusCategory
{
    Unknown,
    Information,
    Success,
    ClientError,
    ServerError
}
```

```csharp
// src/StarDust.CasparCG.Protocol.Amcp/AmcpResponse.cs
namespace StarDust.CasparCG.Protocol.Amcp;

public sealed class AmcpResponse
{
    public required int StatusCode { get; init; }
    public required AmcpStatusCategory Category { get; init; }
    public required string CommandText { get; init; }
    public required string StatusLine { get; init; }
    public required IReadOnlyList<string> Lines { get; init; }
    public required string Raw { get; init; }
    public bool IsSuccess => Category == AmcpStatusCategory.Success;
}
```

```csharp
// src/StarDust.CasparCG.Protocol.Amcp/AmcpCommandException.cs
namespace StarDust.CasparCG.Protocol.Amcp;

public sealed class AmcpCommandException : InvalidOperationException
{
    public AmcpCommandException(AmcpResponse response)
        : base($"AMCP command failed with status code {response.StatusCode}: {response.StatusLine}")
    {
        Response = response;
    }

    public AmcpResponse Response { get; }
}
```

```csharp
// src/StarDust.CasparCG.Protocol.Amcp/AmcpResponseParser.cs
using System.Text.RegularExpressions;

namespace StarDust.CasparCG.Protocol.Amcp;

public static partial class AmcpResponseParser
{
    [GeneratedRegex(@"^(?<code>\d{3})\s*(?<text>.*)$")]
    private static partial Regex HeaderRegex();

    public static AmcpResponse Parse(string raw)
    {
        var normalized = raw.Replace("\r\n", "\n");
        var lines = normalized.Split('\n');
        var header = lines[0].TrimEnd();
        var match = HeaderRegex().Match(header);
        if (!match.Success)
            throw new InvalidOperationException($"Invalid AMCP header: {header}");

        var statusCode = int.Parse(match.Groups["code"].Value);
        var commandText = match.Groups["text"].Value.Replace(" OK", string.Empty).Trim();
        var payload = lines
            .Skip(1)
            .Where(x => !string.IsNullOrEmpty(x))
            .ToArray();

        return new AmcpResponse
        {
            StatusCode = statusCode,
            Category = statusCode switch
            {
                >= 100 and < 200 => AmcpStatusCategory.Information,
                >= 200 and < 300 => AmcpStatusCategory.Success,
                >= 400 and < 500 => AmcpStatusCategory.ClientError,
                >= 500 and < 600 => AmcpStatusCategory.ServerError,
                _ => AmcpStatusCategory.Unknown
            },
            CommandText = commandText,
            StatusLine = header,
            Lines = payload,
            Raw = raw
        };
    }
}
```

- [ ] **Step 4: Run the parser tests to verify they pass**

Run: `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter AmcpResponseParserTests -v minimal`

Expected: PASS with four tests executed.

- [ ] **Step 5: Commit the parser slice**

```bash
git add src/StarDust.CasparCG.Protocol.Amcp src/StarDust.CasparCG.UnitTests/AmcpResponseParserTests.cs
git commit -m "feat: add typed AMCP response parser"
```

### Task 2: Retrofit the AMCP transport and low-level client semantics

**Files:**
- Modify: `src/StarDust.CasparCG.Transport/IAmcpTransport.cs`
- Modify: `src/StarDust.CasparCG.Transport/TcpAmcpTransport.cs`
- Modify: `src/StarDust.CasparCG/CasparClient.cs`
- Create: `src/StarDust.CasparCG.UnitTests/AmcpClientErrorHandlingTests.cs`

- [ ] **Step 1: Write the failing low-level/high-level behavior tests**

```csharp
// src/StarDust.CasparCG.UnitTests/AmcpClientErrorHandlingTests.cs
using StarDust.CasparCG;
using StarDust.CasparCG.Protocol.Amcp;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class AmcpClientErrorHandlingTests
{
    [Fact]
    public async Task SendAsync_returns_raw_response_for_errors()
    {
        var client = new CasparClient(new StubTransport(new AmcpResponse
        {
            StatusCode = 404,
            Category = AmcpStatusCategory.ClientError,
            CommandText = "PLAY FAILED",
            StatusLine = "404 PLAY FAILED",
            Lines = Array.Empty<string>(),
            Raw = "404 PLAY FAILED\r\n"
        }));

        var response = await client.SendAsync(new StarDust.CasparCG.Protocol.Amcp.Commands.PlayCommand(1, 10, "MISSING"), CancellationToken.None);

        Assert.Equal(404, response.StatusCode);
        Assert.False(response.IsSuccess);
    }

    [Fact]
    public async Task PlayAsync_throws_exception_for_non_success_response()
    {
        var client = new CasparClient(new StubTransport(new AmcpResponse
        {
            StatusCode = 404,
            Category = AmcpStatusCategory.ClientError,
            CommandText = "PLAY FAILED",
            StatusLine = "404 PLAY FAILED",
            Lines = Array.Empty<string>(),
            Raw = "404 PLAY FAILED\r\n"
        }));

        var ex = await Assert.ThrowsAsync<AmcpCommandException>(() => client.PlayAsync(1, 10, "MISSING", CancellationToken.None).AsTask());
        Assert.Equal(404, ex.Response.StatusCode);
    }

    private sealed class StubTransport : IAmcpTransport
    {
        private readonly AmcpResponse _response;
        public StubTransport(AmcpResponse response) => _response = response;
        public ValueTask ConnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
        public ValueTask DisconnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
        public ValueTask<AmcpResponse> SendAsync(StarDust.CasparCG.Protocol.Amcp.AmcpCommand command, CancellationToken cancellationToken)
            => ValueTask.FromResult(_response);
    }
}
```

- [ ] **Step 2: Run the error-handling tests to verify the transport contract is obsolete**

Run: `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter AmcpClientErrorHandlingTests -v minimal`

Expected: FAIL with interface and method signature errors on `IAmcpTransport` and `CasparClient.SendAsync`.

- [ ] **Step 3: Update the transport and client to use typed responses**

```csharp
// src/StarDust.CasparCG.Transport/IAmcpTransport.cs
using StarDust.CasparCG.Protocol.Amcp;

namespace StarDust.CasparCG.Transport;

public interface IAmcpTransport
{
    ValueTask ConnectAsync(CancellationToken cancellationToken);
    ValueTask DisconnectAsync(CancellationToken cancellationToken);
    ValueTask<AmcpResponse> SendAsync(AmcpCommand command, CancellationToken cancellationToken);
}
```

```csharp
// src/StarDust.CasparCG.Transport/TcpAmcpTransport.cs
using System.Net.Sockets;
using System.Text;
using StarDust.CasparCG.Protocol.Amcp;

namespace StarDust.CasparCG.Transport;

public sealed class TcpAmcpTransport : IAmcpTransport, IAsyncDisposable
{
    private readonly string _host;
    private readonly int _port;
    private readonly SemaphoreSlim _sendLock = new(1, 1);
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

    public async ValueTask<AmcpResponse> SendAsync(AmcpCommand command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(_stream);

        await _sendLock.WaitAsync(cancellationToken);
        try
        {
            var bytes = Encoding.UTF8.GetBytes(command.Serialize());
            await _stream.WriteAsync(bytes, cancellationToken);
            await _stream.FlushAsync(cancellationToken);

            using var reader = new StreamReader(_stream, Encoding.UTF8, leaveOpen: true);
            var raw = await ReadResponseBlockAsync(reader, cancellationToken);
            return AmcpResponseParser.Parse(raw);
        }
        finally
        {
            _sendLock.Release();
        }
    }

    public ValueTask DisposeAsync() => DisconnectAsync(CancellationToken.None);

    private static async Task<string> ReadResponseBlockAsync(StreamReader reader, CancellationToken cancellationToken)
    {
        var lines = new List<string>();
        string? header = await reader.ReadLineAsync(cancellationToken);
        if (header is null)
            throw new InvalidOperationException("No AMCP response header received.");

        lines.Add(header);
        if (!int.TryParse(header[..3], out var code))
            return string.Join("\r\n", lines) + "\r\n";

        if (code == 200)
        {
            while (true)
            {
                var line = await reader.ReadLineAsync(cancellationToken);
                if (line is null || line.Length == 0)
                    break;
                lines.Add(line);
            }
        }
        else if (code == 201 || code == 101)
        {
            var line = await reader.ReadLineAsync(cancellationToken);
            if (line is not null)
                lines.Add(line);
        }

        return string.Join("\r\n", lines) + "\r\n";
    }
}
```

```csharp
// src/StarDust.CasparCG/CasparClient.cs (relevant members)
using StarDust.CasparCG.Protocol.Amcp;

public async ValueTask<AmcpResponse> SendAsync(AmcpCommand command, CancellationToken cancellationToken)
{
    var response = await RequireTransport().SendAsync(command, cancellationToken);
    Diagnostics.LastSuccessfulAmcpInteraction = DateTimeOffset.UtcNow;
    return response;
}

internal async ValueTask EnsureSuccessAsync(AmcpCommand command, CancellationToken cancellationToken)
{
    var response = await SendAsync(command, cancellationToken);
    if (!response.IsSuccess)
        throw new AmcpCommandException(response);
}
```

- [ ] **Step 4: Move `PlayAsync`, `LoadBackgroundAsync`, and `StopAsync` to `EnsureSuccessAsync` and rerun the tests**

Run: `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter "AmcpClientErrorHandlingTests|CasparClientCommandTests" -v minimal`

Expected: PASS with all selected tests green.

- [ ] **Step 5: Commit the transport semantics slice**

```bash
git add src/StarDust.CasparCG.Transport src/StarDust.CasparCG src/StarDust.CasparCG.UnitTests/AmcpClientErrorHandlingTests.cs
git commit -m "feat: return typed AMCP responses from transport"
```

### Task 3: Complete the playout and query AMCP surfaces

**Files:**
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/LoadCommand.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/PauseCommand.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/ResumeCommand.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/ClearCommand.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/VersionCommand.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/InfoCommand.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/TlsCommand.cs`
- Create: `src/StarDust.CasparCG/Query/CasparQueryClient.cs`
- Modify: `src/StarDust.CasparCG/CasparClient.cs`
- Create: `src/StarDust.CasparCG.UnitTests/QueryCommandTests.cs`

- [ ] **Step 1: Write the failing query and playout tests**

```csharp
// src/StarDust.CasparCG.UnitTests/QueryCommandTests.cs
using StarDust.CasparCG;
using StarDust.CasparCG.Protocol.Amcp;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class QueryCommandTests
{
    [Fact]
    public async Task PauseAsync_serializes_expected_amcp_command()
    {
        var transport = new RecordingTransport("202 PAUSE OK\r\n");
        var client = new CasparClient(transport);

        await client.PauseAsync(1, 10, CancellationToken.None);

        Assert.Equal("PAUSE 1-10\r\n", transport.LastSerialized);
    }

    [Fact]
    public async Task VersionAsync_returns_payload_line()
    {
        var transport = new RecordingTransport("201 VERSION OK\r\n2.4.1 Stable\r\n");
        var client = new CasparClient(transport);

        var version = await client.Query.VersionAsync(CancellationToken.None);

        Assert.Equal("2.4.1 Stable", version);
    }

    private sealed class RecordingTransport : IAmcpTransport
    {
        private readonly string _rawResponse;
        public RecordingTransport(string rawResponse) => _rawResponse = rawResponse;
        public string? LastSerialized { get; private set; }
        public ValueTask ConnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
        public ValueTask DisconnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
        public ValueTask<AmcpResponse> SendAsync(AmcpCommand command, CancellationToken cancellationToken)
        {
            LastSerialized = command.Serialize();
            return ValueTask.FromResult(AmcpResponseParser.Parse(_rawResponse));
        }
    }
}
```

- [ ] **Step 2: Run the query tests to verify the APIs are missing**

Run: `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter QueryCommandTests -v minimal`

Expected: FAIL with missing `PauseAsync`, `Query`, `VersionAsync`, and command classes.

- [ ] **Step 3: Implement the minimal playout/query command surface**

```csharp
// src/StarDust.CasparCG.Protocol.Amcp/Commands/PauseCommand.cs
namespace StarDust.CasparCG.Protocol.Amcp.Commands;

public sealed record PauseCommand(int Channel, int Layer) : AmcpCommand
{
    public override string Serialize() => $"PAUSE {Address(Channel, Layer)}\r\n";
}
```

```csharp
// src/StarDust.CasparCG.Protocol.Amcp/Commands/ResumeCommand.cs
namespace StarDust.CasparCG.Protocol.Amcp.Commands;

public sealed record ResumeCommand(int Channel, int Layer) : AmcpCommand
{
    public override string Serialize() => $"RESUME {Address(Channel, Layer)}\r\n";
}
```

```csharp
// src/StarDust.CasparCG.Protocol.Amcp/Commands/ClearCommand.cs
namespace StarDust.CasparCG.Protocol.Amcp.Commands;

public sealed record ClearCommand(int Channel, int Layer) : AmcpCommand
{
    public override string Serialize() => $"CLEAR {Address(Channel, Layer)}\r\n";
}
```

```csharp
// src/StarDust.CasparCG.Protocol.Amcp/Commands/VersionCommand.cs
namespace StarDust.CasparCG.Protocol.Amcp.Commands;

public sealed record VersionCommand : AmcpCommand
{
    public override string Serialize() => "VERSION\r\n";
}
```

```csharp
// src/StarDust.CasparCG.Protocol.Amcp/Commands/InfoCommand.cs
namespace StarDust.CasparCG.Protocol.Amcp.Commands;

public sealed record InfoCommand : AmcpCommand
{
    public override string Serialize() => "INFO\r\n";
}
```

```csharp
// src/StarDust.CasparCG.Protocol.Amcp/Commands/TlsCommand.cs
namespace StarDust.CasparCG.Protocol.Amcp.Commands;

public sealed record TlsCommand : AmcpCommand
{
    public override string Serialize() => "TLS\r\n";
}
```

```csharp
// src/StarDust.CasparCG/Query/CasparQueryClient.cs
using StarDust.CasparCG.Protocol.Amcp.Commands;

namespace StarDust.CasparCG.Query;

public sealed class CasparQueryClient
{
    private readonly CasparClient _client;
    public CasparQueryClient(CasparClient client) => _client = client;

    public async ValueTask<string> VersionAsync(CancellationToken cancellationToken)
    {
        var response = await _client.SendAsync(new VersionCommand(), cancellationToken);
        if (!response.IsSuccess)
            throw new Protocol.Amcp.AmcpCommandException(response);
        return response.Lines.Single();
    }
}
```

```csharp
// src/StarDust.CasparCG/CasparClient.cs (relevant additions)
using StarDust.CasparCG.Protocol.Amcp.Commands;
using StarDust.CasparCG.Query;

public CasparQueryClient Query => new(this);

public ValueTask PauseAsync(int channel, int layer, CancellationToken cancellationToken) =>
    EnsureSuccessAsync(new PauseCommand(channel, layer), cancellationToken);

public ValueTask ResumeAsync(int channel, int layer, CancellationToken cancellationToken) =>
    EnsureSuccessAsync(new ResumeCommand(channel, layer), cancellationToken);

public ValueTask ClearAsync(int channel, int layer, CancellationToken cancellationToken) =>
    EnsureSuccessAsync(new ClearCommand(channel, layer), cancellationToken);
```

- [ ] **Step 4: Run the query tests and the existing command tests**

Run: `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter "QueryCommandTests|CasparClientCommandTests" -v minimal`

Expected: PASS with all selected tests green.

- [ ] **Step 5: Commit the playout/query slice**

```bash
git add src/StarDust.CasparCG.Protocol.Amcp src/StarDust.CasparCG src/StarDust.CasparCG.UnitTests/QueryCommandTests.cs
git commit -m "feat: add playout and query command surfaces"
```

### Task 4: Add CG and data command families

**Files:**
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/CgAddCommand.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/CgPlayCommand.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/CgStopCommand.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/CgUpdateCommand.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/DataStoreCommand.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/DataRetrieveCommand.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/DataListCommand.cs`
- Create: `src/StarDust.CasparCG/Cg/CasparCgClient.cs`
- Create: `src/StarDust.CasparCG/Data/CasparDataClient.cs`
- Modify: `src/StarDust.CasparCG/CasparClient.cs`
- Create: `src/StarDust.CasparCG.UnitTests/CgCommandTests.cs`
- Create: `src/StarDust.CasparCG.UnitTests/DataCommandTests.cs`

- [ ] **Step 1: Write the failing CG and data tests**

```csharp
// src/StarDust.CasparCG.UnitTests/CgCommandTests.cs
using StarDust.CasparCG;
using StarDust.CasparCG.Protocol.Amcp;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class CgCommandTests
{
    [Fact]
    public async Task Cg_add_serializes_template_command()
    {
        var transport = new RecordingTransport();
        var client = new CasparClient(transport);

        await client.Cg.AddAsync(1, 10, 7, "LOWER_THIRD", "<templateData></templateData>", CancellationToken.None);

        Assert.Equal("CG 1-10 ADD 7 \"LOWER_THIRD\" 1 \"<templateData></templateData>\"\r\n", transport.LastSerialized);
    }

    private sealed class RecordingTransport : IAmcpTransport
    {
        public string? LastSerialized { get; private set; }
        public ValueTask ConnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
        public ValueTask DisconnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
        public ValueTask<AmcpResponse> SendAsync(AmcpCommand command, CancellationToken cancellationToken)
        {
            LastSerialized = command.Serialize();
            return ValueTask.FromResult(AmcpResponseParser.Parse("202 CG ADD OK\r\n"));
        }
    }
}
```

```csharp
// src/StarDust.CasparCG.UnitTests/DataCommandTests.cs
using StarDust.CasparCG;
using StarDust.CasparCG.Protocol.Amcp;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class DataCommandTests
{
    [Fact]
    public async Task Data_store_serializes_expected_command()
    {
        var transport = new RecordingTransport("202 DATA STORE OK\r\n");
        var client = new CasparClient(transport);

        await client.Data.StoreAsync("my-key", "{\"name\":\"value\"}", CancellationToken.None);

        Assert.Equal("DATA STORE my-key \"{\\\"name\\\":\\\"value\\\"}\"\r\n", transport.LastSerialized);
    }

    [Fact]
    public async Task Data_retrieve_returns_payload()
    {
        var transport = new RecordingTransport("201 DATA RETRIEVE OK\r\n{\"name\":\"value\"}\r\n");
        var client = new CasparClient(transport);

        var payload = await client.Data.RetrieveAsync("my-key", CancellationToken.None);

        Assert.Equal("{\"name\":\"value\"}", payload);
    }

    private sealed class RecordingTransport : IAmcpTransport
    {
        private readonly string _raw;
        public RecordingTransport(string raw) => _raw = raw;
        public string? LastSerialized { get; private set; }
        public ValueTask ConnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
        public ValueTask DisconnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
        public ValueTask<AmcpResponse> SendAsync(AmcpCommand command, CancellationToken cancellationToken)
        {
            LastSerialized = command.Serialize();
            return ValueTask.FromResult(AmcpResponseParser.Parse(_raw));
        }
    }
}
```

- [ ] **Step 2: Run the CG/data tests to verify the grouped clients are missing**

Run: `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter "CgCommandTests|DataCommandTests" -v minimal`

Expected: FAIL with missing `Cg`, `Data`, and their command definitions.

- [ ] **Step 3: Implement the grouped command surfaces and rerun**

Use the following patterns for each command:

```csharp
// src/StarDust.CasparCG/Cg/CasparCgClient.cs
using StarDust.CasparCG.Protocol.Amcp;
using StarDust.CasparCG.Protocol.Amcp.Commands;

namespace StarDust.CasparCG.Cg;

public sealed class CasparCgClient
{
    private readonly CasparClient _client;
    public CasparCgClient(CasparClient client) => _client = client;

    public ValueTask AddAsync(int channel, int layer, int cgLayer, string templateName, string data, CancellationToken cancellationToken) =>
        _client.EnsureSuccessAsync(new CgAddCommand(channel, layer, cgLayer, templateName, data), cancellationToken);
}
```

```csharp
// src/StarDust.CasparCG/Data/CasparDataClient.cs
using StarDust.CasparCG.Protocol.Amcp;
using StarDust.CasparCG.Protocol.Amcp.Commands;

namespace StarDust.CasparCG.Data;

public sealed class CasparDataClient
{
    private readonly CasparClient _client;
    public CasparDataClient(CasparClient client) => _client = client;

    public ValueTask StoreAsync(string key, string payload, CancellationToken cancellationToken) =>
        _client.EnsureSuccessAsync(new DataStoreCommand(key, payload), cancellationToken);

    public async ValueTask<string> RetrieveAsync(string key, CancellationToken cancellationToken)
    {
        var response = await _client.SendAsync(new DataRetrieveCommand(key), cancellationToken);
        if (!response.IsSuccess)
            throw new AmcpCommandException(response);
        return response.Lines.Single();
    }
}
```

Run: `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter "CgCommandTests|DataCommandTests" -v minimal`

Expected: PASS.

- [ ] **Step 4: Commit the CG/data slice**

```bash
git add src/StarDust.CasparCG src/StarDust.CasparCG.Protocol.Amcp src/StarDust.CasparCG.UnitTests/CgCommandTests.cs src/StarDust.CasparCG.UnitTests/DataCommandTests.cs
git commit -m "feat: add CG and data AMCP families"
```

### Task 5: Add representative mixer command coverage

**Files:**
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/MixerVolumeCommand.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/MixerBrightnessCommand.cs`
- Create: `src/StarDust.CasparCG.Protocol.Amcp/Commands/MixerCommitCommand.cs`
- Create: `src/StarDust.CasparCG/Mixer/CasparMixerClient.cs`
- Modify: `src/StarDust.CasparCG/CasparClient.cs`
- Create: `src/StarDust.CasparCG.UnitTests/MixerCommandTests.cs`

- [ ] **Step 1: Write the failing mixer tests**

```csharp
// src/StarDust.CasparCG.UnitTests/MixerCommandTests.cs
using StarDust.CasparCG;
using StarDust.CasparCG.Protocol.Amcp;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class MixerCommandTests
{
    [Fact]
    public async Task Mixer_volume_serializes_expected_command()
    {
        var transport = new RecordingTransport();
        var client = new CasparClient(transport);

        await client.Mixer.VolumeAsync(1, 10, 0.5m, CancellationToken.None);

        Assert.Equal("MIXER 1-10 VOLUME 0.5\r\n", transport.LastSerialized);
    }

    private sealed class RecordingTransport : IAmcpTransport
    {
        public string? LastSerialized { get; private set; }
        public ValueTask ConnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
        public ValueTask DisconnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;
        public ValueTask<AmcpResponse> SendAsync(AmcpCommand command, CancellationToken cancellationToken)
        {
            LastSerialized = command.Serialize();
            return ValueTask.FromResult(AmcpResponseParser.Parse("202 MIXER VOLUME OK\r\n"));
        }
    }
}
```

- [ ] **Step 2: Run the mixer tests to verify the surface is missing**

Run: `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter MixerCommandTests -v minimal`

Expected: FAIL with missing `Mixer` and mixer commands.

- [ ] **Step 3: Implement the first mixer commands and grouped client**

Use this pattern:

```csharp
// src/StarDust.CasparCG/Mixer/CasparMixerClient.cs
using StarDust.CasparCG.Protocol.Amcp.Commands;

namespace StarDust.CasparCG.Mixer;

public sealed class CasparMixerClient
{
    private readonly CasparClient _client;
    public CasparMixerClient(CasparClient client) => _client = client;

    public ValueTask VolumeAsync(int channel, int layer, decimal value, CancellationToken cancellationToken) =>
        _client.EnsureSuccessAsync(new MixerVolumeCommand(channel, layer, value), cancellationToken);
}
```

Run: `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter MixerCommandTests -v minimal`

Expected: PASS.

- [ ] **Step 4: Commit the mixer slice**

```bash
git add src/StarDust.CasparCG src/StarDust.CasparCG.Protocol.Amcp src/StarDust.CasparCG.UnitTests/MixerCommandTests.cs
git commit -m "feat: add mixer AMCP family"
```

### Task 6: Implement OSC UDP intake and domain mapping

**Files:**
- Create: `src/StarDust.CasparCG.Protocol.Osc/OscMessage.cs`
- Create: `src/StarDust.CasparCG.Protocol.Osc/OscPacketParser.cs`
- Create: `src/StarDust.CasparCG.Protocol.Osc/DefaultOscMessageMapper.cs`
- Create: `src/StarDust.CasparCG.Transport/IOscTransport.cs`
- Create: `src/StarDust.CasparCG.Transport/UdpOscListener.cs`
- Modify: `src/StarDust.CasparCG.Protocol.Osc/IOscMessageMapper.cs`
- Modify: `src/StarDust.CasparCG/CasparClient.cs`
- Create: `src/StarDust.CasparCG/Events/LayerPausedChangedEvent.cs`
- Create: `src/StarDust.CasparCG/Events/LayerTimeChangedEvent.cs`
- Modify: `src/StarDust.CasparCG/State/CasparStateStore.cs`
- Create: `src/StarDust.CasparCG.UnitTests/OscMessageMapperTests.cs`

- [ ] **Step 1: Write the failing OSC mapper tests**

```csharp
// src/StarDust.CasparCG.UnitTests/OscMessageMapperTests.cs
using StarDust.CasparCG.Events;
using StarDust.CasparCG.Protocol.Osc;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class OscMessageMapperTests
{
    [Fact]
    public void Paused_address_maps_to_domain_event()
    {
        var mapper = new DefaultOscMessageMapper("studio-a");
        var mapped = mapper.TryMap(
            new OscMessage("/channel/1/stage/layer/10/paused", new object?[] { 1 }),
            out var evt);

        Assert.True(mapped);
        var paused = Assert.IsType<LayerPausedChangedEvent>(evt);
        Assert.Equal(1, paused.Channel);
        Assert.Equal(10, paused.Layer);
        Assert.True(paused.IsPaused);
    }
}
```

- [ ] **Step 2: Run the OSC mapper tests to verify the types are missing**

Run: `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter OscMessageMapperTests -v minimal`

Expected: FAIL with missing OSC parser/message/mapper and event types.

- [ ] **Step 3: Implement the minimal OSC message and mapping layer**

Use this pattern:

```csharp
// src/StarDust.CasparCG.Protocol.Osc/OscMessage.cs
namespace StarDust.CasparCG.Protocol.Osc;

public sealed record OscMessage(string Address, IReadOnlyList<object?> Arguments);
```

```csharp
// src/StarDust.CasparCG.Protocol.Osc/DefaultOscMessageMapper.cs
using System.Text.RegularExpressions;
using StarDust.CasparCG.Events;

namespace StarDust.CasparCG.Protocol.Osc;

public sealed partial class DefaultOscMessageMapper : IOscMessageMapper
{
    private readonly string _clientName;
    public DefaultOscMessageMapper(string clientName) => _clientName = clientName;

    [GeneratedRegex(@"^/channel/(?<channel>\d+)/stage/layer/(?<layer>\d+)/paused$")]
    private static partial Regex PausedRegex();

    public bool TryMap(OscMessage message, out CasparEvent? evt)
    {
        var paused = PausedRegex().Match(message.Address);
        if (paused.Success)
        {
            evt = new LayerPausedChangedEvent(
                _clientName,
                int.Parse(paused.Groups["channel"].Value),
                int.Parse(paused.Groups["layer"].Value),
                Convert.ToInt32(message.Arguments[0]) == 1);
            return true;
        }

        evt = null;
        return false;
    }
}
```

- [ ] **Step 4: Run the OSC mapper tests**

Run: `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter OscMessageMapperTests -v minimal`

Expected: PASS.

- [ ] **Step 5: Commit the OSC mapping slice**

```bash
git add src/StarDust.CasparCG.Protocol.Osc src/StarDust.CasparCG.Transport src/StarDust.CasparCG src/StarDust.CasparCG.UnitTests/OscMessageMapperTests.cs
git commit -m "feat: add OSC UDP mapping pipeline"
```

### Task 7: Expand dummy and local-server integration coverage

**Files:**
- Create: `src/StarDust.CasparCG.Testing/DummyServer/DummyOscEmitter.cs`
- Modify: `src/StarDust.CasparCG.IntegrationTests/CasparClientDummyServerTests.cs`
- Create: `src/StarDust.CasparCG.IntegrationTests/CasparClientLocalServerTests.cs`
- Create: `src/StarDust.CasparCG.IntegrationTests/OscLocalServerTests.cs`

- [ ] **Step 1: Write a failing local-server gated test**

```csharp
// src/StarDust.CasparCG.IntegrationTests/CasparClientLocalServerTests.cs
using StarDust.CasparCG;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.IntegrationTests;

public class CasparClientLocalServerTests
{
    [Fact]
    public async Task Version_query_returns_value_from_local_server()
    {
        if (Environment.GetEnvironmentVariable("CASPARCG_LOCAL_TESTS") != "1")
            return;

        var client = new CasparClient(new TcpAmcpTransport("127.0.0.1", 5250));
        await client.ConnectAsync(CancellationToken.None);

        var version = await client.Query.VersionAsync(CancellationToken.None);

        Assert.False(string.IsNullOrWhiteSpace(version));
    }
}
```

- [ ] **Step 2: Run the integration test to verify the local-server query API is not complete yet**

Run: `dotnet test src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --filter CasparClientLocalServerTests -v minimal`

Expected: FAIL until the query surface is fully wired in integration.

- [ ] **Step 3: Expand dummy-server tests and add the gated local tests**

Required final coverage:

- dummy server:
  - `PLAY`
  - `VERSION`
  - one `CG ADD`
  - one `DATA STORE`
- local server:
  - `VERSION`
  - one playout command
  - one mixer command
- local OSC:
  - receive at least one mapped event while the server is running

- [ ] **Step 4: Run the deterministic integration suite**

Run: `dotnet test src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --filter CasparClientDummyServerTests -v minimal`

Expected: PASS.

- [ ] **Step 5: Run the local-server checks manually**

Run:

```bash
CASPARCG_LOCAL_TESTS=1 dotnet test src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --filter "CasparClientLocalServerTests|OscLocalServerTests" -v minimal
```

Expected: PASS against the running local server.

- [ ] **Step 6: Commit the integration slice**

```bash
git add src/StarDust.CasparCG.Testing src/StarDust.CasparCG.IntegrationTests
git commit -m "test: add local server and OSC integration coverage"
```

### Task 8: Retire the legacy projects from the active solution and docs

**Files:**
- Modify: `src/StarDust.CasparCG.net.sln`
- Modify: `README.md`
- Modify: `BREAKING_CHANGES.md`
- Modify: `CONTRIBUTING.md`
- Delete from solution: all legacy `StarDust.CasparCG.net.*`, `StartDust.CasparCG.net.*`, and demo projects

- [ ] **Step 1: Write the failing solution-shape check**

```bash
# temporary manual check command
grep -q "StarDust.CasparCG.UnitTests" src/StarDust.CasparCG.net.sln
! grep -q "StarDust.CasparCG.net.OSC" src/StarDust.CasparCG.net.sln
! grep -q "StarDust.CasparCg.net.AmcpProtocol" src/StarDust.CasparCG.net.sln
```

- [ ] **Step 2: Remove the legacy projects from the solution**

Run:

```bash
dotnet sln src/StarDust.CasparCG.net.sln remove src/StarDust.CasparCG.net.Models/StarDust.CasparCg.net.Models.csproj
dotnet sln src/StarDust.CasparCG.net.sln remove src/StarDust.CasparCG.net.Connection/StarDust.CasparCg.net.Connection.csproj
dotnet sln src/StarDust.CasparCG.net.sln remove src/StarDust.CasparCg.net.AmcpProtocol/StarDust.CasparCg.net.AmcpProtocol.csproj
dotnet sln src/StarDust.CasparCG.net.sln remove src/StarDust.CasparCg.net.Device/StarDust.CasparCg.net.Device.csproj
dotnet sln src/StarDust.CasparCG.net.sln remove src/StarDust.CasparCG.net.OSC/StarDust.CasparCG.net.OSC.csproj
dotnet sln src/StarDust.CasparCG.net.sln remove src/StarDust.CasparCG.net.OSC.EventHub/StarDust.CasparCG.net.OSC.EventHub.csproj
dotnet sln src/StarDust.CasparCG.net.sln remove src/StarDust.CasparCG.net.Microsoft.DependencyInjection/StarDust.CasparCG.net.Microsoft.DependencyInjection.csproj
```

- [ ] **Step 3: Update docs to state that vNext is now the active solution path**

Required doc edits:

- `README.md` says the vNext projects are the only active solution members
- `BREAKING_CHANGES.md` explicitly mentions legacy solution removal
- `CONTRIBUTING.md` removes legacy validation paths

- [ ] **Step 4: Verify the solution no longer references legacy projects**

Run: `grep -n "StarDust.CasparCG.net.OSC\\|StarDust.CasparCg.net.AmcpProtocol\\|StarDust.CasparCG.net.Models" src/StarDust.CasparCG.net.sln`

Expected: no matches.

- [ ] **Step 5: Commit the legacy-retirement slice**

```bash
git add src/StarDust.CasparCG.net.sln README.md BREAKING_CHANGES.md CONTRIBUTING.md
git commit -m "chore: retire legacy projects from active solution"
```

### Task 9: Repair packaging and workflow verification around the vNext graph

**Files:**
- Modify: `src/StarDust.CasparCG/StarDust.CasparCG.csproj`
- Modify: `src/StarDust.CasparCG.Transport/StarDust.CasparCG.Transport.csproj`
- Modify: `.github/workflows/ci.yml`
- Modify: `.github/workflows/package.yml`
- Modify: `scripts/verify-workflows.sh`

- [ ] **Step 1: Write the failing package verification command**

Run: `dotnet pack src/StarDust.CasparCG/StarDust.CasparCG.csproj --configuration Release -o artifacts/packages`

Expected: FAIL until the project graph and pack metadata are corrected.

- [ ] **Step 2: Fix pack metadata and project graph issues**

Required adjustments:

- add explicit package metadata to `StarDust.CasparCG.csproj`
- ensure referenced vNext projects report target frameworks cleanly
- keep workflows building and testing the vNext project graph only

Recommended metadata block:

```xml
<PropertyGroup>
  <PackageId>StarDust.CasparCG</PackageId>
  <Authors>Romain Jourde</Authors>
  <Company>StarDust</Company>
  <PackageLicenseExpression>MIT</PackageLicenseExpression>
  <Description>Modern CasparCG client for AMCP, OSC, events, and hosting integration.</Description>
</PropertyGroup>
```

- [ ] **Step 3: Run workflow and packaging checks**

Run:

```bash
./scripts/verify-workflows.sh
dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --configuration Release
dotnet test src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --configuration Release
dotnet pack src/StarDust.CasparCG/StarDust.CasparCG.csproj --configuration Release -o artifacts/packages
```

Expected: PASS, with `.nupkg` files under `artifacts/packages`.

- [ ] **Step 4: Commit the packaging slice**

```bash
git add src/StarDust.CasparCG/*.csproj src/StarDust.CasparCG.Transport/*.csproj .github/workflows scripts/verify-workflows.sh
git commit -m "build: fix vNext packaging and CI validation"
```

### Task 10: Final verification and cleanup

**Files:**
- Modify: `README.md`
- Modify: `BREAKING_CHANGES.md`
- Modify: `CONTRIBUTING.md`
- Modify: `docs/vnext/*.md` as needed

- [ ] **Step 1: Run the full verification matrix**

Run:

```bash
dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --configuration Release
dotnet test src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --configuration Release
CASPARCG_LOCAL_TESTS=1 dotnet test src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --filter "CasparClientLocalServerTests|OscLocalServerTests" -v minimal
./scripts/verify-docs.sh
./scripts/verify-workflows.sh
dotnet pack src/StarDust.CasparCG/StarDust.CasparCG.csproj --configuration Release -o artifacts/packages
git diff --check
```

Expected: PASS end-to-end.

- [ ] **Step 2: Review the branch diff for accidental legacy churn**

Run: `git diff --stat master...HEAD`

Expected: the diff should show the vNext projects, tests, docs, scripts, workflows, and solution cleanup. Stop if unrelated legacy runtime edits remain.

- [ ] **Step 3: Commit the final verification pass**

```bash
git add README.md BREAKING_CHANGES.md CONTRIBUTING.md docs/vnext scripts .github/workflows src/StarDust.CasparCG.net.sln
git commit -m "chore: finalize vNext protocol runtime"
```
