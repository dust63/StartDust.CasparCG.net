using StarDust.CasparCG.Models;
using System;
using System.Collections.Generic;

namespace StarDust.CasparCG.AmcpProtocol
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
