namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class StreamFramerateEventArgs : FFMpegProducerEventArgs
    {
        public float Fps { get; protected set; }
        public uint StreamId { get; protected set; }

        public StreamFramerateEventArgs(float fps,uint streamId, bool isBackground, ushort channelId, ushort layerId)
        {
            ChannelId = channelId;
            LayerId = layerId;          
            Fps = fps;
            StreamId = streamId;
            IsBackground = isBackground;
        }
    }
}
