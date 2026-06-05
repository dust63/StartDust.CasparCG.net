namespace StarDust.CasparCG.Query;

/// <summary>
/// Describes the kind of media entry returned by CasparCG query commands.
/// </summary>
public enum MediaFileKind
{
    /// <summary>
    /// Represents an unrecognized media kind.
    /// </summary>
    Unknown,
    /// <summary>
    /// Represents a movie clip.
    /// </summary>
    Movie,
    /// <summary>
    /// Represents a still image.
    /// </summary>
    Still,
    /// <summary>
    /// Represents an audio clip.
    /// </summary>
    Audio
}
