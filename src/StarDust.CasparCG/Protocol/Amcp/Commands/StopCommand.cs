namespace StarDust.CasparCG.Protocol.Amcp.Commands;

/// <summary>
/// Represents a STOP command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
public sealed record StopCommand(int Channel, int Layer) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"STOP {Address(Channel, Layer)}\r\n";
}
