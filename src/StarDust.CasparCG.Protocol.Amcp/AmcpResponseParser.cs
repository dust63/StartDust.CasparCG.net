namespace StarDust.CasparCG.Protocol.Amcp;

/// <summary>
/// Parses raw AMCP response blocks into typed response objects.
/// </summary>
public static class AmcpResponseParser
{
    /// <summary>
    /// Parses a raw AMCP response block.
    /// </summary>
    /// <param name="raw">The raw AMCP response block.</param>
    /// <returns>The parsed response.</returns>
    public static AmcpResponse Parse(string raw)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(raw);

        var statusLineEnd = raw.IndexOf("\r\n", StringComparison.Ordinal);
        ReadOnlySpan<char> statusLine = statusLineEnd < 0 ? raw.AsSpan() : raw.AsSpan(0, statusLineEnd);
        if (statusLine.Length < 3 || !int.TryParse(statusLine[..3], out var statusCode))
        {
            throw new FormatException("The AMCP response does not start with a valid status code.");
        }

        ReadOnlySpan<char> payload = statusLineEnd < 0 ? ReadOnlySpan<char>.Empty : raw.AsSpan(statusLineEnd + 2);
        var lines = ParsePayloadLines(statusCode, payload);

        return new AmcpResponse
        {
            StatusCode = statusCode,
            Category = Categorize(statusCode),
            CommandText = ExtractCommandText(statusLine),
            StatusLine = statusLine.ToString(),
            Lines = lines,
            Raw = raw
        };
    }

    private static IReadOnlyList<string> ParsePayloadLines(int statusCode, ReadOnlySpan<char> payload)
    {
        if (statusCode == 200)
        {
            return SplitLines(payload, "\r\n\r\n");
        }

        if (statusCode == 201 || statusCode == 400)
        {
            return SplitLines(payload, "\r\n");
        }

        return Array.Empty<string>();
    }

    private static IReadOnlyList<string> SplitLines(ReadOnlySpan<char> payload, string terminator)
    {
        if (payload.IsEmpty)
        {
            return Array.Empty<string>();
        }

        var terminatorSpan = terminator.AsSpan();
        if (payload.EndsWith(terminatorSpan, StringComparison.Ordinal))
        {
            payload = payload[..^terminatorSpan.Length];
        }

        if (payload.IsEmpty)
        {
            return Array.Empty<string>();
        }

        var lines = new List<string>();
        while (!payload.IsEmpty)
        {
            var lineEnd = payload.IndexOf("\r\n", StringComparison.Ordinal);
            if (lineEnd < 0)
            {
                lines.Add(payload.ToString());
                break;
            }

            lines.Add(payload[..lineEnd].ToString());
            payload = payload[(lineEnd + 2)..];
        }

        return lines;
    }

    private static string ExtractCommandText(ReadOnlySpan<char> statusLine)
    {
        if (statusLine.Length <= 4)
        {
            return string.Empty;
        }

        var text = statusLine[4..].Trim();
        return text.EndsWith(" OK".AsSpan(), StringComparison.Ordinal)
            ? text[..^3].ToString()
            : text.ToString();
    }

    private static AmcpStatusCategory Categorize(int statusCode) =>
        statusCode switch
        {
            >= 100 and < 200 => AmcpStatusCategory.Information,
            >= 200 and < 300 => AmcpStatusCategory.Success,
            >= 400 and < 500 => AmcpStatusCategory.ClientError,
            >= 500 and < 600 => AmcpStatusCategory.ServerError,
            _ => AmcpStatusCategory.Unknown
        };
}
