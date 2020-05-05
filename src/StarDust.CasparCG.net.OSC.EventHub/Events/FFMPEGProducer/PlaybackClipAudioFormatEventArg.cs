namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class PlaybackClipAudioFormatEventArg : FFMpegProducerEventArgs
    {
        public string Format { get; set; }

        public PlaybackClipAudioFormatEventArg(string format, bool isBackground, ushort channelId, ushort layerId)
        {
            ChannelId = channelId;
            LayerId = layerId;
            Format = format;
            IsBackground = isBackground;
        }
    }
}
