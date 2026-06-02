namespace StarDust.CasparCG.Protocol.Amcp.Commands;

/// <summary>
/// Represents a THUMBNAIL LIST command.
/// </summary>
/// <param name="SubDirectory">The optional subdirectory to query.</param>
public sealed record ThumbnailListCommand(string? SubDirectory = null) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() =>
        string.IsNullOrWhiteSpace(SubDirectory)
            ? "THUMBNAIL LIST\r\n"
            : $"THUMBNAIL LIST {SubDirectory}\r\n";
}

/// <summary>
/// Represents a THUMBNAIL RETRIEVE command.
/// </summary>
/// <param name="FileName">The media file name.</param>
public sealed record ThumbnailRetrieveCommand(string FileName) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"THUMBNAIL RETRIEVE {FileName}\r\n";
}

/// <summary>
/// Represents a THUMBNAIL GENERATE command.
/// </summary>
/// <param name="FileName">The media file name.</param>
public sealed record ThumbnailGenerateCommand(string FileName) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"THUMBNAIL GENERATE {FileName}\r\n";
}

/// <summary>
/// Represents a THUMBNAIL GENERATE_ALL command.
/// </summary>
public sealed record ThumbnailGenerateAllCommand() : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => "THUMBNAIL GENERATE_ALL\r\n";
}
