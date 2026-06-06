# CasparCG REST Proxy CG and Thumbnails Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Extend `StarDust.CasparCG.AspNetCore` with CG template routes and thumbnail routes, including JSON/XML CG payload support and browser-friendly thumbnail retrieval.

**Architecture:** Add a dedicated CG payload translator and thumbnail response parser inside the ASP.NET Core addon, then wire them through thin Minimal API handlers that continue to delegate to `CasparClient`. Keep the slice focused on `CG + thumbnails`, with route docs updated to show the implemented REST surface clearly.

**Tech Stack:** .NET 10, ASP.NET Core Minimal API, xUnit, existing Caspar dummy server integration harness

---

## File Structure

### New files

- `src/StarDust.CasparCG.AspNetCore/Contracts/CgAddJsonRequest.cs`
  - JSON request contract for `POST /channels/{channel}/layers/{layer}/cg/add`
- `src/StarDust.CasparCG.AspNetCore/Contracts/CgUpdateJsonRequest.cs`
  - JSON request contract for `POST /channels/{channel}/layers/{layer}/cg/update`
- `src/StarDust.CasparCG.AspNetCore/Contracts/CgInvokeRequest.cs`
  - JSON request contract for `POST /channels/{channel}/layers/{layer}/cg/invoke`
- `src/StarDust.CasparCG.AspNetCore/Contracts/TemplateDataEnvelope.cs`
  - typed JSON envelope for deterministic JSON-to-XML translation
- `src/StarDust.CasparCG.AspNetCore/Contracts/ThumbnailListItemResponse.cs`
  - structured thumbnail list response DTO
- `src/StarDust.CasparCG.AspNetCore/Internal/CgPayloadTranslationResult.cs`
  - internal result object describing translated XML plus route metadata
- `src/StarDust.CasparCG.AspNetCore/Internal/CgPayloadTranslator.cs`
  - content-type aware translator for `application/json`, `application/xml`, and `text/xml`
- `src/StarDust.CasparCG.AspNetCore/Internal/ThumbnailResponseParser.cs`
  - parser for `THUMBNAIL LIST` and `THUMBNAIL RETRIEVE` responses
- `test/StarDust.CasparCG.UnitTests/RestApiCgPayloadTranslatorTests.cs`
  - unit tests for JSON/XML translation and media type validation
- `test/StarDust.CasparCG.UnitTests/RestApiThumbnailResponseParserTests.cs`
  - unit tests for thumbnail list and retrieve parsing
- `test/StarDust.CasparCG.IntegrationTests/CasparRestApiCgTests.cs`
  - integration tests for the new CG routes
- `test/StarDust.CasparCG.IntegrationTests/CasparRestApiThumbnailTests.cs`
  - integration tests for the new thumbnail routes

### Modified files

- `src/StarDust.CasparCG.AspNetCore/EndpointRouteBuilderExtensions.cs`
  - map the new CG and thumbnail routes
- `src/StarDust.CasparCG.AspNetCore/Internal/CasparCommandHandlers.cs`
  - add CG command handlers and thumbnail generation handlers
- `src/StarDust.CasparCG.AspNetCore/Internal/CasparQueryHandlers.cs`
  - add thumbnail list and retrieve handlers
- `test/StarDust.CasparCG.UnitTests/RestApiEndpointTests.cs`
  - assert the expanded route table
- `README.md`
  - add the new REST routes and note partial AMCP parity
- `docs/vnext/hosting-and-di.md`
  - add the new route families to the ASP.NET Core addon usage docs
- `docs/vnext/rest-api-addon.md`
  - expand the route table and document the JSON/XML CG contract

---

### Task 1: Add failing unit tests for CG payload translation

**Files:**
- Create: `test/StarDust.CasparCG.UnitTests/RestApiCgPayloadTranslatorTests.cs`
- Create: `src/StarDust.CasparCG.AspNetCore/Contracts/CgAddJsonRequest.cs`
- Create: `src/StarDust.CasparCG.AspNetCore/Contracts/CgUpdateJsonRequest.cs`
- Create: `src/StarDust.CasparCG.AspNetCore/Contracts/CgInvokeRequest.cs`
- Create: `src/StarDust.CasparCG.AspNetCore/Contracts/TemplateDataEnvelope.cs`
- Create: `src/StarDust.CasparCG.AspNetCore/Internal/CgPayloadTranslationResult.cs`
- Create: `src/StarDust.CasparCG.AspNetCore/Internal/CgPayloadTranslator.cs`

- [ ] **Step 1: Write the failing translator tests**

