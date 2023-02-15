using StarDust.CasparCG.net.Models;

namespace StarDust.CasparCG.net.RestApi.Contracts;

public class AddConsumerRequestDto
{
    public uint? Id{ get; set; }
    public required ConsumerType Consumer{ get; set; }
    public string? Parameters{ get; set; }
}
