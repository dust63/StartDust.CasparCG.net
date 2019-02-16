using System;

namespace StarDust.CasparCG.net.AmcpProtocol
{
  public class LoadEventArgs : EventArgs
  {
    public string ClipName { get; set; }

    public LoadEventArgs(string clipName)
    {
      this.ClipName = clipName;
    }
  }
}
