using StarDust.CasparCG.Models.Media;
using System;

namespace StarDust.CasparCG.AmcpProtocol
{
    public class ThumbnailsRetreiveEventArgs : EventArgs
    {
        public ThumbnailsRetreiveEventArgs(string base64Image)
        {
            this.Base64Image = base64Image;
        }

        public string Base64Image { get; }
    }
}