```csharp
using System.Text;
using Microsoft.AspNetCore.Http;
using StarDust.CasparCG.AspNetCore.Contracts;
using StarDust.CasparCG.AspNetCore.Internal;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public sealed class RestApiCgPayloadTranslatorTests
{
    [Fact]
    public async Task Translate_add_json_builds_expected_template_xml()
    {
        var context = new DefaultHttpContext();
        context.Request.ContentType = "application/json";
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(
            """
            {
              "template": "LowerThird",
              "playOnLoad": true,
              "templateData": {
                "components": [
                  {
                    "id": "f0",
                    "data": [
                      { "id": "headline", "value": "Hello" }
                    ]
                  }
                ]
              }
            }
            """));

        var result = await CgPayloadTranslator.TranslateAddAsync(context.Request, CancellationToken.None);

        Assert.Equal("LowerThird", result.Template);
        Assert.True(result.PlayOnLoad);
        Assert.Equal(
            "<templateData><componentData id=\"f0\"><data id=\"headline\" value=\"Hello\" /></componentData></templateData>",
            result.TemplateXml);
    }

    [Fact]
    public async Task Translate_add_xml_uses_query_metadata_and_raw_body()
    {
        var context = new DefaultHttpContext();
        context.Request.ContentType = "application/xml";
        context.Request.QueryString = new QueryString("?template=LowerThird&playOnLoad=true");
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("<templateData><componentData id=\"f0\" /></templateData>"));

        var result = await CgPayloadTranslator.TranslateAddAsync(context.Request, CancellationToken.None);

        Assert.Equal("LowerThird", result.Template);
        Assert.True(result.PlayOnLoad);
        Assert.Equal("<templateData><componentData id=\"f0\" /></templateData>", result.TemplateXml);
    }

    [Fact]
    public async Task Translate_update_rejects_unsupported_content_type()
    {
        var context = new DefaultHttpContext();
        context.Request.ContentType = "text/plain";
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("nope"));

        var exception = await Assert.ThrowsAsync<BadHttpRequestException>(
            () => CgPayloadTranslator.TranslateUpdateAsync(context.Request, CancellationToken.None).AsTask());

        Assert.Contains("Unsupported content type", exception.Message);
    }
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run:

```bash
rtk dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter RestApiCgPayloadTranslatorTests
```

Expected:

- FAIL because `CgPayloadTranslator` and the new request contracts do not exist yet

- [ ] **Step 3: Write the minimal translator and contracts**

```csharp
namespace StarDust.CasparCG.AspNetCore.Contracts;

public sealed record TemplateDataEnvelope(IReadOnlyList<TemplateComponent> Components)
{
    public sealed record TemplateComponent(string Id, IReadOnlyList<TemplateValue> Data);
    public sealed record TemplateValue(string Id, string Value);
}

public sealed record CgAddJsonRequest(string Template, bool PlayOnLoad, TemplateDataEnvelope TemplateData);
public sealed record CgUpdateJsonRequest(TemplateDataEnvelope TemplateData);
public sealed record CgInvokeRequest(string Method);
```

```csharp
namespace StarDust.CasparCG.AspNetCore.Internal;

internal sealed record CgPayloadTranslationResult(string? Template, bool PlayOnLoad, string TemplateXml);
```

```csharp
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using StarDust.CasparCG.AspNetCore.Contracts;

namespace StarDust.CasparCG.AspNetCore.Internal;

internal static class CgPayloadTranslator
{
    public static async ValueTask<CgPayloadTranslationResult> TranslateAddAsync(HttpRequest request, CancellationToken cancellationToken)
    {
        if (request.ContentType?.StartsWith("application/json", StringComparison.OrdinalIgnoreCase) == true)
        {
            var payload = await JsonSerializer.DeserializeAsync<CgAddJsonRequest>(request.Body, cancellationToken: cancellationToken)
                ?? throw new BadHttpRequestException("Request body is required.");

            return new(payload.Template, payload.PlayOnLoad, ToTemplateXml(payload.TemplateData));
        }

        if (IsXml(request.ContentType))
        {
            var template = request.Query["template"].ToString();
            var playOnLoad = bool.TryParse(request.Query["playOnLoad"], out var parsed) && parsed;
            if (string.IsNullOrWhiteSpace(template))
            {
                throw new BadHttpRequestException("Query parameter 'template' is required for XML CG add requests.");
            }

            return new(template, playOnLoad, await ReadBodyAsync(request, cancellationToken));
        }

        throw new BadHttpRequestException($"Unsupported content type '{request.ContentType}'.");
    }

