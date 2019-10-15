using System;
using System.Collections.Generic;

namespace StarDust.CasparCG.net.AmcpProtocol
{

    /// <summary>
    /// Used when data list command was received
    /// </summary>
    public class DataListEventArgs : EventArgs
    {

        /// <summary>
        /// Data present on the server
        /// </summary>
        public IList<string> Data { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="data"></param>
        public DataListEventArgs(IList<string> data)
        {
            this.Data = data;
        }
    }
}
