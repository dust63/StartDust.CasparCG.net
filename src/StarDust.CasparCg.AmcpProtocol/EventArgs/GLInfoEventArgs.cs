using StarDust.CasparCG.Models.Info;
using System;

namespace StarDust.CasparCG.AmcpProtocol
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
