namespace StarDust.CasparCG.Protocol.Amcp;

/// <summary>
/// Categorizes AMCP status codes by protocol family.
/// </summary>
public enum AmcpStatusCategory
{
    /// <summary>
    /// The status code does not map to a known family.
    /// </summary>
    Unknown,

    /// <summary>
    /// Informational response.
    /// </summary>
    Information,

    /// <summary>
    /// Successful response.
    /// </summary>
    Success,

    /// <summary>
    /// Client-side command or usage error.
    /// </summary>
    ClientError,

    /// <summary>
    /// Server-side failure.
    /// </summary>
    ServerError
}
