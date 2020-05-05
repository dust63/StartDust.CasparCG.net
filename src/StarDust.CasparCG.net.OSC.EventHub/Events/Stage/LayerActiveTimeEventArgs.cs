namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class LayerActiveTimeEventArgs : StageEventArgs
    {
        public LayerActiveTimeEventArgs(float activeTime,ushort channelId, ushort layerId)
        {
            ChannelId = channelId;
            LayerId = layerId;
            ActiveTime = activeTime;          
        }

        public float ActiveTime { get; }
    }
}
