using StarDust.CasparCG.Models.Info;
using System;

namespace StarDust.CasparCG.AmcpProtocol
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
