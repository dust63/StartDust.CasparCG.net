using System.Net;
using System.Net.Sockets;
using System.Text;

namespace StarDust.CasparCG.Testing.DummyServer;

/// <summary>
/// Hosts a minimal loopback TCP server for integration testing.
/// </summary>
public sealed class DummyCasparServer : IAsyncDisposable
{
    private static readonly UTF8Encoding Utf8NoBom = new(false);

    private readonly TcpListener _listener;
    private readonly DummyScenario _scenario;
    private readonly List<string> _receivedCommands = new();
    private readonly CancellationTokenSource _shutdown = new();
    private readonly Task _acceptLoop;

    private DummyCasparServer(TcpListener listener, DummyScenario scenario)
    {
        _listener = listener;
        _scenario = scenario;
        _acceptLoop = AcceptLoopAsync(_shutdown.Token);
    }

    /// <summary>
    /// Gets the AMCP port bound by the server.
    /// </summary>
    public int AmcpPort => ((IPEndPoint)_listener.LocalEndpoint).Port;

    /// <summary>
    /// Gets the placeholder OSC port for the server.
    /// </summary>
    public int OscPort => 6250;

    /// <summary>
    /// Gets the commands received by the server.
    /// </summary>
    public IReadOnlyList<string> ReceivedCommands => _receivedCommands;

    /// <summary>
    /// Starts a dummy server with the provided scenario.
    /// </summary>
    /// <param name="scenario">The scripted reply scenario.</param>
    /// <param name="cancellationToken">The startup cancellation token.</param>
    /// <returns>The started server.</returns>
    public static ValueTask<DummyCasparServer> StartAsync(DummyScenario scenario, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ValueTask.FromResult(new DummyCasparServer(listener, scenario));
    }

    private async Task AcceptLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var client = await _listener.AcceptTcpClientAsync(cancellationToken);
            await using var stream = client.GetStream();
            using var reader = new StreamReader(stream, Utf8NoBom);
            using var writer = new StreamWriter(stream, Utf8NoBom) { AutoFlush = true };

            while (!cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync(cancellationToken);
                if (line is null)
                {
                    break;
                }

                _receivedCommands.Add(line);

                if (_scenario.TryGetReply(line, out var reply))
                {
                    await writer.WriteAsync(reply);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
        catch (ObjectDisposedException)
        {
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        _shutdown.Cancel();
        _listener.Stop();

        try
        {
            await _acceptLoop;
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _shutdown.Dispose();
        }
    }
}
