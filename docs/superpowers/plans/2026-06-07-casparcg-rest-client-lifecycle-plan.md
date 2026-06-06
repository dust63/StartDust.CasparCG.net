# CasparCG REST Client Lifecycle Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Make the REST addon use long-lived CasparCG clients with lazy AMCP connect, optional warm-up at startup, a per-server health route that initializes connection on demand, and clean shutdown disconnection.

**Architecture:** Keep one `CasparClient` per configured server for the full app lifetime. The REST resolver stays responsible only for selecting the correct client, while the client itself owns lazy connect and disconnect behavior. A small health endpoint exposes connection state and can trigger the initial connect, returning `ProblemDetails` with `503` if the server is unreachable.

**Tech Stack:** .NET 10, ASP.NET Core minimal APIs, xUnit, current `StarDust.CasparCG` / `StarDust.CasparCG.AspNetCore` projects.

---

### Task 1: Add lifecycle options and startup warm-up wiring

**Files:**
- Modify: `src/StarDust.CasparCG.AspNetCore/CasparRestApiOptions.cs`
- Modify: `src/StarDust.CasparCG.AspNetCore/ServiceCollectionExtensions.cs`
- Create: `src/StarDust.CasparCG.AspNetCore/Internal/CasparRestApiHostedService.cs`
- Test: `test/StarDust.CasparCG.UnitTests/RestApiRegistrationTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
[Fact]
public void AddCasparCGRestApi_registers_lifecycle_options_and_hosted_service()
{
    var services = new ServiceCollection();

    services.AddCasparCG();
    services.AddCasparCGRestApi(options =>
    {
        options.WarmUpClientsOnStartup = true;
        options.MapHealthEndpoints = true;
    });

    using var provider = services.BuildServiceProvider();

    var options = provider.GetRequiredService<IOptions<CasparRestApiOptions>>().Value;
    Assert.True(options.WarmUpClientsOnStartup);
    Assert.True(options.MapHealthEndpoints);

    var hostedServices = provider.GetServices<IHostedService>().ToArray();
    Assert.Contains(hostedServices, service => service.GetType().Name == "CasparRestApiHostedService");
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter AddCasparCGRestApi_registers_lifecycle_options_and_hosted_service -v normal`
Expected: fail because `WarmUpClientsOnStartup`, `MapHealthEndpoints`, and the hosted service registration do not exist yet.

- [ ] **Step 3: Write minimal implementation**

```csharp
public sealed class CasparRestApiOptions
{
    public string RoutePrefix { get; set; } = string.Empty;
    public bool MapAdminEndpoints { get; set; }
    public bool EnableSse { get; set; } = true;
    public bool EnableOpenApi { get; set; } = true;
    public bool WarmUpClientsOnStartup { get; set; }
    public bool MapHealthEndpoints { get; set; } = true;
}
```

```csharp
services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, CasparRestApiHostedService>());
```

```csharp
internal sealed class CasparRestApiHostedService : IHostedService
{
    private readonly ICasparClientFactory _clientFactory;
    private readonly IEnumerable<CasparClientOptions> _clientOptions;
    private readonly IOptions<CasparRestApiOptions> _options;

    public CasparRestApiHostedService(
        ICasparClientFactory clientFactory,
        IEnumerable<CasparClientOptions> clientOptions,
        IOptions<CasparRestApiOptions> options)
    {
        _clientFactory = clientFactory;
        _clientOptions = clientOptions;
        _options = options;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.Value.WarmUpClientsOnStartup)
        {
            return;
        }

        foreach (var clientName in _clientOptions.Select(options => options.Name).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            await _clientFactory.GetClient(clientName).ConnectAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var clientName in _clientOptions.Select(options => options.Name).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            await _clientFactory.GetClient(clientName).DisconnectAsync(cancellationToken);
        }
    }
}
```

- [ ] **Step 4: Run the test to verify it passes**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter AddCasparCGRestApi_registers_lifecycle_options_and_hosted_service -v normal`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add src/StarDust.CasparCG.AspNetCore/CasparRestApiOptions.cs src/StarDust.CasparCG.AspNetCore/ServiceCollectionExtensions.cs src/StarDust.CasparCG.AspNetCore/Internal/CasparRestApiHostedService.cs test/StarDust.CasparCG.UnitTests/RestApiRegistrationTests.cs
git commit -m "Add REST lifecycle options and warm-up hosting"
```

### Task 2: Implement lazy connect and clean disconnect in the client

**Files:**
- Modify: `src/StarDust.CasparCG/CasparClient.cs`
- Test: `test/StarDust.CasparCG.UnitTests/CasparClientCommandTests.cs`
- Test: `test/StarDust.CasparCG.UnitTests/HealthAndReconnectTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
[Fact]
public async Task First_command_lazily_connects_before_sending_amcp()
{
    await using var server = await DummyCasparServer.StartAsync(
        DummyScenario.Empty().WithAmcpReply("VERSION SERVER", "201 VERSION OK\r\n2.5.0\r\n"),
        CancellationToken.None);

    var client = new CasparClient(new TcpAmcpTransport("127.0.0.1", server.AmcpPort));

    Assert.Equal(ConnectionHealthStatus.Disconnected, client.HealthStatus);

    var version = await client.Server().VersionAsync();

    Assert.Equal("2.5.0", version);
    Assert.Equal(ConnectionHealthStatus.Connected, client.HealthStatus);
    Assert.Equal(["VERSION SERVER"], server.ReceivedCommands);
}
```

