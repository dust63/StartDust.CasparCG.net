using System.Globalization;
using System.Xml.Linq;
using StarDust.CasparCG.Protocol.Amcp;

namespace StarDust.CasparCG.Query;

/// <summary>
/// Projects AMCP query responses into higher-level query result objects.
/// </summary>
public static class CasparQueryResultParser
{
    /// <summary>
    /// Parses a <c>CLS</c> response into media catalog entries.
    /// </summary>
    /// <param name="response">The response to parse.</param>
    /// <returns>The parsed media entries.</returns>
    public static IReadOnlyList<MediaFile> ParseMediaFiles(AmcpResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        var items = new List<MediaFile>(response.Lines.Count);
        foreach (var line in response.Lines)
        {
            var entry = ParseStableMediaLine(line);
            items.Add(new MediaFile(
                entry.Name,
                entry.Kind,
                entry.SizeBytes ?? throw CreateMalformedPayloadException(line),
                entry.LastModified,
                entry.FrameCount,
                entry.FrameRateOrDuration));
        }

        return items;
    }

    /// <summary>
    /// Parses a <c>CINF</c> response into a media information record.
    /// </summary>
    /// <param name="response">The response to parse.</param>
    /// <returns>The parsed media information.</returns>
    public static MediaInfo ParseMediaInfo(AmcpResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        if (response.Lines.Count != 1)
        {
            throw new CasparQueryParseException("CINF responses must contain exactly one payload line.");
        }

        return ParseStableMediaLine(response.Lines[0]);
    }

    /// <summary>
    /// Parses a <c>TLS</c> response into template catalog entries.
    /// </summary>
    /// <param name="response">The response to parse.</param>
    /// <returns>The parsed template entries.</returns>
    public static IReadOnlyList<TemplateFile> ParseTemplateFiles(AmcpResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        var items = new List<TemplateFile>(response.Lines.Count);
        foreach (var line in response.Lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                throw CreateMalformedPayloadException(line);
            }

            items.Add(new TemplateFile(line));
        }

