using StarDust.CasparCG.net.Models;

namespace StarDust.CasparCG.net.RestApi.Contracts;

public class RemoveConsumerRequestDto{
    public required ConsumerType Consumer{ get; set; }
    public string? Parameters{ get; set; }
}