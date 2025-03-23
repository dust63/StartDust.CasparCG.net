using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace StarDust.CasparCG.net.Connection
{
    /// <summary>
    /// <see cref="IServerConnection"/>
    /// </summary>
    public class ServerConnection : IServerConnection
    {
        #region Fields
        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(0);
        private readonly SimpleTcpClient Client = new SimpleTcpClient();
        private readonly object lockObject = new object();

        #endregion

        #region Properties

        /// <inheritdoc cref="IServerConnection"/>
        public string CommandDelimiter { get; set; } = "\r\n";

        /// <inheritdoc cref="IServerConnection"/>
        public string LineDelimiter { get; set; } = "\n";

        /// <inheritdoc cref="IServerConnection"/>
        public event EventHandler<DatasReceivedEventArgs> DataReceived;

        /// <inheritdoc cref="IServerConnection"/>
        public CasparCGConnectionSettings ConnectionSettings { get; private set; }

        /// <inheritdoc cref="IServerConnection"/>
        public bool IsConnected => Client != null && Client.IsConnected;

        /// <inheritdoc/>
        public event EventHandler<ConnectionEventArgs> ConnectionStateChanged;

        #endregion

        #region Contructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ServerConnection()
        {
            Client.AutoTrimStrings = true;
            Client.SendDelimiter = LineDelimiter;
            Client.ConnectedEvent += Client_ConnectedEvent;
            Client.DisconnectedEvent += Client_DisconnectedEvent;
            Client.DataReceived += Client_DataReceived;
        }

        #endregion

        #region Public Methods

        /// <inheritdoc cref="IServerConnection"/>
        public void Connect(CasparCGConnectionSettings settings)
        {
            if (IsConnected && settings.Equals(ConnectionSettings))
                Disconnect();
            ConnectionSettings = settings;
            Connect();
        }

        /// <inheritdoc cref="IServerConnection"/>
        public void Connect()
        {
            if (IsConnected)
                return;
            if (ConnectionSettings == null)
                throw new InvalidOperationException("No settings found. Please set the connection settings first");
            Client.AutoReconnect = ConnectionSettings.AutoConnect;
            Client.CheckConnectivityInterval = ConnectionSettings.ReconnectInterval;
            Client.Connect(ConnectionSettings.Hostname, ConnectionSettings.Port);
        }

        /// <inheritdoc cref="IServerConnection"/>
        public void Disconnect()
        {
            if (!Client.IsConnected)
                return;
            Client.Disconnect();
        }

        /// <inheritdoc cref="IServerConnection"/>
        public void Send(byte[] data)
        {
            if (!IsConnected)
                return;
            lock (lockObject)
            {
                Client.Send(data);
            }
        }


        /// <summary>
        /// Replace \ by \\ to be send to CasparCG
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private static string EscapeChars(string command)
        {
            Regex.Replace(command, @"^\\$", "\\\\");
            Regex.Replace(command, "^\r\n$", "\n");
            return command;
        }

        /// <inheritdoc cref="IServerConnection"/>
        public void SendString(string str)
        {
            Client.SendLine(EscapeChars(str) + CommandDelimiter);
        }

        /// <inheritdoc cref="IServerConnection"/>
        public string SendStringWithResult(string str, TimeSpan timeout)
        {
            if (!IsConnected)
                return string.Empty;

            lock (lockObject)
            {
                return Client.SendLineAndGetReply(EscapeChars(str) + CommandDelimiter, timeout)?.MessageString;
            }
        }

        public async Task SendAsync(byte[] data, CancellationToken cancellationToken)
        {
            if (!IsConnected)
                return;

            await Client.SendAsync(data, cancellationToken);
        }

        public async Task SendStringAsync(string command, CancellationToken cancellationToken)
        {
            if (!IsConnected)
                return;

            await Client.SendLineAsync(EscapeChars(command) + CommandDelimiter, cancellationToken);
        }
       
        public async Task<string> SendStringWithResultAsync(string command, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (!IsConnected)
                return string.Empty;

            await _semaphoreSlim.WaitAsync(cancellationToken);
            try
            {
                var message = await Client.SendLineAndGetReplyAsync(EscapeChars(command) + CommandDelimiter, timeout, cancellationToken);
                return message.MessageString;
            } 
            finally
            {
                _semaphoreSlim.Release();
            }
        }
        #endregion

        #region Private methods

        private void Client_DisconnectedEvent(object sender, EventArgs e)
        {
            ConnectionStateChanged?.Invoke(this, new ConnectionEventArgs(ConnectionSettings.Hostname, ConnectionSettings.Port, false));
        }

        private void Client_ConnectedEvent(object sender, EventArgs e)
        {
            ConnectionStateChanged?.Invoke(this, new ConnectionEventArgs(ConnectionSettings.Hostname, ConnectionSettings.Port, true));
        }


        private void Client_DataReceived(object sender, Message e)
        {
            DataReceived?.Invoke(this, new DatasReceivedEventArgs(e?.MessageString));
        }
        #endregion
    }
}
