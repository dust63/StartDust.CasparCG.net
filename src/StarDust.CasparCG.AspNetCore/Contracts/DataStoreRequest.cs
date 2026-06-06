namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents a request to store a data value.
/// </summary>
/// <param name="Value">The payload value to store.</param>
public sealed record DataStoreRequest(string Value);
