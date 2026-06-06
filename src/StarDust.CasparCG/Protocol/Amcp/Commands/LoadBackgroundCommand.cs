using StarDust.CasparCG.Protocol.Amcp;

namespace StarDust.CasparCG.Protocol.Amcp.Commands;

/// <summary>
/// Represents a LOADBG command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Clip">The clip identifier.</param>
/// <param name="Options">The optional typed playback options.</param>
public sealed record LoadBackgroundCommand(
    int Channel,
    int Layer,
    string Clip,
    LoadBackgroundOptions? Options = null) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize()
    {
        var parts = new List<string> { "LOADBG", Address(Channel, Layer), Clip };
        PlaybackCommandSerializer.AppendLoadBackgroundOptions(parts, Options);

        return string.Join(' ', parts) + "\r\n";
    }
}
