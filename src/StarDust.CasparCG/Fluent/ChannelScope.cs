namespace StarDust.CasparCG.Fluent;

/// <summary>
/// Represents a fluent channel scope for command composition.
/// </summary>
public sealed class ChannelScope
{
    private readonly CasparClient _client;
    private readonly int _channel;

    /// <summary>
    /// Initializes a new channel scope.
    /// </summary>
    /// <param name="client">The owning client.</param>
    /// <param name="channel">The target channel.</param>
    public ChannelScope(CasparClient client, int channel)
    {
        _client = client;
        _channel = channel;
    }

    /// <summary>
    /// Selects the target layer within the channel.
    /// </summary>
    /// <param name="layer">The target layer.</param>
    /// <returns>A fluent layer scope.</returns>
    public LayerScope Layer(int layer) => new(_client, _channel, layer);
}
