namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class LayerTypeEventArgs : StageEventArgs
    {
        public LayerTypeEventArgs(string layerType, ushort channelId, ushort layerId)
        {
            ChannelId = channelId;
            LayerId = layerId;
            LayerType = layerType;
        }

        public string LayerType { get; }
    }
}
