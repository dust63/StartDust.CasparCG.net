namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public abstract class OutputPortEventArgs : ChannelEventArgs
    {
        public uint PortId { get;protected set; }
    }
}
