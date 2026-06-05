using StarDust.CasparCG;
using StarDust.CasparCG.Health;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class HealthAndReconnectTests
{
    [Fact]
    public async Task ConnectAsync_sets_health_to_connected()
    {
        var transport = new ToggleTransport();
        var client = new CasparClient(transport);

        await client.ConnectAsync(CancellationToken.None);

        Assert.Equal(ConnectionHealthStatus.Connected, client.HealthStatus);
    }

    [Fact]
    public async Task Failed_send_records_reconnect_attempt_and_fault_reason()
    {
        var transport = new ToggleTransport(failSend: true);
        var client = new CasparClient(transport);

        await Assert.ThrowsAsync<InvalidOperationException>(() => client.PlayAsync(1, 10, "AMB", CancellationToken.None).AsTask());

        Assert.Equal(ConnectionHealthStatus.Faulted, client.HealthStatus);
        Assert.Equal(1, client.Diagnostics.ReconnectCount);
        Assert.NotNull(client.Diagnostics.LastFailure);
    }

    private sealed class ToggleTransport : IAmcpTransport
    {
        private readonly bool _failSend;

        public ToggleTransport(bool failSend = false)
        {
            _failSend = failSend;
        }

        public ValueTask ConnectAsync(CancellationToken cancellationToken = default) => ValueTask.CompletedTask;

        public ValueTask DisconnectAsync(CancellationToken cancellationToken = default) => ValueTask.CompletedTask;

        public ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken = default) =>
            _failSend
                ? ValueTask.FromException<string>(new InvalidOperationException("send failed"))
                : ValueTask.FromResult("202 OK");
    }
}
