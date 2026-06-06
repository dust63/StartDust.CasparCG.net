using System.Threading.Channels;

using StarDust.CasparCG.Diagnostics;
using StarDust.CasparCG.Events;
using StarDust.CasparCG.Fluent;
using StarDust.CasparCG.Health;
using StarDust.CasparCG.Osc;
using StarDust.CasparCG.Protocol.Amcp;
using StarDust.CasparCG.Protocol.Amcp.Commands;
using StarDust.CasparCG.Protocol.Osc;
using StarDust.CasparCG.Query;
using StarDust.CasparCG.State;
using StarDust.CasparCG.Transport;
using CasparEventChannel = System.Threading.Channels.Channel;
using ChannelBuffer = System.Threading.Channels.Channel<StarDust.CasparCG.Events.CasparEvent>;

namespace StarDust.CasparCG;

/// <summary>
/// Provides the primary vNext entry point for interacting with a CasparCG server.
/// </summary>
public sealed class CasparClient
{
    private readonly ChannelBuffer _eventChannel = CasparEventChannel.CreateBounded<CasparEvent>(256);
    private readonly CasparStateStore _state = new();
    private readonly IAmcpTransport? _transport;
    private readonly IOscTransport? _oscTransport;
    private IOscMessageMapper _oscMessageMapper = new DefaultOscMessageMapper();
    private CasparEvent? _lastPublishedOscEvent;

    /// <summary>
    /// Initializes a client instance without a configured transport.
    /// </summary>
    public CasparClient()
    {
    }

    /// <summary>
    /// Initializes a client instance with the transport used for AMCP communication.
    /// </summary>
    /// <param name="transport">The AMCP transport implementation.</param>
    public CasparClient(IAmcpTransport transport)
    {
        _transport = transport;
    }

    /// <summary>
    /// Initializes a client instance with AMCP and optional OSC transports.
    /// </summary>
    /// <param name="transport">The AMCP transport implementation.</param>
    /// <param name="oscTransport">The OSC transport implementation.</param>
    /// <param name="oscMessageMapper">The OSC message mapper.</param>
    /// <param name="clientName">The client registration name used by OSC events.</param>
    public CasparClient(
        IAmcpTransport transport,
        IOscTransport? oscTransport,
        IOscMessageMapper? oscMessageMapper = null,
        string clientName = "default")
        : this(transport)
    {
        _oscTransport = oscTransport;
        _oscMessageMapper = oscMessageMapper ?? new DefaultOscMessageMapper(clientName);
    }

    /// <summary>
    /// Starts a fluent command chain for the specified channel.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <returns>A fluent channel scope.</returns>
    public ChannelScope Channel(int channel) => new(this, channel);

    /// <summary>
    /// Starts a fluent command chain for server-level operations.
    /// </summary>
    /// <returns>A fluent server scope.</returns>
    public ServerScope Server() => new(this);

    /// <summary>
    /// Starts a fluent command chain for data operations.
    /// </summary>
    /// <returns>A fluent data scope.</returns>
    public DataScope Data() => new(this);

    /// <summary>
    /// Starts a fluent command chain for thumbnail operations.
    /// </summary>
    /// <returns>A fluent thumbnail scope.</returns>
    public ThumbnailScope Thumbnails() => new(this);

    /// <summary>
    /// Starts a fluent command chain for OSC operations.
    /// </summary>
    /// <returns>A fluent OSC scope.</returns>
    public OscScope Osc() => new(this);

    /// <summary>
    /// Starts a fluent command chain for administrative operations.
    /// </summary>
    /// <returns>A fluent administrative scope.</returns>
    public AdminScope Admin() => new(this);

    /// <summary>
    /// Starts a client-side parallel orchestration over independent layer sequences.
    /// </summary>
    /// <param name="sequences">The sequences to execute in parallel.</param>
    /// <returns>A parallel sequence orchestrator.</returns>
    public ParallelSequenceBuilder Parallel(params LayerSequenceBuilder[] sequences) => new(sequences);

