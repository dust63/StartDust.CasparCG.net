namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents a request to update a layer property.
/// </summary>
/// <param name="Key">The property name.</param>
/// <param name="Value">The property value.</param>
public sealed record SetRequest(string Key, string Value);
