using StarDust.CasparCG.Transport;

namespace StarDust.CasparCG.Hosting;

internal sealed class NoOpAmcpTransport : IAmcpTransport
{
    public ValueTask ConnectAsync(CancellationToken cancellationToken = default) => ValueTask.CompletedTask;

    public ValueTask DisconnectAsync(CancellationToken cancellationToken = default) => ValueTask.CompletedTask;

    public ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken = default) =>
        ValueTask.FromResult(string.Empty);
}
