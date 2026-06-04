# Typed Query Results Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Replace high-level raw/string AMCP query results with typed records for stable payloads and dictionary-based objects for looser query payloads.

**Architecture:** Keep `AmcpResponseParser` as the low-level AMCP framing parser and add a focused query projection layer above it. Stable responses (`CLS`, `CINF`, `TLS`, `FLS`) map to dedicated records, while loose responses (`INFO`, `INFO CONFIG`, `INFO PATHS`, `GL INFO`) map to a `QueryDataMap` that preserves both parsed values and raw content.

**Tech Stack:** .NET 10, C#, xUnit, existing `CasparClient`, AMCP command/query pipeline, `ReadOnlySpan<char>`, `System.Xml.Linq`.

---

## File Map

- Create: `src/StarDust.CasparCG/Query/MediaFile.cs`
- Create: `src/StarDust.CasparCG/Query/MediaInfo.cs`
- Create: `src/StarDust.CasparCG/Query/TemplateFile.cs`
- Create: `src/StarDust.CasparCG/Query/FontFile.cs`
- Create: `src/StarDust.CasparCG/Query/QueryDataMap.cs`
- Create: `src/StarDust.CasparCG/Query/MediaFileKind.cs`
- Create: `src/StarDust.CasparCG/Query/CasparQueryParseException.cs`
- Create: `src/StarDust.CasparCG/Query/CasparQueryResultParser.cs`
- Modify: `src/StarDust.CasparCG/CasparClient.cs`
- Modify: `src/StarDust.CasparCG/Fluent/ServerScope.cs`
- Modify: `src/Demo/Demo AMCP/Demo.AMCP.netcore/Executor.cs`
- Modify: `test/StarDust.CasparCG.UnitTests/AmcpResponseParserTests.cs`
- Modify: `test/StarDust.CasparCG.UnitTests/CasparClientCommandTests.cs`
- Modify: `test/StarDust.CasparCG.UnitTests/CasparClientBootstrapTests.cs`
- Modify: `test/StarDust.CasparCG.UnitTests/FluentScopeCoverageTests.cs`
- Create: `test/StarDust.CasparCG.UnitTests/CasparQueryResultParserTests.cs`
- Modify: `docs/vnext/fluent-api-cookbook.md`

### Task 1: Add the query result models and parser tests

**Files:**
- Create: `src/StarDust.CasparCG/Query/MediaFile.cs`
- Create: `src/StarDust.CasparCG/Query/MediaInfo.cs`
- Create: `src/StarDust.CasparCG/Query/TemplateFile.cs`
- Create: `src/StarDust.CasparCG/Query/FontFile.cs`
- Create: `src/StarDust.CasparCG/Query/QueryDataMap.cs`
- Create: `src/StarDust.CasparCG/Query/MediaFileKind.cs`
- Create: `src/StarDust.CasparCG/Query/CasparQueryParseException.cs`
- Create: `test/StarDust.CasparCG.UnitTests/CasparQueryResultParserTests.cs`

- [ ] **Step 1: Write the failing parser tests**

