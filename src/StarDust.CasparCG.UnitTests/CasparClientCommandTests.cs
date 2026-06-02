using StarDust.CasparCG;
using StarDust.CasparCG.Protocol.Amcp;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class CasparClientCommandTests
{
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
