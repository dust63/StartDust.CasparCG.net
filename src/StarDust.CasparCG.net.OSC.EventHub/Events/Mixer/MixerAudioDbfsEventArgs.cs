namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class MixerAudioDbfsEventArgs : ChannelEventArgs
    {
        public MixerAudioDbfsEventArgs(float dbfs, ushort channelId)
        {
            ChannelId = channelId;
            Dbfs = dbfs;
        }

        public float Dbfs { get; }
    }
}
