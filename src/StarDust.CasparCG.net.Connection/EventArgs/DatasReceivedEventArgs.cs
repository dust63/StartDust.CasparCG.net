using System;

namespace StarDust.CasparCG.net.Connection
{
  public class DatasReceivedEventArgs : EventArgs
  {
    public DatasReceivedEventArgs(string datas)
    {
      this.Datas = datas;
    }

    public string Datas { get; }
  }
}
