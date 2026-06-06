using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using StarDust.CasparCG.AspNetCore;
using StarDust.CasparCG.Hosting;
using StarDust.CasparCG.Testing.DummyServer;

namespace StarDust.CasparCG.IntegrationTests;

internal sealed class CasparRestApiTestHost : IAsyncDisposable
{
    private readonly WebApplication _application;
    private readonly DummyCasparServer _server;

    private CasparRestApiTestHost(WebApplication application, DummyCasparServer server)
    {
        _application = application;
        _server = server;
        Client = application.GetTestClient();
    }

    public HttpClient Client { get; }

    public static async Task<CasparRestApiTestHost> StartAsync(
        Func<DummyScenario, DummyScenario>? configure = null,
        CancellationToken cancellationToken = default)
    {
        var scenario = configure?.Invoke(DummyScenario.Empty()) ?? DummyScenario.Empty();
        var server = await DummyCasparServer.StartAsync(scenario, cancellationToken);

        var builder = WebApplication.CreateSlimBuilder();
        builder.WebHost.UseTestServer();
        builder.Services.AddCasparCG()
            .ConnectTo("127.0.0.1", server.AmcpPort);
        builder.Services.AddCasparCGRestApi();

        var app = builder.Build();
        app.MapCasparCGApi();
        await app.StartAsync(cancellationToken);
        await app.Services.GetRequiredService<CasparClient>().ConnectAsync(cancellationToken);

        return new CasparRestApiTestHost(app, server);
    }

    public ValueTask DisposeAsync() =>
        new(Task.WhenAll(_application.StopAsync(), _application.DisposeAsync().AsTask(), _server.DisposeAsync().AsTask()));
}
