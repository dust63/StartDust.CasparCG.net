namespace StarDust.CasparCG.Protocol.Amcp.Commands;

/// <summary>
/// Represents a DIAG command.
/// </summary>
public sealed record DiagCommand() : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => "DIAG\r\n";
}

/// <summary>
/// Represents a BYE command.
/// </summary>
public sealed record ByeCommand() : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => "BYE\r\n";
}

/// <summary>
/// Represents a KILL command.
/// </summary>
public sealed record KillCommand() : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => "KILL\r\n";
}

/// <summary>
/// Represents a RESTART command.
/// </summary>
public sealed record RestartCommand() : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => "RESTART\r\n";
}

/// <summary>
/// Represents a LOCK command.
/// </summary>
public sealed record LockCommand() : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => "LOCK\r\n";
}
