using StarDust.CasparCG.net.Models;

namespace StarDust.CasparCG.net.RestApi.Contracts;

public class PlayClipRequestDto
{
    public required uint LayerId { get; set; }
    public required string ClipName { get; set; }
    public bool Auto { get; set; }
    public Transition? Transition { get; set; }
}
