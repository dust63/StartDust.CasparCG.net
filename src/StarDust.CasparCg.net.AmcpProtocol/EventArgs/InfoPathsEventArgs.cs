using StarDust.CasparCG.net.Models.Info;
using System;

namespace StarDust.CasparCG.net.AmcpProtocol
{
    public class InfoPathsEventArgs : EventArgs
    {
        public InfoPathsEventArgs(PathsInfo pathsInfo)
        {
            this.PathsInfo = pathsInfo;
        }

        public PathsInfo PathsInfo { get; }


    }
}
