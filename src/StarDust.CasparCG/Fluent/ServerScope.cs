using StarDust.CasparCG.Protocol.Amcp;

namespace StarDust.CasparCG.Fluent;

/// <summary>
/// Provides fluent access to server-level AMCP operations.
/// </summary>
/// <param name="client">The client used to send commands.</param>
public sealed class ServerScope(CasparClient client)
{
    /// <summary>
    /// Gets the server version string reported by AMCP.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The server version string.</returns>
    public ValueTask<string> VersionAsync(CancellationToken cancellationToken) =>
        client.GetVersionAsync(cancellationToken);

    /// <summary>
    /// Gets the media listing reported by AMCP.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The media listing lines.</returns>
    public ValueTask<IReadOnlyList<string>> MediaFilesAsync(CancellationToken cancellationToken) =>
        client.GetMediaFilesAsync(cancellationToken);

    /// <summary>
    /// Gets detailed information for a media file reported by AMCP.
    /// </summary>
    /// <param name="fileName">The media file name to query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> MediaInfoAsync(string fileName, CancellationToken cancellationToken) =>
        client.MediaInfoAsync(fileName, cancellationToken);

    /// <summary>
    /// Gets the font listing reported by AMCP.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> FileListAsync(CancellationToken cancellationToken) =>
        client.FileListAsync(cancellationToken);

    /// <summary>
    /// Gets the template listing reported by AMCP.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> TemplateListAsync(CancellationToken cancellationToken) =>
        client.TemplateListAsync(null, cancellationToken);

    /// <summary>
    /// Gets the template listing reported by AMCP.
    /// </summary>
    /// <param name="subDirectory">The optional subdirectory to query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> TemplateListAsync(string? subDirectory, CancellationToken cancellationToken) =>
        client.TemplateListAsync(subDirectory, cancellationToken);

    /// <summary>
    /// Gets the response returned by the INFO command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> InfoAsync(CancellationToken cancellationToken) =>
        client.InfoAsync(cancellationToken);

    /// <summary>
    /// Gets the response returned by the INFO CONFIG command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> InfoConfigAsync(CancellationToken cancellationToken) =>
        client.InfoConfigAsync(cancellationToken);

    /// <summary>
    /// Gets the response returned by the INFO PATHS command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> InfoPathsAsync(CancellationToken cancellationToken) =>
        client.InfoPathsAsync(cancellationToken);

    /// <summary>
    /// Sends a DIAG command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask DiagAsync(CancellationToken cancellationToken) =>
        client.DiagAsync(cancellationToken);

    /// <summary>
    /// Gets the response returned by the GL INFO command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> GlInfoAsync(CancellationToken cancellationToken) =>
        client.GlInfoAsync(cancellationToken);

    /// <summary>
    /// Sends a GL GC command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask GlGcAsync(CancellationToken cancellationToken) =>
        client.GlGcAsync(cancellationToken);
}
