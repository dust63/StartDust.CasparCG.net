namespace StarDust.CasparCG.Fluent;

/// <summary>
/// Represents a fluent layer scope for command composition.
/// </summary>
public sealed class LayerScope(CasparClient client, int channel, int layer)
{
    /// <summary>
    /// Starts building a fluent play command.
    /// </summary>
    /// <param name="clip">The clip identifier.</param>
    /// <returns>A play command builder.</returns>
    public PlayCommandBuilder Play(string clip) => new(client, channel, layer, clip);
}
