namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class TemplateHeightEventArgs : StageEventArgs
    {
        public TemplateHeightEventArgs(uint templateHeight, ushort channelId, ushort layerId)
        {
            ChannelId = channelId;
            LayerId = layerId;
            TemplateHeight = templateHeight;
        }

        public uint TemplateHeight { get; }
    }
}
