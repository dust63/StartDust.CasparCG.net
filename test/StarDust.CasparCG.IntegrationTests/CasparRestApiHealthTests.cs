using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using StarDust.CasparCG.AspNetCore;
using StarDust.CasparCG.AspNetCore.Contracts;
using StarDust.CasparCG.Hosting;
using StarDust.CasparCG.Health;
using StarDust.CasparCG.Testing.DummyServer;
using Xunit;

namespace StarDust.CasparCG.IntegrationTests;

public sealed class CasparRestApiHealthTests
{
    [Fact]
    public async Task Health_route_lazy_connects_and_returns_health_payload()
    {
        await using var server = await DummyCasparServer.StartAsync(
            DummyScenario.Empty().WithAmcpReply("VERSION SERVER", "201 VERSION OK\r\n2.5.0\r\n"),
            CancellationToken.None);

        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddCasparCG()
            .ConnectTo("127.0.0.1", server.AmcpPort);
        builder.Services.AddCasparCGRestApi();

        var app = builder.Build();
        app.MapCasparCGApi();
        await app.StartAsync();

        var response = await app.GetTestClient().GetAsync("/health");

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<ServerHealthResponse>();

        Assert.NotNull(payload);
        Assert.Equal("default", payload!.ServerName);
        Assert.Equal(ConnectionHealthStatus.Connected, payload.Status);
        Assert.True(payload.Connected);
        Assert.NotNull(payload.LastSuccessfulAmcpInteraction);
        Assert.Null(payload.LastFailure);

        await app.StopAsync();
        await app.DisposeAsync();
    }

    [Fact]
    public async Task Named_health_route_targets_named_client()
    {
        await using var defaultServer = await DummyCasparServer.StartAsync(DummyScenario.Empty(), CancellationToken.None);
        await using var studioServer = await DummyCasparServer.StartAsync(
            DummyScenario.Empty().WithAmcpReply("VERSION SERVER", "201 VERSION OK\r\n2.6.0\r\n"),
            CancellationToken.None);

        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddCasparCG()
            .ConnectTo("127.0.0.1", defaultServer.AmcpPort);
        builder.Services.AddCasparCG("studio-a")
            .ConnectTo("127.0.0.1", studioServer.AmcpPort);
        builder.Services.AddCasparCGRestApi();

        var app = builder.Build();
        app.MapCasparCGApi();
        await app.StartAsync();

        var client = app.GetTestClient();
        var response = await client.GetAsync("/servers/studio-a/health");

        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<ServerHealthResponse>();

        Assert.NotNull(payload);
        Assert.Equal("studio-a", payload!.ServerName);
        Assert.Equal(ConnectionHealthStatus.Connected, payload.Status);
        Assert.True(payload.Connected);

        await app.StopAsync();
        await app.DisposeAsync();
    }

    [Fact]
    public async Task Health_route_returns_problem_details_when_connection_fails()
    {
        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddCasparCG()
            .ConnectTo("127.0.0.1", GetUnusedTcpPort());
        builder.Services.AddCasparCGRestApi();

        var app = builder.Build();
        app.MapCasparCGApi();
        await app.StartAsync();

        var client = app.GetTestClient();
        var response = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(payload);
        Assert.Equal(StatusCodes.Status503ServiceUnavailable, payload!.Status);
        Assert.Contains("connection", payload.Detail ?? string.Empty, StringComparison.OrdinalIgnoreCase);

        await app.StopAsync();
        await app.DisposeAsync();
    }

    private static int GetUnusedTcpPort()
    {
        var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
        listener.Start();
        var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }
}
