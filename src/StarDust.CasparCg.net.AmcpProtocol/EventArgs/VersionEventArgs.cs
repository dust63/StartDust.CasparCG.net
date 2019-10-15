using System;

namespace StarDust.CasparCG.net.AmcpProtocol
{
    /// <summary>
    /// Use when we received the version of the server
    /// </summary>
    public class VersionEventArgs : EventArgs
    {

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="version"></param>
        public VersionEventArgs(string version)
        {
            this.Version = version;
        }


        /// <summary>
        /// Version of the CasparCG Server
        /// </summary>
        public string Version { get; private set; }
    }
}
