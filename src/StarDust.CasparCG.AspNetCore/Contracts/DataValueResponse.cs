namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents a data payload read through the REST API.
/// </summary>
/// <param name="Key">The data key.</param>
/// <param name="Lines">The raw data payload lines.</param>
public sealed record DataValueResponse(string Key, IReadOnlyList<string> Lines);