```csharp
using StarDust.CasparCG.Protocol.Amcp;
using StarDust.CasparCG.Query;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public sealed class CasparQueryResultParserTests
{
    [Fact]
    public void ParseMediaFiles_parses_complete_cls_lines()
    {
        var response = AmcpResponseParser.Parse(
            "200 CLS OK\r\n" +
            "\"AMB\" MOVIE 42 20240101120000 240 1/25\r\n" +
            "\"PROMO WITH SPACE\" STILL 13 20240101120001\r\n\r\n");

        var result = CasparQueryResultParser.ParseMediaFiles(response);

        Assert.Equal(2, result.Count);
        Assert.Equal("AMB", result[0].Name);
        Assert.Equal(MediaFileKind.Movie, result[0].Kind);
        Assert.Equal("PROMO WITH SPACE", result[1].Name);
        Assert.Equal(MediaFileKind.Still, result[1].Kind);
    }

    [Fact]
    public void ParseMediaInfo_preserves_extra_fields_as_properties()
    {
        var response = AmcpResponseParser.Parse(
            "200 CINF OK\r\n" +
            "\"AMB\" MOVIE 42 20240101120000 240 1/25 FIELD_A VALUE_A FIELD_B VALUE_B\r\n\r\n");

        var result = CasparQueryResultParser.ParseMediaInfo(response);

        Assert.Equal("AMB", result.Name);
        Assert.Equal("VALUE_A", result.Properties["FIELD_A"]);
        Assert.Equal("VALUE_B", result.Properties["FIELD_B"]);
    }

    [Fact]
    public void ParseTemplateFiles_returns_template_records()
    {
        var response = AmcpResponseParser.Parse("200 TLS OK\r\nLOWERTHIRD\r\nFULLFRAME\r\n\r\n");

        var result = CasparQueryResultParser.ParseTemplateFiles(response);

        Assert.Equal(["LOWERTHIRD", "FULLFRAME"], result.Select(x => x.Name).ToArray());
    }

    [Fact]
    public void ParseFontFiles_returns_font_records()
    {
        var response = AmcpResponseParser.Parse("200 FLS OK\r\n\"Roboto\" fonts/roboto.ttf\r\n\r\n");

        var result = CasparQueryResultParser.ParseFontFiles(response);

        Assert.Equal("Roboto", result.Single().Name);
        Assert.Equal("fonts/roboto.ttf", result.Single().Path);
    }

    [Fact]
    public void ParseQueryDataMap_flattens_xml_payloads()
    {
        var response = AmcpResponseParser.Parse(
            "200 INFO PATHS OK\r\n" +
            "<paths><media-path>media/</media-path><log-path>log/</log-path></paths>\r\n\r\n");

        var result = CasparQueryResultParser.ParseQueryDataMap(response);

        Assert.Equal("media/", result.Values["paths.media-path"]);
        Assert.Equal("log/", result.Values["paths.log-path"]);
    }

    [Fact]
    public void ParseQueryDataMap_keeps_lines_when_payload_is_not_fully_recognized()
    {
        var response = AmcpResponseParser.Parse("201 GL INFO OK\r\nrenderer-info vendor-info opaque-token\r\n");

        var result = CasparQueryResultParser.ParseQueryDataMap(response);

        Assert.Single(result.Lines);
        Assert.Equal("renderer-info vendor-info opaque-token", result.Lines[0]);
        Assert.Equal("201 GL INFO OK\r\nrenderer-info vendor-info opaque-token\r\n", result.Raw);
    }
}
```

- [ ] **Step 2: Run the parser tests to verify they fail**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "CasparQueryResultParserTests" -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: FAIL with missing `StarDust.CasparCG.Query` types and missing `CasparQueryResultParser`.

- [ ] **Step 3: Add the minimal query result model files**

```csharp
namespace StarDust.CasparCG.Query;

public enum MediaFileKind
{
    Unknown,
    Movie,
    Still,
    Audio
}

public sealed record MediaFile(
    string Name,
    MediaFileKind Kind,
    long SizeBytes,
    DateTimeOffset? LastModified,
    long? FrameCount,
    string? FrameRateOrDuration);

public sealed record MediaInfo(
    string Name,
    MediaFileKind Kind,
    long? SizeBytes,
    DateTimeOffset? LastModified,
    long? FrameCount,
    string? FrameRateOrDuration,
    IReadOnlyDictionary<string, string> Properties);

public sealed record TemplateFile(string Name);

public sealed record FontFile(string Name, string Path);

public sealed record QueryDataMap(
    IReadOnlyDictionary<string, string> Values,
    IReadOnlyList<string> Lines,
    string Raw);

public sealed class CasparQueryParseException(string message) : Exception(message);
```

- [ ] **Step 4: Run the parser tests again**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "CasparQueryResultParserTests" -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: FAIL with missing `CasparQueryResultParser` methods rather than missing model types.

- [ ] **Step 5: Commit the model and test scaffold**

```bash
git add src/StarDust.CasparCG/Query test/StarDust.CasparCG.UnitTests/CasparQueryResultParserTests.cs
git commit -m "test: add typed query parser coverage"
```

### Task 2: Implement the query projection parser

**Files:**
- Create: `src/StarDust.CasparCG/Query/CasparQueryResultParser.cs`
- Modify: `test/StarDust.CasparCG.UnitTests/CasparQueryResultParserTests.cs`

- [ ] **Step 1: Write one stricter failing test for malformed stable payloads**

```csharp
[Fact]
public void ParseMediaFiles_throws_for_malformed_cls_line()
{
    var response = AmcpResponseParser.Parse("200 CLS OK\r\nBROKEN_LINE\r\n\r\n");

    var exception = Assert.Throws<CasparQueryParseException>(
        () => CasparQueryResultParser.ParseMediaFiles(response));

    Assert.Contains("BROKEN_LINE", exception.Message);
}
```

- [ ] **Step 2: Run the parser tests to verify the new failure**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "CasparQueryResultParserTests" -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: FAIL because `CasparQueryResultParser` is not implemented.

