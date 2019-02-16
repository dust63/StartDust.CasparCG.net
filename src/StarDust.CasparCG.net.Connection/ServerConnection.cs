using SimpleTCP;
using System;

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

        /// <inheritdoc cref=""/>
        public string CommandDelimiter { get; set; } = "\r\n";


        /// <inheritdoc cref=""/>
        public string LineDelimiter { get; set; } = "\n";


        /// <inheritdoc cref=""/>
        public event EventHandler<DatasReceivedEventArgs> DatasReceived;


        /// <inheritdoc cref=""/>
        public CasparCGConnectionSettings ConnectionSettings { get; }


        /// <inheritdoc cref=""/>
        public bool IsConnected
        {
            get
            {
                if (Client == null)
                    return false;
                return Client.IsConnected;
            }
        }

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
            Client.CheckConnectivityIntervall = settings.ReconnectInterval;

            Client.ConnectedEvent += Client_ConnectedEvent;
            Client.DisconnectedEvent += Client_DisconnectedEvent;
            Client.DataReceived += Client_DataReceived;
        }

        #endregion

        #region Public Methods


        /// <inheritdoc cref=""/>
        public void Connect()
        {
            if (IsConnected)
                return;

            Client.Connect(ConnectionSettings.Hostname, ConnectionSettings.Port);
        }

        /// <inheritdoc cref=""/>
        public void Disconnect()
        {
            if (!Client.IsConnected)
                return;
            Client.Disconnect();
        }

        /// <inheritdoc cref=""/>
        public void Send(byte[] data)
        {
            if (!IsConnected)
                return;
            lock (lockObject)
            {
                Client.Send(data);
            }
        }

        private static string EscapeFilename(string command)
        {
            return command.Replace("\\", "\\\\");
        }


        /// <inheritdoc cref=""/>
        public void SendString(string str)
        {
            string datas = SendStringWithResult(EscapeFilename(str) + CommandDelimiter, TimeSpan.Zero);
            DatasReceived?.Invoke(this, new DatasReceivedEventArgs(datas));
        }

        /// <inheritdoc cref=""/>
        public string SendStringWithResult(string str, TimeSpan timeout)
        {
            if (IsConnected)
                lock (lockObject)
                {
                    return Client.SendLineAndGetReply(EscapeFilename(str) + CommandDelimiter, timeout)?.MessageString;
                }

            return string.Empty;
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
            DatasReceived?.Invoke(this, new DatasReceivedEventArgs(e?.MessageString));
        }

        #endregion
    }
}
