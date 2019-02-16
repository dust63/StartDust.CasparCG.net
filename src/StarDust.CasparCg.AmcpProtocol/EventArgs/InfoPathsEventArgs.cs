using StarDust.CasparCG.Models.Info;
using System;

namespace StarDust.CasparCG.AmcpProtocol
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
