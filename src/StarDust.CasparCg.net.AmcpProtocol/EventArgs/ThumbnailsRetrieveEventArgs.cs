using StarDust.CasparCG.net.Models.Media;
using System;

namespace StarDust.CasparCG.net.AmcpProtocol
{

    /// <summary>
    /// When we retrieve a specific thumbnail
    /// </summary>
    public class ThumbnailsRetrieveEventArgs : EventArgs
    {

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="base64Image"></param>
        public ThumbnailsRetrieveEventArgs(string base64Image)
        {
            this.Base64Image = base64Image;
        }


        /// <summary>
        /// The base 64 code for the thumbnail. Use a converter to display the image.
        /// </summary>
        public string Base64Image { get; }
    }
}
