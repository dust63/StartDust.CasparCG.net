# AMCP Problem Details Mapping Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Map AMCP error codes into REST `ProblemDetails` with the right HTTP status class and expose AMCP diagnostics in `ProblemDetails.extensions`.

**Architecture:** Keep `CasparProblemDetailsFactory` as the single translation layer from AMCP exceptions to REST error payloads. The factory will map the AMCP status code to an HTTP status, preserve the AMCP status line and command text, and return a `ProblemDetails` object that the REST handlers serialize through a shared minimal-API helper. Success responses stay unchanged.

**Tech Stack:** .NET 10, ASP.NET Core minimal APIs, `ProblemDetails`, xUnit, existing `StarDust.CasparCG` / `StarDust.CasparCG.AspNetCore` projects.

---

### Task 1: Add AMCP-to-HTTP mapping and diagnostic extensions to `ProblemDetails`

**Files:**
- Modify: `src/StarDust.CasparCG.AspNetCore/Internal/CasparProblemDetailsFactory.cs`
- Test: `test/StarDust.CasparCG.UnitTests/RestApiProblemDetailsTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
[Theory]
[InlineData(400, StatusCodes.Status400BadRequest)]
[InlineData(401, StatusCodes.Status400BadRequest)]
[InlineData(402, StatusCodes.Status400BadRequest)]
[InlineData(403, StatusCodes.Status400BadRequest)]
[InlineData(404, StatusCodes.Status404NotFound)]
[InlineData(500, StatusCodes.Status502BadGateway)]
[InlineData(501, StatusCodes.Status502BadGateway)]
[InlineData(502, StatusCodes.Status502BadGateway)]
[InlineData(503, StatusCodes.Status403Forbidden)]
[InlineData(504, StatusCodes.Status429TooManyRequests)]
[InlineData(600, StatusCodes.Status501NotImplemented)]
public void Amcp_command_failures_map_to_http_status_and_extensions(int amcpStatusCode, int expectedHttpStatus)
{
    var response = new AmcpResponse
    {
        StatusCode = amcpStatusCode,
        Category = amcpStatusCode >= 500 ? AmcpStatusCategory.ServerError : AmcpStatusCategory.ClientError,
        CommandText = "PLAY",
        StatusLine = $"{amcpStatusCode} PLAY FAILED",
        Lines = [],
        Raw = $"{amcpStatusCode} PLAY FAILED\r\n"
    };

    var problem = CasparProblemDetailsFactory.FromException(new AmcpCommandException(response));

    Assert.Equal(expectedHttpStatus, problem.Status);
    Assert.Equal(amcpStatusCode, problem.Extensions["amcpStatusCode"]);
    Assert.Equal($"{amcpStatusCode} PLAY FAILED", problem.Extensions["amcpStatusLine"]);
    Assert.Equal("PLAY", problem.Extensions["amcpCommandText"]);
    Assert.Equal(response.Category.ToString(), problem.Extensions["amcpCategory"]);
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter Amcp_command_failures_map_to_http_status_and_extensions -v normal`
Expected: fail because the factory does not yet map all AMCP codes and does not yet populate diagnostic extensions.

- [ ] **Step 3: Write minimal implementation**

```csharp
public static ProblemDetails FromException(Exception exception) =>
    exception switch
    {
        AmcpCommandException amcpException => FromAmcpResponse(amcpException.Response, amcpException.Message),
        IOException ioException => new ProblemDetails
        {
            Title = "CasparCG upstream failure",
            Status = StatusCodes.Status502BadGateway,
            Detail = ioException.Message
        },
        InvalidOperationException invalidOperationException => new ProblemDetails
        {
            Title = "CasparCG client is unavailable",
            Status = StatusCodes.Status503ServiceUnavailable,
            Detail = invalidOperationException.Message
        },
        _ => new ProblemDetails
        {
            Title = "CasparCG request failed",
            Status = StatusCodes.Status503ServiceUnavailable,
            Detail = exception.Message
        }
    };

private static ProblemDetails FromAmcpResponse(AmcpResponse response, string detail) =>
    new()
    {
        Title = "CasparCG upstream failure",
        Status = MapHttpStatus(response.StatusCode),
        Detail = detail,
        Extensions =
        {
            ["amcpStatusCode"] = response.StatusCode,
            ["amcpStatusLine"] = response.StatusLine,
            ["amcpCommandText"] = response.CommandText,
            ["amcpCategory"] = response.Category.ToString()
        }
    };
```

