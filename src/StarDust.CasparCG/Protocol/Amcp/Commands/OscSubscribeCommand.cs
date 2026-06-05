namespace StarDust.CasparCG.Protocol.Amcp.Commands;

/// <summary>
/// Represents an OSC SUBSCRIBE command.
/// </summary>
/// <param name="Port">The OSC UDP port.</param>
public sealed record OscSubscribeCommand(int Port) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"OSC SUBSCRIBE {Port}\r\n";
}
