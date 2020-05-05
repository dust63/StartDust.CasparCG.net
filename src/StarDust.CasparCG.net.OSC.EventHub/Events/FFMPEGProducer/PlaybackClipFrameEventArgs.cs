namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class PlaybackClipFrameEventArgs : FFMpegProducerEventArgs
    {
        public uint FramesElapsed { get; protected set; }

        public uint TotalFrames { get; protected set; }

        public PlaybackClipFrameEventArgs(uint elapsedClipFrames, uint totalClipFrames, bool isBackground, ushort channelId, ushort layerId)
        {
            ChannelId = channelId;
            LayerId = layerId;
            FramesElapsed = elapsedClipFrames;
            TotalFrames = totalClipFrames;
            IsBackground = isBackground;
        }
    }
}
