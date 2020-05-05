namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class PlaybackClipAudioChannelsEventArg : FFMpegProducerEventArgs
    {
        public uint Channels { get; protected set; }

        public PlaybackClipAudioChannelsEventArg(uint channels, bool isBackground, ushort channelId, ushort layerId)
        {

            ChannelId = channelId;
            LayerId = layerId;
            Channels = channels;
            IsBackground = isBackground;
        }
    }
}