    /// <summary>
    /// Sends a LOAD command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask LoadAsync(int channel, int layer, string clip, CancellationToken cancellationToken = default) =>
        LoadAsync(channel, layer, clip, options: null, cancellationToken);

    /// <summary>
    /// Sends a LOAD command with optional typed playback options.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="options">The optional typed playback options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask LoadAsync(int channel, int layer, string clip, PlaybackOptions? options, CancellationToken cancellationToken = default) =>
        SendAsync(new LoadCommand(channel, layer, clip, options), cancellationToken);

    /// <summary>
    /// Sends a CALLBG command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="arguments">The raw CALLBG parameter string.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CallBgAsync(int channel, int layer, string arguments, CancellationToken cancellationToken = default) =>
        SendAsync(new CallBgCommand(channel, layer, arguments), cancellationToken);

    /// <summary>
    /// Sends a PAUSE command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask PauseAsync(int channel, int layer, CancellationToken cancellationToken = default) =>
        SendAsync(new PauseCommand(channel, layer), cancellationToken);

    /// <summary>
    /// Sends a RESUME command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ResumeAsync(int channel, int layer, CancellationToken cancellationToken = default) =>
        SendAsync(new ResumeCommand(channel, layer), cancellationToken);

    /// <summary>
    /// Sends a CLEAR command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ClearAsync(int channel, int layer, CancellationToken cancellationToken = default) =>
        SendAsync(new ClearCommand(channel, layer), cancellationToken);

    /// <summary>
    /// Sends a CLEAR command for the whole channel.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ClearAsync(int channel, CancellationToken cancellationToken = default) =>
        SendAsync(new ClearCommand(channel), cancellationToken);

    /// <summary>
    /// Sends a CALL command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="arguments">The raw CALL parameter string.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CallAsync(int channel, int layer, string arguments, CancellationToken cancellationToken = default) =>
        SendAsync(new CallCommand(channel, layer, arguments), cancellationToken);

    /// <summary>
    /// Sends a SWAP command.
    /// </summary>
    /// <param name="channel">The source channel.</param>
    /// <param name="layer">The source layer.</param>
    /// <param name="otherChannel">The target channel.</param>
    /// <param name="otherLayer">The target layer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask SwapAsync(int channel, int layer, int otherChannel, int otherLayer, CancellationToken cancellationToken = default) =>
        SwapAsync(channel, layer, otherChannel, otherLayer, swapTransforms: false, cancellationToken);

    /// <summary>
    /// Sends a SWAP command.
    /// </summary>
    /// <param name="channel">The source channel.</param>
    /// <param name="layer">The source layer.</param>
    /// <param name="otherChannel">The target channel.</param>
    /// <param name="otherLayer">The target layer.</param>
    /// <param name="swapTransforms">Whether mixer transforms should be swapped too.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask SwapAsync(int channel, int layer, int otherChannel, int otherLayer, bool swapTransforms, CancellationToken cancellationToken = default) =>
        SendAsync(new SwapCommand(channel, layer, otherChannel, otherLayer, swapTransforms), cancellationToken);

    /// <summary>
    /// Sends an ADD command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="consumer">The consumer identifier.</param>
    /// <param name="arguments">The optional raw consumer arguments.</param>
    /// <param name="consumerIndex">The optional consumer index override.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask AddAsync(int channel, string consumer, string? arguments = null, int? consumerIndex = null, CancellationToken cancellationToken = default) =>
        SendAsync(new AddCommand(channel, consumer, arguments, consumerIndex), cancellationToken);

    /// <summary>
    /// Sends a REMOVE command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="consumerIndex">The optional consumer index override.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask RemoveAsync(int channel, int consumerIndex, CancellationToken cancellationToken = default) =>
        SendAsync(new RemoveCommand(channel, ConsumerIndex: consumerIndex), cancellationToken);

