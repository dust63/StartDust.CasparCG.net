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
