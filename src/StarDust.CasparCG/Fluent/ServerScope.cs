using StarDust.CasparCG.Query;

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
    public ValueTask<string> VersionAsync(CancellationToken cancellationToken = default) =>
        client.GetVersionAsync(cancellationToken);

    /// <summary>
    /// Gets the media listing reported by AMCP.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed media listing.</returns>
    public ValueTask<IReadOnlyList<MediaFile>> MediaFilesAsync(CancellationToken cancellationToken = default) =>
        client.GetMediaFilesAsync(cancellationToken);

    /// <summary>
    /// Gets detailed information for a media file reported by AMCP.
    /// </summary>
    /// <param name="fileName">The media file name to query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed media information.</returns>
    public ValueTask<MediaInfo> MediaInfoAsync(string fileName, CancellationToken cancellationToken = default) =>
        client.MediaInfoAsync(fileName, cancellationToken);

    /// <summary>
    /// Gets the font listing reported by AMCP.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed font listing.</returns>
    public ValueTask<IReadOnlyList<FontFile>> FontFilesAsync(CancellationToken cancellationToken = default) =>
        client.GetFontFilesAsync(cancellationToken);

    /// <summary>
    /// Gets the template listing reported by AMCP.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed template listing.</returns>
    public ValueTask<IReadOnlyList<TemplateFile>> TemplateFilesAsync(CancellationToken cancellationToken = default) =>
        client.GetTemplateFilesAsync(null, cancellationToken);

    /// <summary>
    /// Gets the template listing reported by AMCP.
    /// </summary>
    /// <param name="subDirectory">The optional subdirectory to query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed template listing.</returns>
    public ValueTask<IReadOnlyList<TemplateFile>> TemplateFilesAsync(string? subDirectory, CancellationToken cancellationToken = default) =>
        client.GetTemplateFilesAsync(subDirectory, cancellationToken);

    /// <summary>
    /// Gets the response returned by the INFO command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed query data.</returns>
    public ValueTask<QueryDataMap> InfoAsync(CancellationToken cancellationToken = default) =>
        client.InfoAsync(cancellationToken);

    /// <summary>
    /// Gets the response returned by the INFO CONFIG command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed query data.</returns>
    public ValueTask<QueryDataMap> InfoConfigAsync(CancellationToken cancellationToken = default) =>
        client.InfoConfigAsync(cancellationToken);

    /// <summary>
    /// Gets the response returned by the INFO PATHS command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed query data.</returns>
    public ValueTask<QueryDataMap> InfoPathsAsync(CancellationToken cancellationToken = default) =>
        client.InfoPathsAsync(cancellationToken);

    /// <summary>
    /// Sends a DIAG command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask DiagAsync(CancellationToken cancellationToken = default) =>
        client.DiagAsync(cancellationToken);

    /// <summary>
    /// Gets the response returned by the GL INFO command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed query data.</returns>
    public ValueTask<QueryDataMap> GlInfoAsync(CancellationToken cancellationToken = default) =>
        client.GlInfoAsync(cancellationToken);

    /// <summary>
    /// Sends a GL GC command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask GlGcAsync(CancellationToken cancellationToken = default) =>
        client.GlGcAsync(cancellationToken);
}
