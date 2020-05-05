using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class PlaybackClipClipChangedEventArgs : FFMpegProducerEventArgs
    {
        public string ActiveClip { get; protected set; }      
               

        public PlaybackClipClipChangedEventArgs(string activeClip,bool isBackground, ushort channelId, ushort layerId)
        {
            ChannelId = channelId;
            LayerId = layerId;
            ActiveClip = activeClip;
            IsBackground = isBackground;
        }
    }
}
