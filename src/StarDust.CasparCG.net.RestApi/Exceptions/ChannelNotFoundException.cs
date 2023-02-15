namespace StarDust.CasparCG.net.RestApi.Exceptions;

public class ChannelNotFoundException : InvalidOperationException
{
    public int ChannelId { get; set; }
    public ChannelNotFoundException(int channelId) : base($"Channel with Id: {channelId} not exists on this CasparCG server")
    {
        this.ChannelId = channelId;
    }
}

