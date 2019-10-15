using StarDust.CasparCG.net.Models.Info;
using System;
using System.Collections.Generic;

namespace StarDust.CasparCG.net.AmcpProtocol
{

    /// <summary>
    /// Info on the threads received from the server
    /// </summary>
    public class InfoThreadsEventArgs : EventArgs
    {

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="threadsInfo"></param>
        public InfoThreadsEventArgs(List<ThreadsInfo> threadsInfo)
        {
            this.ThreadsInfo = threadsInfo;
        }


        /// <summary>
        /// List of the threads information
        /// </summary>
        public List<ThreadsInfo> ThreadsInfo { get; }


    }
}
