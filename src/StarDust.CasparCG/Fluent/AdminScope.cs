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
    public ValueTask ByeAsync(CancellationToken cancellationToken = default) =>
        client.ByeAsync(cancellationToken);

    /// <summary>
    /// Sends a KILL command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask KillAsync(CancellationToken cancellationToken = default) =>
        client.KillAsync(cancellationToken);

    /// <summary>
    /// Sends a RESTART command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask RestartAsync(CancellationToken cancellationToken = default) =>
        client.RestartAsync(cancellationToken);

    /// <summary>
    /// Gets the current AMCP log level.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The current log level.</returns>
    public ValueTask<string> GetLogLevelAsync(CancellationToken cancellationToken = default) =>
        client.GetLogLevelAsync(cancellationToken);

    /// <summary>
    /// Sets the AMCP log level.
    /// </summary>
    /// <param name="level">The log level to apply.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask SetLogLevelAsync(string level, CancellationToken cancellationToken = default) =>
        client.SetLogLevelAsync(level, cancellationToken);

    /// <summary>
    /// Sends a LOCK ACQUIRE command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="phrase">The lock phrase.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask AcquireLockAsync(int channel, string phrase, CancellationToken cancellationToken = default) =>
        client.AcquireLockAsync(channel, phrase, cancellationToken);

    /// <summary>
    /// Sends a LOCK RELEASE command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ReleaseLockAsync(int channel, CancellationToken cancellationToken = default) =>
        client.ReleaseLockAsync(channel, cancellationToken);

    /// <summary>
    /// Sends a LOCK CLEAR command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="overridePhrase">The optional override phrase.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ClearLockAsync(int channel, string? overridePhrase = null, CancellationToken cancellationToken = default) =>
        client.ClearLockAsync(channel, overridePhrase, cancellationToken);
}
