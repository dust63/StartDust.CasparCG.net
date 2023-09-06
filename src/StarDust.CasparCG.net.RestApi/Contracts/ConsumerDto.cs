using StarDust.CasparCG.net.Models;

namespace StarDust.CasparCG.net.RestApi.Contracts;

public class ConsumerDto
{
    public string Type { get; set;}
    public string Device { get; internal set; }
    public bool Windowed { get; internal set; }
    public bool Embeddedaudio { get; internal set; }
    public bool Lowlatency { get; internal set; }
}