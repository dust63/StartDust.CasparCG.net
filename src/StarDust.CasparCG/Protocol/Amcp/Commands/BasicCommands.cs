namespace StarDust.CasparCG.Protocol.Amcp.Commands;

/// <summary>
/// Represents a LOAD command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Clip">The clip identifier.</param>
public sealed record LoadCommand(int Channel, int Layer, string Clip) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"LOAD {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Clip}\r\n";
}

/// <summary>
/// Represents a PAUSE command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
public sealed record PauseCommand(int Channel, int Layer) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"PAUSE {AmcpCommandFormatting.ChannelLayer(Channel, Layer)}\r\n";
}

/// <summary>
/// Represents a CALLBG command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Clip">The clip identifier.</param>
public sealed record CallBgCommand(int Channel, int Layer, string Clip) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"CALLBG {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Clip}\r\n";
}

/// <summary>
/// Represents a RESUME command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
public sealed record ResumeCommand(int Channel, int Layer) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"RESUME {AmcpCommandFormatting.ChannelLayer(Channel, Layer)}\r\n";
}

/// <summary>
/// Represents a CLEAR command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
public sealed record ClearCommand(int Channel, int Layer) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"CLEAR {AmcpCommandFormatting.ChannelLayer(Channel, Layer)}\r\n";
}

/// <summary>
/// Represents a CALL command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Clip">The clip identifier.</param>
public sealed record CallCommand(int Channel, int Layer, string Clip) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"CALL {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Clip}\r\n";
}

/// <summary>
/// Represents a SWAP command.
/// </summary>
/// <param name="Channel">The source channel.</param>
/// <param name="Layer">The source layer.</param>
/// <param name="OtherChannel">The target channel.</param>
/// <param name="OtherLayer">The target layer.</param>
public sealed record SwapCommand(int Channel, int Layer, int OtherChannel, int OtherLayer) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() =>
        $"SWAP {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {AmcpCommandFormatting.ChannelLayer(OtherChannel, OtherLayer)}\r\n";
}

/// <summary>
/// Represents an ADD command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Clip">The clip identifier.</param>
public sealed record AddCommand(int Channel, int Layer, string Clip) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"ADD {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Clip}\r\n";
}

/// <summary>
/// Represents a REMOVE command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Clip">The clip identifier.</param>
public sealed record RemoveCommand(int Channel, int Layer, string Clip) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"REMOVE {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Clip}\r\n";
}

/// <summary>
/// Represents an APPLY command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Clip">The clip identifier.</param>
public sealed record ApplyCommand(int Channel, int Layer, string Clip) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"APPLY {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Clip}\r\n";
}

/// <summary>
/// Represents a PRINT command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Clip">The clip identifier.</param>
public sealed record PrintCommand(int Channel, int Layer, string Clip) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"PRINT {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Clip}\r\n";
}

/// <summary>
/// Represents a CLEAR ALL command.
/// </summary>
public sealed record ClearAllCommand() : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => "CLEAR ALL\r\n";
}

/// <summary>
/// Represents a SET command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Key">The property name.</param>
/// <param name="Value">The property value.</param>
public sealed record SetCommand(int Channel, int Layer, string Key, string Value) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"SET {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Key} {Value}\r\n";
}
