using StarDust.CasparCG;
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

    private sealed class RecordingAmcpTransport : IAmcpTransport
    {
        public string? LastCommandText { get; private set; }

        public ValueTask ConnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

        public ValueTask DisconnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

        public ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken)
        {
            LastCommandText = commandText;
            return ValueTask.FromResult("202 PLAY OK");
        }
    }
}
