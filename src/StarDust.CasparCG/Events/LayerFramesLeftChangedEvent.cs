namespace StarDust.CasparCG.Events;

/// <summary>
/// Represents a change to the number of frames left before autoplay.
/// </summary>
/// <param name="ClientName">The client registration name.</param>
/// <param name="Channel">The affected channel.</param>
/// <param name="Layer">The affected layer.</param>
/// <param name="FramesLeft">The remaining frame count.</param>
public sealed record LayerFramesLeftChangedEvent(
    string ClientName,
    int Channel,
    int Layer,
    long FramesLeft)
    : CasparEvent(ClientName, Channel, Layer);

