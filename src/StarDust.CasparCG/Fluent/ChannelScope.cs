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
    /// Sends a CHANNEL_GRID command for this channel.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask GridAsync(CancellationToken cancellationToken = default) =>
        client.ChannelGridAsync(channel, cancellationToken);
}
