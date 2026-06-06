namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents a request to swap a layer with another layer.
/// </summary>
/// <param name="OtherChannel">The destination channel.</param>
/// <param name="OtherLayer">The destination layer.</param>
/// <param name="SwapTransforms">Whether mixer transforms should be swapped too.</param>
public sealed record SwapRequest(int OtherChannel, int OtherLayer, bool SwapTransforms = false);
