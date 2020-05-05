namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class PlaybackClipHeightEventArgs : FFMpegProducerEventArgs
    {
        public uint Height { get; protected set; }


        public PlaybackClipHeightEventArgs(uint height, bool isBackground, ushort channelId, ushort layerId)
        {
            ChannelId = channelId;
            LayerId = layerId;
            Height = height;
            IsBackground = isBackground;
        }
    }
}
