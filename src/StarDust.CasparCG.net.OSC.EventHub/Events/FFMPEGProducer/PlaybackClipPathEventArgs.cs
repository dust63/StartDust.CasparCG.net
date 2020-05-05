using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class PlaybackClipPathEventArgs : FFMpegProducerEventArgs
    {
        public string Path { get; protected set; }      

        public PlaybackClipPathEventArgs(string path,bool isBackground, ushort channelId, ushort layerId)
        {
            ChannelId = channelId;
            LayerId = layerId;
            Path = path;
            IsBackground = isBackground;

        }
    }
}