    /// <summary>
    /// Sends a REMOVE command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="arguments">The raw consumer arguments.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask RemoveAsync(int channel, string arguments, CancellationToken cancellationToken = default) =>
        SendAsync(new RemoveCommand(channel, arguments), cancellationToken);

    /// <summary>
    /// Sends an APPLY command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The optional target layer.</param>
    /// <param name="arguments">The raw argument tail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ApplyAsync(int channel, int? layer, string arguments, CancellationToken cancellationToken = default) =>
        SendAsync(new ApplyCommand(channel, layer, arguments), cancellationToken);

    /// <summary>
    /// Sends a PRINT command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask PrintAsync(int channel, CancellationToken cancellationToken = default) =>
        SendAsync(new PrintCommand(channel), cancellationToken);

    /// <summary>
    /// Sends a CLEAR ALL command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ClearAllAsync(CancellationToken cancellationToken = default) =>
        SendAsync(new ClearAllCommand(), cancellationToken);

    /// <summary>
    /// Sends a SET command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="key">The property name.</param>
    /// <param name="value">The property value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask SetAsync(int channel, string key, string value, CancellationToken cancellationToken = default) =>
        SendAsync(new SetCommand(channel, key, value), cancellationToken);

    /// <summary>
    /// Sends a DATA STORE command.
    /// </summary>
    /// <param name="key">The dataset name.</param>
    /// <param name="value">The dataset payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> DataStoreAsync(string key, string value, CancellationToken cancellationToken = default) =>
        QueryAsync(new DataStoreCommand(key, value), cancellationToken);

    /// <summary>
    /// Sends a DATA RETRIEVE command.
    /// </summary>
    /// <param name="key">The dataset name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> DataRetrieveAsync(string key, CancellationToken cancellationToken = default) =>
        QueryAsync(new DataRetrieveCommand(key), cancellationToken);

    /// <summary>
    /// Sends a DATA LIST command.
    /// </summary>
    /// <param name="subDirectory">The optional subdirectory to query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> DataListAsync(string? subDirectory, CancellationToken cancellationToken = default) =>
        QueryAsync(new DataListCommand(subDirectory), cancellationToken);

    /// <summary>
    /// Sends a DATA REMOVE command.
    /// </summary>
    /// <param name="key">The dataset name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> DataRemoveAsync(string key, CancellationToken cancellationToken = default) =>
        QueryAsync(new DataRemoveCommand(key), cancellationToken);

    /// <summary>
    /// Sends a CG ADD command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="template">The template name.</param>
    /// <param name="playOnLoad">Whether the template should play on load.</param>
    /// <param name="data">The optional inline XML payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CgAddAsync(int channel, int layer, string template, bool playOnLoad, string? data, CancellationToken cancellationToken = default) =>
        SendAsync(new CgAddCommand(channel, layer, template, playOnLoad, data), cancellationToken);

    /// <summary>
    /// Sends a CG PLAY command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CgPlayAsync(int channel, int layer, CancellationToken cancellationToken = default) =>
        SendAsync(new CgPlayCommand(channel, layer), cancellationToken);

    /// <summary>
    /// Sends a CG STOP command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CgStopAsync(int channel, int layer, CancellationToken cancellationToken = default) =>
        SendAsync(new CgStopCommand(channel, layer), cancellationToken);

    /// <summary>
    /// Sends a CG NEXT command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CgNextAsync(int channel, int layer, CancellationToken cancellationToken = default) =>
        SendAsync(new CgNextCommand(channel, layer), cancellationToken);

    /// <summary>
    /// Sends a CG REMOVE command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CgRemoveAsync(int channel, int layer, CancellationToken cancellationToken = default) =>
        SendAsync(new CgRemoveCommand(channel, layer), cancellationToken);

    /// <summary>
    /// Sends a CG CLEAR command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CgClearAsync(int channel, int layer, CancellationToken cancellationToken = default) =>
        SendAsync(new CgClearCommand(channel, layer), cancellationToken);

