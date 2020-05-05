namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class TemplateFpsEventArgs : StageEventArgs
    {
        public TemplateFpsEventArgs(float fps, ushort channelId, ushort layerId)
        {
            ChannelId = channelId;
            LayerId = layerId;
            TemplateFps = fps;
        }

        public float TemplateFps { get; }

    }
}
