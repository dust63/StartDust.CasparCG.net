namespace StarDust.CasparCG.Events;

/// <summary>
/// Represents a change to the active clip on a channel layer.
/// </summary>
/// <param name="ClientName">The client registration name.</param>
/// <param name="Channel">The affected channel.</param>
/// <param name="Layer">The affected layer.</param>
/// <param name="Clip">The active clip identifier.</param>
public sealed record PlaybackClipChangedEvent(string ClientName, int Channel, int Layer, string Clip)
    : CasparEvent(ClientName, Channel, Layer);
