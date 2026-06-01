namespace StarDust.CasparCG.Protocol.Amcp.Commands;

/// <summary>
/// Represents a LOADBG command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Clip">The clip identifier.</param>
public sealed record LoadBackgroundCommand(int Channel, int Layer, string Clip) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"LOADBG {Address(Channel, Layer)} {Clip}\r\n";
}
