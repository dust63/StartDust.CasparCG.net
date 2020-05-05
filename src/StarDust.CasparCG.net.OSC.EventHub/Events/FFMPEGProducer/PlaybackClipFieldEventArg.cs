namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class PlaybackClipFieldEventArg : FFMpegProducerEventArgs
    {
        public bool IsProgressive { get; protected set; }

        public PlaybackClipFieldEventArg(bool isProgressive, bool isBackground, ushort channelId, ushort layerId)
        {
            ChannelId = channelId;
            LayerId = layerId;
            IsProgressive = isProgressive;
            IsBackground = isBackground;
        }
    }
}
