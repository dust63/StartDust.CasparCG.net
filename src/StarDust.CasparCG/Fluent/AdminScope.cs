namespace StarDust.CasparCG.Fluent;

/// <summary>
/// Provides fluent access to administrative AMCP commands.
/// </summary>
/// <param name="client">The client used to send commands.</param>
public sealed class AdminScope(CasparClient client)
{
    /// <summary>
    /// Sends a BYE command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ByeAsync(CancellationToken cancellationToken) =>
        client.ByeAsync(cancellationToken);

    /// <summary>
    /// Sends a KILL command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask KillAsync(CancellationToken cancellationToken) =>
        client.KillAsync(cancellationToken);

    /// <summary>
    /// Sends a RESTART command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask RestartAsync(CancellationToken cancellationToken) =>
        client.RestartAsync(cancellationToken);

    /// <summary>
    /// Sends a LOCK command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask LockAsync(CancellationToken cancellationToken) =>
        client.LockAsync(cancellationToken);
}
