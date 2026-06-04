namespace StarDust.CasparCG.Fluent;

/// <summary>
/// Coordinates multiple layer sequences so they execute concurrently while each sequence keeps its local order.
/// </summary>
public sealed class ParallelSequenceBuilder(params LayerSequenceBuilder[] sequences)
{
    private readonly IReadOnlyList<LayerSequenceBuilder> _sequences = sequences ?? throw new ArgumentNullException(nameof(sequences));

    /// <summary>
    /// Executes all configured sequences concurrently.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public async ValueTask SendAsync(CancellationToken cancellationToken)
    {
        if (_sequences.Count == 0)
        {
            return;
        }

        var tasks = _sequences
            .Select(sequence => sequence.SendAsync(cancellationToken).AsTask())
            .ToArray();

        await Task.WhenAll(tasks);
    }
}
