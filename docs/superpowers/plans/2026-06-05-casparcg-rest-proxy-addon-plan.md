# CasparCG REST Proxy Addon Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a reusable ASP.NET Core addon that exposes resource-oriented HTTP and SSE endpoints over `CasparClient`, plus a thin sample proxy host built on the same package.

**Architecture:** Introduce a new `StarDust.CasparCG.AspNetCore` package above the existing client and hosting projects. Build the feature in thin layers: package scaffold and registration first, then query and command endpoints, then SSE and the sample proxy, all driven by focused unit and integration tests.

**Tech Stack:** .NET 10, ASP.NET Core Minimal APIs, xUnit, existing `DummyServer` integration helpers, GitHub-hosted solution structure

---

## File Map

- Create: `src/StarDust.CasparCG.AspNetCore/StarDust.CasparCG.AspNetCore.csproj`
- Create: `src/StarDust.CasparCG.AspNetCore/ServiceCollectionExtensions.cs`
- Create: `src/StarDust.CasparCG.AspNetCore/EndpointRouteBuilderExtensions.cs`
- Create: `src/StarDust.CasparCG.AspNetCore/CasparRestApiOptions.cs`
- Create: `src/StarDust.CasparCG.AspNetCore/Internal/DefaultCasparClientResolver.cs`
- Create: `src/StarDust.CasparCG.AspNetCore/Internal/ProblemDetailsFactory.cs`
- Create: `src/StarDust.CasparCG.AspNetCore/Contracts/...`
- Create: `src/StarDust.CasparCG.AspNetCore/Sse/...`
- Modify: `src/StarDust.CasparCG.net.sln`
- Create: `test/StarDust.CasparCG.UnitTests/RestApiRegistrationTests.cs`
- Create: `test/StarDust.CasparCG.UnitTests/RestApiEndpointTests.cs`
- Create: `test/StarDust.CasparCG.UnitTests/RestApiProblemDetailsTests.cs`
- Create: `test/StarDust.CasparCG.IntegrationTests/CasparRestApiQueryTests.cs`
- Create: `test/StarDust.CasparCG.IntegrationTests/CasparRestApiCommandTests.cs`
- Create: `test/StarDust.CasparCG.IntegrationTests/CasparRestApiSseTests.cs`
- Create: `demo/StarDust.CasparCG.RestProxy.Sample/StarDust.CasparCG.RestProxy.Sample.csproj`
- Create: `demo/StarDust.CasparCG.RestProxy.Sample/Program.cs`
- Modify: `README.md`
- Modify: `docs/vnext/hosting-and-di.md`

### Task 1: Scaffold the ASP.NET Core addon package

**Files:**
- Create: `src/StarDust.CasparCG.AspNetCore/StarDust.CasparCG.AspNetCore.csproj`
- Create: `src/StarDust.CasparCG.AspNetCore/CasparRestApiOptions.cs`
- Create: `src/StarDust.CasparCG.AspNetCore/ServiceCollectionExtensions.cs`
- Modify: `src/StarDust.CasparCG.net.sln`
- Test: `test/StarDust.CasparCG.UnitTests/RestApiRegistrationTests.cs`
- Modify: `test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj`

- [ ] **Step 1: Write the failing registration tests**

```csharp
namespace StarDust.CasparCG.UnitTests;

public sealed class RestApiRegistrationTests
{
    [Fact]
    public void AddCasparCGRestApi_registers_options_and_client_resolver()
    {
        var services = new ServiceCollection();
        services.AddCasparCG().ConnectTo("127.0.0.1", 5250);

        services.AddCasparCGRestApi();

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IOptions<CasparRestApiOptions>>();
        provider.GetRequiredService<ICasparClientResolver>();
    }
}
```

- [ ] **Step 2: Add the unit test project reference to the new package and run the test to verify it fails**

```xml
<ProjectReference Include="..\..\src\StarDust.CasparCG.AspNetCore\StarDust.CasparCG.AspNetCore.csproj" />
```

Run: `rtk dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter RestApiRegistrationTests`

Expected: FAIL because the new package or types do not exist yet.

- [ ] **Step 3: Create the minimal addon project and registration surface**

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <LangVersion>preview</LangVersion>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <Deterministic>true</Deterministic>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
  </PropertyGroup>
  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\StarDust.CasparCG\StarDust.CasparCG.csproj" />
    <ProjectReference Include="..\StarDust.CasparCG.Hosting\StarDust.CasparCG.Hosting.csproj" />
  </ItemGroup>
</Project>
```

```csharp
namespace StarDust.CasparCG.AspNetCore;

