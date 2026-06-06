using StarDust.CasparCG;
using StarDust.CasparCG.Protocol.Amcp;
using StarDust.CasparCG.Protocol.Amcp.Commands;
using StarDust.CasparCG.Query;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class CasparClientCommandTests
{
    [Fact]
    public void Amcp_namespace_remains_available_after_merge()
    {
        var command = new StarDust.CasparCG.Protocol.Amcp.Commands.VersionCommand("SERVER");

        Assert.Equal("VERSION SERVER\r\n", command.Serialize());
    }

    [Fact]
    public void LoadCommand_serializes_expected_amcp_command()
    {
        var command = new LoadCommand(
            1,
            10,
            "AMB",
            new PlaybackOptions
            {
                Transition = PlaybackTransition.Mix(12),
                Loop = true,
                Seek = 24,
                Length = 48,
                Filter = "hflip",
                ClearOn404 = true
            });

        Assert.Equal("LOAD 1-10 AMB MIX 12 LOOP SEEK 24 LENGTH 48 FILTER hflip CLEAR_ON_404\r\n", command.Serialize());
    }

    [Fact]
    public void PauseCommand_serializes_expected_amcp_command()
    {
        var command = new PauseCommand(1, 10);

        Assert.Equal("PAUSE 1-10\r\n", command.Serialize());
    }

    [Fact]
    public void CallBgCommand_serializes_expected_amcp_command()
    {
        var command = new CallBgCommand(1, 10, "AMB");

        Assert.Equal("CALLBG 1-10 AMB\r\n", command.Serialize());
    }

    [Fact]
    public void LoadBackgroundCommand_serializes_expected_amcp_command()
    {
        var command = new LoadBackgroundCommand(
            1,
            10,
            "AMB",
            new LoadBackgroundOptions
            {
                Transition = PlaybackTransition.Slide(10, PlaybackTransitionDirection.Left, "linear"),
                Loop = true,
                Seek = 100,
                Length = 200,
                Filter = "hflip",
                ClearOn404 = true,
                AutoPlay = true
            });

        Assert.Equal("LOADBG 1-10 AMB SLIDE 10 linear LEFT LOOP SEEK 100 LENGTH 200 FILTER hflip CLEAR_ON_404 AUTO\r\n", command.Serialize());
    }

    [Fact]
    public void ResumeCommand_serializes_expected_amcp_command()
    {
        var command = new ResumeCommand(1, 10);

        Assert.Equal("RESUME 1-10\r\n", command.Serialize());
    }

    [Fact]
    public void ClearCommand_serializes_expected_amcp_command()
    {
        var command = new ClearCommand(1, 10);

        Assert.Equal("CLEAR 1-10\r\n", command.Serialize());
    }

    [Fact]
    public void CallCommand_serializes_expected_amcp_command()
    {
        var command = new CallCommand(1, 10, "AMB");

        Assert.Equal("CALL 1-10 AMB\r\n", command.Serialize());
    }

    [Fact]
    public void PlayCommand_serializes_expected_amcp_command()
    {
        var command = new PlayCommand(
            1,
            10,
            "AMB",
            new PlaybackOptions
            {
                Transition = PlaybackTransition.Wipe(8, PlaybackTransitionDirection.Right),
                Loop = true,
                Seek = 12,
                Length = 24,
                Filter = "hflip",
                ClearOn404 = true
            });

        Assert.Equal("PLAY 1-10 AMB WIPE 8 RIGHT LOOP SEEK 12 LENGTH 24 FILTER hflip CLEAR_ON_404\r\n", command.Serialize());
    }

    [Fact]
    public void SwapCommand_serializes_expected_amcp_command()
    {
        var command = new SwapCommand(1, 10, 1, 11);

        Assert.Equal("SWAP 1-10 1-11\r\n", command.Serialize());
    }

    [Fact]
    public void AddCommand_serializes_expected_amcp_command()
    {
        var command = new AddCommand(1, "FILE", "filename.mov", 700);

        Assert.Equal("ADD 1-700 FILE filename.mov\r\n", command.Serialize());
    }

    [Fact]
    public void RemoveCommand_serializes_expected_amcp_command()
    {
        var command = new RemoveCommand(1, ConsumerIndex: 300);

        Assert.Equal("REMOVE 1-300\r\n", command.Serialize());
    }

    [Fact]
    public void ApplyCommand_serializes_expected_amcp_command()
    {
        var command = new ApplyCommand(1, 10, "AMB");

        Assert.Equal("APPLY 1-10 AMB\r\n", command.Serialize());
    }

    [Fact]
    public void PrintCommand_serializes_expected_amcp_command()
    {
        var command = new PrintCommand(1);

        Assert.Equal("PRINT 1\r\n", command.Serialize());
    }

    [Fact]
    public void ClearAllCommand_serializes_expected_amcp_command()
    {
        var command = new ClearAllCommand();

        Assert.Equal("CLEAR ALL\r\n", command.Serialize());
    }

    [Fact]
    public void SetCommand_serializes_expected_amcp_command()
    {
        var command = new SetCommand(1, "KEY", "VALUE");

        Assert.Equal("SET 1 KEY VALUE\r\n", command.Serialize());
    }

    [Fact]
    public void InfoConfigCommand_serializes_expected_amcp_command()
    {
        var command = new InfoConfigCommand();

        Assert.Equal("INFO CONFIG\r\n", command.Serialize());
    }

    [Fact]
    public void InfoPathsCommand_serializes_expected_amcp_command()
    {
        var command = new InfoPathsCommand();

        Assert.Equal("INFO PATHS\r\n", command.Serialize());
    }

    [Fact]
    public void CinfCommand_serializes_expected_amcp_command()
    {
        var command = new CinfCommand();

        Assert.Equal("CINF\r\n", command.Serialize());
    }

    [Fact]
    public void ClsCommand_serializes_expected_amcp_command()
    {
        var command = new ClsCommand();

        Assert.Equal("CLS\r\n", command.Serialize());
    }

    [Fact]
    public void FlsCommand_serializes_expected_amcp_command()
    {
        var command = new FlsCommand();

        Assert.Equal("FLS\r\n", command.Serialize());
    }

    [Fact]
    public void TlsCommand_serializes_expected_amcp_command()
    {
        var command = new TlsCommand();

        Assert.Equal("TLS\r\n", command.Serialize());
    }

    [Fact]
    public void GlInfoCommand_serializes_expected_amcp_command()
    {
        var command = new GlInfoCommand();

        Assert.Equal("GL INFO\r\n", command.Serialize());
    }

    [Fact]
    public void GlGcCommand_serializes_expected_amcp_command()
    {
        var command = new GlGcCommand();

        Assert.Equal("GL GC\r\n", command.Serialize());
    }

    [Fact]
    public void DataStoreCommand_serializes_expected_amcp_command()
    {
        var command = new DataStoreCommand("foo", "bar");

        Assert.Equal("DATA STORE foo bar\r\n", command.Serialize());
    }

    [Fact]
    public void CgUpdateCommand_serializes_expected_amcp_command()
    {
        var command = new CgUpdateCommand(1, 10, "{xml}");

        Assert.Equal("CG UPDATE 1-10 {xml}\r\n", command.Serialize());
    }

    [Fact]
    public void MixerVolumeCommand_serializes_expected_amcp_command()
    {
        var command = new MixerVolumeCommand(1, 10, 0.5);

        Assert.Equal("MIXER VOLUME 1-10 0.5\r\n", command.Serialize());
    }

    [Fact]
    public void ThumbnailGenerateAllCommand_serializes_expected_amcp_command()
    {
        var command = new ThumbnailGenerateAllCommand();

        Assert.Equal("THUMBNAIL GENERATE_ALL\r\n", command.Serialize());
    }

    [Fact]
    public void OscUnsubscribeCommand_serializes_expected_amcp_command()
    {
        var command = new OscUnsubscribeCommand(5253);

        Assert.Equal("OSC UNSUBSCRIBE 5253\r\n", command.Serialize());
    }

    [Fact]
    public void KillCommand_serializes_expected_amcp_command()
    {
        var command = new KillCommand();

        Assert.Equal("KILL\r\n", command.Serialize());
    }

    [Fact]
    public void LogLevelCommand_serializes_expected_amcp_command_without_level()
    {
        var command = new LogLevelCommand();

        Assert.Equal("LOG LEVEL\r\n", command.Serialize());
    }

    [Fact]
    public void LogLevelCommand_serializes_expected_amcp_command_with_level()
    {
        var command = new LogLevelCommand("debug");

        Assert.Equal("LOG LEVEL debug\r\n", command.Serialize());
    }

    [Fact]
    public void LockAcquireCommand_serializes_expected_amcp_command()
    {
        var command = new LockAcquireCommand(1, "phrase");

        Assert.Equal("LOCK 1 ACQUIRE phrase\r\n", command.Serialize());
    }

    [Fact]
    public void LockReleaseCommand_serializes_expected_amcp_command()
    {
        var command = new LockReleaseCommand(1);

        Assert.Equal("LOCK 1 RELEASE\r\n", command.Serialize());
    }

    [Fact]
    public void LockClearCommand_serializes_expected_amcp_command_without_override_phrase()
    {
        var command = new LockClearCommand(1);

        Assert.Equal("LOCK 1 CLEAR\r\n", command.Serialize());
    }

    [Fact]
    public void LockClearCommand_serializes_expected_amcp_command_with_override_phrase()
    {
        var command = new LockClearCommand(1, "override");

        Assert.Equal("LOCK 1 CLEAR override\r\n", command.Serialize());
    }

    [Fact]
    public async Task GetLogLevelAsync_serializes_expected_amcp_command_and_returns_current_level()
    {
        var transport = new RecordingAmcpTransport("201 LOG OK\r\nINFO\r\n");
        var client = new CasparClient(transport);

        var logLevel = await client.GetLogLevelAsync(CancellationToken.None);

        Assert.Equal("LOG LEVEL\r\n", transport.LastCommandText);
        Assert.Equal("INFO", logLevel);
    }

    [Fact]
    public async Task SetLogLevelAsync_serializes_expected_amcp_command()
    {
        var transport = new RecordingAmcpTransport("202 LOG OK\r\n");
        var client = new CasparClient(transport);

        await client.SetLogLevelAsync("debug", CancellationToken.None);

        Assert.Equal("LOG LEVEL debug\r\n", transport.LastCommandText);
    }

    [Fact]
    public async Task PlayAsync_serializes_expected_amcp_command()
    {
        var transport = new RecordingAmcpTransport();
        var client = new CasparClient(transport);

        await client.PlayAsync(1, 10, "AMB", CancellationToken.None);

        Assert.Equal("PLAY 1-10 AMB\r\n", transport.LastCommandText);
    }

    [Fact]
    public async Task LoadBackgroundAsync_serializes_expected_amcp_command()
    {
        var transport = new RecordingAmcpTransport();
        var client = new CasparClient(transport);

        await client.LoadBackgroundAsync(1, 10, "AMB", CancellationToken.None);

        Assert.Equal("LOADBG 1-10 AMB\r\n", transport.LastCommandText);
    }

    [Fact]
    public async Task StopAsync_serializes_expected_amcp_command()
    {
        var transport = new RecordingAmcpTransport();
        var client = new CasparClient(transport);

        await client.StopAsync(1, 10, CancellationToken.None);

        Assert.Equal("STOP 1-10\r\n", transport.LastCommandText);
    }

    [Fact]
    public async Task GetVersionAsync_serializes_expected_amcp_command_and_returns_version()
    {
        var transport = new RecordingAmcpTransport("201 VERSION OK\r\n2.4.1 Stable\r\n");
        var client = new CasparClient(transport);

        var version = await client.GetVersionAsync(CancellationToken.None);

        Assert.Equal("VERSION SERVER\r\n", transport.LastCommandText);
        Assert.Equal("2.4.1 Stable", version);
    }

    [Fact]
    public async Task GetMediaFilesAsync_serializes_expected_amcp_command_and_returns_media_records()
    {
        var transport = new RecordingAmcpTransport(
            "200 CLS OK\r\n" +
            "\"AMB\" MOVIE 42 20240101120000 240 1/25\r\n" +
            "\"PROMO\" MOVIE 13 20240101120001 120 1/25\r\n" +
            "\r\n");
        var client = new CasparClient(transport);

        var mediaFiles = await client.GetMediaFilesAsync(CancellationToken.None);

        Assert.Equal("CLS\r\n", transport.LastCommandText);
        Assert.Equal(2, mediaFiles.Count);
        Assert.Equal("AMB", mediaFiles[0].Name);
        Assert.Equal(MediaFileKind.Movie, mediaFiles[0].Kind);
        Assert.Equal("PROMO", mediaFiles[1].Name);
        Assert.Equal(MediaFileKind.Movie, mediaFiles[1].Kind);
    }

    [Fact]
    public async Task MediaInfoAsync_serializes_expected_amcp_command_and_returns_media_info()
    {
        var transport = new RecordingAmcpTransport(
            "200 CINF OK\r\n" +
            "\"AMB\" MOVIE 42 20240101120000 240 1/25 FIELD_A VALUE_A\r\n\r\n");
        var client = new CasparClient(transport);

        var mediaInfo = await client.MediaInfoAsync("AMB", CancellationToken.None);

        Assert.Equal("CINF AMB\r\n", transport.LastCommandText);
        Assert.Equal("AMB", mediaInfo.Name);
        Assert.Equal("VALUE_A", mediaInfo.Properties["FIELD_A"]);
    }

    [Fact]
    public async Task GetTemplateFilesAsync_serializes_expected_amcp_command_and_returns_template_records()
    {
        var transport = new RecordingAmcpTransport("200 TLS OK\r\nLOWERTHIRD\r\nFULLFRAME\r\n\r\n");
        var client = new CasparClient(transport);

        var templates = await client.GetTemplateFilesAsync(null, CancellationToken.None);

        Assert.Equal("TLS\r\n", transport.LastCommandText);
        Assert.Equal(["LOWERTHIRD", "FULLFRAME"], templates.Select(x => x.Name).ToArray());
    }

    [Fact]
    public async Task GetFontFilesAsync_serializes_expected_amcp_command_and_returns_font_records()
    {
        var transport = new RecordingAmcpTransport("200 FLS OK\r\n\"Roboto\" fonts/roboto.ttf\r\n\r\n");
        var client = new CasparClient(transport);

        var fonts = await client.GetFontFilesAsync(CancellationToken.None);

        Assert.Equal("FLS\r\n", transport.LastCommandText);
        Assert.Equal("Roboto", fonts.Single().Name);
        Assert.Equal("fonts/roboto.ttf", fonts.Single().Path);
    }

    [Fact]
    public async Task InfoPathsAsync_serializes_expected_amcp_command_and_returns_query_data_map()
    {
        var transport = new RecordingAmcpTransport(
            "200 INFO PATHS OK\r\n<paths><media-path>media/</media-path></paths>\r\n\r\n");
        var client = new CasparClient(transport);

        var infoPaths = await client.InfoPathsAsync(CancellationToken.None);

        Assert.Equal("INFO PATHS\r\n", transport.LastCommandText);
        Assert.Equal("media/", infoPaths.Values["paths.media-path"]);
    }

    [Fact]
    public async Task SubscribeOscAsync_serializes_expected_amcp_command()
    {
        var transport = new RecordingAmcpTransport("202 OSC SUBSCRIBE OK\r\n");
        var client = new CasparClient(transport);

        await client.SubscribeOscAsync(6250, CancellationToken.None);

        Assert.Equal("OSC SUBSCRIBE 6250\r\n", transport.LastCommandText);
    }

    [Fact]
    public async Task OscUnsubscribeAsync_serializes_expected_amcp_command()
    {
        var transport = new RecordingAmcpTransport("202 OSC UNSUBSCRIBE OK\r\n");
        var client = new CasparClient(transport);

        await client.OscUnsubscribeAsync(5253, CancellationToken.None);

        Assert.Equal("OSC UNSUBSCRIBE 5253\r\n", transport.LastCommandText);
    }

    [Fact]
    public async Task PlayAsync_throws_when_server_returns_error_response()
    {
        var transport = new RecordingAmcpTransport("404 PLAY FAILED\r\n");
        var client = new CasparClient(transport);

        var exception = await Assert.ThrowsAsync<AmcpCommandException>(
            () => client.PlayAsync(1, 10, "AMB", CancellationToken.None).AsTask());

        Assert.Equal(404, exception.Response.StatusCode);
        Assert.Equal("PLAY FAILED", exception.Response.CommandText);
    }

    private sealed class RecordingAmcpTransport : IAmcpTransport
    {
        private readonly string _responseText;

        public RecordingAmcpTransport(string responseText = "202 PLAY OK\r\n")
        {
            _responseText = responseText;
        }

        public string? LastCommandText { get; private set; }

        public ValueTask ConnectAsync(CancellationToken cancellationToken = default) => ValueTask.CompletedTask;

        public ValueTask DisconnectAsync(CancellationToken cancellationToken = default) => ValueTask.CompletedTask;

        public ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken = default)
        {
            LastCommandText = commandText;
            return ValueTask.FromResult(_responseText);
        }
    }
}