- [ ] **Step 3: Implement the minimal parser**

```csharp
using System.Globalization;
using System.Xml.Linq;
using StarDust.CasparCG.Protocol.Amcp;

namespace StarDust.CasparCG.Query;

internal static class CasparQueryResultParser
{
    public static IReadOnlyList<MediaFile> ParseMediaFiles(AmcpResponse response) =>
        response.Lines.Select(ParseMediaFile).ToArray();

    public static MediaInfo ParseMediaInfo(AmcpResponse response)
    {
        var media = ParseMediaFile(response.Lines.Single());
        var extras = ParseTrailingPairs(response.Lines.Single());

        return new MediaInfo(
            media.Name,
            media.Kind,
            media.SizeBytes,
            media.LastModified,
            media.FrameCount,
            media.FrameRateOrDuration,
            extras);
    }

    public static IReadOnlyList<TemplateFile> ParseTemplateFiles(AmcpResponse response) =>
        response.Lines.Select(line => new TemplateFile(line.Trim())).ToArray();

    public static IReadOnlyList<FontFile> ParseFontFiles(AmcpResponse response) =>
        response.Lines.Select(ParseFontFile).ToArray();

    public static QueryDataMap ParseQueryDataMap(AmcpResponse response)
    {
        var values = TryParseXml(response.Raw) ?? TryParseKeyValueLines(response.Lines) ?? new Dictionary<string, string>();
        return new QueryDataMap(values, response.Lines, response.Raw);
    }
}
```

- [ ] **Step 4: Run the parser tests to verify they pass**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "CasparQueryResultParserTests" -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: PASS with all `CasparQueryResultParserTests` green.

- [ ] **Step 5: Commit the parser**

```bash
git add src/StarDust.CasparCG/Query/CasparQueryResultParser.cs test/StarDust.CasparCG.UnitTests/CasparQueryResultParserTests.cs
git commit -m "feat: parse typed query results"
```

### Task 3: Switch `CasparClient` query methods to typed results

**Files:**
- Modify: `src/StarDust.CasparCG/CasparClient.cs`
- Modify: `test/StarDust.CasparCG.UnitTests/CasparClientCommandTests.cs`
- Modify: `test/StarDust.CasparCG.UnitTests/CasparClientBootstrapTests.cs`

- [ ] **Step 1: Write the failing client tests for typed query results**

```csharp
[Fact]
public async Task GetMediaFilesAsync_returns_typed_media_records()
{
    var transport = new RecordingAmcpTransport(
        "200 CLS OK\r\n" +
        "\"AMB\" MOVIE 42 20240101120000 240 1/25\r\n\r\n");
    var client = new CasparClient(transport);

    var mediaFiles = await client.GetMediaFilesAsync(CancellationToken.None);

    Assert.Equal("CLS\r\n", transport.LastCommandText);
    Assert.Equal("AMB", mediaFiles.Single().Name);
    Assert.Equal(MediaFileKind.Movie, mediaFiles.Single().Kind);
}

[Fact]
public async Task InfoPathsAsync_returns_query_data_map()
{
    var transport = new RecordingAmcpTransport(
        "200 INFO PATHS OK\r\n<paths><media-path>media/</media-path></paths>\r\n\r\n");
    var client = new CasparClient(transport);

    var result = await client.InfoPathsAsync(CancellationToken.None);

    Assert.Equal("INFO PATHS\r\n", transport.LastCommandText);
    Assert.Equal("media/", result.Values["paths.media-path"]);
}
```

- [ ] **Step 2: Run the focused client tests to verify they fail**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "GetMediaFilesAsync_returns_typed_media_records|InfoPathsAsync_returns_query_data_map|CasparClient_exposes_basic_and_query_helpers_with_thin_wrappers" -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: FAIL because current signatures return `IReadOnlyList<string>` and `AmcpResponse`.

- [ ] **Step 3: Update `CasparClient` to project query responses**

