using SimpleTCP;
using System;
using System.Text.RegularExpressions;

namespace StarDust.CasparCG.net.Connection
{

    /// <summary>
    /// <see cref="IServerConnection"/>
    /// </summary>
    public class ServerConnection : IServerConnection
    {

        #region Fields

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
        public CasparCGConnectionSettings ConnectionSettings { get; }


        /// <inheritdoc cref="IServerConnection"/>
        public bool IsConnected => Client != null && Client.IsConnected;

        /// <inheritdoc/>
        public event EventHandler<ConnectionEventArgs> ConnectionStateChanged;

        #endregion

        #region Contructor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="settings">Settings to connect to the CasparCG Server</param>
        public ServerConnection(CasparCGConnectionSettings settings)
        {
            ConnectionSettings = settings;

            Client.AutoTrimStrings = true;
            Client.SendDelimiter = LineDelimiter;
            Client.AutoReconnect = settings.AutoConnect;
            Client.CheckConnectivityInterval = settings.ReconnectInterval;

            Client.ConnectedEvent += Client_ConnectedEvent;
            Client.DisconnectedEvent += Client_DisconnectedEvent;
            Client.DataReceived += Client_DataReceived;
        }

        #endregion

        #region Public Methods


        /// <inheritdoc cref="IServerConnection"/>
        public void Connect()
        {
            if (IsConnected)
                return;

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
            Regex.Replace(@"^\\$", command, "\\\\");          
            Regex.Replace("^\r\n$", command, "\n");           
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
