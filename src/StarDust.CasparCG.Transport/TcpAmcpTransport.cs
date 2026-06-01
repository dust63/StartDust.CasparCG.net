using System.Net.Sockets;
using System.Text;

namespace StarDust.CasparCG.Transport;

/// <summary>
/// Sends AMCP commands over a TCP connection.
/// </summary>
public sealed class TcpAmcpTransport : IAmcpTransport, IAsyncDisposable
{
    private readonly string _host;
    private readonly int _port;
    private TcpClient? _client;
    private NetworkStream? _stream;

    /// <summary>
    /// Initializes a new TCP AMCP transport.
    /// </summary>
    /// <param name="host">The remote host.</param>
    /// <param name="port">The remote port.</param>
    public TcpAmcpTransport(string host, int port)
    {
        _host = host;
        _port = port;
    }

    /// <inheritdoc />
    public async ValueTask ConnectAsync(CancellationToken cancellationToken)
    {
        _client = new TcpClient();
        await _client.ConnectAsync(_host, _port, cancellationToken);
        _stream = _client.GetStream();
    }

    /// <inheritdoc />
    public ValueTask DisconnectAsync(CancellationToken cancellationToken)
    {
        _stream?.Dispose();
        _stream = null;
        _client?.Dispose();
        _client = null;
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public async ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(_stream);

        var bytes = Encoding.UTF8.GetBytes(commandText);
        await _stream.WriteAsync(bytes, cancellationToken);
        return "202 OK";
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync() => DisconnectAsync(CancellationToken.None);
}
