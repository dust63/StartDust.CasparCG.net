using System;
using System.Threading;
using System.Threading.Tasks;

namespace StarDust.CasparCG.net.Connection
{
    /// <summary>
    /// Represent the tcp connection to the CasparCG Server
    /// </summary>
    public interface IServerConnection
    {
        /// <summary>
        /// End command delimiter
        /// </summary>
        string CommandDelimiter { get; set; }

        /// <summary>
        /// Line delimiter
        /// </summary>
        string LineDelimiter { get; set; }

        /// <summary>
        /// Occurs when Server are connected or disconnected
        /// </summary>
        event EventHandler<ConnectionEventArgs> ConnectionStateChanged;

        /// <summary>
        /// Occurs when Server connection received data
        /// </summary>
        event EventHandler<DatasReceivedEventArgs> DataReceived;

        /// <summary>
        /// Settings to connect to CasparCg Server
        /// </summary>
        CasparCGConnectionSettings ConnectionSettings { get; }

        /// <summary>
        /// Is the CasparCG Server is connected or not
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Initialize the tcp connection to the CasparCG Server with settings
        /// </summary>
        void Connect(CasparCGConnectionSettings settings);

        /// <summary>
        /// Initialize the tcp connection to the CasparCG Server
        /// </summary>
        void Connect();

        /// <summary>
        /// Disconnect from CasparCG Server
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Send tcp data as byte
        /// </summary>
        /// <param name="data">Data to send</param>
        void Send(byte[] data);

        /// <summary>
        /// Send Tcp data as string
        /// </summary>
        /// <param name="command">Command to send</param>
        void SendString(string command);

        /// <summary>
        /// Send data and wait for a Replys
        /// </summary>
        /// <param name="command">Command to send</param>
        /// <param name="timeout">max time to wait a response</param>
        /// <returns></returns>
        string SendStringWithResult(string command, TimeSpan timeout);

        /// <summary>
        /// Send tcp data as byte
        /// </summary>
        /// <param name="data">Data to send</param>
        /// <param name="cancellationToken">The token to monitor cancellation request</param>
        Task SendAsync(byte[] data, CancellationToken cancellationToken);

        /// <summary>
        /// Send Tcp data as string
        /// </summary>
        /// <param name="command">Command to send</param>
        /// <param name="cancellationToken">The token to monitor cancellation request</param>
        Task SendStringAsync(string command, CancellationToken cancellationToken);

        /// <summary>
        /// Send data and wait for a Replys
        /// </summary>
        /// <param name="command">Command to send</param>
        /// <param name="timeout">max time to wait a response</param>
        /// <param name="cancellationToken">The token to monitor cancellation request</param>
        /// <returns></returns>
        Task<string> SendStringWithResultAsync(string command, TimeSpan timeout, CancellationToken cancellationToken);
    }
}
