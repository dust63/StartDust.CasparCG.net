namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents a numeric value request.
/// </summary>
/// <param name="Value">The numeric value.</param>
public sealed record NumericValueRequest(double Value);