    public static async ValueTask<CgPayloadTranslationResult> TranslateUpdateAsync(HttpRequest request, CancellationToken cancellationToken)
    {
        if (request.ContentType?.StartsWith("application/json", StringComparison.OrdinalIgnoreCase) == true)
        {
            var payload = await JsonSerializer.DeserializeAsync<CgUpdateJsonRequest>(request.Body, cancellationToken: cancellationToken)
                ?? throw new BadHttpRequestException("Request body is required.");

            return new(null, false, ToTemplateXml(payload.TemplateData));
        }

        if (IsXml(request.ContentType))
        {
            return new(null, false, await ReadBodyAsync(request, cancellationToken));
        }

        throw new BadHttpRequestException($"Unsupported content type '{request.ContentType}'.");
    }

    private static bool IsXml(string? contentType) =>
        contentType?.StartsWith("application/xml", StringComparison.OrdinalIgnoreCase) == true
        || contentType?.StartsWith("text/xml", StringComparison.OrdinalIgnoreCase) == true;

    private static async Task<string> ReadBodyAsync(HttpRequest request, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var xml = await reader.ReadToEndAsync(cancellationToken);
        _ = XDocument.Parse(xml);
        return xml;
    }

    private static string ToTemplateXml(TemplateDataEnvelope envelope)
    {
        var document = new XElement(
            "templateData",
            envelope.Components.Select(component =>
                new XElement(
                    "componentData",
                    new XAttribute("id", component.Id),
                    component.Data.Select(item =>
                        new XElement(
                            "data",
                            new XAttribute("id", item.Id),
                            new XAttribute("value", item.Value))))));

        return document.ToString(SaveOptions.DisableFormatting);
    }
}
```

- [ ] **Step 4: Run the translator tests to verify they pass**

Run:

```bash
rtk dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter RestApiCgPayloadTranslatorTests
```

Expected:

- PASS for the three translator tests

- [ ] **Step 5: Commit the translator slice**

```bash
git add test/StarDust.CasparCG.UnitTests/RestApiCgPayloadTranslatorTests.cs \
  src/StarDust.CasparCG.AspNetCore/Contracts/CgAddJsonRequest.cs \
  src/StarDust.CasparCG.AspNetCore/Contracts/CgUpdateJsonRequest.cs \
  src/StarDust.CasparCG.AspNetCore/Contracts/CgInvokeRequest.cs \
  src/StarDust.CasparCG.AspNetCore/Contracts/TemplateDataEnvelope.cs \
  src/StarDust.CasparCG.AspNetCore/Internal/CgPayloadTranslationResult.cs \
  src/StarDust.CasparCG.AspNetCore/Internal/CgPayloadTranslator.cs
git commit -m "feat: add cg payload translator"
```

### Task 2: Add failing route tests and implement CG endpoints

**Files:**
- Modify: `src/StarDust.CasparCG.AspNetCore/EndpointRouteBuilderExtensions.cs`
- Modify: `src/StarDust.CasparCG.AspNetCore/Internal/CasparCommandHandlers.cs`
- Modify: `test/StarDust.CasparCG.UnitTests/RestApiEndpointTests.cs`
- Create: `test/StarDust.CasparCG.IntegrationTests/CasparRestApiCgTests.cs`

- [ ] **Step 1: Add the failing endpoint and integration tests**

```csharp
[Fact]
public void MapCasparCGApi_includes_cg_routes()
{
    var patterns = RestApiEndpointTestHost.GetRoutePatterns();

    Assert.Contains("/channels/{channel:int}/layers/{layer:int}/cg/add", patterns);
    Assert.Contains("/channels/{channel:int}/layers/{layer:int}/cg/play", patterns);
    Assert.Contains("/channels/{channel:int}/layers/{layer:int}/cg/stop", patterns);
    Assert.Contains("/channels/{channel:int}/layers/{layer:int}/cg/next", patterns);
    Assert.Contains("/channels/{channel:int}/layers/{layer:int}/cg/remove", patterns);
    Assert.Contains("/channels/{channel:int}/layers/{layer:int}/cg/clear", patterns);
    Assert.Contains("/channels/{channel:int}/layers/{layer:int}/cg/update", patterns);
    Assert.Contains("/channels/{channel:int}/layers/{layer:int}/cg/invoke", patterns);
}
```

```csharp
[Fact]
public async Task Post_cg_add_with_json_sends_expected_command()
{
    await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
        .WithAmcpReply(
            "CG ADD 1-10 LowerThird 1 <templateData><componentData id=\"f0\"><data id=\"headline\" value=\"Hello\" /></componentData></templateData>",
            "202 CG ADD OK\r\n"));

    var response = await fixture.Client.PostAsJsonAsync(
        "/channels/1/layers/10/cg/add",
        new CgAddJsonRequest(
            "LowerThird",
            true,
            new TemplateDataEnvelope(
                [
                    new("f0", [new("headline", "Hello")])
                ])));

    response.EnsureSuccessStatusCode();
    Assert.Equal(
        ["CG ADD 1-10 LowerThird 1 <templateData><componentData id=\"f0\"><data id=\"headline\" value=\"Hello\" /></componentData></templateData>"],
        fixture.ReceivedCommands);
}

