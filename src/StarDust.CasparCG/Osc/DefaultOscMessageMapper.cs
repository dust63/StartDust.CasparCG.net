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
    public IReadOnlyList<CasparEvent> Map(string address, IReadOnlyList<object?> arguments)
    {
        if (!TryParseChannelPath(address, out var channel, out var path))
        {
            return Array.Empty<CasparEvent>();
        }

        var events = new List<CasparEvent>
        {
            new OscStateChangedEvent(_clientName, channel, path, arguments)
        };

        if (TryParsePlaybackClipChanged(channel, path, arguments, out var clipChanged))
        {
            events.Add(clipChanged);
        }

        if (TryParseLayerProducerChanged(channel, path, arguments, out var producerChanged))
        {
            events.Add(producerChanged);
        }

        if (TryParseLayerPausedChanged(channel, path, arguments, out var pausedChanged))
        {
            events.Add(pausedChanged);
        }

        if (TryParseLayerProgressChanged(channel, path, arguments, out var progressChanged))
        {
            events.Add(progressChanged);
        }

        if (TryParseLayerFramesLeftChanged(channel, path, arguments, out var framesLeftChanged))
        {
            events.Add(framesLeftChanged);
        }

        return events;
    }

    private static bool TryParseChannelPath(string address, out int channel, out string path)
    {
        channel = 0;
        path = string.Empty;

        const string prefix = "/channel/";
        ReadOnlySpan<char> addressSpan = address.AsSpan();
        ReadOnlySpan<char> prefixSpan = prefix.AsSpan();

        if (!addressSpan.StartsWith(prefixSpan, StringComparison.Ordinal))
        {
            return false;
        }

        var pathStart = addressSpan[prefixSpan.Length..].IndexOf('/');
        if (pathStart <= 0)
        {
            return false;
        }

        var channelText = addressSpan[prefixSpan.Length..(prefixSpan.Length + pathStart)];
        if (!int.TryParse(channelText, out channel))
        {
            return false;
        }

        path = addressSpan[(prefixSpan.Length + pathStart + 1)..].ToString();
        return true;
    }

    private bool TryParsePlaybackClipChanged(
        int channel,
        string path,
        IReadOnlyList<object?> arguments,
        out CasparEvent evt)
    {
        evt = null!;

        if (!TryParseLayerSlotPath(path, "file/name", out var layer, out _) ||
            arguments.Count != 1 ||
            arguments[0] is not string clip)
        {
            return false;
        }

        evt = new PlaybackClipChangedEvent(_clientName, channel, layer, clip);
        return true;
    }

    private bool TryParseLayerProducerChanged(
        int channel,
        string path,
        IReadOnlyList<object?> arguments,
        out CasparEvent evt)
    {
        evt = null!;

        if (!TryParseLayerSlotPath(path, "producer", out var layer, out var slot) ||
            arguments.Count != 1 ||
            arguments[0] is not string producer)
        {
            return false;
        }

        evt = new LayerProducerChangedEvent(_clientName, channel, layer, slot, producer);
        return true;
    }

    private bool TryParseLayerPausedChanged(
        int channel,
        string path,
        IReadOnlyList<object?> arguments,
        out CasparEvent evt)
    {
        evt = null!;

        if (!TryParseLayerSlotPath(path, "paused", out var layer, out var slot) ||
            slot != "foreground" ||
            arguments.Count != 1 ||
            arguments[0] is not bool paused)
        {
            return false;
        }

        evt = new LayerPausedChangedEvent(_clientName, channel, layer, paused);
        return true;
    }

    private bool TryParseLayerProgressChanged(
        int channel,
        string path,
        IReadOnlyList<object?> arguments,
        out CasparEvent evt)
    {
        evt = null!;

        if (!TryParseLayerSlotPath(path, "file/time", out var layer, out var slot) ||
            arguments.Count != 2 ||
            !TryGetDouble(arguments[0], out var positionSeconds) ||
            !TryGetDouble(arguments[1], out var durationSeconds))
        {
            return false;
        }

        evt = new LayerProgressChangedEvent(_clientName, channel, layer, slot, positionSeconds, durationSeconds);
        return true;
    }

    private bool TryParseLayerFramesLeftChanged(
        int channel,
        string path,
        IReadOnlyList<object?> arguments,
        out CasparEvent evt)
    {
        evt = null!;

        if (!TryParseLayerSlotPath(path, "frames_left", out var layer, out var slot) ||
            slot != "foreground" ||
            arguments.Count != 1 ||
            !TryGetLong(arguments[0], out var framesLeft))
        {
            return false;
        }

        evt = new LayerFramesLeftChangedEvent(_clientName, channel, layer, framesLeft);
        return true;
    }

    private static bool TryParseLayerSlotPath(string path, string expectedTail, out int layer, out string slot)
    {
        layer = 0;
        slot = string.Empty;

        const string prefix = "stage/layer/";
        if (!path.StartsWith(prefix, StringComparison.Ordinal) ||
            !path.EndsWith('/' + expectedTail, StringComparison.Ordinal))
        {
            return false;
        }

        var remaining = path[prefix.Length..^expectedTail.Length];
        if (remaining.EndsWith('/'))
        {
            remaining = remaining[..^1];
        }

        var separatorIndex = remaining.IndexOf('/');
        if (separatorIndex <= 0 || separatorIndex == remaining.Length - 1)
        {
            return false;
        }

        if (!int.TryParse(remaining[..separatorIndex], out layer))
        {
            return false;
        }

        slot = remaining[(separatorIndex + 1)..];
        return slot is "foreground" or "background";
    }

    private static bool TryGetDouble(object? value, out double result)
    {
        result = value switch
        {
            double doubleValue => doubleValue,
            float floatValue => floatValue,
            int intValue => intValue,
            long longValue => longValue,
            _ => double.NaN
        };

        return !double.IsNaN(result);
    }

    private static bool TryGetLong(object? value, out long result)
    {
        result = value switch
        {
            long longValue => longValue,
            int intValue => intValue,
            _ => 0
        };

        return value is long or int;
    }
}
