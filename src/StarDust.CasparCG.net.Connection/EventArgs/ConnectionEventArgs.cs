using System;

namespace StarDust.CasparCG.net.Connection
{
    /// <summary>
    /// Event when CasparCG Server is connected or not
    /// </summary>
    public class ConnectionEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host">Hostname of the CasparCG Server</param>
        /// <param name="port">Port of the Amcp TCP Socket</param>
        /// <param name="connected">Is connected or not</param>
        public ConnectionEventArgs(string host, int port, bool connected)
        {
            this.Hostname = host;
            this.Port = port;
            this.Connected = connected;
            this.Exception = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="host">Hostname of the CasparCG Server</param>
        /// <param name="port">Port of the Amcp TCP Socket</param>
        /// <param name="connected">Is connected or not</param>
        /// <param name="exception">Exception is something go wrong</param>
        public ConnectionEventArgs(string host, int port, bool connected, Exception exception)
        {
            this.Hostname = host;
            this.Port = port;
            this.Connected = connected;
            this.Exception = exception;
        }

        /// <summary>
        /// CasparCG Server Hostname that we want to connect
        /// </summary>
        public string Hostname { get; private set; }

        /// <summary>
        /// CasparCG Amcp Tcp socket port.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Indicate is the Server is connected
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        /// Exception sent if some problem during the connection
        /// </summary>
        public Exception Exception { get; private set; }
    }
}
