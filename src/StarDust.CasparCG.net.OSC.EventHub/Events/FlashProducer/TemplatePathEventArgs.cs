namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class TemplatePathEventArgs : StageEventArgs
    {
        public TemplatePathEventArgs(string templatePath, ushort channelId, ushort layerId)
        {
            ChannelId = channelId;
            LayerId = layerId;
            TemplatePath = templatePath;
        }

        public string TemplatePath { get; }
    }
}
