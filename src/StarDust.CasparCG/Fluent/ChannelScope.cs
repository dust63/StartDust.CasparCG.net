namespace StarDust.CasparCG.Fluent;

/// <summary>
/// Represents a fluent channel scope for command composition.
/// </summary>
public sealed class ChannelScope(CasparClient client, int channel)
{
    /// <summary>
    /// Selects the target layer within the channel.
    /// </summary>
    /// <param name="layer">The target layer.</param>
    /// <returns>A fluent layer scope.</returns>
    public LayerScope Layer(int layer) => new(client, channel, layer);

    /// <summary>
    /// Sends an ADD command for this channel.
    /// </summary>
    /// <param name="consumer">The consumer identifier.</param>
    /// <param name="arguments">The optional raw consumer arguments.</param>
    /// <param name="consumerIndex">The optional consumer index override.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask AddAsync(string consumer, string? arguments = null, int? consumerIndex = null, CancellationToken cancellationToken = default) =>
        client.AddAsync(channel, consumer, arguments, consumerIndex, cancellationToken);

    /// <summary>
    /// Sends a REMOVE command for this channel.
    /// </summary>
    /// <param name="consumerIndex">The consumer index override.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask RemoveAsync(int consumerIndex, CancellationToken cancellationToken = default) =>
        client.RemoveAsync(channel, consumerIndex, cancellationToken);

    /// <summary>
    /// Sends a REMOVE command for this channel.
    /// </summary>
    /// <param name="arguments">The raw consumer arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask RemoveAsync(string arguments, CancellationToken cancellationToken = default) =>
        client.RemoveAsync(channel, arguments, cancellationToken);

    /// <summary>
    /// Sends an APPLY command for this channel.
    /// </summary>
    /// <param name="layer">The optional target layer.</param>
    /// <param name="arguments">The raw argument tail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ApplyAsync(int? layer, string arguments, CancellationToken cancellationToken = default) =>
        client.ApplyAsync(channel, layer, arguments, cancellationToken);

    /// <summary>
    /// Sends a PRINT command for this channel.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask PrintAsync(CancellationToken cancellationToken = default) =>
        client.PrintAsync(channel, cancellationToken);

    /// <summary>
    /// Sends a SET command for this channel.
    /// </summary>
    /// <param name="key">The property name.</param>
    /// <param name="value">The property value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask SetAsync(string key, string value, CancellationToken cancellationToken = default) =>
        client.SetAsync(channel, key, value, cancellationToken);

    /// <summary>
    /// Sends a CHANNEL_GRID command for this channel.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask GridAsync(CancellationToken cancellationToken = default) =>
        client.ChannelGridAsync(channel, cancellationToken);
}
