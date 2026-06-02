using System.Net.Sockets;
using System.Text;
using StarDust.CasparCG.Protocol.Amcp;

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
    private StreamReader? _reader;
    private StreamWriter? _writer;

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
        _reader = new StreamReader(_stream, Encoding.UTF8, leaveOpen: true);
        _writer = new StreamWriter(_stream, Encoding.UTF8, leaveOpen: true)
        {
            AutoFlush = true
        };
    }

    /// <inheritdoc />
    public ValueTask DisconnectAsync(CancellationToken cancellationToken)
    {
        _writer?.Dispose();
        _writer = null;
        _reader?.Dispose();
        _reader = null;
        _stream?.Dispose();
        _stream = null;
        _client?.Dispose();
        _client = null;
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public async ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(_writer);
        ArgumentNullException.ThrowIfNull(_reader);

        await _writer.WriteAsync(commandText.AsMemory(), cancellationToken);

        var response = new StringBuilder();
        var statusLine = await _reader.ReadLineAsync(cancellationToken);
        if (statusLine is null)
        {
            throw new IOException("The AMCP server closed the connection before sending a response.");
        }

        response.Append(statusLine).Append("\r\n");

        if (IsMultiLineResponse(statusLine))
        {
            while (true)
            {
                var line = await _reader.ReadLineAsync(cancellationToken);
                if (line is null)
                {
                    break;
                }

                response.Append(line).Append("\r\n");
                if (line.Length == 0)
                {
                    break;
                }
            }
        }

        return response.ToString();
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync() => DisconnectAsync(CancellationToken.None);

    private static bool IsMultiLineResponse(string statusLine)
    {
        if (statusLine.Length < 3 || !int.TryParse(statusLine[..3], out var statusCode))
        {
            return false;
        }

        return statusCode is 200 or 201;
    }
}
