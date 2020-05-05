namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class PlaybackClipFrameRateEventArgs : FFMpegProducerEventArgs
    {
        public float FramesRate { get; protected set; }
            

        public PlaybackClipFrameRateEventArgs(float frameRate, bool isBackground, ushort channelId, ushort layerId)
        {
            ChannelId = channelId;
            LayerId = layerId;
            FramesRate = frameRate;         
            IsBackground = isBackground;
        }
    }
}
