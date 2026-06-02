using StarDust.CasparCG.Events;

namespace StarDust.CasparCG.Osc;

/// <summary>
/// Maps OSC messages into domain events.
/// </summary>
public sealed class DefaultOscMessageMapper : IOscMessageMapper
{
    private readonly string _clientName;

    /// <summary>
    /// Initializes a new mapper.
    /// </summary>
    /// <param name="clientName">The client registration name.</param>
    public DefaultOscMessageMapper(string clientName = "default")
    {
        _clientName = clientName;
    }

    /// <inheritdoc />
    public bool TryMap(string address, IReadOnlyList<object?> arguments, out CasparEvent? evt)
    {
        evt = null;

        if (!TryParsePlaybackClipChanged(address, arguments, out evt))
        {
            return false;
        }

        return true;
    }

    private bool TryParsePlaybackClipChanged(string address, IReadOnlyList<object?> arguments, out CasparEvent? evt)
    {
        evt = null;

        const string prefix = "/channel/";
        const string suffix = "/stage/layer/";
        const string backgroundSuffix = "/background/file/name";
        const string foregroundSuffix = "/foreground/file/name";
        ReadOnlySpan<char> addressSpan = address.AsSpan();
        ReadOnlySpan<char> prefixSpan = prefix.AsSpan();
        ReadOnlySpan<char> suffixSpan = suffix.AsSpan();
        ReadOnlySpan<char> backgroundSuffixSpan = backgroundSuffix.AsSpan();
        ReadOnlySpan<char> foregroundSuffixSpan = foregroundSuffix.AsSpan();

        var suffixIndex = addressSpan.IndexOf(suffixSpan, StringComparison.Ordinal);
        var isBackground = addressSpan.EndsWith(backgroundSuffixSpan, StringComparison.Ordinal);
        var isForeground = addressSpan.EndsWith(foregroundSuffixSpan, StringComparison.Ordinal);
        var clipSuffixSpan = isBackground ? backgroundSuffixSpan : foregroundSuffixSpan;
        if (!addressSpan.StartsWith(prefixSpan, StringComparison.Ordinal) ||
            suffixIndex < 0 ||
            (!isBackground && !isForeground) ||
            arguments.Count != 1 ||
            arguments[0] is not string clip)
        {
            return false;
        }

        var channelText = addressSpan[prefixSpan.Length..suffixIndex];
        var layerStart = suffixIndex + suffixSpan.Length;
        var layerText = addressSpan[layerStart..^clipSuffixSpan.Length];

        if (!int.TryParse(channelText, out var channel) || !int.TryParse(layerText, out var layer))
        {
            return false;
        }

        evt = new PlaybackClipChangedEvent(_clientName, channel, layer, clip);
        return true;
    }
}