```csharp
using StarDust.CasparCG.Query;

public async ValueTask<QueryDataMap> InfoAsync(CancellationToken cancellationToken) =>
    CasparQueryResultParser.ParseQueryDataMap(
        await QueryAsync(new InfoCommand(), cancellationToken));

public async ValueTask<QueryDataMap> InfoConfigAsync(CancellationToken cancellationToken) =>
    CasparQueryResultParser.ParseQueryDataMap(
        await QueryAsync(new InfoConfigCommand(), cancellationToken));

public async ValueTask<QueryDataMap> InfoPathsAsync(CancellationToken cancellationToken) =>
    CasparQueryResultParser.ParseQueryDataMap(
        await QueryAsync(new InfoPathsCommand(), cancellationToken));

public async ValueTask<IReadOnlyList<MediaFile>> GetMediaFilesAsync(CancellationToken cancellationToken) =>
    CasparQueryResultParser.ParseMediaFiles(
        await QueryAsync(new ListMediaFilesCommand(), cancellationToken));

public async ValueTask<MediaInfo> MediaInfoAsync(string fileName, CancellationToken cancellationToken) =>
    CasparQueryResultParser.ParseMediaInfo(
        await QueryAsync(new CinfCommand(fileName), cancellationToken));

public async ValueTask<IReadOnlyList<FontFile>> GetFontFilesAsync(CancellationToken cancellationToken) =>
    CasparQueryResultParser.ParseFontFiles(
        await QueryAsync(new FlsCommand(), cancellationToken));

public async ValueTask<IReadOnlyList<TemplateFile>> GetTemplateFilesAsync(string? subDirectory, CancellationToken cancellationToken) =>
    CasparQueryResultParser.ParseTemplateFiles(
        await QueryAsync(new TlsCommand(subDirectory), cancellationToken));

public async ValueTask<QueryDataMap> GlInfoAsync(CancellationToken cancellationToken) =>
    CasparQueryResultParser.ParseQueryDataMap(
        await QueryAsync(new GlInfoCommand(), cancellationToken));
```

- [ ] **Step 4: Run the focused client tests to verify they pass**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "GetMediaFilesAsync_returns_typed_media_records|InfoPathsAsync_returns_query_data_map|CasparClient_exposes_basic_and_query_helpers_with_thin_wrappers" -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: PASS for the new typed query tests and updated bootstrap assertions.

- [ ] **Step 5: Commit the client API switch**

```bash
git add src/StarDust.CasparCG/CasparClient.cs test/StarDust.CasparCG.UnitTests/CasparClientCommandTests.cs test/StarDust.CasparCG.UnitTests/CasparClientBootstrapTests.cs
git commit -m "feat: return typed caspar query results"
```

### Task 4: Update `ServerScope` and fluent coverage to match the new query contracts

**Files:**
- Modify: `src/StarDust.CasparCG/Fluent/ServerScope.cs`
- Modify: `test/StarDust.CasparCG.UnitTests/FluentScopeCoverageTests.cs`
- Modify: `docs/vnext/fluent-api-cookbook.md`

- [ ] **Step 1: Write the failing fluent scope coverage updates**

```csharp
[Fact]
public async Task ServerScope_query_methods_forward_typed_query_results()
{
    var transport = new RecordingAmcpTransport(
        "200 CLS OK\r\n\"AMB\" MOVIE 42 20240101120000 240 1/25\r\n\r\n",
        "200 CINF OK\r\n\"AMB\" MOVIE 42 20240101120000 240 1/25\r\n\r\n",
        "200 FLS OK\r\n\"Roboto\" fonts/roboto.ttf\r\n\r\n",
        "200 TLS OK\r\nLOWERTHIRD\r\n\r\n",
        "200 INFO PATHS OK\r\n<paths><media-path>media/</media-path></paths>\r\n\r\n",
        "201 GL INFO OK\r\nrenderer: opengl\r\n");
    var client = new CasparClient(transport);

    var mediaFiles = await client.Server().MediaFilesAsync(CancellationToken.None);
    var mediaInfo = await client.Server().MediaInfoAsync("AMB", CancellationToken.None);
    var fonts = await client.Server().FontFilesAsync(CancellationToken.None);
    var templates = await client.Server().TemplateFilesAsync(CancellationToken.None);
    var paths = await client.Server().InfoPathsAsync(CancellationToken.None);
    var glInfo = await client.Server().GlInfoAsync(CancellationToken.None);

    Assert.Equal("AMB", mediaFiles.Single().Name);
    Assert.Equal("AMB", mediaInfo.Name);
    Assert.Equal("Roboto", fonts.Single().Name);
    Assert.Equal("LOWERTHIRD", templates.Single().Name);
    Assert.Equal("media/", paths.Values["paths.media-path"]);
    Assert.Equal("opengl", glInfo.Values["renderer"]);
}
```

- [ ] **Step 2: Run the fluent scope tests to verify they fail**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "FluentScopeCoverageTests" -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: FAIL because `ServerScope` still returns old types and old method names.

- [ ] **Step 3: Update `ServerScope` and the cookbook**

