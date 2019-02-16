using StarDust.CasparCG.Models.Info;
using System;

namespace StarDust.CasparCG.AmcpProtocol
{
    public class InfoSystemEventArgs : EventArgs
    {
        public InfoSystemEventArgs(SystemInfo systemInfo)
        {
            this.SystemInfo = systemInfo;
        }

        public SystemInfo SystemInfo { get; }


    }
}
