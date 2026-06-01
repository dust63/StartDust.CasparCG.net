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
        if (statusLineEnd < 0)
        {
            throw new FormatException("The AMCP response does not contain a status line terminator.");
        }

        var statusLine = raw[..statusLineEnd];
        if (statusLine.Length < 3 || !int.TryParse(statusLine[..3], out var statusCode))
        {
            throw new FormatException("The AMCP response does not start with a valid status code.");
        }

        var payload = raw[(statusLineEnd + 2)..];
        var lines = ParsePayloadLines(statusCode, payload);

        return new AmcpResponse
        {
            StatusCode = statusCode,
            Category = Categorize(statusCode),
            CommandText = ExtractCommandText(statusLine),
            StatusLine = statusLine,
            Lines = lines,
            Raw = raw
        };
    }

    private static IReadOnlyList<string> ParsePayloadLines(int statusCode, string payload)
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

    private static IReadOnlyList<string> SplitLines(string payload, string terminator)
    {
        if (string.IsNullOrEmpty(payload))
        {
            return Array.Empty<string>();
        }

        var normalizedPayload = payload.EndsWith(terminator, StringComparison.Ordinal)
            ? payload[..^terminator.Length]
            : payload;

        if (normalizedPayload.Length == 0)
        {
            return Array.Empty<string>();
        }

        return normalizedPayload
            .Split("\r\n", StringSplitOptions.None)
            .ToArray();
    }

    private static string ExtractCommandText(string statusLine)
    {
        if (statusLine.Length <= 4)
        {
            return string.Empty;
        }

        var text = statusLine[4..].Trim();
        return text.EndsWith(" OK", StringComparison.Ordinal)
            ? text[..^3]
            : text;
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
