using StarDust.CasparCG.Transport;

namespace StarDust.CasparCG.Hosting;

internal sealed class NoOpAmcpTransport : IAmcpTransport
{
    public ValueTask ConnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

    public ValueTask DisconnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

    public ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken) =>
        ValueTask.FromResult(string.Empty);
}
