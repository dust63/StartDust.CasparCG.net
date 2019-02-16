using StarDust.CasparCG.net.Models.Info;
using System;

namespace StarDust.CasparCG.net.AmcpProtocol
{
    public class TemplateInfoEventArgs : EventArgs
    {
        public TemplateInfoEventArgs(TemplateInfo templateInfo)
        {
            TemplateInfo = templateInfo;
        }

        public TemplateInfo TemplateInfo { get; }


    }
}
