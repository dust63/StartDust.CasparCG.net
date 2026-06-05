using StarDust.CasparCG.Protocol.Amcp.Commands;

namespace StarDust.CasparCG.Fluent;

/// <summary>
/// Builds a fluent play command before it is sent to the server.
/// </summary>
public sealed class PlayCommandBuilder(CasparClient client, int channel, int layer, string clip)
{
    private string? _transitionKind;
    private int? _transitionDuration;
    private bool _loop;

    /// <summary>
    /// Indicates that a transition will be applied to the play command.
    /// </summary>
    /// <returns>The current builder.</returns>
    public PlayCommandBuilder WithTransition() => this;

    /// <summary>
    /// Applies a mix transition.
    /// </summary>
    /// <param name="duration">The transition duration.</param>
    /// <returns>The current builder.</returns>
    public PlayCommandBuilder Mix(int duration)
    {
        _transitionKind = "MIX";
        _transitionDuration = duration;
        return this;
    }

    /// <summary>
    /// Configures the command to loop playback.
    /// </summary>
    /// <returns>The current builder.</returns>
    public PlayCommandBuilder WithLoop()
    {
        _loop = true;
        return this;
    }

    /// <summary>
    /// Sends the configured play command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask SendAsync(CancellationToken cancellationToken = default) =>
        client.SendAsync(
            new PlayCommand(channel, layer, clip, _transitionKind, _transitionDuration, _loop),
            cancellationToken);
}
