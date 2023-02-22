using StarDust.CasparCG.net.Models;

namespace StarDust.CasparCG.net.RestApi.Contracts;

public class PlayRequestDto
{
    public required uint LayerId { get; set; }
    public required string ClipName { get; set; }
    public Transition? Transition { get; set; }
}
