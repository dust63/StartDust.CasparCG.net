using StarDust.CasparCG.Protocol.Amcp;
using StarDust.CasparCG.Protocol.Amcp.Commands;

namespace StarDust.CasparCG.Fluent;

/// <summary>
/// Builds a fluent play command before it is sent to the server.
/// </summary>
public sealed class PlayCommandBuilder(CasparClient client, int channel, int layer, string clip)
{
    private PlaybackOptions _options = new();

    /// <summary>
    /// Applies a transition to the play command.
    /// </summary>
    /// <param name="transition">The typed transition.</param>
    /// <returns>The current builder.</returns>
    public PlayCommandBuilder WithTransition(PlaybackTransition transition)
    {
        _options = _options with { Transition = transition };
        return this;
    }

    /// <summary>
    /// Configures the command to loop playback.
    /// </summary>
    /// <returns>The current builder.</returns>
    public PlayCommandBuilder WithLoop()
    {
        _options = _options with { Loop = true };
        return this;
    }

    /// <summary>
    /// Sets the seek position.
    /// </summary>
    /// <param name="seek">The seek position.</param>
    /// <returns>The current builder.</returns>
    public PlayCommandBuilder Seek(int seek)
    {
        _options = _options with { Seek = seek };
        return this;
    }

    /// <summary>
    /// Sets the clip length.
    /// </summary>
    /// <param name="length">The clip length.</param>
    /// <returns>The current builder.</returns>
    public PlayCommandBuilder Length(int length)
    {
        _options = _options with { Length = length };
        return this;
    }

    /// <summary>
    /// Sets the media filter.
    /// </summary>
    /// <param name="filter">The filter name.</param>
    /// <returns>The current builder.</returns>
    public PlayCommandBuilder Filter(string filter)
    {
        _options = _options with { Filter = filter };
        return this;
    }

    /// <summary>
    /// Configures the command to clear on file-not-found.
    /// </summary>
    /// <returns>The current builder.</returns>
    public PlayCommandBuilder ClearOn404()
    {
        _options = _options with { ClearOn404 = true };
        return this;
    }

    /// <summary>
    /// Sends the configured play command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask SendAsync(CancellationToken cancellationToken = default) =>
        client.SendAsync(new PlayCommand(channel, layer, clip, _options), cancellationToken);
}