```csharp
[Fact]
public async Task DisconnectAsync_closes_transport_and_marks_client_disconnected()
{
    await using var server = await DummyCasparServer.StartAsync(DummyScenario.Empty(), CancellationToken.None);

    var client = new CasparClient(new TcpAmcpTransport("127.0.0.1", server.AmcpPort));
    await client.ConnectAsync();
    await client.DisconnectAsync();

    Assert.Equal(ConnectionHealthStatus.Disconnected, client.HealthStatus);
}
```

- [ ] **Step 2: Run the tests to verify they fail**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter \"First_command_lazily_connects_before_sending_amcp|DisconnectAsync_closes_transport_and_marks_client_disconnected\" -v normal`
Expected: fail because `DisconnectAsync` on `CasparClient` does not exist yet and lazy connect is only partially wired.

- [ ] **Step 3: Write minimal implementation**

```csharp
public async ValueTask DisconnectAsync(CancellationToken cancellationToken = default)
{
    HealthStatus = ConnectionHealthStatus.Stopping;
    if (_transport is not null)
    {
        await _transport.DisconnectAsync(cancellationToken);
    }

    if (_oscTransport is not null)
    {
        await _oscTransport.StopAsync(cancellationToken);
    }

    HealthStatus = ConnectionHealthStatus.Disconnected;
}
```

```csharp
private async ValueTask EnsureConnectedAsync(CancellationToken cancellationToken = default)
{
    if (HealthStatus == ConnectionHealthStatus.Connected)
    {
        return;
    }

    await _connectLock.WaitAsync(cancellationToken);
    try
    {
        if (HealthStatus == ConnectionHealthStatus.Connected)
        {
            return;
        }

        HealthStatus = ConnectionHealthStatus.Connecting;
        await RequireTransport().ConnectAsync(cancellationToken);
        HealthStatus = ConnectionHealthStatus.Connected;
        Diagnostics.LastSuccessfulAmcpInteraction = DateTimeOffset.UtcNow;
    }
    finally
    {
        _connectLock.Release();
    }
}
```

- [ ] **Step 4: Run the tests to verify they pass**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter \"First_command_lazily_connects_before_sending_amcp|DisconnectAsync_closes_transport_and_marks_client_disconnected\" -v normal`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add src/StarDust.CasparCG/CasparClient.cs test/StarDust.CasparCG.UnitTests/CasparClientCommandTests.cs test/StarDust.CasparCG.UnitTests/HealthAndReconnectTests.cs
git commit -m "Make CasparClient lazy-connect and disposable"
```

### Task 3: Add the per-server health route with ProblemDetails 503

**Files:**
- Modify: `src/StarDust.CasparCG.AspNetCore/EndpointRouteBuilderExtensions.cs`
- Modify: `src/StarDust.CasparCG.AspNetCore/Internal/CasparQueryHandlers.cs`
- Modify: `src/StarDust.CasparCG.AspNetCore/Internal/CasparProblemDetailsFactory.cs`
- Create: `src/StarDust.CasparCG.AspNetCore/Internal/ServerHealthResponse.cs`
- Test: `test/StarDust.CasparCG.IntegrationTests/CasparRestApiHealthTests.cs`
- Test: `test/StarDust.CasparCG.UnitTests/RestApiEndpointTests.cs`

- [ ] **Step 1: Write the failing test**

```csharp
[Fact]
public async Task Health_route_lazy_connects_and_returns_health_payload()
{
    await using var fixture = await CasparRestApiTestHost.StartAsync(
        scenario => scenario.WithAmcpReply("VERSION SERVER", "201 VERSION OK\r\n2.5.0\r\n"));

    var response = await fixture.Client.GetAsync("/servers/default/health");

    response.EnsureSuccessStatusCode();
    var payload = await response.Content.ReadFromJsonAsync<ServerHealthResponse>();

    Assert.Equal("default", payload?.ServerName);
    Assert.True(payload?.Connected);
}
```

```csharp
[Fact]
public async Task Health_route_returns_problem_details_on_connection_failure()
{
    using var listener = new TcpListener(IPAddress.Loopback, 0);
    listener.Start();
    var unavailablePort = ((IPEndPoint)listener.LocalEndpoint).Port;
    listener.Stop();

    var builder = WebApplication.CreateSlimBuilder();
    builder.WebHost.UseTestServer();
    builder.Services.AddCasparCG()
        .ConnectTo("127.0.0.1", unavailablePort);
    builder.Services.AddCasparCGRestApi();

    var app = builder.Build();
    app.MapCasparCGApi();
    await app.StartAsync();
    var client = app.GetTestClient();

    var response = await client.GetAsync("/servers/default/health");

    Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    var payload = await response.Content.ReadFromJsonAsync<ProblemDetails>();
    Assert.Contains("AMCP", payload?.Detail);

    await app.StopAsync();
    await app.DisposeAsync();
}
```

- [ ] **Step 2: Run the tests to verify they fail**

Run: `dotnet test test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --filter \"Health_route_lazy_connects_and_returns_problem_details_when_unreachable|Health_route_returns_problem_details_on_connection_failure\" -v normal`
Expected: fail because `/servers/{name}/health` and the response contract do not exist yet.

