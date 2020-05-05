namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class PlaybackLoopEventArgs : FFMpegProducerEventArgs
    {
        public bool IsLoop { get; set; }

        public PlaybackLoopEventArgs(bool isLoop, bool isBackground, ushort channelId, ushort layerId)
        {
            ChannelId = channelId;
            LayerId = layerId;
            IsLoop = isLoop;
            IsBackground = isBackground;
        }
    }
}