    /// <summary>
    /// Sends a CG UPDATE command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="data">The inline XML payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> CgUpdateAsync(int channel, int layer, string data, CancellationToken cancellationToken = default) =>
        QueryAsync(new CgUpdateCommand(channel, layer, data), cancellationToken);

    /// <summary>
    /// Sends a CG INVOKE command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="method">The method name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CgInvokeAsync(int channel, int layer, string method, CancellationToken cancellationToken = default) =>
        SendAsync(new CgInvokeCommand(channel, layer, method), cancellationToken);

    /// <summary>
    /// Sends a THUMBNAIL LIST command.
    /// </summary>
    /// <param name="subDirectory">The optional subdirectory.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> ThumbnailListAsync(string? subDirectory, CancellationToken cancellationToken = default) =>
        QueryAsync(new ThumbnailListCommand(subDirectory), cancellationToken);

    /// <summary>
    /// Sends a THUMBNAIL RETRIEVE command.
    /// </summary>
    /// <param name="fileName">The media file name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> ThumbnailRetrieveAsync(string fileName, CancellationToken cancellationToken = default) =>
        QueryAsync(new ThumbnailRetrieveCommand(fileName), cancellationToken);

    /// <summary>
    /// Sends a THUMBNAIL GENERATE command.
    /// </summary>
    /// <param name="fileName">The media file name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ThumbnailGenerateAsync(string fileName, CancellationToken cancellationToken = default) =>
        SendAsync(new ThumbnailGenerateCommand(fileName), cancellationToken);

    /// <summary>
    /// Sends a THUMBNAIL GENERATE_ALL command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ThumbnailGenerateAllAsync(CancellationToken cancellationToken = default) =>
        SendAsync(new ThumbnailGenerateAllCommand(), cancellationToken);

    /// <summary>
    /// Sends a MIXER KEYER command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="keyer">Whether the keyer is enabled.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerKeyerAsync(int channel, int layer, bool keyer, CancellationToken cancellationToken = default) =>
        SendAsync(new MixerKeyerCommand(channel, layer, keyer), cancellationToken);

    /// <summary>
    /// Sends a MIXER INVERT command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="invert">Whether colors are inverted.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerInvertAsync(int channel, int layer, bool invert, CancellationToken cancellationToken = default) =>
        SendAsync(new MixerInvertCommand(channel, layer, invert), cancellationToken);

    /// <summary>
    /// Sends a MIXER CHROMA command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="arguments">The raw command tail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerChromaAsync(int channel, int layer, string? arguments, CancellationToken cancellationToken = default) =>
        SendAsync(new MixerChromaCommand(channel, layer, arguments), cancellationToken);

    /// <summary>
    /// Sends a MIXER BLEND command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="blend">The blend mode.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerBlendAsync(int channel, int layer, string blend, CancellationToken cancellationToken = default) =>
        SendAsync(new MixerBlendCommand(channel, layer, blend), cancellationToken);

    /// <summary>
    /// Sends a MIXER OPACITY command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="opacity">The opacity value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerOpacityAsync(int channel, int layer, double opacity, CancellationToken cancellationToken = default) =>
        SendAsync(new MixerOpacityCommand(channel, layer, opacity), cancellationToken);

    /// <summary>
    /// Sends a MIXER BRIGHTNESS command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="brightness">The brightness value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerBrightnessAsync(int channel, int layer, double brightness, CancellationToken cancellationToken = default) =>
        SendAsync(new MixerBrightnessCommand(channel, layer, brightness), cancellationToken);

    /// <summary>
    /// Sends a MIXER SATURATION command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="saturation">The saturation value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerSaturationAsync(int channel, int layer, double saturation, CancellationToken cancellationToken = default) =>
        SendAsync(new MixerSaturationCommand(channel, layer, saturation), cancellationToken);

