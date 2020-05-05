namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class OutputPortTypeEventArgs : OutputPortEventArgs
    {
        public string Type { get; set; }

        public uint PortId { get; set; }

        public OutputPortTypeEventArgs(string type,uint portId, ushort channelId)
        {
            ChannelId = channelId;
            Type = type;
            PortId = portId;
        }
    }
}
