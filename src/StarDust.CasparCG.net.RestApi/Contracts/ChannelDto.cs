using StarDust.CasparCG.net.Models;

namespace StarDust.CasparCG.net.RestApi.Contracts;

public class ChannelDto
{
    public uint Id { get; set; }
    public ChannelStatus Status { get; internal set; }
    public VideoMode VideoMode { get; internal set; }
    public string? ActiveClip { get; internal set; }

    public IReadOnlyCollection<ConsumerDto>? Consumers { get; set; }
}
