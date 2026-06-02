using Microsoft.Extensions.DependencyInjection;
using StarDust.CasparCG;
using StarDust.CasparCG.Protocol.Amcp;
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

        var dataStore = await client.DataStoreAsync("foo", "bar", CancellationToken.None);
        Assert.Equal("DATA STORE foo bar\r\n", transport.LastCommandText);
        Assert.Equal(202, dataStore.StatusCode);
        Assert.Equal("DATA STORE foo bar", dataStore.CommandText);

        var cgUpdate = await client.CgUpdateAsync(1, 10, "{xml}", CancellationToken.None);
        Assert.Equal("CG UPDATE 1-10 {xml}\r\n", transport.LastCommandText);
        Assert.Equal(202, cgUpdate.StatusCode);
        Assert.Equal("CG UPDATE 1-10 {xml}", cgUpdate.CommandText);

        await client.MixerVolumeAsync(1, 10, 0.5, CancellationToken.None);
        Assert.Equal("MIXER VOLUME 1-10 0.5\r\n", transport.LastCommandText);

        await client.MixerChromaAsync(1, 10, "0.1 0.2 0.3", CancellationToken.None);
        Assert.Equal("MIXER CHROMA 1-10 0.1 0.2 0.3\r\n", transport.LastCommandText);

        await client.MixerLevelsAsync(1, 10, "0 1 1 0 1", CancellationToken.None);
        Assert.Equal("MIXER LEVELS 1-10 0 1 1 0 1\r\n", transport.LastCommandText);

        await client.MixerFillAsync(1, 10, "0 0 1 1", CancellationToken.None);
        Assert.Equal("MIXER FILL 1-10 0 0 1 1\r\n", transport.LastCommandText);

        await client.MixerClipAsync(1, 10, "0 0 1 1", CancellationToken.None);
        Assert.Equal("MIXER CLIP 1-10 0 0 1 1\r\n", transport.LastCommandText);

        await client.MixerAnchorAsync(1, 10, "0.5 0.5", CancellationToken.None);
        Assert.Equal("MIXER ANCHOR 1-10 0.5 0.5\r\n", transport.LastCommandText);

        await client.MixerCropAsync(1, 10, "0 0 0 0", CancellationToken.None);
        Assert.Equal("MIXER CROP 1-10 0 0 0 0\r\n", transport.LastCommandText);

        await client.MixerRotationAsync(1, 10, "45", CancellationToken.None);
        Assert.Equal("MIXER ROTATION 1-10 45\r\n", transport.LastCommandText);

        await client.MixerPerspectiveAsync(1, 10, "0 0 1 0 1 1 0 1", CancellationToken.None);
        Assert.Equal("MIXER PERSPECTIVE 1-10 0 0 1 0 1 1 0 1\r\n", transport.LastCommandText);

        await client.MixerGridAsync(1, 10, "2 2", CancellationToken.None);
        Assert.Equal("MIXER GRID 1-10 2 2\r\n", transport.LastCommandText);

        await client.ThumbnailGenerateAllAsync(CancellationToken.None);
        Assert.Equal("THUMBNAIL GENERATE_ALL\r\n", transport.LastCommandText);

        await client.KillAsync(CancellationToken.None);
        Assert.Equal("KILL\r\n", transport.LastCommandText);

        await client.OscUnsubscribeAsync(5253, CancellationToken.None);
        Assert.Equal("OSC UNSUBSCRIBE 5253\r\n", transport.LastCommandText);
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
