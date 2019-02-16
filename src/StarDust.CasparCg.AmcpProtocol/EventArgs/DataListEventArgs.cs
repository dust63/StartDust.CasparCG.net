using System;
using System.Collections.Generic;

namespace StarDust.CasparCG.AmcpProtocol
{
  public class DataListEventArgs : EventArgs
  {
    public IList<string> Datas { get; private set; }

    public DataListEventArgs(IList<string> datas)
    {
      this.Datas = datas;
    }
  }
}
