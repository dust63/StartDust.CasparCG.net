using StarDust.CasparCG.Models.Info;
using System;
using System.Collections.Generic;

namespace StarDust.CasparCG.AmcpProtocol
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
