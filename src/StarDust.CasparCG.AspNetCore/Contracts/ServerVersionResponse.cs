namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents the server version returned by the REST API.
/// </summary>
/// <param name="Version">The CasparCG server version string.</param>
public sealed record ServerVersionResponse(string Version);
