namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class TemplateWidthEventArgs : StageEventArgs
    {
        public TemplateWidthEventArgs(uint templateWidth, ushort channelId, ushort layerId)
        {
            ChannelId = channelId;
            LayerId = layerId;
            TemplateWidth = templateWidth;
        }

        public uint TemplateWidth { get; }
    }
}
