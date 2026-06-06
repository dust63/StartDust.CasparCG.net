namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents a request to apply a producer or consumer call.
/// </summary>
/// <param name="Layer">The optional target layer.</param>
/// <param name="Arguments">The raw argument tail.</param>
public sealed record ApplyRequest(int? Layer, string Arguments);