public sealed class CasparRestApiOptions
{
    public string RoutePrefix { get; set; } = string.Empty;
    public bool MapAdminEndpoints { get; set; }
    public bool EnableSse { get; set; } = true;
}
```

```csharp
namespace StarDust.CasparCG.AspNetCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCasparCGRestApi(
        this IServiceCollection services,
        Action<CasparRestApiOptions>? configure = null)
    {
        services.AddOptions<CasparRestApiOptions>();
        if (configure is not null)
        {
            services.Configure(configure);
        }

        services.TryAddSingleton<ICasparClientResolver, DefaultCasparClientResolver>();
        return services;
    }
}
```

- [ ] **Step 4: Add the project to the solution**

```text
Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "StarDust.CasparCG.AspNetCore", "StarDust.CasparCG.AspNetCore\StarDust.CasparCG.AspNetCore.csproj", "{NEW-GUID-HERE}"
EndProject
```

Run: `rtk dotnet sln src/StarDust.CasparCG.net.sln add src/StarDust.CasparCG.AspNetCore/StarDust.CasparCG.AspNetCore.csproj`

Expected: the solution now includes the new addon project.

- [ ] **Step 5: Run the registration test to verify it passes**

Run: `rtk dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter RestApiRegistrationTests`

Expected: PASS

- [ ] **Step 6: Commit**

```bash
git add src/StarDust.CasparCG.AspNetCore src/StarDust.CasparCG.net.sln test/StarDust.CasparCG.UnitTests
git commit -m "feat: scaffold aspnetcore rest addon"
```

### Task 2: Add endpoint mapping, route grouping, and query endpoints

**Files:**
- Create: `src/StarDust.CasparCG.AspNetCore/EndpointRouteBuilderExtensions.cs`
- Create: `src/StarDust.CasparCG.AspNetCore/Contracts/ServerVersionResponse.cs`
- Create: `src/StarDust.CasparCG.AspNetCore/Contracts/MediaFileResponse.cs`
- Create: `src/StarDust.CasparCG.AspNetCore/Internal/CasparQueryHandlers.cs`
- Test: `test/StarDust.CasparCG.UnitTests/RestApiEndpointTests.cs`
- Test: `test/StarDust.CasparCG.IntegrationTests/CasparRestApiQueryTests.cs`
- Modify: `test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj`

- [ ] **Step 1: Write the failing route mapping unit test**

```csharp
namespace StarDust.CasparCG.UnitTests;

public sealed class RestApiEndpointTests
{
    [Fact]
    public async Task MapCasparCGApi_maps_server_and_data_routes()
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.Services.AddCasparCG().ConnectTo("127.0.0.1", 5250);
        builder.Services.AddCasparCGRestApi();

        var app = builder.Build();
        app.MapCasparCGApi();

        var endpoints = app.Services.GetRequiredService<EndpointDataSource>().Endpoints;
        endpoints.ShouldContain(e => e.DisplayName!.Contains("GET /server/version"));
        endpoints.ShouldContain(e => e.DisplayName!.Contains("GET /data/{key}"));
    }
}
```

- [ ] **Step 2: Write the failing integration query test**

```csharp
namespace StarDust.CasparCG.IntegrationTests;

public sealed class CasparRestApiQueryTests
{
    [Fact]
    public async Task Get_server_version_returns_typed_payload()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync("VERSION SERVER 2.4.0");

        var response = await fixture.Client.GetAsync("/server/version");

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<ServerVersionResponse>();
        Assert.Equal("2.4.0", payload!.Version);
    }
}
```

- [ ] **Step 3: Run the focused tests to verify they fail**

Run: `rtk dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter RestApiEndpointTests`

Run: `rtk dotnet test test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --filter CasparRestApiQueryTests`

Expected: FAIL because `MapCasparCGApi` and the test host/query endpoints do not exist yet.

- [ ] **Step 4: Implement minimal endpoint mapping and query handlers**

```csharp
namespace StarDust.CasparCG.AspNetCore;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapCasparCGApi(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup(string.Empty);

        group.MapGet("/server/version", CasparQueryHandlers.GetServerVersionAsync)
            .WithDisplayName("GET /server/version");
        group.MapGet("/data/{key}", CasparQueryHandlers.GetDataAsync)
            .WithDisplayName("GET /data/{key}");
        group.MapGet("/media/files", CasparQueryHandlers.GetMediaFilesAsync)
            .WithDisplayName("GET /media/files");

        return endpoints;
    }
}
```

```csharp
namespace StarDust.CasparCG.AspNetCore.Contracts;

