namespace StarDust.CasparCG.Query;

/// <summary>
/// Represents a loosely structured query payload projected as key/value data plus raw content.
/// </summary>
public sealed record QueryDataMap
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueryDataMap"/> record.
    /// </summary>
    /// <param name="values">Flattened values extracted from the payload.</param>
    /// <param name="lines">The original payload lines.</param>
    /// <param name="raw">The full raw AMCP response.</param>
    public QueryDataMap(
        IReadOnlyDictionary<string, string> values,
        IReadOnlyList<string> lines,
        string raw)
    {
        Values = values;
        Lines = lines;
        Raw = raw;
    }

    /// <summary>
    /// Gets flattened values extracted from the payload.
    /// </summary>
    public IReadOnlyDictionary<string, string> Values { get; init; }

    /// <summary>
    /// Gets the original payload lines.
    /// </summary>
    public IReadOnlyList<string> Lines { get; init; }

    /// <summary>
    /// Gets the full raw AMCP response.
    /// </summary>
    public string Raw { get; init; }
}
