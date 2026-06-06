namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents a request to update layer opacity.
/// </summary>
/// <param name="Value">The opacity value.</param>
public sealed record MixerOpacityRequest(double Value);
