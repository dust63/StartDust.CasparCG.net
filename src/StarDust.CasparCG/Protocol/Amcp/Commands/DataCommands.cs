namespace StarDust.CasparCG.Protocol.Amcp.Commands;

/// <summary>
/// Represents a DATA STORE command.
/// </summary>
/// <param name="Key">The dataset name.</param>
/// <param name="Value">The dataset payload.</param>
public sealed record DataStoreCommand(string Key, string Value) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"DATA STORE {Key} {Value}\r\n";
}

/// <summary>
/// Represents a DATA RETRIEVE command.
/// </summary>
/// <param name="Key">The dataset name.</param>
public sealed record DataRetrieveCommand(string Key) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"DATA RETRIEVE {Key}\r\n";
}

/// <summary>
/// Represents a DATA LIST command.
/// </summary>
/// <param name="SubDirectory">The optional subdirectory to query.</param>
public sealed record DataListCommand(string? SubDirectory = null) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() =>
        string.IsNullOrWhiteSpace(SubDirectory)
            ? "DATA LIST\r\n"
            : $"DATA LIST {SubDirectory}\r\n";
}

/// <summary>
/// Represents a DATA REMOVE command.
/// </summary>
/// <param name="Key">The dataset name.</param>
public sealed record DataRemoveCommand(string Key) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"DATA REMOVE {Key}\r\n";
}
