namespace StarDust.CasparCG.Events;

/// <summary>
/// Represents a raw OSC monitor state change from a CasparCG channel.
/// </summary>
/// <param name="ClientName">The client registration name.</param>
/// <param name="Channel">The affected channel.</param>
/// <param name="Path">The OSC path relative to the channel root.</param>
/// <param name="Arguments">The OSC arguments.</param>
public sealed record OscStateChangedEvent(
    string ClientName,
    int Channel,
    string Path,
    IReadOnlyList<object?> Arguments)
    : CasparEvent(ClientName, Channel, Layer: 0);

