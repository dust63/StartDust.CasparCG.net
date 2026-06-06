using System.Globalization;
using StarDust.CasparCG.AspNetCore.Contracts;

namespace StarDust.CasparCG.AspNetCore.Internal;

internal static class ThumbnailResponseParser
{
    public static IReadOnlyList<ThumbnailListItemResponse> ParseList(string responseText)
    {
        var lines = responseText.Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

        return lines
            .Skip(1)
            .Select(ParseListLine)
            .ToArray();
    }

    public static byte[] ParseBinary(string responseText)
    {
        var payload = responseText
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Last();

        return Convert.FromBase64String(payload);
    }

    private static ThumbnailListItemResponse ParseListLine(string line)
    {
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var name = parts[0].Trim('"');
        long? sizeBytes = long.TryParse(parts.ElementAtOrDefault(1), out var parsedSize) ? parsedSize : null;
        DateTimeOffset? lastModified = DateTimeOffset.TryParseExact(
            parts.ElementAtOrDefault(2),
            "yyyyMMdd'T'HHmmss",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out var parsedDate)
            ? parsedDate
            : null;

        return new(name, sizeBytes, lastModified);
    }
}
