using StarDust.CasparCG.net.Models.Media;
using System;
using System.Collections.Generic;

namespace StarDust.CasparCG.net.AmcpProtocol
{
  public class CLSEventArgs : EventArgs
  {
    public CLSEventArgs(List<MediaInfo> medias)
    {
      this.Medias = medias;
    }

    public List<MediaInfo> Medias { get; }
  }
}
