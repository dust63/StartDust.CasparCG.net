using StarDust.CasparCG.net.Models;

namespace StarDust.CasparCG.net.RestApi.Contracts;

public class LoadBgRequestDto
{
    /// <summary>
    /// In wich layer you want to load the clip
    /// </summary>
    /// <value></value>
    public required uint LayerId { get; set; }

    /// <summary>
    /// Name of the clip to load
    /// </summary>
    /// <value></value>
    public required string ClipName { get; set; }

    /// <summary>
    /// Will play the clip after the previous one is ended. 
    /// If no clip playing, start the clip directly.
    /// </summary>
    /// <value></value>
    public bool Auto { get; set; }

    /// <summary>
    /// Will play clip in Loop
    /// </summary>
    /// <value></value>
    public bool Loop { get; set; }
    
    /// <summary>
    /// Begin to play the clip in milliseconds. You need to set 'FrameRate' to work
    /// </summary>
    /// <value></value>
    public decimal? StartAtMs { get; set; }

    /// <summary>
    /// How many time the clip will play. You need to set 'FrameRate' to work
    /// </summary>
    /// <value></value>
    public decimal? DurationInMs { get; set; }

    /// <summary>
    /// Frame rate of the clip (used to calculate 'Start' and 'Duration' of the play)
    /// </summary>
    /// <value></value>
    public decimal? FrameRate { get; set; }

    /// <summary>
    /// Transition to apply when the clip will start
    /// </summary>
    /// <value></value>
    public Transition? Transition { get; set; }
}