[Fact]
public async Task Post_cg_update_with_xml_body_sends_expected_command()
{
    await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
        .WithAmcpReply(
            "CG UPDATE 1-10 <templateData><componentData id=\"f0\" /></templateData>",
            "201 CG UPDATE OK\r\n"));

    using var content = new StringContent("<templateData><componentData id=\"f0\" /></templateData>", Encoding.UTF8, "application/xml");
    var response = await fixture.Client.PostAsync("/channels/1/layers/10/cg/update", content);

    response.EnsureSuccessStatusCode();
    Assert.Equal(
        ["CG UPDATE 1-10 <templateData><componentData id=\"f0\" /></templateData>"],
        fixture.ReceivedCommands);
}

[Fact]
public async Task Post_cg_invoke_sends_expected_command()
{
    await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
        .WithAmcpReply("CG INVOKE 1-10 next()", "202 CG INVOKE OK\r\n"));

    var response = await fixture.Client.PostAsJsonAsync(
        "/channels/1/layers/10/cg/invoke",
        new CgInvokeRequest("next()"));

    response.EnsureSuccessStatusCode();
    Assert.Equal(["CG INVOKE 1-10 next()"], fixture.ReceivedCommands);
}
```

- [ ] **Step 2: Run the CG tests to verify they fail**

Run:

```bash
rtk dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter RestApiEndpointTests
rtk dotnet test test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --filter CasparRestApiCgTests
```

Expected:

- FAIL because the new routes and handlers are not mapped yet

- [ ] **Step 3: Implement the CG routes and handlers**

```csharp
group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/add", CasparCommandHandlers.CgAddAsync)
    .WithDisplayName("POST /channels/{channel}/layers/{layer}/cg/add");
group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/play", CasparCommandHandlers.CgPlayAsync)
    .WithDisplayName("POST /channels/{channel}/layers/{layer}/cg/play");
group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/stop", CasparCommandHandlers.CgStopAsync)
    .WithDisplayName("POST /channels/{channel}/layers/{layer}/cg/stop");
group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/next", CasparCommandHandlers.CgNextAsync)
    .WithDisplayName("POST /channels/{channel}/layers/{layer}/cg/next");
group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/remove", CasparCommandHandlers.CgRemoveAsync)
    .WithDisplayName("POST /channels/{channel}/layers/{layer}/cg/remove");
group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/clear", CasparCommandHandlers.CgClearAsync)
    .WithDisplayName("POST /channels/{channel}/layers/{layer}/cg/clear");
group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/update", CasparCommandHandlers.CgUpdateAsync)
    .WithDisplayName("POST /channels/{channel}/layers/{layer}/cg/update");
group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/invoke", CasparCommandHandlers.CgInvokeAsync)
    .WithDisplayName("POST /channels/{channel}/layers/{layer}/cg/invoke");
```

```csharp
public static async Task<IResult> CgAddAsync(
    int channel,
    int layer,
    HttpRequest request,
    ICasparClientResolver clientResolver,
    CancellationToken cancellationToken)
{
    try
    {
        var payload = await CgPayloadTranslator.TranslateAddAsync(request, cancellationToken);
        await clientResolver.ResolveDefaultClient()
            .Channel(channel)
            .Layer(layer)
            .CgAddAsync(payload.Template!, payload.PlayOnLoad, payload.TemplateXml, cancellationToken);
        return Results.Ok();
    }
    catch (BadHttpRequestException exception)
    {
        return Results.Problem(title: "Invalid CG request", detail: exception.Message, statusCode: StatusCodes.Status400BadRequest);
    }
    catch (Exception exception)
    {
        return ToProblemResult(exception);
    }
}

public static Task<IResult> CgPlayAsync(int channel, int layer, ICasparClientResolver clientResolver, CancellationToken cancellationToken) =>
    ExecuteAsync(client => client.Channel(channel).Layer(layer).CgPlayAsync(cancellationToken), clientResolver, cancellationToken);