    /// <summary>
    /// Sends a MIXER CONTRAST command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="contrast">The contrast value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerContrastAsync(int channel, int layer, double contrast, CancellationToken cancellationToken = default) =>
        SendAsync(new MixerContrastCommand(channel, layer, contrast), cancellationToken);

    /// <summary>
    /// Sends a MIXER LEVELS command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="arguments">The raw command tail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerLevelsAsync(int channel, int layer, string? arguments, CancellationToken cancellationToken = default) =>
        SendAsync(new MixerLevelsCommand(channel, layer, arguments), cancellationToken);

    /// <summary>
    /// Sends a MIXER FILL command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="arguments">The raw command tail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerFillAsync(int channel, int layer, string? arguments, CancellationToken cancellationToken = default) =>
        SendAsync(new MixerFillCommand(channel, layer, arguments), cancellationToken);

    /// <summary>
    /// Sends a MIXER CLIP command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="arguments">The raw command tail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerClipAsync(int channel, int layer, string? arguments, CancellationToken cancellationToken = default) =>
        SendAsync(new MixerClipCommand(channel, layer, arguments), cancellationToken);

    /// <summary>
    /// Sends a MIXER ANCHOR command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="arguments">The raw command tail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerAnchorAsync(int channel, int layer, string? arguments, CancellationToken cancellationToken = default) =>
        SendAsync(new MixerAnchorCommand(channel, layer, arguments), cancellationToken);

    /// <summary>
    /// Sends a MIXER CROP command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="arguments">The raw command tail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerCropAsync(int channel, int layer, string? arguments, CancellationToken cancellationToken = default) =>
        SendAsync(new MixerCropCommand(channel, layer, arguments), cancellationToken);

    /// <summary>
    /// Sends a MIXER ROTATION command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="arguments">The raw command tail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerRotationAsync(int channel, int layer, string? arguments, CancellationToken cancellationToken = default) =>
        SendAsync(new MixerRotationCommand(channel, layer, arguments), cancellationToken);

    /// <summary>
    /// Sends a MIXER PERSPECTIVE command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="arguments">The raw command tail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerPerspectiveAsync(int channel, int layer, string? arguments, CancellationToken cancellationToken = default) =>
        SendAsync(new MixerPerspectiveCommand(channel, layer, arguments), cancellationToken);

    /// <summary>
    /// Sends a MIXER VOLUME command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="volume">The volume value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerVolumeAsync(int channel, int layer, double volume, CancellationToken cancellationToken = default) =>
        SendAsync(new MixerVolumeCommand(channel, layer, volume), cancellationToken);

    /// <summary>
    /// Sends a MIXER MASTERVOLUME command.
    /// </summary>
    /// <param name="volume">The master volume value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerMasterVolumeAsync(double volume, CancellationToken cancellationToken = default) =>
        SendAsync(new MixerMasterVolumeCommand(volume), cancellationToken);

    /// <summary>
    /// Sends a MIXER COMMIT command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerCommitAsync(int channel, int layer, CancellationToken cancellationToken = default) =>
        SendAsync(new MixerCommitCommand(channel, layer), cancellationToken);

    /// <summary>
    /// Sends a MIXER CLEAR command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerClearAsync(int channel, int layer, CancellationToken cancellationToken = default) =>
        SendAsync(new MixerClearCommand(channel, layer), cancellationToken);

    /// <summary>
    /// Sends a CHANNEL_GRID command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ChannelGridAsync(int channel, CancellationToken cancellationToken = default) =>
        SendAsync(new ChannelGridCommand(channel), cancellationToken);

    /// <summary>
    /// Sends a MIXER GRID command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="arguments">The raw command tail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerGridAsync(int channel, int layer, string? arguments, CancellationToken cancellationToken = default) =>
        SendAsync(new MixerGridCommand(channel, layer, arguments), cancellationToken);

    /// <summary>
    /// Sends a DIAG command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask DiagAsync(CancellationToken cancellationToken = default) =>
        SendAsync(new DiagCommand(), cancellationToken);

