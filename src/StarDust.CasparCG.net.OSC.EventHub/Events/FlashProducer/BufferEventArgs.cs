namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class BufferEventArgs : StageEventArgs
    {
        public BufferEventArgs(int buufer, ushort channelId, ushort layerId)
        {
            ChannelId = channelId;
            LayerId = layerId;
            Buffer = buufer;
        }

        public int Buffer { get; }
    }
}
