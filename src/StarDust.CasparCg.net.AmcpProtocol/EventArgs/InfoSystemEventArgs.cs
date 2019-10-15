using StarDust.CasparCG.net.Models.Info;
using System;

namespace StarDust.CasparCG.net.AmcpProtocol
{

    /// <summary>
    /// Info system received from the server
    /// </summary>
    public class InfoSystemEventArgs : EventArgs
    {

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="systemInfo"></param>
        public InfoSystemEventArgs(SystemInfo systemInfo)
        {
            this.SystemInfo = systemInfo;
        }


        /// <summary>
        /// System info where the server is installed
        /// </summary>
        public SystemInfo SystemInfo { get; }


    }
}
