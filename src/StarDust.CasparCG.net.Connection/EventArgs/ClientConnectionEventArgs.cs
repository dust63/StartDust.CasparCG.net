using System;

namespace StarDust.CasparCG.net.Connection
{

    /// <summary>
    /// Event data when CasparCG connection status changed
    /// </summary>
    public class ClientConnectionEventArgs : ConnectionEventArgs
    {
        internal ClientConnectionEventArgs(string host, int port, bool connected, Exception ex, bool remote)
          : base(host, port, connected, ex)
        {
            this.Remote = remote;
        }

        /// <summary>
        /// Is remote
        /// </summary>
        public bool Remote { get; private set; }
    }
}
