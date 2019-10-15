using StarDust.CasparCG.net.Models.Media;
using System;
using System.Collections.Generic;

namespace StarDust.CasparCG.net.AmcpProtocol
{

    /// <summary>
    /// Use when CLS command received
    /// </summary>
    public class CLSEventArgs : EventArgs
  {
      /// <summary>
      /// Ctor
      /// </summary>
      /// <param name="medias"></param>
    public CLSEventArgs(List<MediaInfo> medias)
    {
      this.Medias = medias;
    }


    /// <summary>
    /// List of the medias present on the server
    /// </summary>
    public List<MediaInfo> Medias { get; }
  }
}
