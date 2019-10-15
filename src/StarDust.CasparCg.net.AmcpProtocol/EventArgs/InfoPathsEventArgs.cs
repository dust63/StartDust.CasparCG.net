using StarDust.CasparCG.net.Models.Info;
using System;

namespace StarDust.CasparCG.net.AmcpProtocol
{
    /// <summary>
    /// Info paths received from the server
    /// </summary>
    public class InfoPathsEventArgs : EventArgs
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="pathsInfo"></param>
        public InfoPathsEventArgs(PathsInfo pathsInfo)
        {
            this.PathsInfo = pathsInfo;
        }

        /// <summary>
        /// Paths info of the server
        /// </summary>
        public PathsInfo PathsInfo { get; }


    }
}
