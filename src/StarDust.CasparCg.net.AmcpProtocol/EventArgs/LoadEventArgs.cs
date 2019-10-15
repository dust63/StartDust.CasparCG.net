using System;

namespace StarDust.CasparCG.net.AmcpProtocol
{

    /// <summary>
    /// When we received load message from the server
    /// </summary>
    public class LoadEventArgs : EventArgs
    {

        /// <summary>
        /// Clip name that are loaded by the server
        /// </summary>
        public string ClipName { get; set; }


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="clipName"></param>
        public LoadEventArgs(string clipName)
        {
            this.ClipName = clipName;
        }
    }
}
