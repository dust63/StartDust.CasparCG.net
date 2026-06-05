using System.Net.Sockets;

namespace StarDust.CasparCG.Transport;

/// <summary>
/// Receives OSC datagrams over UDP.
/// </summary>
public sealed class UdpOscTransport : IOscTransport
{
    private UdpClient? _client;
    private Task? _receiveLoop;
    private CancellationTokenSource? _shutdown;

    /// <inheritdoc />
    public ValueTask StartAsync(int port, Func<ReadOnlyMemory<byte>, CancellationToken, ValueTask> packetHandler, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(packetHandler);
        if (_client is not null)
        {
            throw new InvalidOperationException("The OSC transport is already running.");
        }

        _client = new UdpClient(port);
        _shutdown = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _receiveLoop = ReceiveLoopAsync(packetHandler, _shutdown.Token);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public async ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        if (_client is null)
        {
            return;
        }

        _shutdown?.Cancel();
        _client.Dispose();
        _client = null;

        if (_receiveLoop is not null)
        {
            await _receiveLoop.WaitAsync(cancellationToken);
        }

        _shutdown?.Dispose();
        _shutdown = null;
        _receiveLoop = null;
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync() => StopAsync(CancellationToken.None);

    private async Task ReceiveLoopAsync(Func<ReadOnlyMemory<byte>, CancellationToken, ValueTask> packetHandler, CancellationToken cancellationToken = default)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var result = await _client!.ReceiveAsync(cancellationToken);
                try
                {
                    await packetHandler(result.Buffer, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception)
                {
                    // Ignore malformed or unexpected OSC packets so the listener keeps running.
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (ObjectDisposedException)
        {
        }
    }
}
