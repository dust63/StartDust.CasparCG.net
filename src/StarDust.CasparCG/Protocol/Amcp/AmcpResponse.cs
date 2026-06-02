namespace StarDust.CasparCG.Protocol.Amcp;

/// <summary>
/// Represents a parsed AMCP response block.
/// </summary>
public sealed class AmcpResponse
{
    /// <summary>
    /// Gets the numeric AMCP status code.
    /// </summary>
    public required int StatusCode { get; init; }

    /// <summary>
    /// Gets the status category derived from the status code.
    /// </summary>
    public required AmcpStatusCategory Category { get; init; }

    /// <summary>
    /// Gets the command text associated with the status line.
    /// </summary>
    public required string CommandText { get; init; }

    /// <summary>
    /// Gets the full AMCP status line without the terminating CRLF.
    /// </summary>
    public required string StatusLine { get; init; }

    /// <summary>
    /// Gets the payload lines returned after the status line.
    /// </summary>
    public required IReadOnlyList<string> Lines { get; init; }

    /// <summary>
    /// Gets the raw AMCP block exactly as received from the server.
    /// </summary>
    public required string Raw { get; init; }

    /// <summary>
    /// Gets a value indicating whether the response is successful.
    /// </summary>
    public bool IsSuccess => Category == AmcpStatusCategory.Success;
}
