namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class MixerAudioChannelsCountEventArgs : ChannelEventArgs
    {
        public MixerAudioChannelsCountEventArgs(int channelsCount, ushort channelId)
        {
            ChannelId = channelId;
            ChannelsCount = channelsCount;
        }

        public int ChannelsCount { get; }
    }
}
