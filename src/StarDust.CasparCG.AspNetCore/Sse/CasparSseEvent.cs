namespace StarDust.CasparCG.AspNetCore.Sse;

/// <summary>
/// Represents the payload envelope written to the SSE stream.
/// </summary>
/// <param name="Type">The normalized event type.</param>
/// <param name="Timestamp">The event timestamp.</param>
/// <param name="Target">The logical client target.</param>
/// <param name="Payload">The original event payload.</param>
public sealed record CasparSseEvent(
    string Type,
    DateTimeOffset Timestamp,
    string Target,
    object Payload);
