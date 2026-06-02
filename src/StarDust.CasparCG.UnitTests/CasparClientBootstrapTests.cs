using Microsoft.Extensions.DependencyInjection;
using StarDust.CasparCG;
using StarDust.CasparCG.Transport;
using StarDust.CasparCG.Hosting;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class CasparClientBootstrapTests
{
    [Fact]
    public void AddCasparCg_registers_default_client_and_factory()
    {
        var services = new ServiceCollection();

        services.AddCasparCG();

        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<CasparClient>());
        Assert.NotNull(provider.GetRequiredService<ICasparClientFactory>());
    }

    [Fact]
    public async Task CasparClient_exposes_basic_and_query_helpers_with_thin_wrappers()
    {
        var transport = new RecordingAmcpTransport();
        var client = new CasparClient(transport);

        await client.LoadAsync(1, 10, "AMB", CancellationToken.None);
        Assert.Equal("LOAD 1-10 AMB\r\n", transport.LastCommandText);

        await client.CallBgAsync(1, 10, "AMB", CancellationToken.None);
        Assert.Equal("CALLBG 1-10 AMB\r\n", transport.LastCommandText);

        await client.PauseAsync(1, 10, CancellationToken.None);
        Assert.Equal("PAUSE 1-10\r\n", transport.LastCommandText);

        await client.ResumeAsync(1, 10, CancellationToken.None);
        Assert.Equal("RESUME 1-10\r\n", transport.LastCommandText);

        await client.ClearAsync(1, 10, CancellationToken.None);
        Assert.Equal("CLEAR 1-10\r\n", transport.LastCommandText);

        await client.SwapAsync(1, 10, 1, 11, CancellationToken.None);
        Assert.Equal("SWAP 1-10 1-11\r\n", transport.LastCommandText);

        await client.SetAsync(1, 10, "KEY", "VALUE", CancellationToken.None);
        Assert.Equal("SET 1-10 KEY VALUE\r\n", transport.LastCommandText);

        await client.ClearAllAsync(CancellationToken.None);
        Assert.Equal("CLEAR ALL\r\n", transport.LastCommandText);

        var info = await client.InfoAsync(CancellationToken.None);
        Assert.Equal("INFO\r\n", transport.LastCommandText);
        Assert.Equal(202, info.StatusCode);
        Assert.Equal("INFO", info.CommandText);

        var infoConfig = await client.InfoConfigAsync(CancellationToken.None);
        Assert.Equal("INFO CONFIG\r\n", transport.LastCommandText);
        Assert.Equal(202, infoConfig.StatusCode);
        Assert.Equal("INFO CONFIG", infoConfig.CommandText);

        var infoPaths = await client.InfoPathsAsync(CancellationToken.None);
        Assert.Equal("INFO PATHS\r\n", transport.LastCommandText);
        Assert.Equal(202, infoPaths.StatusCode);
        Assert.Equal("INFO PATHS", infoPaths.CommandText);

        var mediaFiles = await client.GetMediaFilesAsync(CancellationToken.None);
        Assert.Equal("CLS\r\n", transport.LastCommandText);
        Assert.Empty(mediaFiles);
    }

    private sealed class RecordingAmcpTransport : IAmcpTransport
    {
        public string? LastCommandText { get; private set; }

        public ValueTask ConnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

        public ValueTask DisconnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

        public ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken)
        {
            LastCommandText = commandText;
            return ValueTask.FromResult($"202 {commandText.TrimEnd('\r', '\n')} OK\r\n");
        }
    }
}
