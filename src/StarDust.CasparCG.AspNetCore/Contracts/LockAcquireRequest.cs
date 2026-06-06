namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents a lock acquire request.
/// </summary>
/// <param name="Phrase">The lock phrase.</param>
public sealed record LockAcquireRequest(string Phrase);
