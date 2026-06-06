namespace StarDust.CasparCG.Protocol.Amcp;

/// <summary>
/// Represents typed playback options shared by LOAD and PLAY.
/// </summary>
public record PlaybackOptions
{
    /// <summary>
    /// Gets the optional transition.
    /// </summary>
    public PlaybackTransition? Transition { get; init; }

    /// <summary>
    /// Gets the optional seek position.
    /// </summary>
    public int? Seek { get; init; }

    /// <summary>
    /// Gets the optional length.
    /// </summary>
    public int? Length { get; init; }

    /// <summary>
    /// Gets the optional filter.
    /// </summary>
    public string? Filter { get; init; }

    /// <summary>
    /// Gets whether playback should loop.
    /// </summary>
    public bool Loop { get; init; }

    /// <summary>
    /// Gets whether the command should clear the layer when the media file cannot be found.
    /// </summary>
    public bool ClearOn404 { get; init; }
}

/// <summary>
/// Represents typed playback options for LOADBG.
/// </summary>
public sealed record LoadBackgroundOptions : PlaybackOptions
{
    /// <summary>
    /// Gets whether playback should auto-start after loading.
    /// </summary>
    public bool AutoPlay { get; init; }
}