public static Task<IResult> CgStopAsync(int channel, int layer, ICasparClientResolver clientResolver, CancellationToken cancellationToken) =>
    ExecuteAsync(client => client.Channel(channel).Layer(layer).CgStopAsync(cancellationToken), clientResolver, cancellationToken);

public static Task<IResult> CgNextAsync(int channel, int layer, ICasparClientResolver clientResolver, CancellationToken cancellationToken) =>
    ExecuteAsync(client => client.Channel(channel).Layer(layer).CgNextAsync(cancellationToken), clientResolver, cancellationToken);

public static Task<IResult> CgRemoveAsync(int channel, int layer, ICasparClientResolver clientResolver, CancellationToken cancellationToken) =>
    ExecuteAsync(client => client.Channel(channel).Layer(layer).CgRemoveAsync(cancellationToken), clientResolver, cancellationToken);

public static Task<IResult> CgClearAsync(int channel, int layer, ICasparClientResolver clientResolver, CancellationToken cancellationToken) =>
    ExecuteAsync(client => client.Channel(channel).Layer(layer).CgClearAsync(cancellationToken), clientResolver, cancellationToken);
```

```csharp
public static async Task<IResult> CgUpdateAsync(
    int channel,
    int layer,
    HttpRequest request,
    ICasparClientResolver clientResolver,
    CancellationToken cancellationToken)
{
    try
    {
        var payload = await CgPayloadTranslator.TranslateUpdateAsync(request, cancellationToken);
        await clientResolver.ResolveDefaultClient().Channel(channel).Layer(layer).CgUpdateAsync(payload.TemplateXml, cancellationToken);
        return Results.Ok();
    }
    catch (BadHttpRequestException exception)
    {
        return Results.Problem(title: "Invalid CG request", detail: exception.Message, statusCode: StatusCodes.Status400BadRequest);
    }
    catch (Exception exception)
    {
        return ToProblemResult(exception);
    }
}

public static async Task<IResult> CgInvokeAsync(
    int channel,
    int layer,
    CgInvokeRequest request,
    ICasparClientResolver clientResolver,
    CancellationToken cancellationToken)
{
    try
    {
        await clientResolver.ResolveDefaultClient().Channel(channel).Layer(layer).CgInvokeAsync(request.Method, cancellationToken);
        return Results.Ok();
    }
    catch (Exception exception)
    {
        return ToProblemResult(exception);
    }
}
```

- [ ] **Step 4: Run the CG tests to verify they pass**

Run:

```bash
rtk dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter RestApiEndpointTests
rtk dotnet test test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --filter CasparRestApiCgTests
```

Expected:

- PASS for the endpoint route assertions
- PASS for the new CG integration tests

- [ ] **Step 5: Commit the CG route slice**

```bash
git add src/StarDust.CasparCG.AspNetCore/EndpointRouteBuilderExtensions.cs \
  src/StarDust.CasparCG.AspNetCore/Internal/CasparCommandHandlers.cs \
  test/StarDust.CasparCG.UnitTests/RestApiEndpointTests.cs \
  test/StarDust.CasparCG.IntegrationTests/CasparRestApiCgTests.cs
git commit -m "feat: add cg rest routes"
```

### Task 3: Add failing thumbnail parser tests and implement thumbnail query support

**Files:**
- Create: `src/StarDust.CasparCG.AspNetCore/Contracts/ThumbnailListItemResponse.cs`
- Create: `src/StarDust.CasparCG.AspNetCore/Internal/ThumbnailResponseParser.cs`
- Create: `test/StarDust.CasparCG.UnitTests/RestApiThumbnailResponseParserTests.cs`
- Modify: `src/StarDust.CasparCG.AspNetCore/Internal/CasparQueryHandlers.cs`

- [ ] **Step 1: Write the failing thumbnail parser tests**

```csharp
using StarDust.CasparCG.AspNetCore.Internal;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public sealed class RestApiThumbnailResponseParserTests
{
    [Fact]
    public void Parse_list_returns_structured_items()
    {
        const string response = """
        200 THUMBNAIL LIST OK
        "AMB" 42 20240101T120000
        "BKG" 84 20240101T121500

        """;

        var items = ThumbnailResponseParser.ParseList(response);

        Assert.Collection(
            items,
            item =>
            {
                Assert.Equal("AMB", item.Name);
                Assert.Equal(42, item.SizeBytes);
            },
            item =>
            {
                Assert.Equal("BKG", item.Name);
                Assert.Equal(84, item.SizeBytes);
            });
    }

    [Fact]
    public void Parse_retrieve_decodes_base64_payload()
    {
        var bytes = ThumbnailResponseParser.ParseBinary(
            "201 THUMBNAIL RETRIEVE OK\r\nSGVsbG8=\r\n");

        Assert.Equal("Hello", System.Text.Encoding.UTF8.GetString(bytes));
    }
}
```

- [ ] **Step 2: Run the thumbnail parser tests to verify they fail**

Run:

```bash
rtk dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter RestApiThumbnailResponseParserTests
```

Expected:

- FAIL because `ThumbnailResponseParser` and `ThumbnailListItemResponse` do not exist yet

- [ ] **Step 3: Implement the parser and thumbnail query handlers**

```csharp
namespace StarDust.CasparCG.AspNetCore.Contracts;

