namespace StarDust.CasparCG.Protocol.Amcp.Commands;

/// <summary>
/// Represents an OSC SUBSCRIBE command.
/// </summary>
public sealed record OscSubscribeCommand(int Port) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"OSC SUBSCRIBE {Port}\r\n";
}
