namespace StarDust.CasparCG.Fluent;

/// <summary>
/// Represents a fluent layer scope for command composition.
/// </summary>
public sealed class LayerScope
{
    private readonly CasparClient _client;
    private readonly int _channel;
    private readonly int _layer;

    /// <summary>
    /// Initializes a new layer scope.
    /// </summary>
    /// <param name="client">The owning client.</param>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    public LayerScope(CasparClient client, int channel, int layer)
    {
        _client = client;
        _channel = channel;
        _layer = layer;
    }

    /// <summary>
    /// Starts building a fluent play command.
    /// </summary>
    /// <param name="clip">The clip identifier.</param>
    /// <returns>A play command builder.</returns>
    public PlayCommandBuilder Play(string clip) => new(_client, _channel, _layer, clip);
}
