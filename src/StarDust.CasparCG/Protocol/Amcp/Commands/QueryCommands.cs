namespace StarDust.CasparCG.Protocol.Amcp.Commands;

/// <summary>
/// Represents an INFO query command.
/// </summary>
/// <param name="Target">The optional query target.</param>
public record InfoCommand(string? Target = null) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() =>
        string.IsNullOrWhiteSpace(Target)
            ? "INFO\r\n"
            : $"INFO {Target}\r\n";
}

/// <summary>
/// Represents an INFO CONFIG query command.
/// </summary>
public sealed record InfoConfigCommand() : InfoCommand("CONFIG");

/// <summary>
/// Represents an INFO PATHS query command.
/// </summary>
public sealed record InfoPathsCommand() : InfoCommand("PATHS");

/// <summary>
/// Represents a GL GC query command.
/// </summary>
public sealed record GlGcCommand() : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => "GL GC\r\n";
}

/// <summary>
/// Represents a CINF query command.
/// </summary>
/// <param name="FileName">The optional media file name to query.</param>
public sealed record CinfCommand(string? FileName = null) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() =>
        string.IsNullOrWhiteSpace(FileName)
            ? "CINF\r\n"
            : $"CINF {FileName}\r\n";
}

/// <summary>
/// Represents a CLS query command.
/// </summary>
public sealed record ClsCommand() : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => "CLS\r\n";
}

/// <summary>
/// Represents an FLS query command.
/// </summary>
public sealed record FlsCommand() : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => "FLS\r\n";
}

/// <summary>
/// Represents a TLS query command.
/// </summary>
/// <param name="SubDirectory">The optional subdirectory to query.</param>
public sealed record TlsCommand(string? SubDirectory = null) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() =>
        string.IsNullOrWhiteSpace(SubDirectory)
            ? "TLS\r\n"
            : $"TLS {SubDirectory}\r\n";
}

/// <summary>
/// Represents a GL INFO query command.
/// </summary>
public sealed record GlInfoCommand() : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => "GL INFO\r\n";
}
