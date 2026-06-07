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
            UpdateLayer(clipChanged.Channel, clipChanged.Layer, layer => layer with { Clip = clipChanged.Clip });
            return;
        }

        if (evt is LayerProducerChangedEvent producerChanged)
        {
            UpdateLayer(
                producerChanged.Channel,
                producerChanged.Layer,
                layer => layer with { Producer = producerChanged.Producer });
            return;
        }

        if (evt is LayerPausedChangedEvent pausedChanged)
        {
            UpdateLayer(pausedChanged.Channel, pausedChanged.Layer, layer => layer with { Paused = pausedChanged.Paused });
            return;
        }

        if (evt is LayerProgressChangedEvent progressChanged)
        {
            UpdateLayer(
                progressChanged.Channel,
                progressChanged.Layer,
                layer => layer with
                {
                    PositionSeconds = progressChanged.PositionSeconds,
                    DurationSeconds = progressChanged.DurationSeconds
                });
            return;
        }

        if (evt is LayerFramesLeftChangedEvent framesLeftChanged)
        {
            UpdateLayer(
                framesLeftChanged.Channel,
                framesLeftChanged.Layer,
                layer => layer with { FramesLeft = framesLeftChanged.FramesLeft });
        }
    }

    /// <summary>
    /// Creates an immutable snapshot of the current state.
    /// </summary>
    /// <returns>The current state snapshot.</returns>
    public CasparStateSnapshot GetSnapshot() =>
        new(_channels.ToDictionary(x => x.Key, x => new ChannelStateSnapshot(x.Value)));

    private void UpdateLayer(int channel, int layer, Func<LayerStateSnapshot, LayerStateSnapshot> update)
    {
        var layers = _channels.TryGetValue(channel, out var existing)
            ? existing
            : _channels[channel] = new Dictionary<int, LayerStateSnapshot>();

        var current = layers.TryGetValue(layer, out var snapshot)
            ? snapshot
            : new LayerStateSnapshot();

        layers[layer] = update(current);
    }
}
