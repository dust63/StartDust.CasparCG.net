using StarDust.CasparCG.Models.Media;
using System;
using System.Collections.Generic;

namespace StarDust.CasparCG.AmcpProtocol
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
