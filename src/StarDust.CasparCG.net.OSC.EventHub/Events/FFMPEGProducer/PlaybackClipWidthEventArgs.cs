namespace StarDust.CasparCG.net.OSC.EventHub.Events
{

    public class PlaybackClipWidthEventArgs : FFMpegProducerEventArgs
    {
        public uint Width { get; protected set; }


        public PlaybackClipWidthEventArgs(uint width, bool isBackground, ushort channelId, ushort layerId)
        {
            ChannelId = channelId;
            LayerId = layerId;
            Width = width;           
            IsBackground = isBackground;
        }
    }
}
