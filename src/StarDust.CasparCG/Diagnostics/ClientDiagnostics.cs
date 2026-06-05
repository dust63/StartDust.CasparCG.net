namespace StarDust.CasparCG.Diagnostics;

/// <summary>
/// Stores diagnostic information about recent client activity.
/// </summary>
public sealed class ClientDiagnostics
{
    /// <summary>
    /// Gets or sets the time of the last successful AMCP interaction.
    /// </summary>
    public DateTimeOffset? LastSuccessfulAmcpInteraction { get; set; }

    /// <summary>
    /// Gets or sets the last observed failure.
    /// </summary>
    public Exception? LastFailure { get; set; }

    /// <summary>
    /// Gets or sets the number of reconnect attempts recorded.
    /// </summary>
    public int ReconnectCount { get; set; }
}
