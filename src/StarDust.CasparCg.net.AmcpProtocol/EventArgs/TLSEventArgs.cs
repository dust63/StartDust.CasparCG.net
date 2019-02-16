using StarDust.CasparCG.net.Models;
using System;
using System.Collections.Generic;

namespace StarDust.CasparCG.net.AmcpProtocol
{
  public class TLSEventArgs : EventArgs
  {
    public TLSEventArgs(List<TemplateBaseInfo> templates)
    {
      this.Templates = templates;
    }

    public List<TemplateBaseInfo> Templates { get; }
  }
}