    /// <summary>
    /// Sends a BYE command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ByeAsync(CancellationToken cancellationToken = default) =>
        SendAsync(new ByeCommand(), cancellationToken);

    /// <summary>
    /// Sends a KILL command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask KillAsync(CancellationToken cancellationToken = default) =>
        SendAsync(new KillCommand(), cancellationToken);

    /// <summary>
    /// Sends a RESTART command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask RestartAsync(CancellationToken cancellationToken = default) =>
        SendAsync(new RestartCommand(), cancellationToken);

    /// <summary>
    /// Gets the current AMCP log level.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The current log level.</returns>
    public async ValueTask<string> GetLogLevelAsync(CancellationToken cancellationToken = default)
    {
        var response = await QueryAsync(new LogLevelCommand(), cancellationToken);
        return response.Lines.FirstOrDefault() ?? string.Empty;
    }

    /// <summary>
    /// Sets the AMCP log level.
    /// </summary>
    /// <param name="level">The log level to apply.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask SetLogLevelAsync(string level, CancellationToken cancellationToken = default) =>
        SendAsync(new LogLevelCommand(level), cancellationToken);

    /// <summary>
    /// Sends a LOCK ACQUIRE command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="phrase">The lock phrase.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask AcquireLockAsync(int channel, string phrase, CancellationToken cancellationToken = default) =>
        SendAsync(new LockAcquireCommand(channel, phrase), cancellationToken);

    /// <summary>
    /// Sends a LOCK RELEASE command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ReleaseLockAsync(int channel, CancellationToken cancellationToken = default) =>
        SendAsync(new LockReleaseCommand(channel), cancellationToken);

    /// <summary>
    /// Sends a LOCK CLEAR command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="overridePhrase">The optional override phrase.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ClearLockAsync(int channel, string? overridePhrase = null, CancellationToken cancellationToken = default) =>
        SendAsync(new LockClearCommand(channel, overridePhrase), cancellationToken);

    /// <summary>
    /// Gets the live event stream for this client.
    /// </summary>
    public CasparEventStream Events => new(_eventChannel.Reader);

    /// <summary>
    /// Gets the current state projection for this client.
    /// </summary>
    public CasparStateStore State => _state;

    /// <summary>
    /// Gets the current connection health status.
    /// </summary>
    public ConnectionHealthStatus HealthStatus { get; private set; } = ConnectionHealthStatus.Disconnected;

    /// <summary>
    /// Raised when a raw OSC packet is received before parsing.
    /// </summary>
    public event Action<ReadOnlyMemory<byte>>? OscPacketReceived;

    /// <summary>
    /// Gets the client diagnostics snapshot.
    /// </summary>
    public ClientDiagnostics Diagnostics { get; } = new();

