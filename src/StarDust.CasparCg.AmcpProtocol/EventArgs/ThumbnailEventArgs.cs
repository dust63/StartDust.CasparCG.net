using StarDust.CasparCG.Models.Media;
using System;
using System.Collections.Generic;

namespace StarDust.CasparCG.AmcpProtocol
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