```csharp
using StarDust.CasparCG.Query;

public ValueTask<IReadOnlyList<MediaFile>> MediaFilesAsync(CancellationToken cancellationToken) =>
    client.GetMediaFilesAsync(cancellationToken);

public ValueTask<MediaInfo> MediaInfoAsync(string fileName, CancellationToken cancellationToken) =>
    client.MediaInfoAsync(fileName, cancellationToken);

public ValueTask<IReadOnlyList<FontFile>> FontFilesAsync(CancellationToken cancellationToken) =>
    client.GetFontFilesAsync(cancellationToken);

public ValueTask<IReadOnlyList<TemplateFile>> TemplateFilesAsync(CancellationToken cancellationToken) =>
    client.GetTemplateFilesAsync(null, cancellationToken);

public ValueTask<IReadOnlyList<TemplateFile>> TemplateFilesAsync(string? subDirectory, CancellationToken cancellationToken) =>
    client.GetTemplateFilesAsync(subDirectory, cancellationToken);
```

```csharp
var media = await client.Server().MediaFilesAsync(ct);
var lowerThirds = await client.Server().TemplateFilesAsync(ct);
var fonts = await client.Server().FontFilesAsync(ct);
```

- [ ] **Step 4: Run the fluent scope tests to verify they pass**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "FluentScopeCoverageTests" -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: PASS with all server-scope query forwarding assertions green.

- [ ] **Step 5: Commit the scope and cookbook update**

```bash
git add src/StarDust.CasparCG/Fluent/ServerScope.cs test/StarDust.CasparCG.UnitTests/FluentScopeCoverageTests.cs docs/vnext/fluent-api-cookbook.md
git commit -m "feat: expose typed query results in server scope"
```

### Task 5: Update the demo and run full verification

**Files:**
- Modify: `src/Demo/Demo AMCP/Demo.AMCP.netcore/Executor.cs`
- Modify: `test/StarDust.CasparCG.UnitTests/AmcpResponseParserTests.cs`

- [ ] **Step 1: Write one regression test proving the low-level parser contract stays generic**

```csharp
[Fact]
public void Parse_200_query_response_keeps_payload_lines_for_projection_layers()
{
    var response = AmcpResponseParser.Parse("200 INFO CONFIG OK\r\n<config><channel>1</channel></config>\r\n\r\n");

    Assert.Equal("INFO CONFIG", response.CommandText);
    Assert.Equal("<config><channel>1</channel></config>", response.Lines.Single());
}
```

- [ ] **Step 2: Run the parser regression test to verify the current contract still holds**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release --filter "AmcpResponseParserTests" -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: PASS and no contract drift in the low-level parser.

- [ ] **Step 3: Update the demo output to use typed query results**

```csharp
var medias = await client.GetMediaFilesAsync(cancellationToken);
Console.WriteLine($"Media files ({medias.Count}):");
foreach (var media in medias)
{
    Console.WriteLine($"{media.Name} [{media.Kind}]");
}
```

- [ ] **Step 4: Run the full verification suite**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: PASS with all unit tests green.

Run: `dotnet test test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj -c Release -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: PASS with all integration tests green.

Run: `dotnet build src/StarDust.CasparCG.net.sln -c Release -m:1 -p:BuildInParallel=false -nr:false -v minimal`

Expected: PASS with `0 Warning(s)` and `0 Error(s)`.

Run: `git diff --check`

Expected: no output.

- [ ] **Step 5: Commit the demo and verification slice**

```bash
git add src/Demo/Demo\ AMCP/Demo.AMCP.netcore/Executor.cs test/StarDust.CasparCG.UnitTests/AmcpResponseParserTests.cs
git commit -m "docs: update query result examples"
```

## Self-Review Checklist

- Spec coverage:
  - typed `CLS`, `CINF`, `TLS`, `FLS` are covered by Tasks 1-3
  - dictionary-based `INFO`, `INFO CONFIG`, `INFO PATHS`, `GL INFO` are covered by Tasks 1-4
  - `CasparClient` API switch is covered by Task 3
  - `ServerScope` API switch is covered by Task 4
  - low-level `AmcpResponseParser` stability is covered by Task 5
- Placeholder scan:
  - no `TODO`, `TBD`, or “similar to previous task” placeholders remain
  - each code-changing task includes concrete code examples
- Type consistency:
  - stable query methods use `MediaFile`, `MediaInfo`, `TemplateFile`, `FontFile`
  - loose query methods use `QueryDataMap`
  - scope method names switch from `FileListAsync` / `TemplateListAsync` to `FontFilesAsync` / `TemplateFilesAsync`
