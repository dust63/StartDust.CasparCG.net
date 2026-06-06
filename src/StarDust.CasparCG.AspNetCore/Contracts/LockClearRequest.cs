namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents a lock clear request.
/// </summary>
/// <param name="OverridePhrase">The optional override phrase.</param>
public sealed record LockClearRequest(string? OverridePhrase);
