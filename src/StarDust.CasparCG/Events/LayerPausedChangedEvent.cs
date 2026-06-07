namespace StarDust.CasparCG.Events;

/// <summary>
/// Represents a change to a layer pause state.
/// </summary>
/// <param name="ClientName">The client registration name.</param>
/// <param name="Channel">The affected channel.</param>
/// <param name="Layer">The affected layer.</param>
/// <param name="Paused">A value indicating whether playback is paused.</param>
public sealed record LayerPausedChangedEvent(
    string ClientName,
    int Channel,
    int Layer,
    bool Paused)
    : CasparEvent(ClientName, Channel, Layer);

