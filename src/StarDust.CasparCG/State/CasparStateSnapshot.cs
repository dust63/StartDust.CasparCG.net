namespace StarDust.CasparCG.State;

/// <summary>
/// Represents a snapshot of known CasparCG state.
/// </summary>
/// <param name="Channels">The channels in the snapshot.</param>
public sealed record CasparStateSnapshot(IReadOnlyDictionary<int, ChannelStateSnapshot> Channels);

/// <summary>
/// Represents a snapshot of a single channel.
/// </summary>
/// <param name="Layers">The layers in the channel.</param>
public sealed record ChannelStateSnapshot(IReadOnlyDictionary<int, LayerStateSnapshot> Layers);

/// <summary>
/// Represents a snapshot of a single layer.
/// </summary>
/// <param name="Clip">The active clip.</param>
/// <param name="Producer">The active producer name.</param>
/// <param name="Paused">The current pause state.</param>
/// <param name="PositionSeconds">The current media position in seconds.</param>
/// <param name="DurationSeconds">The current media duration in seconds.</param>
/// <param name="FramesLeft">The remaining frame count before autoplay.</param>
public sealed record LayerStateSnapshot(
    string? Clip = null,
    string? Producer = null,
    bool? Paused = null,
    double? PositionSeconds = null,
    double? DurationSeconds = null,
    long? FramesLeft = null);
