using StarDust.CasparCG.Query;

namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents a media catalog entry returned by the REST API.
/// </summary>
/// <param name="Name">The media name.</param>
/// <param name="Kind">The media kind.</param>
/// <param name="SizeBytes">The media size in bytes.</param>
/// <param name="LastModified">The last modification timestamp, when available.</param>
/// <param name="FrameCount">The frame count, when available.</param>
/// <param name="FrameRateOrDuration">The raw frame rate or duration token, when available.</param>
public sealed record MediaFileResponse(
    string Name,
    MediaFileKind Kind,
    long SizeBytes,
    DateTimeOffset? LastModified,
    long? FrameCount,
    string? FrameRateOrDuration)
{
    /// <summary>
    /// Creates a REST response from a domain media file.
    /// </summary>
    /// <param name="mediaFile">The source media file.</param>
    /// <returns>The mapped REST response.</returns>
    public static MediaFileResponse FromDomain(MediaFile mediaFile) =>
        new(
            mediaFile.Name,
            mediaFile.Kind,
            mediaFile.SizeBytes,
            mediaFile.LastModified,
            mediaFile.FrameCount,
            mediaFile.FrameRateOrDuration);
}
