using System;

namespace StarDust.CasparCG.AmcpProtocol
{
  public class DataRetrieveEventArgs : EventArgs
  {
    public DataRetrieveEventArgs(string data)
    {
      this.Data = data;
    }

    public string Data { get; private set; }
  }
}
