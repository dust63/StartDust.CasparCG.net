using StarDust.CasparCG;
using StarDust.CasparCG.Protocol.Amcp;
using StarDust.CasparCG.Protocol.Amcp.Commands;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class CasparClientCommandTests
{
    [Fact]
    public void LoadCommand_serializes_expected_amcp_command()
    {
        var command = new LoadCommand(1, 10, "AMB");

        Assert.Equal("LOAD 1-10 AMB\r\n", command.Serialize());
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
    public void SwapCommand_serializes_expected_amcp_command()
    {
        var command = new SwapCommand(1, 10, 1, 11);

        Assert.Equal("SWAP 1-10 1-11\r\n", command.Serialize());
    }

    [Fact]
    public void AddCommand_serializes_expected_amcp_command()
    {
        var command = new AddCommand(1, 10, "AMB");

        Assert.Equal("ADD 1-10 AMB\r\n", command.Serialize());
    }

    [Fact]
    public void RemoveCommand_serializes_expected_amcp_command()
    {
        var command = new RemoveCommand(1, 10, "AMB");

        Assert.Equal("REMOVE 1-10 AMB\r\n", command.Serialize());
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
        var command = new PrintCommand(1, 10, "AMB");

        Assert.Equal("PRINT 1-10 AMB\r\n", command.Serialize());
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
        var command = new SetCommand(1, 10, "KEY", "VALUE");

        Assert.Equal("SET 1-10 KEY VALUE\r\n", command.Serialize());
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
    public async Task GetMediaFilesAsync_serializes_expected_amcp_command_and_returns_media_lines()
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
        Assert.Equal("\"AMB\" MOVIE 42 20240101120000 240 1/25", mediaFiles[0]);
        Assert.Equal("\"PROMO\" MOVIE 13 20240101120001 120 1/25", mediaFiles[1]);
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

        public ValueTask ConnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

        public ValueTask DisconnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

        public ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken)
        {
            LastCommandText = commandText;
            return ValueTask.FromResult(_responseText);
        }
    }
}
