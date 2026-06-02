namespace StarDust.CasparCG.Transport;

/// <summary>
/// Receives OSC datagrams from a CasparCG server.
/// </summary>
public interface IOscTransport : IAsyncDisposable
{
    /// <summary>
    /// Starts listening on the specified UDP port.
    /// </summary>
    /// <param name="port">The UDP port to bind.</param>
    /// <param name="packetHandler">The callback that receives raw OSC packets.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the start operation.</returns>
    ValueTask StartAsync(int port, Func<ReadOnlyMemory<byte>, CancellationToken, ValueTask> packetHandler, CancellationToken cancellationToken);

    /// <summary>
    /// Stops listening for OSC datagrams.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the stop operation.</returns>
    ValueTask StopAsync(CancellationToken cancellationToken);
}
