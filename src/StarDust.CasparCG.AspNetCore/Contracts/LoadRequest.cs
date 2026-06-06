using StarDust.CasparCG.Protocol.Amcp;

namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents a request to load a clip on a channel layer.
/// </summary>
/// <param name="Clip">The clip identifier.</param>
/// <param name="Options">The optional typed playback options.</param>
public sealed record LoadRequest(string Clip, PlaybackOptions? Options = null);
