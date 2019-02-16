using StarDust.CasparCG.net.Models.Info;
using System;
using System.Collections.Generic;

namespace StarDust.CasparCG.net.AmcpProtocol
{
    public class InfoThreadsEventArgs : EventArgs
    {
        public InfoThreadsEventArgs(List<ThreadsInfo> threadsinfos)
        {
            this.ThreadsInfo = threadsinfos;
        }

        public List<ThreadsInfo> ThreadsInfo { get; }


    }
}
