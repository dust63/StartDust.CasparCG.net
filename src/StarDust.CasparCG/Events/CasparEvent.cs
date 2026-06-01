namespace StarDust.CasparCG.Events;

/// <summary>
/// Represents a domain event emitted by a CasparCG client.
/// </summary>
/// <param name="ClientName">The client registration name.</param>
/// <param name="Channel">The affected channel.</param>
/// <param name="Layer">The affected layer.</param>
public abstract record CasparEvent(string ClientName, int Channel, int Layer);
