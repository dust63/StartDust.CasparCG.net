using StarDust.CasparCG.net.Models.Info;
using System;

namespace StarDust.CasparCG.net.AmcpProtocol
{
    /// <summary>
    /// When we received GL Info from the server
    /// </summary>
    public class GLInfoEventArgs : EventArgs
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="glInfo"></param>
        public GLInfoEventArgs(GLInfo glInfo)
        {
            this.GLInfo = glInfo;
        }

        /// <summary>
        /// Gl info
        /// </summary>
        public GLInfo GLInfo { get; }
    }
}
