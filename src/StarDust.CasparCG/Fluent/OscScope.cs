namespace StarDust.CasparCG.Fluent;

/// <summary>
/// Provides fluent access to OSC operations.
/// </summary>
/// <param name="client">The client used to send commands.</param>
public sealed class OscScope(CasparClient client)
{
    /// <summary>
    /// Starts the OSC listener on the specified port.
    /// </summary>
    /// <param name="port">The OSC UDP port.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous start operation.</returns>
    public ValueTask StartAsync(int port, CancellationToken cancellationToken = default) =>
        client.StartOscAsync(port, cancellationToken);

    /// <summary>
    /// Subscribes the current AMCP session to OSC messages on the specified UDP port.
    /// </summary>
    /// <param name="port">The OSC UDP port.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous subscribe operation.</returns>
    public ValueTask SubscribeAsync(int port, CancellationToken cancellationToken = default) =>
        client.SubscribeOscAsync(port, cancellationToken);

    /// <summary>
    /// Unsubscribes the current AMCP session from OSC messages on the specified UDP port.
    /// </summary>
    /// <param name="port">The OSC UDP port.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous unsubscribe operation.</returns>
    public ValueTask UnsubscribeAsync(int port, CancellationToken cancellationToken = default) =>
        client.OscUnsubscribeAsync(port, cancellationToken);

    /// <summary>
    /// Stops the OSC listener.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous stop operation.</returns>
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        client.StopOscAsync(cancellationToken);
}