public sealed record ThumbnailListItemResponse(
    string Name,
    long? SizeBytes,
    DateTimeOffset? LastModified);
```

```csharp
using System.Globalization;
using System.Text;
using StarDust.CasparCG.AspNetCore.Contracts;

namespace StarDust.CasparCG.AspNetCore.Internal;

internal static class ThumbnailResponseParser
{
    public static IReadOnlyList<ThumbnailListItemResponse> ParseList(string responseText)
    {
        var lines = responseText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);
        return lines
            .Skip(1)
            .Select(ParseListLine)
            .ToArray();
    }

    public static byte[] ParseBinary(string responseText)
    {
        var payload = responseText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).Last();
        return Convert.FromBase64String(payload);
    }

    private static ThumbnailListItemResponse ParseListLine(string line)
    {
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var name = parts[0].Trim('"');
        var sizeBytes = long.TryParse(parts.ElementAtOrDefault(1), out var parsedSize) ? parsedSize : null;
        var lastModified = DateTimeOffset.TryParseExact(
            parts.ElementAtOrDefault(2),
            "yyyyMMdd'T'HHmmss",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal,
            out var parsedDate)
            ? parsedDate
            : null;

        return new(name, sizeBytes, lastModified);
    }
}
```

```csharp
public static async Task<IResult> GetThumbnailsAsync(
    ICasparClientResolver clientResolver,
    CancellationToken cancellationToken)
{
    try
    {
        var response = await clientResolver.ResolveDefaultClient().Thumbnails().ListAsync(null, cancellationToken);
        return Results.Ok(ThumbnailResponseParser.ParseList(response.Body));
    }
    catch (Exception exception)
    {
        return ToProblemResult(exception);
    }
}

public static async Task<IResult> GetThumbnailAsync(
    string fileName,
    ICasparClientResolver clientResolver,
    CancellationToken cancellationToken)
{
    try
    {
        var response = await clientResolver.ResolveDefaultClient().Thumbnails().RetrieveAsync(fileName, cancellationToken);
        var bytes = ThumbnailResponseParser.ParseBinary(response.Body);
        return Results.File(bytes, "application/octet-stream");
    }
    catch (Exception exception)
    {
        return ToProblemResult(exception);
    }
}
```

- [ ] **Step 4: Run the thumbnail parser tests to verify they pass**

Run:

```bash
rtk dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter RestApiThumbnailResponseParserTests
```

Expected:

- PASS for list parsing and binary decode

- [ ] **Step 5: Commit the thumbnail parser slice**

```bash
git add src/StarDust.CasparCG.AspNetCore/Contracts/ThumbnailListItemResponse.cs \
  src/StarDust.CasparCG.AspNetCore/Internal/ThumbnailResponseParser.cs \
  src/StarDust.CasparCG.AspNetCore/Internal/CasparQueryHandlers.cs \
  test/StarDust.CasparCG.UnitTests/RestApiThumbnailResponseParserTests.cs
