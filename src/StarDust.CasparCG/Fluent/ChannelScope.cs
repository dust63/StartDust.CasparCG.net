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
}
