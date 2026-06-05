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
public sealed record LayerStateSnapshot(string Clip);
