namespace StarDust.CasparCG.Protocol.Amcp.Commands;

/// <summary>
/// Represents a MIXER KEYER command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Keyer">Whether the keyer is enabled.</param>
public sealed record MixerKeyerCommand(int Channel, int Layer, bool Keyer = false) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"MIXER KEYER {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {(Keyer ? 1 : 0)}\r\n";
}

/// <summary>
/// Represents a MIXER INVERT command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Invert">Whether colors are inverted.</param>
public sealed record MixerInvertCommand(int Channel, int Layer, bool Invert = false) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"MIXER INVERT {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {(Invert ? 1 : 0)}\r\n";
}

/// <summary>
/// Represents a MIXER BLEND command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Blend">The blend mode.</param>
public sealed record MixerBlendCommand(int Channel, int Layer, string Blend = "normal") : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"MIXER BLEND {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Blend}\r\n";
}

/// <summary>
/// Represents a MIXER OPACITY command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Opacity">The opacity value.</param>
public sealed record MixerOpacityCommand(int Channel, int Layer, double Opacity) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"MIXER OPACITY {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Opacity}\r\n";
}

/// <summary>
/// Represents a MIXER BRIGHTNESS command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Brightness">The brightness value.</param>
public sealed record MixerBrightnessCommand(int Channel, int Layer, double Brightness) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"MIXER BRIGHTNESS {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Brightness}\r\n";
}

/// <summary>
/// Represents a MIXER SATURATION command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Saturation">The saturation value.</param>
public sealed record MixerSaturationCommand(int Channel, int Layer, double Saturation) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"MIXER SATURATION {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Saturation}\r\n";
}

/// <summary>
/// Represents a MIXER CONTRAST command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Contrast">The contrast value.</param>
public sealed record MixerContrastCommand(int Channel, int Layer, double Contrast) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"MIXER CONTRAST {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Contrast}\r\n";
}

/// <summary>
/// Represents a MIXER VOLUME command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Volume">The volume value.</param>
public sealed record MixerVolumeCommand(int Channel, int Layer, double Volume) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"MIXER VOLUME {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Volume}\r\n";
}

/// <summary>
/// Represents a MIXER CHROMA command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Arguments">The raw command tail.</param>
public sealed record MixerChromaCommand(int Channel, int Layer, string? Arguments = null) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() =>
        string.IsNullOrWhiteSpace(Arguments)
            ? $"MIXER CHROMA {AmcpCommandFormatting.ChannelLayer(Channel, Layer)}\r\n"
            : $"MIXER CHROMA {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Arguments}\r\n";
}

/// <summary>
/// Represents a MIXER LEVELS command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Arguments">The raw command tail.</param>
public sealed record MixerLevelsCommand(int Channel, int Layer, string? Arguments = null) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() =>
        string.IsNullOrWhiteSpace(Arguments)
            ? $"MIXER LEVELS {AmcpCommandFormatting.ChannelLayer(Channel, Layer)}\r\n"
            : $"MIXER LEVELS {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Arguments}\r\n";
}

/// <summary>
/// Represents a MIXER FILL command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Arguments">The raw command tail.</param>
public sealed record MixerFillCommand(int Channel, int Layer, string? Arguments = null) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() =>
        string.IsNullOrWhiteSpace(Arguments)
            ? $"MIXER FILL {AmcpCommandFormatting.ChannelLayer(Channel, Layer)}\r\n"
            : $"MIXER FILL {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Arguments}\r\n";
}

/// <summary>
/// Represents a MIXER CLIP command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Arguments">The raw command tail.</param>
public sealed record MixerClipCommand(int Channel, int Layer, string? Arguments = null) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() =>
        string.IsNullOrWhiteSpace(Arguments)
            ? $"MIXER CLIP {AmcpCommandFormatting.ChannelLayer(Channel, Layer)}\r\n"
            : $"MIXER CLIP {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Arguments}\r\n";
}

/// <summary>
/// Represents a MIXER ANCHOR command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Arguments">The raw command tail.</param>
public sealed record MixerAnchorCommand(int Channel, int Layer, string? Arguments = null) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() =>
        string.IsNullOrWhiteSpace(Arguments)
            ? $"MIXER ANCHOR {AmcpCommandFormatting.ChannelLayer(Channel, Layer)}\r\n"
            : $"MIXER ANCHOR {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Arguments}\r\n";
}

/// <summary>
/// Represents a MIXER CROP command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Arguments">The raw command tail.</param>
public sealed record MixerCropCommand(int Channel, int Layer, string? Arguments = null) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() =>
        string.IsNullOrWhiteSpace(Arguments)
            ? $"MIXER CROP {AmcpCommandFormatting.ChannelLayer(Channel, Layer)}\r\n"
            : $"MIXER CROP {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Arguments}\r\n";
}

/// <summary>
/// Represents a MIXER ROTATION command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Arguments">The raw command tail.</param>
public sealed record MixerRotationCommand(int Channel, int Layer, string? Arguments = null) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() =>
        string.IsNullOrWhiteSpace(Arguments)
            ? $"MIXER ROTATION {AmcpCommandFormatting.ChannelLayer(Channel, Layer)}\r\n"
            : $"MIXER ROTATION {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Arguments}\r\n";
}

/// <summary>
/// Represents a MIXER PERSPECTIVE command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Arguments">The raw command tail.</param>
public sealed record MixerPerspectiveCommand(int Channel, int Layer, string? Arguments = null) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() =>
        string.IsNullOrWhiteSpace(Arguments)
            ? $"MIXER PERSPECTIVE {AmcpCommandFormatting.ChannelLayer(Channel, Layer)}\r\n"
            : $"MIXER PERSPECTIVE {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Arguments}\r\n";
}

/// <summary>
/// Represents a MIXER MASTERVOLUME command.
/// </summary>
/// <param name="Volume">The master volume value.</param>
public sealed record MixerMasterVolumeCommand(double Volume) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"MIXER MASTERVOLUME {Volume}\r\n";
}

/// <summary>
/// Represents a MIXER COMMIT command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
public sealed record MixerCommitCommand(int Channel, int Layer) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"MIXER COMMIT {AmcpCommandFormatting.ChannelLayer(Channel, Layer)}\r\n";
}

/// <summary>
/// Represents a MIXER CLEAR command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
public sealed record MixerClearCommand(int Channel, int Layer) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"MIXER CLEAR {AmcpCommandFormatting.ChannelLayer(Channel, Layer)}\r\n";
}

/// <summary>
/// Represents a CHANNEL_GRID command.
/// </summary>
/// <param name="Channel">The target channel.</param>
public sealed record ChannelGridCommand(int Channel) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() => $"CHANNEL_GRID {AmcpCommandFormatting.Channel(Channel)}\r\n";
}

/// <summary>
/// Represents a MIXER GRID command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Arguments">The raw command tail.</param>
public sealed record MixerGridCommand(int Channel, int Layer, string? Arguments = null) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() =>
        string.IsNullOrWhiteSpace(Arguments)
            ? $"MIXER GRID {AmcpCommandFormatting.ChannelLayer(Channel, Layer)}\r\n"
            : $"MIXER GRID {AmcpCommandFormatting.ChannelLayer(Channel, Layer)} {Arguments}\r\n";
}
