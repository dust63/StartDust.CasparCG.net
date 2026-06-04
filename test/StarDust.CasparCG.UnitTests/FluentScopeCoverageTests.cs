using StarDust.CasparCG;
using StarDust.CasparCG.Osc;
using StarDust.CasparCG.Protocol.Amcp;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class FluentScopeCoverageTests
{
    [Fact]
    public async Task ServerScope_version_forwards_to_client_query()
    {
        var transport = new RecordingAmcpTransport("201 VERSION OK\r\n2.5.0\r\n");
        var client = new CasparClient(transport);

        var version = await client.Server().VersionAsync(CancellationToken.None);

        Assert.Equal("2.5.0", version);
        Assert.Equal("VERSION SERVER\r\n", transport.SentCommands.Single());
    }

    [Fact]
    public async Task ServerScope_query_methods_forward_to_client_queries()
    {
        var transport = new RecordingAmcpTransport(
            "200 CLS OK\r\n\"AMB\" MOVIE 42 20240101120000 240 1/25\r\n\r\n",
            "201 INFO OK\r\nserver-info\r\n",
            "201 INFO CONFIG OK\r\nconfig-info\r\n",
            "201 INFO PATHS OK\r\npaths-info\r\n",
            "202 DIAG OK\r\n");
        var client = new CasparClient(transport);

        var mediaFiles = await client.Server().MediaFilesAsync(CancellationToken.None);
        var info = await client.Server().InfoAsync(CancellationToken.None);
        var config = await client.Server().InfoConfigAsync(CancellationToken.None);
        var paths = await client.Server().InfoPathsAsync(CancellationToken.None);
        await client.Server().DiagAsync(CancellationToken.None);

        Assert.Equal("\"AMB\" MOVIE 42 20240101120000 240 1/25", mediaFiles.Single());
        Assert.Equal("server-info", info.Lines.Single());
        Assert.Equal("config-info", config.Lines.Single());
        Assert.Equal("paths-info", paths.Lines.Single());
        Assert.Equal(
            [
                "CLS\r\n",
                "INFO\r\n",
                "INFO CONFIG\r\n",
                "INFO PATHS\r\n",
                "DIAG\r\n"
            ],
            transport.SentCommands);
    }

    [Fact]
    public async Task ServerScope_remaining_query_methods_forward_to_client_queries()
    {
        var transport = new RecordingAmcpTransport(
            "200 CINF OK\r\n\"AMB\" MOVIE 42 20240101120000 240 1/25\r\n\r\n",
            "200 FLS OK\r\n\"Roboto\" fonts/roboto.ttf\r\n\r\n",
            "200 TLS OK\r\nLOWERTHIRD\r\n\r\n",
            "201 GL INFO OK\r\nrenderer-info\r\n",
            "202 GL GC OK\r\n");
        var client = new CasparClient(transport);

        var mediaInfo = await client.Server().MediaInfoAsync("AMB", CancellationToken.None);
        var fonts = await client.Server().FileListAsync(CancellationToken.None);
        var templates = await client.Server().TemplateListAsync(CancellationToken.None);
        var glInfo = await client.Server().GlInfoAsync(CancellationToken.None);
        await client.Server().GlGcAsync(CancellationToken.None);

        Assert.Equal("\"AMB\" MOVIE 42 20240101120000 240 1/25", mediaInfo.Lines.Single());
        Assert.Equal("\"Roboto\" fonts/roboto.ttf", fonts.Lines.Single());
        Assert.Equal("LOWERTHIRD", templates.Lines.Single());
        Assert.Equal("renderer-info", glInfo.Lines.Single());
        Assert.Equal(
            [
                "CINF AMB\r\n",
                "FLS\r\n",
                "TLS\r\n",
                "GL INFO\r\n",
                "GL GC\r\n"
            ],
            transport.SentCommands);
    }

    [Fact]
    public async Task AdminScope_restart_forwards_to_restart_command()
    {
        var transport = new RecordingAmcpTransport("202 RESTART OK\r\n");
        var client = new CasparClient(transport);

        await client.Admin().RestartAsync(CancellationToken.None);

        Assert.Equal("RESTART\r\n", transport.SentCommands.Single());
    }

    [Fact]
    public async Task AdminScope_dangerous_commands_are_kept_behind_admin_scope()
    {
        var transport = new RecordingAmcpTransport(
            "202 BYE OK\r\n",
            "202 KILL OK\r\n",
            "202 RESTART OK\r\n",
            "202 LOCK OK\r\n");
        var client = new CasparClient(transport);
        var admin = client.Admin();

        await admin.ByeAsync(CancellationToken.None);
        await admin.KillAsync(CancellationToken.None);
        await admin.RestartAsync(CancellationToken.None);
        await admin.LockAsync(CancellationToken.None);

        Assert.Equal(
            [
                "BYE\r\n",
                "KILL\r\n",
                "RESTART\r\n",
                "LOCK\r\n"
            ],
            transport.SentCommands);
    }

    [Fact]
    public async Task DataScope_methods_forward_to_data_commands()
    {
        var transport = new RecordingAmcpTransport(
            "202 DATA STORE OK\r\n",
            "201 DATA RETRIEVE OK\r\npayload\r\n",
            "201 DATA LIST OK\r\nitem-a\r\nitem-b\r\n",
            "202 DATA REMOVE OK\r\n");
        var client = new CasparClient(transport);
        var data = client.Data();

        var store = await data.StoreAsync("foo", "bar", CancellationToken.None);
        var retrieve = await data.RetrieveAsync("foo", CancellationToken.None);
        var list = await data.ListAsync("sub", CancellationToken.None);
        var remove = await data.RemoveAsync("foo", CancellationToken.None);

        Assert.Equal(202, store.StatusCode);
        Assert.Equal("payload", retrieve.Lines.Single());
        Assert.Equal(["item-a", "item-b"], list.Lines);
        Assert.Equal(202, remove.StatusCode);
        Assert.Equal(
            [
                "DATA STORE foo bar\r\n",
                "DATA RETRIEVE foo\r\n",
                "DATA LIST sub\r\n",
                "DATA REMOVE foo\r\n"
            ],
            transport.SentCommands);
    }

    [Fact]
    public async Task ThumbnailScope_methods_forward_to_thumbnail_commands()
    {
        var transport = new RecordingAmcpTransport(
            "201 THUMBNAIL LIST OK\r\nAMB\r\n",
            "201 THUMBNAIL RETRIEVE OK\r\nimage-data\r\n",
            "202 THUMBNAIL GENERATE OK\r\n",
            "202 THUMBNAIL GENERATE_ALL OK\r\n");
        var client = new CasparClient(transport);
        var thumbnails = client.Thumbnails();

        var list = await thumbnails.ListAsync("clips", CancellationToken.None);
        var retrieve = await thumbnails.RetrieveAsync("AMB", CancellationToken.None);
        await thumbnails.GenerateAsync("AMB", CancellationToken.None);
        await thumbnails.GenerateAllAsync(CancellationToken.None);

        Assert.Equal("AMB", list.Lines.Single());
        Assert.Equal("image-data", retrieve.Lines.Single());
        Assert.Equal(
            [
                "THUMBNAIL LIST clips\r\n",
                "THUMBNAIL RETRIEVE AMB\r\n",
                "THUMBNAIL GENERATE AMB\r\n",
                "THUMBNAIL GENERATE_ALL\r\n"
            ],
            transport.SentCommands);
    }

    [Fact]
    public async Task OscScope_methods_forward_to_osc_operations()
    {
        var amcpTransport = new RecordingAmcpTransport(
            "202 OSC SUBSCRIBE OK\r\n",
            "202 OSC UNSUBSCRIBE OK\r\n");
        var oscTransport = new RecordingOscTransport();
        var client = new CasparClient(amcpTransport, oscTransport, new NoOpOscMessageMapper());
        var osc = client.Osc();

        await osc.StartAsync(6250, CancellationToken.None);
        await osc.SubscribeAsync(6250, CancellationToken.None);
        await osc.UnsubscribeAsync(5253, CancellationToken.None);
        await osc.StopAsync(CancellationToken.None);

        Assert.Equal(6250, oscTransport.StartedPort);
        Assert.True(oscTransport.Stopped);
        Assert.Equal(
            [
                "OSC SUBSCRIBE 6250\r\n",
                "OSC UNSUBSCRIBE 5253\r\n"
            ],
            amcpTransport.SentCommands);
    }

    [Fact]
    public async Task ChannelScope_grid_forwards_to_channel_grid()
    {
        var transport = new RecordingAmcpTransport("202 CHANNEL_GRID OK\r\n");
        var client = new CasparClient(transport);

        await client.Channel(1).GridAsync(CancellationToken.None);

        Assert.Equal("CHANNEL_GRID 1\r\n", transport.SentCommands.Single());
    }

    [Fact]
    public async Task LayerScope_transport_controls_forward_to_layer_commands()
    {
        var transport = new RecordingAmcpTransport(
            "202 PAUSE OK\r\n",
            "202 RESUME OK\r\n",
            "202 STOP OK\r\n",
            "202 CLEAR OK\r\n");
        var client = new CasparClient(transport);
        var layer = client.Channel(1).Layer(10);

        await layer.PauseAsync(CancellationToken.None);
        await layer.ResumeAsync(CancellationToken.None);
        await layer.StopAsync(CancellationToken.None);
        await layer.ClearAsync(CancellationToken.None);

        Assert.Equal(
            [
                "PAUSE 1-10\r\n",
                "RESUME 1-10\r\n",
                "STOP 1-10\r\n",
                "CLEAR 1-10\r\n"
            ],
            transport.SentCommands);
    }

    [Fact]
    public async Task LayerScope_clip_and_property_commands_forward_to_layer_commands()
    {
        var transport = new RecordingAmcpTransport(
            "202 CALL OK\r\n",
            "202 CALLBG OK\r\n",
            "202 SWAP OK\r\n",
            "202 ADD OK\r\n",
            "202 REMOVE OK\r\n",
            "202 APPLY OK\r\n",
            "202 PRINT OK\r\n",
            "202 SET OK\r\n");
        var client = new CasparClient(transport);
        var layer = client.Channel(1).Layer(10);

        await layer.CallAsync("AMB", CancellationToken.None);
        await layer.CallBgAsync("BG", CancellationToken.None);
        await layer.SwapAsync(2, 20, CancellationToken.None);
        await layer.AddAsync("FILTER", CancellationToken.None);
        await layer.RemoveAsync("FILTER", CancellationToken.None);
        await layer.ApplyAsync("FILTER", CancellationToken.None);
        await layer.PrintAsync("FILTER", CancellationToken.None);
        await layer.SetAsync("volume", "0.5", CancellationToken.None);

        Assert.Equal(
            [
                "CALL 1-10 AMB\r\n",
                "CALLBG 1-10 BG\r\n",
                "SWAP 1-10 2-20\r\n",
                "ADD 1-10 FILTER\r\n",
                "REMOVE 1-10 FILTER\r\n",
                "APPLY 1-10 FILTER\r\n",
                "PRINT 1-10 FILTER\r\n",
                "SET 1-10 volume 0.5\r\n"
            ],
            transport.SentCommands);
    }

    [Fact]
    public async Task LayerScope_cg_commands_forward_to_template_commands()
    {
        var transport = new RecordingAmcpTransport(
            "202 CG PLAY OK\r\n",
            "202 CG STOP OK\r\n",
            "202 CG NEXT OK\r\n",
            "202 CG REMOVE OK\r\n",
            "202 CG CLEAR OK\r\n",
            "202 CG INVOKE OK\r\n");
        var client = new CasparClient(transport);
        var layer = client.Channel(1).Layer(10);

        await layer.CgPlayAsync(CancellationToken.None);
        await layer.CgStopAsync(CancellationToken.None);
        await layer.CgNextAsync(CancellationToken.None);
        await layer.CgRemoveAsync(CancellationToken.None);
        await layer.CgClearAsync(CancellationToken.None);
        await layer.CgInvokeAsync("next", CancellationToken.None);

        Assert.Equal(
            [
                "CG PLAY 1-10\r\n",
                "CG STOP 1-10\r\n",
                "CG NEXT 1-10\r\n",
                "CG REMOVE 1-10\r\n",
                "CG CLEAR 1-10\r\n",
                "CG INVOKE 1-10 next\r\n"
            ],
            transport.SentCommands);
    }

    [Fact]
    public async Task LayerScope_mixer_commands_forward_to_mixer_commands()
    {
        var transport = new RecordingAmcpTransport(
            "202 MIXER KEYER OK\r\n",
            "202 MIXER INVERT OK\r\n",
            "202 MIXER BLEND OK\r\n",
            "202 MIXER OPACITY OK\r\n",
            "202 MIXER BRIGHTNESS OK\r\n",
            "202 MIXER SATURATION OK\r\n",
            "202 MIXER CONTRAST OK\r\n",
            "202 MIXER VOLUME OK\r\n",
            "202 MIXER COMMIT OK\r\n",
            "202 MIXER CLEAR OK\r\n");
        var client = new CasparClient(transport);
        var layer = client.Channel(1).Layer(10);

        await layer.MixerKeyerAsync(true, CancellationToken.None);
        await layer.MixerInvertAsync(false, CancellationToken.None);
        await layer.MixerBlendAsync("add", CancellationToken.None);
        await layer.MixerOpacityAsync(0.75, CancellationToken.None);
        await layer.MixerBrightnessAsync(1.1, CancellationToken.None);
        await layer.MixerSaturationAsync(0.9, CancellationToken.None);
        await layer.MixerContrastAsync(1.2, CancellationToken.None);
        await layer.MixerVolumeAsync(0.6, CancellationToken.None);
        await layer.MixerCommitAsync(CancellationToken.None);
        await layer.MixerClearAsync(CancellationToken.None);

        Assert.Equal(
            [
                "MIXER KEYER 1-10 1\r\n",
                "MIXER INVERT 1-10 0\r\n",
                "MIXER BLEND 1-10 add\r\n",
                "MIXER OPACITY 1-10 0.75\r\n",
                "MIXER BRIGHTNESS 1-10 1.1\r\n",
                "MIXER SATURATION 1-10 0.9\r\n",
                "MIXER CONTRAST 1-10 1.2\r\n",
                "MIXER VOLUME 1-10 0.6\r\n",
                "MIXER COMMIT 1-10\r\n",
                "MIXER CLEAR 1-10\r\n"
            ],
            transport.SentCommands);
    }

    [Fact]
    public async Task LayerScope_remaining_advanced_commands_forward_to_underlying_operations()
    {
        var transport = new RecordingAmcpTransport(
            "202 LOAD OK\r\n",
            "202 CG ADD OK\r\n",
            "201 CG UPDATE OK\r\npayload\r\n",
            "202 MIXER CHROMA OK\r\n",
            "202 MIXER LEVELS OK\r\n",
            "202 MIXER FILL OK\r\n",
            "202 MIXER CLIP OK\r\n",
            "202 MIXER ANCHOR OK\r\n",
            "202 MIXER CROP OK\r\n",
            "202 MIXER ROTATION OK\r\n",
            "202 MIXER PERSPECTIVE OK\r\n",
            "202 MIXER GRID OK\r\n");
        var client = new CasparClient(transport);
        var layer = client.Channel(1).Layer(10);

        await layer.LoadAsync("AMB", CancellationToken.None);
        await layer.CgAddAsync("LOWER", true, "<template/>", CancellationToken.None);
        var cgUpdate = await layer.CgUpdateAsync("{xml}", CancellationToken.None);
        await layer.MixerChromaAsync("0.1 0.2 0.3 0.4 0.5 0.6", CancellationToken.None);
        await layer.MixerLevelsAsync("0 1 1 0 1", CancellationToken.None);
        await layer.MixerFillAsync("0 0 1 1", CancellationToken.None);
        await layer.MixerClipAsync("0 0 1 1", CancellationToken.None);
        await layer.MixerAnchorAsync("0.5 0.5", CancellationToken.None);
        await layer.MixerCropAsync("0 0 0 0", CancellationToken.None);
        await layer.MixerRotationAsync("45", CancellationToken.None);
        await layer.MixerPerspectiveAsync("0 0 1 0 1 1 0 1", CancellationToken.None);
        await layer.MixerGridAsync("2 2", CancellationToken.None);

        Assert.Equal("payload", cgUpdate.Lines.Single());
        Assert.Equal(
            [
                "LOAD 1-10 AMB\r\n",
                "CG ADD 1-10 LOWER 1 <template/>\r\n",
                "CG UPDATE 1-10 {xml}\r\n",
                "MIXER CHROMA 1-10 0.1 0.2 0.3 0.4 0.5 0.6\r\n",
                "MIXER LEVELS 1-10 0 1 1 0 1\r\n",
                "MIXER FILL 1-10 0 0 1 1\r\n",
                "MIXER CLIP 1-10 0 0 1 1\r\n",
                "MIXER ANCHOR 1-10 0.5 0.5\r\n",
                "MIXER CROP 1-10 0 0 0 0\r\n",
                "MIXER ROTATION 1-10 45\r\n",
                "MIXER PERSPECTIVE 1-10 0 0 1 0 1 1 0 1\r\n",
                "MIXER GRID 1-10 2 2\r\n"
            ],
            transport.SentCommands);
    }

    private sealed class RecordingAmcpTransport : IAmcpTransport
    {
        private readonly Queue<string> _responses;

        public RecordingAmcpTransport(params string[] responses)
        {
            _responses = new Queue<string>(responses);
        }

        public List<string> SentCommands { get; } = [];

        public ValueTask ConnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

        public ValueTask DisconnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

        public ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken)
        {
            SentCommands.Add(commandText);
            return ValueTask.FromResult(_responses.Dequeue());
        }
    }

    private sealed class RecordingOscTransport : IOscTransport
    {
        public int? StartedPort { get; private set; }

        public bool Stopped { get; private set; }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        public ValueTask StartAsync(
            int port,
            Func<ReadOnlyMemory<byte>, CancellationToken, ValueTask> packetHandler,
            CancellationToken cancellationToken)
        {
            StartedPort = port;
            return ValueTask.CompletedTask;
        }

        public ValueTask StopAsync(CancellationToken cancellationToken)
        {
            Stopped = true;
            return ValueTask.CompletedTask;
        }
    }

    private sealed class NoOpOscMessageMapper : IOscMessageMapper
    {
        public bool TryMap(string address, IReadOnlyList<object?> arguments, out Events.CasparEvent? evt)
        {
            evt = null;
            return false;
        }
    }
}
