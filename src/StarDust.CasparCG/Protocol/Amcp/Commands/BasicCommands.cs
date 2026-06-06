using StarDust.CasparCG.Protocol.Amcp;

namespace StarDust.CasparCG.Protocol.Amcp.Commands;

/// <summary>
/// Represents a LOAD command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Clip">The clip identifier.</param>
/// <param name="Options">The optional typed playback options.</param>
public sealed record LoadCommand(int Channel, int Layer, string Clip, PlaybackOptions? Options = null) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() =>
        PlaybackCommandSerializer.SerializeWithOptions("LOAD", Channel, Layer, Clip, Options);
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
/// <param name="Arguments">The raw CALLBG parameter string.</param>
public sealed record CallBgCommand(int Channel, int Layer, string Arguments) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"CALLBG {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Arguments}\r\n";
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
/// <param name="Layer">The optional target layer.</param>
public sealed record ClearCommand(int Channel, int? Layer = null) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() =>
        Layer is int layer
            ? $"CLEAR {AmcpCommandFormatting.ChannelLayer(Channel, layer)}\r\n"
            : $"CLEAR {Channel}\r\n";
}

/// <summary>
/// Represents a CALL command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Arguments">The raw CALL parameter string.</param>
public sealed record CallCommand(int Channel, int Layer, string Arguments) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"CALL {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Arguments}\r\n";
}

/// <summary>
/// Represents a SWAP command.
/// </summary>
/// <param name="Channel">The source channel.</param>
/// <param name="Layer">The source layer.</param>
/// <param name="OtherChannel">The target channel.</param>
/// <param name="OtherLayer">The target layer.</param>
/// <param name="SwapTransforms">Whether mixer transforms should be swapped too.</param>
public sealed record SwapCommand(int Channel, int Layer, int OtherChannel, int OtherLayer, bool SwapTransforms = false) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() =>
        SwapTransforms
            ? $"SWAP {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {AmcpCommandFormatting.ChannelLayer(OtherChannel, OtherLayer)} TRANSFORMS\r\n"
            : $"SWAP {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {AmcpCommandFormatting.ChannelLayer(OtherChannel, OtherLayer)}\r\n";
}

/// <summary>
/// Represents an ADD command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Consumer">The consumer identifier.</param>
/// <param name="Arguments">The optional consumer arguments.</param>
/// <param name="ConsumerIndex">The optional consumer index override.</param>
public sealed record AddCommand(int Channel, string Consumer, string? Arguments = null, int? ConsumerIndex = null) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize()
    {
        var target = ConsumerIndex is int consumerIndex
            ? $"{Channel}-{consumerIndex}"
            : AmcpCommandFormatting.Channel(Channel);

        return string.IsNullOrWhiteSpace(Arguments)
            ? $"ADD {target} {Consumer}\r\n"
            : $"ADD {target} {Consumer} {Arguments}\r\n";
    }
}

/// <summary>
/// Represents a REMOVE command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Arguments">The optional raw consumer arguments.</param>
/// <param name="ConsumerIndex">The optional consumer index override.</param>
public sealed record RemoveCommand(int Channel, string? Arguments = null, int? ConsumerIndex = null) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize()
    {
        var target = ConsumerIndex is int consumerIndex
            ? $"{Channel}-{consumerIndex}"
            : AmcpCommandFormatting.Channel(Channel);

        return string.IsNullOrWhiteSpace(Arguments)
            ? $"REMOVE {target}\r\n"
            : $"REMOVE {target} {Arguments}\r\n";
    }
}

/// <summary>
/// Represents an APPLY command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The optional target layer.</param>
/// <param name="Arguments">The raw argument tail.</param>
public sealed record ApplyCommand(int Channel, int? Layer, string Arguments) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() =>
        Layer is int layer
            ? $"APPLY {AmcpCommandFormatting.ChannelLayer(Channel, layer)} {Arguments}\r\n"
            : $"APPLY {AmcpCommandFormatting.Channel(Channel)} {Arguments}\r\n";
}

/// <summary>
/// Represents a PRINT command.
/// </summary>
/// <param name="Channel">The target channel.</param>
public sealed record PrintCommand(int Channel) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"PRINT {AmcpCommandFormatting.Channel(Channel)}\r\n";
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
/// <param name="Key">The property name.</param>
/// <param name="Value">The property value.</param>
public sealed record SetCommand(int Channel, string Key, string Value) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"SET {AmcpCommandFormatting.Channel(Channel)} {Key} {Value}\r\n";
}
