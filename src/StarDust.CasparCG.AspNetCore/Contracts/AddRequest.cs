namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents a request to add a channel consumer.
/// </summary>
/// <param name="Consumer">The consumer identifier.</param>
/// <param name="Arguments">The optional raw consumer arguments.</param>
/// <param name="ConsumerIndex">The optional consumer index override.</param>
public sealed record AddRequest(string Consumer, string? Arguments = null, int? ConsumerIndex = null);
