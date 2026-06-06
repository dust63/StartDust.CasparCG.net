using StarDust.CasparCG.Protocol.Amcp;

namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents a request to load a background clip.
/// </summary>
/// <param name="Clip">The clip identifier.</param>
/// <param name="Options">The optional typed playback options.</param>
public sealed record LoadBackgroundRequest(string Clip, LoadBackgroundOptions? Options = null);
