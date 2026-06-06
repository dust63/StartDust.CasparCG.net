namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents a request to remove a channel consumer.
/// </summary>
/// <param name="Arguments">The optional raw consumer arguments.</param>
/// <param name="ConsumerIndex">The optional consumer index override.</param>
public sealed record RemoveRequest(string? Arguments = null, int? ConsumerIndex = null);
