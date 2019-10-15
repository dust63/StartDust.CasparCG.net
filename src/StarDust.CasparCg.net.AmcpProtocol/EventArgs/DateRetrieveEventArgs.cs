using System;

namespace StarDust.CasparCG.net.AmcpProtocol
{

    /// <summary>
    /// When we received a data from the server
    /// </summary>
    public class DataRetrieveEventArgs : EventArgs
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="data"></param>
        public DataRetrieveEventArgs(string data)
        {
            this.Data = data;
        }

        /// <summary>
        /// A data store on the server
        /// </summary>
        public string Data { get; private set; }
    }
}