git commit -m "feat: add thumbnail rest parsing"
```

### Task 4: Add failing thumbnail route tests and implement thumbnail endpoints

**Files:**
- Modify: `src/StarDust.CasparCG.AspNetCore/EndpointRouteBuilderExtensions.cs`
- Modify: `src/StarDust.CasparCG.AspNetCore/Internal/CasparCommandHandlers.cs`
- Modify: `src/StarDust.CasparCG.AspNetCore/Internal/CasparQueryHandlers.cs`
- Modify: `test/StarDust.CasparCG.UnitTests/RestApiEndpointTests.cs`
- Create: `test/StarDust.CasparCG.IntegrationTests/CasparRestApiThumbnailTests.cs`

- [ ] **Step 1: Add the failing route tests**

```csharp
[Fact]
public void MapCasparCGApi_includes_thumbnail_routes()
{
    var patterns = RestApiEndpointTestHost.GetRoutePatterns();

    Assert.Contains("/thumbnails", patterns);
    Assert.Contains("/thumbnails/{fileName}", patterns);
    Assert.Contains("/thumbnails/{fileName}/generate", patterns);
    Assert.Contains("/thumbnails/generate-all", patterns);
}
```

```csharp
[Fact]
public async Task Get_thumbnails_returns_structured_json()
{
    await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
        .WithAmcpReply("THUMBNAIL LIST", "200 THUMBNAIL LIST OK\r\n\"AMB\" 42 20240101T120000\r\n"));

    var response = await fixture.Client.GetAsync("/thumbnails");

    response.EnsureSuccessStatusCode();
    var payload = await response.Content.ReadAsStringAsync();
    Assert.Contains("\"name\":\"AMB\"", payload);
}

[Fact]
public async Task Get_thumbnail_returns_binary_payload()
{
    await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
        .WithAmcpReply("THUMBNAIL RETRIEVE AMB", "201 THUMBNAIL RETRIEVE OK\r\nSGVsbG8=\r\n"));

    var response = await fixture.Client.GetAsync("/thumbnails/AMB");

    response.EnsureSuccessStatusCode();
    Assert.Equal("application/octet-stream", response.Content.Headers.ContentType?.MediaType);
    Assert.Equal("Hello", Encoding.UTF8.GetString(await response.Content.ReadAsByteArrayAsync()));
}

[Fact]
public async Task Post_generate_routes_send_expected_commands()
{
    await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
        .WithAmcpReply("THUMBNAIL GENERATE AMB", "202 THUMBNAIL GENERATE OK\r\n")
        .WithAmcpReply("THUMBNAIL GENERATE_ALL", "202 THUMBNAIL GENERATE_ALL OK\r\n"));

    var generateResponse = await fixture.Client.PostAsync("/thumbnails/AMB/generate", content: null);
    var generateAllResponse = await fixture.Client.PostAsync("/thumbnails/generate-all", content: null);

    generateResponse.EnsureSuccessStatusCode();
    generateAllResponse.EnsureSuccessStatusCode();
    Assert.Equal(
        ["THUMBNAIL GENERATE AMB", "THUMBNAIL GENERATE_ALL"],
        fixture.ReceivedCommands);
}
```

- [ ] **Step 2: Run the thumbnail route tests to verify they fail**

Run:

```bash
rtk dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter RestApiEndpointTests
rtk dotnet test test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --filter CasparRestApiThumbnailTests
```

Expected:

- FAIL because the thumbnail routes are not mapped yet

- [ ] **Step 3: Implement the thumbnail route mappings and generation handlers**

```csharp
group.MapGet("/thumbnails", CasparQueryHandlers.GetThumbnailsAsync)
    .WithDisplayName("GET /thumbnails");
group.MapGet("/thumbnails/{fileName}", CasparQueryHandlers.GetThumbnailAsync)
    .WithDisplayName("GET /thumbnails/{fileName}");
group.MapPost("/thumbnails/{fileName}/generate", CasparCommandHandlers.GenerateThumbnailAsync)
    .WithDisplayName("POST /thumbnails/{fileName}/generate");
group.MapPost("/thumbnails/generate-all", CasparCommandHandlers.GenerateAllThumbnailsAsync)
    .WithDisplayName("POST /thumbnails/generate-all");
```

```csharp
public static Task<IResult> GenerateThumbnailAsync(
    string fileName,
    ICasparClientResolver clientResolver,
    CancellationToken cancellationToken) =>
    ExecuteAsync(
        client => client.Thumbnails().GenerateAsync(fileName, cancellationToken),
        clientResolver,
        cancellationToken);

public static Task<IResult> GenerateAllThumbnailsAsync(
    ICasparClientResolver clientResolver,
    CancellationToken cancellationToken) =>
    ExecuteAsync(
        client => client.Thumbnails().GenerateAllAsync(cancellationToken),
        clientResolver,
        cancellationToken);
