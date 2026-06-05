namespace StarDust.CasparCG.Protocol.Amcp.Commands;

/// <summary>
/// Represents a CLS command to list media files.
/// </summary>
public sealed record ListMediaFilesCommand : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => "CLS\r\n";
}