    /// <summary>
    /// Connects the AMCP transport.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous connect operation.</returns>
    public async ValueTask ConnectAsync(CancellationToken cancellationToken = default)
    {
        HealthStatus = ConnectionHealthStatus.Connecting;
        await RequireTransport().ConnectAsync(cancellationToken);
        HealthStatus = ConnectionHealthStatus.Connected;
        Diagnostics.LastSuccessfulAmcpInteraction = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Starts the OSC listener on the specified port.
    /// </summary>
    /// <param name="port">The OSC UDP port.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous start operation.</returns>
    public async ValueTask StartOscAsync(int port, CancellationToken cancellationToken = default)
    {
        if (_oscTransport is null)
        {
            throw new InvalidOperationException("No OSC transport has been configured.");
        }

        await _oscTransport.StartAsync(port, OnOscPacketAsync, cancellationToken);
    }

    /// <summary>
    /// Subscribes the current AMCP session to OSC messages on the specified UDP port.
    /// </summary>
    /// <param name="port">The OSC UDP port.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous subscribe operation.</returns>
    public async ValueTask SubscribeOscAsync(int port, CancellationToken cancellationToken = default)
    {
        await SendAsync(new OscSubscribeCommand(port), cancellationToken);
    }

    /// <summary>
    /// Unsubscribes the current AMCP session from OSC messages on the specified UDP port.
    /// </summary>
    /// <param name="port">The OSC UDP port.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous unsubscribe operation.</returns>
    public async ValueTask OscUnsubscribeAsync(int port, CancellationToken cancellationToken = default)
    {
        await SendAsync(new OscUnsubscribeCommand(port), cancellationToken);
    }

    /// <summary>
    /// Stops the OSC listener.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous stop operation.</returns>
    public ValueTask StopOscAsync(CancellationToken cancellationToken = default) =>
        _oscTransport is null ? ValueTask.CompletedTask : _oscTransport.StopAsync(cancellationToken);

    /// <summary>
    /// Sends a play command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public async ValueTask PlayAsync(int channel, int layer, string clip, CancellationToken cancellationToken = default)
    {
        await SendAsync(new PlayCommand(channel, layer, clip), cancellationToken);
    }

    /// <summary>
    /// Sends a PLAY command with optional typed playback options.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="options">The optional typed playback options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public async ValueTask PlayAsync(int channel, int layer, string clip, PlaybackOptions? options, CancellationToken cancellationToken = default)
    {
        await SendAsync(new PlayCommand(channel, layer, clip, options), cancellationToken);
    }

    /// <summary>
    /// Sends a load background command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public async ValueTask LoadBackgroundAsync(int channel, int layer, string clip, CancellationToken cancellationToken = default)
    {
        await SendAsync(new LoadBackgroundCommand(channel, layer, clip), cancellationToken);
    }

    /// <summary>
    /// Sends a load background command with optional typed playback options.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="options">The optional typed playback options.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public async ValueTask LoadBackgroundAsync(int channel, int layer, string clip, LoadBackgroundOptions? options, CancellationToken cancellationToken = default)
    {
        await SendAsync(new LoadBackgroundCommand(channel, layer, clip, options), cancellationToken);
    }

    /// <summary>
    /// Sends a stop command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public async ValueTask StopAsync(int channel, int layer, CancellationToken cancellationToken = default)
    {
        await SendAsync(new StopCommand(channel, layer), cancellationToken);
    }

    /// <summary>
    /// Gets the server version string reported by AMCP.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The server version string.</returns>
    public async ValueTask<string> GetVersionAsync(CancellationToken cancellationToken = default)
    {
        var response = await QueryAsync(new VersionCommand("SERVER"), cancellationToken);
        return response.Lines.FirstOrDefault() ?? string.Empty;
    }

    /// <summary>
    /// Gets the structured response returned by the INFO command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed query data.</returns>
    public async ValueTask<QueryDataMap> InfoAsync(CancellationToken cancellationToken = default)
    {
        var response = await QueryAsync(new InfoCommand(), cancellationToken);
        return CasparQueryResultParser.ParseQueryDataMap(response);
    }

    /// <summary>
    /// Gets the structured response returned by the INFO CONFIG command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed query data.</returns>
    public async ValueTask<QueryDataMap> InfoConfigAsync(CancellationToken cancellationToken = default)
    {
        var response = await QueryAsync(new InfoConfigCommand(), cancellationToken);
        return CasparQueryResultParser.ParseQueryDataMap(response);
    }

    /// <summary>
    /// Gets the structured response returned by the INFO PATHS command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed query data.</returns>
    public async ValueTask<QueryDataMap> InfoPathsAsync(CancellationToken cancellationToken = default)
    {
        var response = await QueryAsync(new InfoPathsCommand(), cancellationToken);
        return CasparQueryResultParser.ParseQueryDataMap(response);
    }

    /// <summary>
    /// Gets the media listing reported by AMCP.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed media listing.</returns>
    public async ValueTask<IReadOnlyList<MediaFile>> GetMediaFilesAsync(CancellationToken cancellationToken = default)
    {
        var response = await QueryAsync(new ListMediaFilesCommand(), cancellationToken);
        return CasparQueryResultParser.ParseMediaFiles(response);
    }

    /// <summary>
    /// Gets detailed information about a media file reported by AMCP.
    /// </summary>
    /// <param name="fileName">The media file name to query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed media information.</returns>
    public async ValueTask<MediaInfo> MediaInfoAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var response = await QueryAsync(new CinfCommand(fileName), cancellationToken);
        return CasparQueryResultParser.ParseMediaInfo(response);
    }

