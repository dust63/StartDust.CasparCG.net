using StarDust.CasparCG.Protocol.Amcp;

namespace StarDust.CasparCG.Fluent;

/// <summary>
/// Builds a fluent LOADBG command before it is sent to the server.
/// </summary>
public sealed class LoadBackgroundCommandBuilder(CasparClient client, int channel, int layer, string clip)
{
    private bool _loop;
    private bool _autoPlay;

    /// <summary>
    /// Configures the command to loop playback.
    /// </summary>
    /// <returns>The current builder.</returns>
    public LoadBackgroundCommandBuilder Loop()
    {
        _loop = true;
        return this;
    }

    /// <summary>
    /// Configures the command to auto-play after loading.
    /// </summary>
    /// <returns>The current builder.</returns>
    public LoadBackgroundCommandBuilder AutoPlay()
    {
        _autoPlay = true;
        return this;
    }

    /// <summary>
    /// Sends the configured load background command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask SendAsync(CancellationToken cancellationToken) =>
        client.SendAsync(new FluentLoadBackgroundCommand(channel, layer, clip, _loop, _autoPlay), cancellationToken);

    private sealed record FluentLoadBackgroundCommand(
        int Channel,
        int Layer,
        string Clip,
        bool Loop,
        bool AutoPlay) : AmcpCommand
    {
        /// <inheritdoc />
        public override string Serialize()
        {
            var parts = new List<string> { "LOADBG", Address(Channel, Layer), Clip };

            if (Loop)
            {
                parts.Add("LOOP");
            }

            if (AutoPlay)
            {
                parts.Add("AUTO");
            }

            return string.Join(' ', parts) + "\r\n";
        }
    }
}
