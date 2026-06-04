using StarDust.CasparCG.Protocol.Amcp;

namespace StarDust.CasparCG.Fluent;

/// <summary>
/// Builds a local ordered sequence of layer operations.
/// </summary>
public sealed class LayerSequenceBuilder(CasparClient client, int channel, int layer)
{
    private readonly CasparClient _client = client;
    private readonly int _channel = channel;
    private readonly int _layer = layer;
    private readonly List<LayerSequenceStep> _steps = [];

    /// <summary>
    /// Adds a LOADBG command step with optional sequence-local modifiers.
    /// </summary>
    /// <param name="clip">The clip identifier.</param>
    /// <returns>A builder for the LOADBG step.</returns>
    public LayerLoadBackgroundStepBuilder LoadBg(string clip) => new(this, clip);

    /// <summary>
    /// Adds a PLAY command step.
    /// </summary>
    /// <param name="clip">The clip identifier.</param>
    /// <returns>The current sequence builder.</returns>
    public LayerSequenceBuilder Play(string clip)
    {
        AddCommand(ct => _client.PlayAsync(_channel, _layer, clip, ct));
        return this;
    }

    /// <summary>
    /// Adds a CLEAR command step.
    /// </summary>
    /// <returns>The current sequence builder.</returns>
    public LayerSequenceBuilder Clear()
    {
        AddCommand(ct => _client.ClearAsync(_channel, _layer, ct));
        return this;
    }

    /// <summary>
    /// Adds a PAUSE command step.
    /// </summary>
    /// <returns>The current sequence builder.</returns>
    public LayerSequenceBuilder Pause()
    {
        AddCommand(ct => _client.PauseAsync(_channel, _layer, ct));
        return this;
    }

    /// <summary>
    /// Adds a STOP command step.
    /// </summary>
    /// <returns>The current sequence builder.</returns>
    public LayerSequenceBuilder Stop()
    {
        AddCommand(ct => _client.StopAsync(_channel, _layer, ct));
        return this;
    }

    /// <summary>
    /// Adds a CG ADD command step.
    /// </summary>
    /// <param name="template">The template name.</param>
    /// <param name="playOnLoad">Whether the template should play on load.</param>
    /// <param name="data">The optional inline XML payload.</param>
    /// <returns>The current sequence builder.</returns>
    public LayerSequenceBuilder CgAdd(string template, bool playOnLoad = true, string? data = null)
    {
        AddCommand(ct => _client.CgAddAsync(_channel, _layer, template, playOnLoad, data, ct));
        return this;
    }

    /// <summary>
    /// Adds a CG STOP command step.
    /// </summary>
    /// <returns>The current sequence builder.</returns>
    public LayerSequenceBuilder CgStop()
    {
        AddCommand(ct => _client.CgStopAsync(_channel, _layer, ct));
        return this;
    }

    /// <summary>
    /// Adds a local wait step.
    /// </summary>
    /// <param name="milliseconds">The delay in milliseconds.</param>
    /// <returns>The current sequence builder.</returns>
    public LayerSequenceBuilder Wait(int milliseconds)
    {
        if (milliseconds < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(milliseconds));
        }

        return Wait(TimeSpan.FromMilliseconds(milliseconds));
    }

    /// <summary>
    /// Adds a local wait step.
    /// </summary>
    /// <param name="delay">The delay to wait before the next step.</param>
    /// <returns>The current sequence builder.</returns>
    public LayerSequenceBuilder Wait(TimeSpan delay)
    {
        if (delay < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(delay));
        }

        _steps.Add(new DelayStep(delay));
        return this;
    }

    /// <summary>
    /// Marks the next step boundary.
    /// </summary>
    /// <returns>The current sequence builder.</returns>
    public LayerSequenceBuilder Then() => this;

    /// <summary>
    /// Executes the sequence in order.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public async ValueTask SendAsync(CancellationToken cancellationToken)
    {
        foreach (var step in _steps)
        {
            switch (step)
            {
                case DelayStep delay:
                    await Task.Delay(delay.Delay, cancellationToken);
                    break;
                case CommandStep command:
                    await command.SendAsync(cancellationToken);
                    break;
            }
        }
    }

    internal void AddCommand(Func<CancellationToken, ValueTask> sendAsync) =>
        _steps.Add(new CommandStep(sendAsync));

    /// <summary>
    /// Builds the local LOADBG step for a sequence.
    /// </summary>
    public sealed class LayerLoadBackgroundStepBuilder
    {
        private readonly LayerSequenceBuilder _sequence;
        private readonly string _clip;
        private bool _loop;
        private bool _autoPlay;

        internal LayerLoadBackgroundStepBuilder(LayerSequenceBuilder sequence, string clip)
        {
            _sequence = sequence;
            _clip = clip;
        }

        /// <summary>
        /// Configures the LOADBG step to loop playback.
        /// </summary>
        /// <returns>The current step builder.</returns>
        public LayerLoadBackgroundStepBuilder Loop()
        {
            _loop = true;
            return this;
        }

        /// <summary>
        /// Configures the LOADBG step to auto-play after loading.
        /// </summary>
        /// <returns>The current step builder.</returns>
        public LayerLoadBackgroundStepBuilder AutoPlay()
        {
            _autoPlay = true;
            return this;
        }

        /// <summary>
        /// Commits the LOADBG step and returns to the sequence builder.
        /// </summary>
        /// <returns>The parent sequence builder.</returns>
        public LayerSequenceBuilder Then()
        {
            _sequence.AddCommand(ct =>
                _sequence._client.SendAsync(
                    new FluentLoadBackgroundCommand(_sequence._channel, _sequence._layer, _clip, _loop, _autoPlay),
                    ct));
            return _sequence;
        }
    }

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
