#pragma warning disable CS1591
using System.Text.Json.Serialization;

namespace StarDust.CasparCG.Protocol.Amcp;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PlaybackTransitionKind
{
    Cut,
    Mix,
    Push,
    Slide,
    Wipe,
    FadeCut,
    CutFade,
    VFade,
    Sting
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PlaybackTransitionDirection
{
    Left,
    Right,
    Top,
    Bottom
}

/// <summary>
/// Represents a typed AMCP playback transition.
/// </summary>
public sealed record PlaybackTransition
{
    /// <summary>
    /// Gets the transition kind.
    /// </summary>
    public PlaybackTransitionKind Kind { get; init; }

    /// <summary>
    /// Gets the transition duration, when applicable.
    /// </summary>
    public int? Duration { get; init; }

    /// <summary>
    /// Gets the tweening function, when applicable.
    /// </summary>
    public string? Tweener { get; init; }

    /// <summary>
    /// Gets the transition direction, when applicable.
    /// </summary>
    public PlaybackTransitionDirection? Direction { get; init; }

    /// <summary>
    /// Gets the sting mask filename, when applicable.
    /// </summary>
    public string? MaskFilename { get; init; }

    /// <summary>
    /// Gets the sting trigger point, when applicable.
    /// </summary>
    public int? TriggerPoint { get; init; }

    /// <summary>
    /// Gets the sting overlay filename, when applicable.
    /// </summary>
    public string? OverlayFilename { get; init; }

    /// <summary>
    /// Gets the sting audio fade start, when applicable.
    /// </summary>
    public int? AudioFadeStart { get; init; }

    /// <summary>
    /// Gets the sting audio fade duration, when applicable.
    /// </summary>
    public int? AudioFadeDuration { get; init; }

    /// <summary>
    /// Creates a cut transition.
    /// </summary>
    public static PlaybackTransition Cut() => new() { Kind = PlaybackTransitionKind.Cut };

    /// <summary>
    /// Creates a mix transition.
    /// </summary>
    public static PlaybackTransition Mix(int duration, string? tweener = null) =>
        new() { Kind = PlaybackTransitionKind.Mix, Duration = duration, Tweener = tweener };

    /// <summary>
    /// Creates a push transition.
    /// </summary>
    public static PlaybackTransition Push(int duration, PlaybackTransitionDirection direction, string? tweener = null) =>
        new() { Kind = PlaybackTransitionKind.Push, Duration = duration, Tweener = tweener, Direction = direction };

    /// <summary>
    /// Creates a wipe transition.
    /// </summary>
    public static PlaybackTransition Wipe(int duration, PlaybackTransitionDirection direction, string? tweener = null) =>
        new() { Kind = PlaybackTransitionKind.Wipe, Duration = duration, Tweener = tweener, Direction = direction };

    /// <summary>
    /// Creates a slide transition.
    /// </summary>
    public static PlaybackTransition Slide(int duration, PlaybackTransitionDirection direction, string? tweener = null) =>
        new() { Kind = PlaybackTransitionKind.Slide, Duration = duration, Tweener = tweener, Direction = direction };

    /// <summary>
    /// Creates a fadecut transition.
    /// </summary>
    public static PlaybackTransition FadeCut(int duration) =>
        new() { Kind = PlaybackTransitionKind.FadeCut, Duration = duration };

    /// <summary>
    /// Creates a cutfade transition.
    /// </summary>
    public static PlaybackTransition CutFade(int duration) =>
        new() { Kind = PlaybackTransitionKind.CutFade, Duration = duration };

    /// <summary>
    /// Creates a vfade transition.
    /// </summary>
    public static PlaybackTransition VFade(int duration) =>
        new() { Kind = PlaybackTransitionKind.VFade, Duration = duration };

    /// <summary>
    /// Creates a sting transition.
    /// </summary>
    public static PlaybackTransition Sting(
        string maskFilename,
        int? triggerPoint = null,
        string? overlayFilename = null,
        int? audioFadeStart = null,
        int? audioFadeDuration = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(maskFilename);

        return new PlaybackTransition
        {
            Kind = PlaybackTransitionKind.Sting,
            MaskFilename = maskFilename,
            TriggerPoint = triggerPoint,
            OverlayFilename = overlayFilename,
            AudioFadeStart = audioFadeStart,
            AudioFadeDuration = audioFadeDuration
        };
    }
}
#pragma warning restore CS1591
