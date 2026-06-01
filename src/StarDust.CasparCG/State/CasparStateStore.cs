using StarDust.CasparCG.Events;

namespace StarDust.CasparCG.State;

/// <summary>
/// Tracks the latest known CasparCG state derived from domain events.
/// </summary>
public sealed class CasparStateStore
{
    private readonly Dictionary<int, Dictionary<int, LayerStateSnapshot>> _channels = new();

    /// <summary>
    /// Applies an event to the state store.
    /// </summary>
    /// <param name="evt">The event to apply.</param>
    public void Apply(CasparEvent evt)
    {
        if (evt is PlaybackClipChangedEvent clipChanged)
        {
            var layers = _channels.TryGetValue(clipChanged.Channel, out var existing)
                ? existing
                : _channels[clipChanged.Channel] = new Dictionary<int, LayerStateSnapshot>();

            layers[clipChanged.Layer] = new LayerStateSnapshot(clipChanged.Clip);
        }
    }

    /// <summary>
    /// Creates an immutable snapshot of the current state.
    /// </summary>
    /// <returns>The current state snapshot.</returns>
    public CasparStateSnapshot GetSnapshot() =>
        new(_channels.ToDictionary(x => x.Key, x => new ChannelStateSnapshot(x.Value)));
}