public sealed record ServerVersionResponse(string Version);
```

- [ ] **Step 5: Add an integration test host helper and run the tests to verify they pass**

```csharp
internal static class CasparRestApiTestHost
{
    public static async Task<(HttpClient Client, IAsyncDisposable Scope)> StartAsync(string amcpResponse)
    {
        // create DummyServer-backed app host and return HttpClient
    }
}
```

Run: `rtk dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter RestApiEndpointTests`

Run: `rtk dotnet test test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --filter CasparRestApiQueryTests`

Expected: PASS

- [ ] **Step 6: Commit**

```bash
git add src/StarDust.CasparCG.AspNetCore test/StarDust.CasparCG.UnitTests test/StarDust.CasparCG.IntegrationTests
git commit -m "feat: add rest api query endpoints"
```

### Task 3: Add command DTOs and command endpoints for playback, mixer, CG, data, and admin

**Files:**
- Create: `src/StarDust.CasparCG.AspNetCore/Contracts/PlayRequest.cs`
- Create: `src/StarDust.CasparCG.AspNetCore/Contracts/LoadBackgroundRequest.cs`
- Create: `src/StarDust.CasparCG.AspNetCore/Contracts/MixerOpacityRequest.cs`
- Create: `src/StarDust.CasparCG.AspNetCore/Internal/CasparCommandHandlers.cs`
- Test: `test/StarDust.CasparCG.IntegrationTests/CasparRestApiCommandTests.cs`
- Test: `test/StarDust.CasparCG.UnitTests/RestApiProblemDetailsTests.cs`

- [ ] **Step 1: Write the failing command integration tests**

```csharp
namespace StarDust.CasparCG.IntegrationTests;

public sealed class CasparRestApiCommandTests
{
    [Fact]
    public async Task Post_play_sends_expected_command()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync("202 PLAY OK");

        var response = await fixture.Client.PostAsJsonAsync(
            "/channels/1/layers/10/play",
            new PlayRequest("AMB", false, null));

        response.EnsureSuccessStatusCode();
        Assert.Equal("PLAY 1-10 AMB", fixture.DummyServer.LastCommand);
    }
}
```

- [ ] **Step 2: Write the failing error translation unit test**

```csharp
namespace StarDust.CasparCG.UnitTests;

public sealed class RestApiProblemDetailsTests
{
    [Fact]
    public void Caspar_transport_errors_map_to_bad_gateway()
    {
        var problem = CasparProblemDetailsFactory.FromException(new IOException("boom"));
        Assert.Equal(StatusCodes.Status502BadGateway, problem.Status);
    }
}
```

- [ ] **Step 3: Run the focused tests to verify they fail**

Run: `rtk dotnet test test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --filter CasparRestApiCommandTests`

Run: `rtk dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter RestApiProblemDetailsTests`

Expected: FAIL because request DTOs, command handlers, and problem translation do not exist yet.

- [ ] **Step 4: Implement the command request contracts and handlers**

```csharp
namespace StarDust.CasparCG.AspNetCore.Contracts;

public sealed record PlayRequest(
    string Clip,
    bool Loop,
    int? Seek);
```

```csharp
group.MapPost("/channels/{channel:int}/layers/{layer:int}/play", CasparCommandHandlers.PlayAsync);
group.MapPost("/channels/{channel:int}/layers/{layer:int}/loadbg", CasparCommandHandlers.LoadBackgroundAsync);
group.MapPost("/channels/{channel:int}/layers/{layer:int}/pause", CasparCommandHandlers.PauseAsync);
group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/opacity", CasparCommandHandlers.SetOpacityAsync);
group.MapPut("/data/{key}", CasparCommandHandlers.PutDataAsync);
group.MapPost("/admin/restart", CasparCommandHandlers.RestartAsync);
```

- [ ] **Step 5: Implement uniform `ProblemDetails` mapping and rerun the tests**

```csharp
internal static class CasparProblemDetailsFactory
{
    public static ProblemDetails FromException(Exception ex) =>
        ex switch
        {
            IOException io => new ProblemDetails
            {
                Title = "CasparCG upstream failure",
                Status = StatusCodes.Status502BadGateway,
                Detail = io.Message
            },
            _ => new ProblemDetails
            {
                Title = "CasparCG request failed",
                Status = StatusCodes.Status503ServiceUnavailable,
                Detail = ex.Message
            }
        };
}
```

Run: `rtk dotnet test test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --filter CasparRestApiCommandTests`

Run: `rtk dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter RestApiProblemDetailsTests`

Expected: PASS

- [ ] **Step 6: Commit**

```bash
git add src/StarDust.CasparCG.AspNetCore test/StarDust.CasparCG.UnitTests test/StarDust.CasparCG.IntegrationTests
git commit -m "feat: add rest api command endpoints"
```

### Task 4: Add SSE event streaming and route options

**Files:**
- Create: `src/StarDust.CasparCG.AspNetCore/Sse/CasparSseEvent.cs`
- Create: `src/StarDust.CasparCG.AspNetCore/Sse/CasparSseWriter.cs`
- Create: `src/StarDust.CasparCG.AspNetCore/Sse/CasparEventStreamBridge.cs`
- Modify: `src/StarDust.CasparCG.AspNetCore/EndpointRouteBuilderExtensions.cs`
- Test: `test/StarDust.CasparCG.IntegrationTests/CasparRestApiSseTests.cs`

- [ ] **Step 1: Write the failing SSE integration test**

```csharp
namespace StarDust.CasparCG.IntegrationTests;

