namespace StarDust.CasparCG.Events;

/// <summary>
/// Represents a change to a layer producer slot.
/// </summary>
/// <param name="ClientName">The client registration name.</param>
/// <param name="Channel">The affected channel.</param>
/// <param name="Layer">The affected layer.</param>
/// <param name="Slot">The affected slot, usually foreground or background.</param>
/// <param name="Producer">The producer name.</param>
public sealed record LayerProducerChangedEvent(
    string ClientName,
    int Channel,
    int Layer,
    string Slot,
    string Producer)
    : CasparEvent(ClientName, Channel, Layer);

