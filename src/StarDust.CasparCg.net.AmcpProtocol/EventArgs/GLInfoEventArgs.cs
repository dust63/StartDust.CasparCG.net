using StarDust.CasparCG.net.Models.Info;
using System;

namespace StarDust.CasparCG.net.AmcpProtocol
{
    public class GLInfoEventArgs : EventArgs
    {
        public GLInfoEventArgs(GLInfo glInfo)
        {
            this.GLInfo = glInfo;
        }

        public GLInfo GLInfo { get; }


    }
}