        return items;
    }

    /// <summary>
    /// Parses a <c>FLS</c> response into font catalog entries.
    /// </summary>
    /// <param name="response">The response to parse.</param>
    /// <returns>The parsed font entries.</returns>
    public static IReadOnlyList<FontFile> ParseFontFiles(AmcpResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        var items = new List<FontFile>(response.Lines.Count);
        foreach (var line in response.Lines)
        {
            var parser = new TokenParser(line);
            var name = parser.ReadName();
            var path = parser.ReadRequiredToken();
            parser.EnsureFullyConsumed();
            items.Add(new FontFile(name, path));
        }

        return items;
    }

    /// <summary>
    /// Parses a loosely structured query response into a dictionary-backed object.
    /// </summary>
    /// <param name="response">The response to parse.</param>
    /// <returns>The flattened query data.</returns>
    public static QueryDataMap ParseQueryDataMap(AmcpResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        var values = TryParseXml(response.Lines);
        if (values.Count == 0)
        {
            values = TryParseKeyValueLines(response.Lines);
        }

        return new QueryDataMap(
            new Dictionary<string, string>(values, StringComparer.OrdinalIgnoreCase),
            response.Lines.ToArray(),
            response.Raw);
    }

    private static MediaInfo ParseStableMediaLine(string line)
    {
        try
        {
            var parser = new TokenParser(line);
            var name = parser.ReadName();
            var kind = ParseMediaFileKind(parser.ReadRequiredToken());
            var sizeBytes = ParseRequiredLong(parser.ReadRequiredToken(), line);
            var lastModified = ParseOptionalTimestamp(parser.ReadRequiredToken(), line);

            long? frameCount = null;
            string? frameRateOrDuration = null;
            if (parser.TryReadToken(out var token))
            {
                frameCount = ParseRequiredLong(token, line);
            }

            if (parser.TryReadToken(out token))
            {
                frameRateOrDuration = token;
            }

            var properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            while (parser.TryReadToken(out var key))
            {
                if (!parser.TryReadToken(out var value))
                {
                    throw CreateMalformedPayloadException(line);
                }

                properties[key] = value;
            }

            return new MediaInfo(
                name,
                kind,
                sizeBytes,
                lastModified,
                frameCount,
                frameRateOrDuration,
                properties);
        }
        catch (CasparQueryParseException)
        {
            throw;
        }
        catch (Exception)
        {
            throw CreateMalformedPayloadException(line);
        }
    }

    private static MediaFileKind ParseMediaFileKind(string token) =>
        token.ToUpperInvariant() switch
        {
            "MOVIE" => MediaFileKind.Movie,
            "STILL" => MediaFileKind.Still,
            "AUDIO" => MediaFileKind.Audio,
            _ => MediaFileKind.Unknown
        };

    private static long ParseRequiredLong(string token, string line)
    {
        if (long.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
        {
            return value;
        }

        throw CreateMalformedPayloadException(line);
    }

    private static DateTimeOffset? ParseOptionalTimestamp(string token, string line)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        if (DateTimeOffset.TryParseExact(
            token,
            "yyyyMMddHHmmss",
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out var parsed))
        {
            return parsed;
        }

        throw CreateMalformedPayloadException(line);
    }

    private static IReadOnlyDictionary<string, string> TryParseXml(IReadOnlyList<string> lines)
    {
        if (lines.Count != 1)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        var payload = lines[0];
        if (string.IsNullOrWhiteSpace(payload) || payload[0] != '<')
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        try
        {
            var document = XDocument.Parse(payload, LoadOptions.None);
            if (document.Root is null)
            {
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }

            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            FlattenXmlElement(document.Root, document.Root.Name.LocalName, values);
            return values;
        }
        catch
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
    }

    private static IReadOnlyDictionary<string, string> TryParseKeyValueLines(IReadOnlyList<string> lines)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var rawLine in lines)
        {
            if (string.IsNullOrWhiteSpace(rawLine))
            {
                continue;
            }

            var line = rawLine.AsSpan().Trim();
            var separatorIndex = line.IndexOfAny(':', '=');
            if (separatorIndex >= 0)
            {
                var key = line[..separatorIndex].Trim().ToString();
                var value = line[(separatorIndex + 1)..].Trim().ToString();
                if (key.Length > 0 && value.Length > 0)
                {
                    values[key] = value;
                }

                continue;
            }

            var firstWhitespace = line.IndexOfAny(' ', '\t');
            if (firstWhitespace <= 0 || firstWhitespace >= line.Length - 1)
            {
                continue;
            }

            var keyToken = line[..firstWhitespace].Trim().ToString();
            var valueToken = line[(firstWhitespace + 1)..].Trim().ToString();
            if (keyToken.Length > 0 && valueToken.Length > 0)
            {
                values[keyToken] = valueToken;
            }
        }

        return values;
    }

    private static void FlattenXmlElement(XElement element, string path, IDictionary<string, string> values)
    {
        var children = element.Elements().ToArray();
        if (children.Length == 0)
        {
            values[path] = element.Value;
            return;
        }

        foreach (var childGroup in children.GroupBy(child => child.Name.LocalName, StringComparer.Ordinal))
        {
            if (childGroup.Count() > 1)
            {
                continue;
            }

            var child = childGroup.First();
            FlattenXmlElement(child, $"{path}.{child.Name.LocalName}", values);
        }
    }

    private static CasparQueryParseException CreateMalformedPayloadException(string line) =>
        new($"Malformed query payload line: {line}");

    private ref struct TokenParser
    {
        private readonly ReadOnlySpan<char> _line;
        private int _position;

        public TokenParser(string line)
        {
            _line = line.AsSpan();
            _position = 0;
        }

        public string ReadName()
        {
            SkipWhitespace();
            if (_position >= _line.Length || _line[_position] != '"')
            {
                throw CreateMalformedPayloadException(_line.ToString());
            }

            _position++;
            var closingQuote = _line[_position..].IndexOf('"');
            if (closingQuote < 0)
            {
                throw CreateMalformedPayloadException(_line.ToString());
            }

            var name = _line.Slice(_position, closingQuote).ToString();
            _position += closingQuote + 1;
            return name;
        }

        public string ReadRequiredToken()
        {
            if (!TryReadToken(out var token))
            {
                throw CreateMalformedPayloadException(_line.ToString());
            }

            return token;
        }

        public bool TryReadToken(out string token)
        {
            SkipWhitespace();
            if (_position >= _line.Length)
            {
                token = string.Empty;
                return false;
            }

            var start = _position;
            while (_position < _line.Length && !char.IsWhiteSpace(_line[_position]))
            {
                _position++;
            }

            token = _line[start.._position].ToString();
            return true;
        }

        public void EnsureFullyConsumed()
        {
            SkipWhitespace();
            if (_position < _line.Length)
            {
                throw CreateMalformedPayloadException(_line.ToString());
            }
        }

        private void SkipWhitespace()
        {
            while (_position < _line.Length && char.IsWhiteSpace(_line[_position]))
            {
                _position++;
            }
        }
    }
}
