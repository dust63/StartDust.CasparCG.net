using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class PlaybackClipTimeEventArgs : FFMpegProducerEventArgs
    {
        public float SecondsElapsed { get; protected set; }

        public float TotalSeconds { get; protected set; }

        public PlaybackClipTimeEventArgs(float elapsedClipTime,  float totalClipTime, bool isBackground, ushort channelId, ushort layerId)
        {
            ChannelId = channelId;
            LayerId = layerId;
            SecondsElapsed = elapsedClipTime;
            TotalSeconds = totalClipTime;
            IsBackground = isBackground;
        }
    }
}
