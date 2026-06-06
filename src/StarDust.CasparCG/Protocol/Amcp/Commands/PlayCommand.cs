using StarDust.CasparCG.Protocol.Amcp;

namespace StarDust.CasparCG.Protocol.Amcp.Commands;

/// <summary>
/// Represents a PLAY command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Clip">The clip identifier.</param>
/// <param name="Options">The optional typed playback options.</param>
public sealed record PlayCommand(
    int Channel,
    int Layer,
    string Clip,
    PlaybackOptions? Options = null) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize()
    {
        var parts = new List<string> { "PLAY", Address(Channel, Layer), Clip };
        PlaybackCommandSerializer.AppendPlaybackOptions(parts, Options);

        return string.Join(' ', parts) + "\r\n";
    }
}
