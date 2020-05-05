namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class LayerPausedEventArgs : StageEventArgs
    {
        public LayerPausedEventArgs(bool isPause, ushort channelId, ushort layerId)
        {
            ChannelId = channelId;
            LayerId = layerId;
            IsPause = isPause;
        }

        public bool IsPause { get; }
    }
}
