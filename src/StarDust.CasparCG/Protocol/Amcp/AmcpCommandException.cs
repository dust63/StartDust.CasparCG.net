namespace StarDust.CasparCG.Protocol.Amcp;

/// <summary>
/// Represents an AMCP command failure surfaced by a high-level API.
/// </summary>
public sealed class AmcpCommandException : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AmcpCommandException"/> class.
    /// </summary>
    /// <param name="response">The failed AMCP response.</param>
    public AmcpCommandException(AmcpResponse response)
        : base($"AMCP command failed with status code {response.StatusCode}: {response.StatusLine}")
    {
        Response = response;
    }

    /// <summary>
    /// Gets the failed AMCP response.
    /// </summary>
    public AmcpResponse Response { get; }
}
