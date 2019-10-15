using StarDust.CasparCG.net.Models.Info;
using System;

namespace StarDust.CasparCG.net.AmcpProtocol
{

    /// <summary>
    /// Template info received from the server
    /// </summary>
    public class TemplateInfoEventArgs : EventArgs
    {

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="templateInfo"></param>
        public TemplateInfoEventArgs(TemplateInfo templateInfo)
        {
            TemplateInfo = templateInfo;
        }

        /// <summary>
        /// Template information
        /// </summary>
        public TemplateInfo TemplateInfo { get; }


    }
}
