namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents a raw AMCP argument tail.
/// </summary>
/// <param name="Arguments">The raw argument string.</param>
public sealed record ArgumentsRequest(string? Arguments);
