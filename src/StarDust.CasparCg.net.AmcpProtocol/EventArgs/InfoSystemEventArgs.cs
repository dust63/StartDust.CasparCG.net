using StarDust.CasparCG.net.Models.Info;
using System;

namespace StarDust.CasparCG.net.AmcpProtocol
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
