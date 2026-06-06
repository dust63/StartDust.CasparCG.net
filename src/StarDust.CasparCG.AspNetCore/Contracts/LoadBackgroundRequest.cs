namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents a request to load a background clip.
/// </summary>
/// <param name="Clip">The clip identifier.</param>
/// <param name="Loop">Whether the clip should loop.</param>
/// <param name="AutoPlay">Whether the clip should auto-play after loading.</param>
public sealed record LoadBackgroundRequest(
    string Clip,
    bool Loop,
    bool AutoPlay);
