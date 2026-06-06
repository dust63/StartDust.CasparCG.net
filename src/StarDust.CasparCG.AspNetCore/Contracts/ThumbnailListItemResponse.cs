namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents one thumbnail entry returned by the REST API.
/// </summary>
/// <param name="Name">The media name.</param>
/// <param name="SizeBytes">The thumbnail size in bytes, when available.</param>
/// <param name="LastModified">The thumbnail timestamp, when available.</param>
public sealed record ThumbnailListItemResponse(
    string Name,
    long? SizeBytes,
    DateTimeOffset? LastModified);