public sealed class CasparRestApiSseTests
{
    [Fact]
    public async Task Get_events_streams_named_sse_messages()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync();
        await fixture.PublishEventAsync(new PlaybackClipChangedEvent(1, 10, "AMB"));

        using var response = await fixture.Client.GetAsync("/events", HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        Assert.Equal("text/event-stream", response.Content.Headers.ContentType!.MediaType);
    }
}
```

- [ ] **Step 2: Run the SSE test to verify it fails**

Run: `rtk dotnet test test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --filter CasparRestApiSseTests`

Expected: FAIL because `/events` and the SSE bridge do not exist yet.

- [ ] **Step 3: Implement the SSE envelope and endpoint**

```csharp
namespace StarDust.CasparCG.AspNetCore.Sse;

public sealed record CasparSseEvent(
    string Type,
    DateTimeOffset Timestamp,
    string Target,
    object Payload);
```

```csharp
if (options.EnableSse)
{
    group.MapGet("/events", CasparSseWriter.StreamAsync)
        .WithDisplayName("GET /events");
}
```

- [ ] **Step 4: Run the SSE test to verify it passes**

Run: `rtk dotnet test test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --filter CasparRestApiSseTests`

Expected: PASS

- [ ] **Step 5: Commit**

```bash
git add src/StarDust.CasparCG.AspNetCore test/StarDust.CasparCG.IntegrationTests
git commit -m "feat: add rest api sse stream"
```

### Task 5: Add the sample proxy host, docs, and full verification

**Files:**
- Create: `demo/StarDust.CasparCG.RestProxy.Sample/StarDust.CasparCG.RestProxy.Sample.csproj`
- Create: `demo/StarDust.CasparCG.RestProxy.Sample/Program.cs`
- Modify: `src/StarDust.CasparCG.net.sln`
- Modify: `README.md`
- Modify: `docs/vnext/hosting-and-di.md`

- [ ] **Step 1: Write a failing smoke test or build check for the sample host**

```bash
rtk dotnet build demo/StarDust.CasparCG.RestProxy.Sample/StarDust.CasparCG.RestProxy.Sample.csproj
```

Expected: FAIL because the sample host project does not exist yet.

- [ ] **Step 2: Create the sample host project**

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <LangVersion>preview</LangVersion>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
    <Deterministic>true</Deterministic>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\src\StarDust.CasparCG\StarDust.CasparCG.csproj" />
    <ProjectReference Include="..\..\src\StarDust.CasparCG.Hosting\StarDust.CasparCG.Hosting.csproj" />
    <ProjectReference Include="..\..\src\StarDust.CasparCG.AspNetCore\StarDust.CasparCG.AspNetCore.csproj" />
  </ItemGroup>
</Project>
```

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddCasparCG()
    .ConnectTo("127.0.0.1", 5250)
    .ListenOscOn(6250);

builder.Services.AddCasparCGRestApi(options =>
{
    options.MapAdminEndpoints = true;
});

var app = builder.Build();
app.MapCasparCGApi();
app.Run();
```

- [ ] **Step 3: Document the new package and sample usage**

```md
## REST and SSE proxy

Use `StarDust.CasparCG.AspNetCore` to expose:

- `GET /server/version`
- `POST /channels/1/layers/10/play`
- `GET /events`
```

- [ ] **Step 4: Run the full verification suite**

Run: `rtk dotnet build src/StarDust.CasparCG.net.sln`

Run: `rtk dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj`

Run: `rtk dotnet test test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj`

Run: `rtk git diff --stat`

Expected: build passes, all tests pass, and only planned files are changed.

- [ ] **Step 5: Commit**

```bash
git add demo/StarDust.CasparCG.RestProxy.Sample src/StarDust.CasparCG.net.sln README.md docs/vnext/hosting-and-di.md
git commit -m "docs: add rest proxy sample and usage"
```

## Self-Review

- Spec coverage: package scaffold, REST routes, SSE, auth-neutral package design, sample proxy, tests, and docs all map to Tasks 1 through 5.
- Placeholder scan: no `TODO`/`TBD`; every task includes target files, commands, and minimal code shape.
- Type consistency: `AddCasparCGRestApi`, `MapCasparCGApi`, `CasparRestApiOptions`, `ICasparClientResolver`, and SSE types are introduced once and reused consistently across tasks.