    /// <summary>
    /// Gets the font listing reported by AMCP.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed font listing.</returns>
    public async ValueTask<IReadOnlyList<FontFile>> GetFontFilesAsync(CancellationToken cancellationToken = default)
    {
        var response = await QueryAsync(new FlsCommand(), cancellationToken);
        return CasparQueryResultParser.ParseFontFiles(response);
    }

    /// <summary>
    /// Gets the template listing reported by AMCP.
    /// </summary>
    /// <param name="subDirectory">The optional subdirectory to query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed template listing.</returns>
    public async ValueTask<IReadOnlyList<TemplateFile>> GetTemplateFilesAsync(string? subDirectory, CancellationToken cancellationToken = default)
    {
        var response = await QueryAsync(new TlsCommand(subDirectory), cancellationToken);
        return CasparQueryResultParser.ParseTemplateFiles(response);
    }

    /// <summary>
    /// Gets the structured response returned by the GL INFO command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The parsed query data.</returns>
    public async ValueTask<QueryDataMap> GlInfoAsync(CancellationToken cancellationToken = default)
    {
        var response = await QueryAsync(new GlInfoCommand(), cancellationToken);
        return CasparQueryResultParser.ParseQueryDataMap(response);
    }

    /// <summary>
    /// Sends a GL GC command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask GlGcAsync(CancellationToken cancellationToken = default) =>
        SendAsync(new GlGcCommand(), cancellationToken);

    internal async ValueTask SendAsync(AmcpCommand command, CancellationToken cancellationToken = default)
    {
        await QueryAsync(command, cancellationToken);
    }

    internal async ValueTask<AmcpResponse> QueryAsync(AmcpCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = AmcpResponseParser.Parse(await RequireTransport().SendAsync(command.Serialize(), cancellationToken));
            if (!response.IsSuccess)
            {
                throw new AmcpCommandException(response);
            }

            Diagnostics.LastSuccessfulAmcpInteraction = DateTimeOffset.UtcNow;
            return response;
        }
        catch (Exception ex)
        {
            HealthStatus = ConnectionHealthStatus.Faulted;
            Diagnostics.LastFailure = ex;
            Diagnostics.ReconnectCount++;
            throw;
        }
    }

    internal ValueTask PublishAsync(CasparEvent evt, CancellationToken cancellationToken = default)
    {
        if (_lastPublishedOscEvent is not null && EqualityComparer<CasparEvent>.Default.Equals(_lastPublishedOscEvent, evt))
        {
            return ValueTask.CompletedTask;
        }

        _lastPublishedOscEvent = evt;
        _state.Apply(evt);
        return _eventChannel.Writer.WriteAsync(evt, cancellationToken);
    }

    private async ValueTask OnOscPacketAsync(ReadOnlyMemory<byte> packet, CancellationToken cancellationToken = default)
    {
        OscPacketReceived?.Invoke(packet);

        try
        {
            foreach (var message in OscPacketParser.ParseMessages(packet.Span))
            {
                if (_oscMessageMapper.TryMap(message.Address, message.Arguments, out var evt) && evt is not null)
                {
                    await PublishAsync(evt, cancellationToken);
                }
            }
        }
        catch (FormatException)
        {
        }
        catch (NotSupportedException)
        {
        }
    }

    private IAmcpTransport RequireTransport() =>
        _transport ?? throw new InvalidOperationException("No AMCP transport has been configured.");
}
