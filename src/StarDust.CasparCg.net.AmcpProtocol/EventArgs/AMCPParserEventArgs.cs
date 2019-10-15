using System;
using System.Collections.Generic;

namespace StarDust.CasparCG.net.AmcpProtocol
{

    /// <summary>
    /// Event raised when data was sent by the server
    /// </summary>
    public class AMCPEventArgs : EventArgs
    {
        /// <summary>
        /// Command received
        /// </summary>
        public AMCPCommand Command { get; set; } = AMCPCommand.None;

        /// <summary>
        /// Error code received
        /// </summary>
        public AMCPError Error { get; set; } = AMCPError.None;

        /// <summary>
        /// Data received
        /// </summary>
        public List<string> Data { get; } = new List<string>();
    }
}
