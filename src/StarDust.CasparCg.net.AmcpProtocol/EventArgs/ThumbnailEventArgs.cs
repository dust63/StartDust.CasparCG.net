using StarDust.CasparCG.net.Models.Media;
using System;
using System.Collections.Generic;

namespace StarDust.CasparCG.net.AmcpProtocol
{
    public class ThumbnailsListEventArgs : EventArgs
  {
    public ThumbnailsListEventArgs(List<Thumbnail> thumbnails)
    {
      this.Thumbnails = thumbnails;
    }

    public List<Thumbnail> Thumbnails { get; }
  }
}
