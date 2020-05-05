namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class PlaybackClipAudioSampleRateEventArg : FFMpegProducerEventArgs
    {
        public uint SampleRate { get; protected set; }
        public PlaybackClipAudioSampleRateEventArg(uint sampleRate, bool isBackground, ushort channelId, ushort layerId)
        {
            ChannelId = channelId;
            LayerId = layerId;
            SampleRate = sampleRate;
            IsBackground = isBackground;
        }
    }
}
