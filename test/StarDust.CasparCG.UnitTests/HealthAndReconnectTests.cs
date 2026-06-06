using StarDust.CasparCG;
using StarDust.CasparCG.Health;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class HealthAndReconnectTests
{
    [Fact]
    public async Task First_command_lazily_connects_before_sending_amcp()
    {
        var transport = new TrackingTransport();
        var client = new CasparClient(transport);

        await client.Server().VersionAsync(CancellationToken.None);

        Assert.Equal(1, transport.ConnectCount);
        Assert.Equal(["VERSION SERVER"], transport.SentCommands);
        Assert.Equal(ConnectionHealthStatus.Connected, client.HealthStatus);
    }

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

    [Fact]
    public async Task DisconnectAsync_closes_transport_and_marks_client_disconnected()
    {
        var transport = new TrackingTransport();
        var client = new CasparClient(transport);

        await client.ConnectAsync();
        await client.DisconnectAsync();

        Assert.Equal(1, transport.ConnectCount);
        Assert.Equal(1, transport.DisconnectCount);
        Assert.Equal(ConnectionHealthStatus.Disconnected, client.HealthStatus);
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

    private sealed class TrackingTransport : IAmcpTransport
    {
        public int ConnectCount { get; private set; }

        public int DisconnectCount { get; private set; }

        public List<string> SentCommands { get; } = [];

        public ValueTask ConnectAsync(CancellationToken cancellationToken = default)
        {
            ConnectCount++;
            return ValueTask.CompletedTask;
        }

        public ValueTask DisconnectAsync(CancellationToken cancellationToken = default)
        {
            DisconnectCount++;
            return ValueTask.CompletedTask;
        }

        public ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken = default)
        {
            SentCommands.Add(commandText.Trim());
            return ValueTask.FromResult("201 VERSION OK\r\n2.6.0\r\n");
        }
    }
}
