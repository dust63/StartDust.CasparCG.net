namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents a raw CALL or CALLBG parameter request.
/// </summary>
/// <param name="Arguments">The raw parameter string.</param>
public sealed record CallRequest(string Arguments);
