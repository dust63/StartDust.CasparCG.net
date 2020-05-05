namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class LayerProfilerEventArgs : StageEventArgs
    {
        public float ActualValue { get; }

        public float ExpectedValue { get; }
        public LayerProfilerEventArgs(float actualValue, float expectedValue, ushort channelId, ushort layerId)
        {
            ChannelId = channelId;
            LayerId = layerId;
            ActualValue = actualValue;
            ExpectedValue = expectedValue;
        }
    }
}
