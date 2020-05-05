namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class LayerActiveFrameEventArgs : StageEventArgs
    {
        public LayerActiveFrameEventArgs(uint activeFrame, ushort channelId, ushort layerId)
        {
            ChannelId = channelId;
            LayerId = layerId;
            ActiveFrame = activeFrame;
        }

        public uint ActiveFrame { get; }
    }
}
