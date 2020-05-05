namespace StarDust.CasparCG.net.OSC.EventHub.Events
{

    public class ConsumerFrameCreatedEventArgs : OutputPortEventArgs
    {
        public float FramesCreated { get; protected set; }

        public float AvailableFrames { get; protected set; }

        public ConsumerFrameCreatedEventArgs(int framesCreated, int availableFrames, uint portId, ushort channelId)
        {
            ChannelId = channelId;
            FramesCreated = framesCreated;
            PortId = portId;
            AvailableFrames = availableFrames;
        }
    }

    public class ProfilerTimeEventArgs : ChannelEventArgs
    {

        public float ExpectedTime { get; protected set; }

        public float ActualTime { get; protected set; }

        public ProfilerTimeEventArgs(float actualTime, float expectedTime, ushort channelId)
        {
            ChannelId = channelId;
            ActualTime = actualTime;
            ExpectedTime = expectedTime;
        }
    }

    public class PlaybackClipCodecEventArg : FFMpegProducerEventArgs
    {
        public string Codec { get; protected set; }

        public PlaybackClipCodecEventArg(string codec, bool isBackground, ushort channelId, ushort layerId)
        {
            ChannelId = channelId;
            LayerId = layerId;
            Codec = codec;
            IsBackground = isBackground;
        }
    }
}
