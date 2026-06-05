namespace StarDust.CasparCG.Query;

/// <summary>
/// Represents detailed media information returned by <c>CINF</c>.
/// </summary>
public sealed record MediaInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MediaInfo"/> record.
    /// </summary>
    /// <param name="name">The media name.</param>
    /// <param name="kind">The media kind.</param>
    /// <param name="sizeBytes">The media size in bytes when available.</param>
    /// <param name="lastModified">The last modification timestamp when available.</param>
    /// <param name="frameCount">The frame count when available.</param>
    /// <param name="frameRateOrDuration">The raw frame rate or duration token when available.</param>
    /// <param name="properties">Additional key/value properties preserved from the payload.</param>
    public MediaInfo(
        string name,
        MediaFileKind kind,
        long? sizeBytes,
        DateTimeOffset? lastModified,
        long? frameCount,
        string? frameRateOrDuration,
        IReadOnlyDictionary<string, string> properties)
    {
        Name = name;
        Kind = kind;
        SizeBytes = sizeBytes;
        LastModified = lastModified;
        FrameCount = frameCount;
        FrameRateOrDuration = frameRateOrDuration;
        Properties = properties;
    }

    /// <summary>
    /// Gets the media name.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets the media kind.
    /// </summary>
    public MediaFileKind Kind { get; init; }

    /// <summary>
    /// Gets the media size in bytes when available.
    /// </summary>
    public long? SizeBytes { get; init; }

    /// <summary>
    /// Gets the last modification timestamp when available.
    /// </summary>
    public DateTimeOffset? LastModified { get; init; }

    /// <summary>
    /// Gets the frame count when available.
    /// </summary>
    public long? FrameCount { get; init; }

    /// <summary>
    /// Gets the raw frame rate or duration token when available.
    /// </summary>
    public string? FrameRateOrDuration { get; init; }

    /// <summary>
    /// Gets additional key/value properties preserved from the payload.
    /// </summary>
    public IReadOnlyDictionary<string, string> Properties { get; init; }
}
