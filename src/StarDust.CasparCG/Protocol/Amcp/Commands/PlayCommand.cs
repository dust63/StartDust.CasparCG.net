namespace StarDust.CasparCG.Protocol.Amcp.Commands;

/// <summary>
/// Represents a PLAY command.
/// </summary>
/// <param name="Channel">The target channel.</param>
/// <param name="Layer">The target layer.</param>
/// <param name="Clip">The clip identifier.</param>
/// <param name="TransitionKind">The transition kind.</param>
/// <param name="TransitionDuration">The transition duration.</param>
/// <param name="Loop">Whether playback should loop.</param>
public sealed record PlayCommand(
    int Channel,
    int Layer,
    string Clip,
    string? TransitionKind = null,
    int? TransitionDuration = null,
    bool Loop = false) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize()
    {
        var parts = new List<string> { "PLAY", Address(Channel, Layer), Clip };

        if (TransitionKind is not null && TransitionDuration is not null)
        {
            parts.Add(TransitionKind);
            parts.Add(TransitionDuration.Value.ToString());
        }

        if (Loop)
        {
            parts.Add("LOOP");
        }

        return string.Join(' ', parts) + "\r\n";
    }
}
