using System;

namespace StarDust.CasparCG.net.Connection
{
    /// <summary>
    /// Event data when data received event raised
    /// </summary>
    public class DatasReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="datas"></param>
        public DatasReceivedEventArgs(string datas)
        {
            Datas = datas;
        }

        /// <summary>
        /// Caspar CG Data
        /// </summary>
        public string Datas { get; }
    }
}