- [ ] **Step 3: Write minimal implementation**

```csharp
group.MapGet("/health", CasparQueryHandlers.GetHealthAsync)
    .WithDisplayName(Display(displayPrefix, "GET /health"));
```

```csharp
public static async Task<IResult> GetHealthAsync(
    ICasparClientResolver clientResolver,
    HttpContext httpContext,
    CancellationToken cancellationToken)
{
    try
    {
        var client = clientResolver.ResolveDefaultClient();
        await client.ConnectAsync(cancellationToken);
        var serverName = httpContext.Request.RouteValues.TryGetValue("name", out var name)
            ? (string?)name ?? "default"
            : "default";

        return Results.Ok(new ServerHealthResponse(
            serverName: serverName,
            status: client.HealthStatus,
            connected: client.HealthStatus == ConnectionHealthStatus.Connected,
            lastSuccessfulAmcpInteraction: client.Diagnostics.LastSuccessfulAmcpInteraction,
            lastFailure: client.Diagnostics.LastFailure?.Message));
    }
    catch (Exception exception)
    {
        var problem = CasparProblemDetailsFactory.FromException(exception);
        return Results.Problem(
            title: problem.Title,
            detail: problem.Detail,
            statusCode: StatusCodes.Status503ServiceUnavailable);
    }
}
```

```csharp
public sealed record ServerHealthResponse(
    string ServerName,
    ConnectionHealthStatus Status,
    bool Connected,
    DateTimeOffset? LastSuccessfulAmcpInteraction,
    string? LastFailure);
```

- [ ] **Step 4: Run the tests to verify they pass**

Run: `dotnet test test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --filter \"Health_route_lazy_connects_and_returns_problem_details_when_unreachable|Health_route_returns_problem_details_on_connection_failure\" -v normal`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add src/StarDust.CasparCG.AspNetCore/EndpointRouteBuilderExtensions.cs src/StarDust.CasparCG.AspNetCore/Internal/CasparQueryHandlers.cs src/StarDust.CasparCG.AspNetCore/Internal/CasparProblemDetailsFactory.cs src/StarDust.CasparCG.AspNetCore/Internal/ServerHealthResponse.cs test/StarDust.CasparCG.IntegrationTests/CasparRestApiHealthTests.cs test/StarDust.CasparCG.UnitTests/RestApiEndpointTests.cs
git commit -m "Add REST health route with ProblemDetails"
```

### Task 4: Update docs and samples to reflect the new lifecycle

**Files:**
- Modify: `README.md`
- Modify: `docs/vnext/hosting-and-di.md`
- Modify: `docs/vnext/rest-api-addon.md`
- Modify: `demo/StarDust.CasparCG.RestProxy.Sample/Program.cs`

- [ ] **Step 1: Write the failing test**

```csharp
[Fact]
public void Rest_docs_include_health_route_and_lazy_connect_note()
{
    var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../docs/vnext/rest-api-addon.md"));
    var text = File.ReadAllText(path);

    Assert.Contains("/servers/{name}/health", text);
    Assert.Contains("lazy", text, StringComparison.OrdinalIgnoreCase);
    Assert.Contains("ProblemDetails", text);
}
```

- [ ] **Step 2: Run the test to verify it fails**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter Rest_docs_include_health_route_and_lazy_connect_note -v normal`
Expected: fail because the docs do not yet mention the health route and lazy connect contract in the right place.

- [ ] **Step 3: Write minimal implementation**

Update the setup snippets to show `ConnectAsync()` as optional warm-up, document `WarmUpClientsOnStartup` and `MapHealthEndpoints`, and add a small health-route section describing the `ProblemDetails` 503 behavior.

- [ ] **Step 4: Run the test to verify it passes**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter Rest_docs_include_health_route_and_lazy_connect_note -v normal`
Expected: PASS.

- [ ] **Step 5: Commit**

```bash
git add README.md docs/vnext/hosting-and-di.md docs/vnext/rest-api-addon.md demo/StarDust.CasparCG.RestProxy.Sample/Program.cs
git commit -m "Document REST lifecycle and health route"
```

## Coverage Check

- Lazy connect on first AMCP call: Task 2
- Optional warm-up at startup: Task 1
- Per-server health route that initiates connection: Task 3
- ProblemDetails on `503`: Task 3
- Clean disconnect at shutdown: Task 1 and Task 2
- SSE compatibility: preserved by the long-lived client model, documented in Task 4
- OSC compatibility: preserved by the same long-lived client model, documented in Task 4
