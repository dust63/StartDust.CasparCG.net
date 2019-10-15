using StarDust.CasparCG.net.Models.Media;
using System;
using System.Collections.Generic;

namespace StarDust.CasparCG.net.AmcpProtocol
{
    /// <summary>
    /// When we receive thumbnail list from the server
    /// </summary>
    public class ThumbnailsListEventArgs : EventArgs
    {

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="thumbnails"></param>
        public ThumbnailsListEventArgs(List<Thumbnail> thumbnails)
        {
            this.Thumbnails = thumbnails;
        }


        /// <summary>
        /// List of the thumbnails present on the server
        /// </summary>
        public List<Thumbnail> Thumbnails { get; }
    }
}
