using System;

namespace StarDust.CasparCG.net.Connection
{
  public class ClientConnectionEventArgs : ConnectionEventArgs
  {
    internal ClientConnectionEventArgs(string host, int port, bool connected, Exception ex, bool remote)
      : base(host, port, connected, ex)
    {
      this.Remote = remote;
    }

    public bool Remote { get; private set; }
  }
}
