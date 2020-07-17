namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public class OutputFormatEventArgs : ChannelEventArgs
    {
        public OutputFormatEventArgs(string format, ushort channelId)
        {
            ChannelId = channelId;
            Format = format;         
        }

        public string Format { get; set; }
    }
}
