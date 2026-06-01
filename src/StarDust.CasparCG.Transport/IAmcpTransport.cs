namespace StarDust.CasparCG.Transport;

/// <summary>
/// Sends AMCP commands to CasparCG.
/// </summary>
public interface IAmcpTransport
{
    /// <summary>
    /// Connects the transport.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous connect operation.</returns>
    ValueTask ConnectAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Disconnects the transport.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous disconnect operation.</returns>
    ValueTask DisconnectAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Sends AMCP command text to the server.
    /// </summary>
    /// <param name="commandText">The command text to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The textual AMCP response.</returns>
    ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken);
}
