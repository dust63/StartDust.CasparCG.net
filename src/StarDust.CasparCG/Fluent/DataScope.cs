using StarDust.CasparCG.Protocol.Amcp;

namespace StarDust.CasparCG.Fluent;

/// <summary>
/// Provides fluent access to data AMCP commands.
/// </summary>
/// <param name="client">The client used to send commands.</param>
public sealed class DataScope(CasparClient client)
{
    /// <summary>
    /// Sends a DATA STORE command.
    /// </summary>
    /// <param name="key">The dataset name.</param>
    /// <param name="value">The dataset payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> StoreAsync(string key, string value, CancellationToken cancellationToken = default) =>
        client.DataStoreAsync(key, value, cancellationToken);

    /// <summary>
    /// Sends a DATA RETRIEVE command.
    /// </summary>
    /// <param name="key">The dataset name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> RetrieveAsync(string key, CancellationToken cancellationToken = default) =>
        client.DataRetrieveAsync(key, cancellationToken);

    /// <summary>
    /// Sends a DATA LIST command.
    /// </summary>
    /// <param name="subDirectory">The optional subdirectory to query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> ListAsync(string? subDirectory, CancellationToken cancellationToken = default) =>
        client.DataListAsync(subDirectory, cancellationToken);

    /// <summary>
    /// Sends a DATA REMOVE command.
    /// </summary>
    /// <param name="key">The dataset name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> RemoveAsync(string key, CancellationToken cancellationToken = default) =>
        client.DataRemoveAsync(key, cancellationToken);
}
