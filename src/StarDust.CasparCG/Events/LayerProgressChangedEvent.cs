namespace StarDust.CasparCG.Events;

/// <summary>
/// Represents a change to layer media progress.
/// </summary>
/// <param name="ClientName">The client registration name.</param>
/// <param name="Channel">The affected channel.</param>
/// <param name="Layer">The affected layer.</param>
/// <param name="Slot">The affected slot, usually foreground or background.</param>
/// <param name="PositionSeconds">The current media position in seconds.</param>
/// <param name="DurationSeconds">The media duration in seconds.</param>
public sealed record LayerProgressChangedEvent(
    string ClientName,
    int Channel,
    int Layer,
    string Slot,
    double PositionSeconds,
    double DurationSeconds)
    : CasparEvent(ClientName, Channel, Layer);

