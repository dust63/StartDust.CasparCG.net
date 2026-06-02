namespace StarDust.CasparCG.Protocol.Amcp.Commands;

/// <summary>
/// Represents a CG ADD command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Template">The template name.</param>
/// <param name="PlayOnLoad">Whether the template should play on load.</param>
/// <param name="Data">The optional inline XML payload.</param>
public sealed record CgAddCommand(int Channel, int Layer, string Template, bool PlayOnLoad = false, string? Data = null) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() =>
        string.IsNullOrWhiteSpace(Data)
            ? $"CG ADD {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Template} {(PlayOnLoad ? 1 : 0)}\r\n"
            : $"CG ADD {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Template} {(PlayOnLoad ? 1 : 0)} {Data}\r\n";
}

/// <summary>
/// Represents a CG PLAY command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
public sealed record CgPlayCommand(int Channel, int Layer) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"CG PLAY {AmcpCommandFormatting.ChannelLayer(Channel, Layer)}\r\n";
}

/// <summary>
/// Represents a CG STOP command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
public sealed record CgStopCommand(int Channel, int Layer) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"CG STOP {AmcpCommandFormatting.ChannelLayer(Channel, Layer)}\r\n";
}

/// <summary>
/// Represents a CG NEXT command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
public sealed record CgNextCommand(int Channel, int Layer) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"CG NEXT {AmcpCommandFormatting.ChannelLayer(Channel, Layer)}\r\n";
}

/// <summary>
/// Represents a CG REMOVE command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
public sealed record CgRemoveCommand(int Channel, int Layer) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"CG REMOVE {AmcpCommandFormatting.ChannelLayer(Channel, Layer)}\r\n";
}

/// <summary>
/// Represents a CG CLEAR command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
public sealed record CgClearCommand(int Channel, int Layer) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"CG CLEAR {AmcpCommandFormatting.ChannelLayer(Channel, Layer)}\r\n";
}

/// <summary>
/// Represents a CG UPDATE command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Data">The inline XML payload.</param>
public sealed record CgUpdateCommand(int Channel, int Layer, string Data) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"CG UPDATE {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Data}\r\n";
}

/// <summary>
/// Represents a CG INVOKE command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Method">The method name.</param>
public sealed record CgInvokeCommand(int Channel, int Layer, string Method) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"CG INVOKE {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Method}\r\n";
}
