using StarDust.CasparCG.Diagnostics;
using StarDust.CasparCG.Fluent;
using StarDust.CasparCG.Events;
using StarDust.CasparCG.Health;
using StarDust.CasparCG.Osc;
using StarDust.CasparCG.Protocol.Amcp;
using StarDust.CasparCG.Protocol.Amcp.Commands;
using StarDust.CasparCG.Protocol.Osc;
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
    /// Sends a LOAD command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask LoadAsync(int channel, int layer, string clip, CancellationToken cancellationToken) =>
        SendAsync(new LoadCommand(channel, layer, clip), cancellationToken);

    /// <summary>
    /// Sends a CALLBG command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CallBgAsync(int channel, int layer, string clip, CancellationToken cancellationToken) =>
        SendAsync(new CallBgCommand(channel, layer, clip), cancellationToken);

    /// <summary>
    /// Sends a PAUSE command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask PauseAsync(int channel, int layer, CancellationToken cancellationToken) =>
        SendAsync(new PauseCommand(channel, layer), cancellationToken);

    /// <summary>
    /// Sends a RESUME command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ResumeAsync(int channel, int layer, CancellationToken cancellationToken) =>
        SendAsync(new ResumeCommand(channel, layer), cancellationToken);

    /// <summary>
    /// Sends a CLEAR command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ClearAsync(int channel, int layer, CancellationToken cancellationToken) =>
        SendAsync(new ClearCommand(channel, layer), cancellationToken);

    /// <summary>
    /// Sends a CALL command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CallAsync(int channel, int layer, string clip, CancellationToken cancellationToken) =>
        SendAsync(new CallCommand(channel, layer, clip), cancellationToken);

    /// <summary>
    /// Sends a SWAP command.
    /// </summary>
    /// <param name="channel">The source channel.</param>
    /// <param name="layer">The source layer.</param>
    /// <param name="otherChannel">The target channel.</param>
    /// <param name="otherLayer">The target layer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask SwapAsync(int channel, int layer, int otherChannel, int otherLayer, CancellationToken cancellationToken) =>
        SendAsync(new SwapCommand(channel, layer, otherChannel, otherLayer), cancellationToken);

    /// <summary>
    /// Sends an ADD command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask AddAsync(int channel, int layer, string clip, CancellationToken cancellationToken) =>
        SendAsync(new AddCommand(channel, layer, clip), cancellationToken);

    /// <summary>
    /// Sends a REMOVE command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask RemoveAsync(int channel, int layer, string clip, CancellationToken cancellationToken) =>
        SendAsync(new RemoveCommand(channel, layer, clip), cancellationToken);

    /// <summary>
    /// Sends an APPLY command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ApplyAsync(int channel, int layer, string clip, CancellationToken cancellationToken) =>
        SendAsync(new ApplyCommand(channel, layer, clip), cancellationToken);

    /// <summary>
    /// Sends a PRINT command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask PrintAsync(int channel, int layer, string clip, CancellationToken cancellationToken) =>
        SendAsync(new PrintCommand(channel, layer, clip), cancellationToken);

    /// <summary>
    /// Sends a CLEAR ALL command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ClearAllAsync(CancellationToken cancellationToken) =>
        SendAsync(new ClearAllCommand(), cancellationToken);

    /// <summary>
    /// Sends a SET command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="key">The property name.</param>
    /// <param name="value">The property value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask SetAsync(int channel, int layer, string key, string value, CancellationToken cancellationToken) =>
        SendAsync(new SetCommand(channel, layer, key, value), cancellationToken);

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
    public async ValueTask ConnectAsync(CancellationToken cancellationToken)
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
    public async ValueTask StartOscAsync(int port, CancellationToken cancellationToken)
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
    public async ValueTask SubscribeOscAsync(int port, CancellationToken cancellationToken)
    {
        await SendAsync(new OscSubscribeCommand(port), cancellationToken);
    }

    /// <summary>
    /// Stops the OSC listener.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous stop operation.</returns>
    public ValueTask StopOscAsync(CancellationToken cancellationToken) =>
        _oscTransport is null ? ValueTask.CompletedTask : _oscTransport.StopAsync(cancellationToken);

    /// <summary>
    /// Sends a play command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public async ValueTask PlayAsync(int channel, int layer, string clip, CancellationToken cancellationToken)
    {
        await SendAsync(new PlayCommand(channel, layer, clip), cancellationToken);
    }

    /// <summary>
    /// Sends a load background command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public async ValueTask LoadBackgroundAsync(int channel, int layer, string clip, CancellationToken cancellationToken)
    {
        await SendAsync(new LoadBackgroundCommand(channel, layer, clip), cancellationToken);
    }

    /// <summary>
    /// Sends a stop command.
    /// </summary>
    /// <param name="channel">The target channel.</param>
    /// <param name="layer">The target layer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public async ValueTask StopAsync(int channel, int layer, CancellationToken cancellationToken)
    {
        await SendAsync(new StopCommand(channel, layer), cancellationToken);
    }

    /// <summary>
    /// Gets the server version string reported by AMCP.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The server version string.</returns>
    public async ValueTask<string> GetVersionAsync(CancellationToken cancellationToken)
    {
        var response = await QueryAsync(new VersionCommand("SERVER"), cancellationToken);
        return response.Lines.FirstOrDefault() ?? string.Empty;
    }

    /// <summary>
    /// Gets the response returned by the INFO command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> InfoAsync(CancellationToken cancellationToken) =>
        QueryAsync(new InfoCommand(), cancellationToken);

    /// <summary>
    /// Gets the response returned by the INFO CONFIG command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> InfoConfigAsync(CancellationToken cancellationToken) =>
        QueryAsync(new InfoConfigCommand(), cancellationToken);

    /// <summary>
    /// Gets the response returned by the INFO PATHS command.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<AmcpResponse> InfoPathsAsync(CancellationToken cancellationToken) =>
        QueryAsync(new InfoPathsCommand(), cancellationToken);

    /// <summary>
    /// Gets the media listing reported by AMCP.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The media listing lines.</returns>
    public async ValueTask<IReadOnlyList<string>> GetMediaFilesAsync(CancellationToken cancellationToken)
    {
        var response = await QueryAsync(new ListMediaFilesCommand(), cancellationToken);
        return response.Lines;
    }

    internal async ValueTask SendAsync(AmcpCommand command, CancellationToken cancellationToken)
    {
        await QueryAsync(command, cancellationToken);
    }

    internal async ValueTask<AmcpResponse> QueryAsync(AmcpCommand command, CancellationToken cancellationToken)
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

    internal ValueTask PublishAsync(CasparEvent evt, CancellationToken cancellationToken)
    {
        if (_lastPublishedOscEvent is not null && EqualityComparer<CasparEvent>.Default.Equals(_lastPublishedOscEvent, evt))
        {
            return ValueTask.CompletedTask;
        }

        _lastPublishedOscEvent = evt;
        _state.Apply(evt);
        return _eventChannel.Writer.WriteAsync(evt, cancellationToken);
    }

    private async ValueTask OnOscPacketAsync(ReadOnlyMemory<byte> packet, CancellationToken cancellationToken)
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
