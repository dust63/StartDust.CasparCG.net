namespace StarDust.CasparCG.Protocol.Amcp.Commands;

/// <summary>
/// Represents an OSC UNSUBSCRIBE command.
/// </summary>
/// <param name="Port">The OSC UDP port.</param>
public sealed record OscUnsubscribeCommand(int Port) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"OSC UNSUBSCRIBE {Port}\r\n";
}
