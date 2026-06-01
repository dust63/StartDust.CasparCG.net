using StarDust.CasparCG;
using StarDust.CasparCG.Testing.DummyServer;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.IntegrationTests;

public class CasparClientDummyServerTests
{
    [Fact]
    public async Task PlayAsync_sends_real_tcp_command_to_dummy_server()
    {
        await using var server = await DummyCasparServer.StartAsync(
            DummyScenario.Empty().WithAmcpReply("PLAY 1-10 AMB", "202 PLAY OK\r\n"),
            CancellationToken.None);

        var client = new CasparClient(new TcpAmcpTransport("127.0.0.1", server.AmcpPort));
        await client.ConnectAsync(CancellationToken.None);
        await client.PlayAsync(1, 10, "AMB", CancellationToken.None);

        Assert.Contains("PLAY 1-10 AMB", server.ReceivedCommands);
    }
}
