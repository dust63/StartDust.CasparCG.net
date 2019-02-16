using System;

namespace StarDust.CasparCG.AmcpProtocol
{
  public class VersionEventArgs : EventArgs
  {
    public VersionEventArgs(string version)
    {
      this.Version = version;
    }

    public string Version { get; private set; }
  }
}
