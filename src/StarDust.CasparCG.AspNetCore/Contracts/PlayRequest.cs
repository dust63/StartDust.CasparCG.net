namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents a request to play a clip on a channel layer.
/// </summary>
/// <param name="Clip">The clip identifier.</param>
/// <param name="Loop">Whether playback should loop.</param>
/// <param name="TransitionDuration">The optional mix transition duration.</param>
public sealed record PlayRequest(
    string Clip,
    bool Loop,
    int? TransitionDuration);