```

- [ ] **Step 4: Run the thumbnail route tests to verify they pass**

Run:

```bash
rtk dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter RestApiEndpointTests
rtk dotnet test test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --filter CasparRestApiThumbnailTests
```

Expected:

- PASS for the new thumbnail route assertions
- PASS for the thumbnail integration tests

- [ ] **Step 5: Commit the thumbnail route slice**

```bash
git add src/StarDust.CasparCG.AspNetCore/EndpointRouteBuilderExtensions.cs \
  src/StarDust.CasparCG.AspNetCore/Internal/CasparCommandHandlers.cs \
  src/StarDust.CasparCG.AspNetCore/Internal/CasparQueryHandlers.cs \
  test/StarDust.CasparCG.UnitTests/RestApiEndpointTests.cs \
  test/StarDust.CasparCG.IntegrationTests/CasparRestApiThumbnailTests.cs
git commit -m "feat: add thumbnail rest routes"
```

### Task 5: Update route documentation and verify the full slice

**Files:**
- Modify: `README.md`
- Modify: `docs/vnext/hosting-and-di.md`
- Modify: `docs/vnext/rest-api-addon.md`

- [ ] **Step 1: Update the REST documentation**

```md
| `POST` | `/channels/{channel}/layers/{layer}/cg/add` | Send `CG ADD` | JSON or XML body |
| `POST` | `/channels/{channel}/layers/{layer}/cg/play` | Send `CG PLAY` | none |
| `POST` | `/channels/{channel}/layers/{layer}/cg/stop` | Send `CG STOP` | none |
| `POST` | `/channels/{channel}/layers/{layer}/cg/next` | Send `CG NEXT` | none |
| `POST` | `/channels/{channel}/layers/{layer}/cg/remove` | Send `CG REMOVE` | none |
| `POST` | `/channels/{channel}/layers/{layer}/cg/clear` | Send `CG CLEAR` | none |
| `POST` | `/channels/{channel}/layers/{layer}/cg/update` | Send `CG UPDATE` | JSON or XML body |
| `POST` | `/channels/{channel}/layers/{layer}/cg/invoke` | Send `CG INVOKE` | `{"method":"next()"}` |
| `GET` | `/thumbnails` | List thumbnails returned by `THUMBNAIL LIST` | none |
| `GET` | `/thumbnails/{fileName}` | Retrieve a thumbnail image | none |
| `POST` | `/thumbnails/{fileName}/generate` | Send `THUMBNAIL GENERATE` | none |
| `POST` | `/thumbnails/generate-all` | Send `THUMBNAIL GENERATE_ALL` | none |
```

```md
## CG content types

`POST /channels/{channel}/layers/{layer}/cg/add` and `POST /channels/{channel}/layers/{layer}/cg/update` support:

- `Content-Type: application/json`
- `Content-Type: application/xml`
- `Content-Type: text/xml`

JSON requests use the REST addon envelope and are translated to template XML before calling CasparCG.
XML requests pass raw template XML through directly.
```

```md
## Coverage note

The REST addon now covers playback, data, selected mixer commands, CG templates, thumbnails, media file listing, admin restart, and SSE.
It still does not expose the full AMCP surface of `StarDust.CasparCG`.
```

- [ ] **Step 2: Run focused tests for the full slice**

Run:

```bash
rtk dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter "RestApi"
rtk dotnet test test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --filter "CasparRestApi"
```

Expected:

- PASS for all REST addon unit tests
- PASS for all REST addon integration tests

- [ ] **Step 3: Run the repo-level verification**

Run:

```bash
rtk dotnet build src/StarDust.CasparCG.net.sln
rtk dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj
rtk dotnet test test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj
```

Expected:

- solution build passes
- unit tests pass
- integration tests pass

- [ ] **Step 4: Commit the docs and verification slice**

```bash
git add README.md docs/vnext/hosting-and-di.md docs/vnext/rest-api-addon.md
git commit -m "docs: expand rest api cg and thumbnail routes"
```

---

## Self-Review

### Spec coverage

- CG route additions: covered by Task 2
- Thumbnail route additions: covered by Tasks 3 and 4
- JSON/XML `Content-Type` contract: covered by Task 1 and documented in Task 5
- Structured thumbnail list response: covered by Task 3
- Binary thumbnail retrieval: covered by Tasks 3 and 4
- Testing and docs updates: covered by Tasks 1 through 5

### Placeholder scan

- No `TODO`, `TBD`, or deferred “implement later” phrasing remains
- Each code-changing step includes concrete code snippets
- Each validation step includes exact commands and expected outcomes

### Type consistency

- `CgAddJsonRequest`, `CgUpdateJsonRequest`, `CgInvokeRequest`, `TemplateDataEnvelope`, `CgPayloadTranslator`, `ThumbnailListItemResponse`, and `ThumbnailResponseParser` are introduced before later tasks depend on them
- Route names and handler names are consistent between test steps and implementation steps

