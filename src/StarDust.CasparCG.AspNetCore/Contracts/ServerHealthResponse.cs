using StarDust.CasparCG.Health;

namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents the health state returned by the REST API for a CasparCG server.
/// </summary>
/// <param name="ServerName">The resolved server name.</param>
/// <param name="Status">The connection status.</param>
/// <param name="Connected">Whether the client is connected.</param>
/// <param name="LastSuccessfulAmcpInteraction">The last successful AMCP interaction time, if any.</param>
/// <param name="LastFailure">The last observed failure message, if any.</param>
/// <param name="ReconnectCount">The number of reconnect attempts recorded.</param>
public sealed record ServerHealthResponse(
    string ServerName,
    ConnectionHealthStatus Status,
    bool Connected,
    DateTimeOffset? LastSuccessfulAmcpInteraction,
    string? LastFailure,
    int ReconnectCount);