- [ ] **Step 4: Run the test to verify it passes**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter Amcp_command_failures_map_to_http_status_and_extensions -v normal`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add src/StarDust.CasparCG.AspNetCore/Internal/CasparProblemDetailsFactory.cs test/StarDust.CasparCG.UnitTests/RestApiProblemDetailsTests.cs
git commit -m "Map AMCP errors into richer problem details"
```

### Task 2: Serialize the enriched `ProblemDetails` from REST handlers

**Files:**
- Modify: `src/StarDust.CasparCG.AspNetCore/Internal/CasparCommandHandlers.cs`
- Modify: `src/StarDust.CasparCG.AspNetCore/Internal/CasparQueryHandlers.cs`
- Create: `test/StarDust.CasparCG.IntegrationTests/CasparRestApiProblemDetailsTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
[Fact]
public async Task Rest_problem_details_include_amcp_metadata_in_json()
{
    await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
        .WithAmcpReply("PLAY 1-10 FAIL", "404 PLAY FAILED\r\n"));

    var response = await fixture.Client.PostAsJsonAsync(
        "/channels/1/layers/10/play",
        new PlayRequest("FAIL"));

    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

    var payload = await response.Content.ReadFromJsonAsync<ProblemDetails>();
    Assert.NotNull(payload);
    Assert.Equal(404, payload!.Status);
    Assert.Equal(404, payload.Extensions["amcpStatusCode"]);
    Assert.Equal("404 PLAY FAILED", payload.Extensions["amcpStatusLine"]);
    Assert.Equal("PLAY", payload.Extensions["amcpCommandText"]);
    Assert.Equal("ClientError", payload.Extensions["amcpCategory"]);
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --filter Rest_problem_details_include_amcp_metadata_in_json -v normal`
Expected: fail because the current `Results.Problem(...)` calls do not carry AMCP extensions into the JSON payload.

- [ ] **Step 3: Write minimal implementation**

```csharp
private static IResult ToProblemResult(Exception exception)
{
    var problem = CasparProblemDetailsFactory.FromException(exception);

    return TypedResults.Problem(
        title: problem.Title,
        detail: problem.Detail,
        statusCode: problem.Status,
        type: problem.Type,
        instance: problem.Instance,
        extensions: problem.Extensions);
}
```

Apply the same helper shape in both `CasparCommandHandlers.cs` and `CasparQueryHandlers.cs` so every REST failure path emits the same enriched payload.

- [ ] **Step 4: Run the test to verify it passes**

Run: `dotnet test test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --filter Rest_problem_details_include_amcp_metadata_in_json -v normal`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add src/StarDust.CasparCG.AspNetCore/Internal/CasparCommandHandlers.cs src/StarDust.CasparCG.AspNetCore/Internal/CasparQueryHandlers.cs test/StarDust.CasparCG.IntegrationTests/CasparRestApiProblemDetailsTests.cs
git commit -m "Emit AMCP metadata in REST problem details"
```

### Task 3: Document the AMCP-to-REST error contract

**Files:**
- Modify: `docs/vnext/rest-api-addon.md`
- Modify: `docs/vnext/hosting-and-di.md` if needed for cross-reference

- [ ] **Step 1: Write the failing documentation expectation**

Update the REST addon docs with a short section that explains:

```md
AMCP failures are projected into REST ProblemDetails.

- HTTP status follows the failure class.
- `ProblemDetails.extensions` includes `amcpStatusCode`, `amcpStatusLine`, `amcpCommandText`, and `amcpCategory`.
- transport failures remain `502`/`503` depending on whether the client reached the server.
```

- [ ] **Step 2: Run the docs check by inspection**

Run: `grep -n "amcpStatusCode\|amcpStatusLine\|amcpCommandText\|amcpCategory" docs/vnext/rest-api-addon.md`
Expected: the new error section mentions the four extension keys exactly once and uses the same names as the implementation.

- [ ] **Step 3: Write minimal implementation**

Add a concise error-handling section to `docs/vnext/rest-api-addon.md` and, if helpful, a short pointer from `docs/vnext/hosting-and-di.md`.

- [ ] **Step 4: Run the full test suite to ensure no code changes regressed**

Run: `dotnet test src/StarDust.CasparCG.net.sln -v minimal`
Expected: all unit and integration tests pass.

- [ ] **Step 5: Commit**

```bash
git add docs/vnext/rest-api-addon.md docs/vnext/hosting-and-di.md
git commit -m "Document AMCP problem details mapping"
```
