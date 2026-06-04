using StarDust.CasparCG.Protocol.Amcp;

namespace StarDust.CasparCG.Fluent;

/// <summary>
/// Provides fluent access to thumbnail AMCP commands.
/// </summary>
/// <param name="client">The client used to send commands.</param>
public sealed class ThumbnailScope(CasparClient client)
{
    /// <summary>
    /// Sends a THUMBNAIL LIST command.
    /// </summary>
    /// <param name="subDirectory">The optional subdirectory.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> ListAsync(string? subDirectory, CancellationToken cancellationToken) =>
        client.ThumbnailListAsync(subDirectory, cancellationToken);

    /// <summary>
    /// Sends a THUMBNAIL RETRIEVE command.
    /// </summary>
    /// <param name="fileName">The media file name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> RetrieveAsync(string fileName, CancellationToken cancellationToken) =>
        client.ThumbnailRetrieveAsync(fileName, cancellationToken);

    /// <summary>
    /// Sends a THUMBNAIL GENERATE command.
    /// </summary>
    /// <param name="fileName">The media file name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask GenerateAsync(string fileName, CancellationToken cancellationToken) =>
        client.ThumbnailGenerateAsync(fileName, cancellationToken);

    /// <summary>
    /// Sends a THUMBNAIL GENERATE_ALL command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask GenerateAllAsync(CancellationToken cancellationToken) =>
        client.ThumbnailGenerateAllAsync(cancellationToken);
}
