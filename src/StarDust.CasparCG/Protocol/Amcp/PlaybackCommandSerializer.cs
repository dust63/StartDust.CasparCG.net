using System.Globalization;
using StarDust.CasparCG.Protocol.Amcp.Commands;

namespace StarDust.CasparCG.Protocol.Amcp;

internal static class PlaybackCommandSerializer
{
    public static string SerializeWithOptions(string command, int channel, int layer, string clip, PlaybackOptions? options)
    {
        var parts = new List<string> { command, AmcpCommandFormatting.ChannelLayer(channel, layer), clip };
        AppendPlaybackOptions(parts, options);
        return string.Join(' ', parts) + "\r\n";
    }

    public static void AppendPlaybackOptions(List<string> parts, PlaybackOptions? options)
    {
        if (options is null)
        {
            return;
        }

        AppendTransition(parts, options.Transition);

        if (options.Loop)
        {
            parts.Add("LOOP");
        }

        if (options.Seek is int seek)
        {
            parts.Add("SEEK");
            parts.Add(seek.ToString(CultureInfo.InvariantCulture));
        }

        if (options.Length is int length)
        {
            parts.Add("LENGTH");
            parts.Add(length.ToString(CultureInfo.InvariantCulture));
        }

        if (!string.IsNullOrWhiteSpace(options.Filter))
        {
            parts.Add("FILTER");
            parts.Add(options.Filter);
        }

        if (options.ClearOn404)
        {
            parts.Add("CLEAR_ON_404");
        }
    }

    public static void AppendLoadBackgroundOptions(List<string> parts, LoadBackgroundOptions? options)
    {
        AppendPlaybackOptions(parts, options);

        if (options?.AutoPlay == true)
        {
            parts.Add("AUTO");
        }
    }

    private static void AppendTransition(List<string> parts, PlaybackTransition? transition)
    {
        if (transition is null)
        {
            return;
        }

        if (transition.Kind == PlaybackTransitionKind.Sting)
        {
            parts.Add("STING");
            parts.Add(transition.MaskFilename ?? string.Empty);

            if (transition.TriggerPoint is int triggerPoint)
            {
                parts.Add(triggerPoint.ToString(CultureInfo.InvariantCulture));
            }

            if (!string.IsNullOrWhiteSpace(transition.OverlayFilename))
            {
                parts.Add(transition.OverlayFilename);
            }

            if (transition.AudioFadeStart is int audioFadeStart)
            {
                parts.Add("audio_fade_start");
                parts.Add(audioFadeStart.ToString(CultureInfo.InvariantCulture));
            }

            if (transition.AudioFadeDuration is int audioFadeDuration)
            {
                parts.Add("audio_fade_duration");
                parts.Add(audioFadeDuration.ToString(CultureInfo.InvariantCulture));
            }

            return;
        }

        parts.Add(transition.Kind switch
        {
            PlaybackTransitionKind.Cut => "CUT",
            PlaybackTransitionKind.Mix => "MIX",
            PlaybackTransitionKind.Push => "PUSH",
            PlaybackTransitionKind.Slide => "SLIDE",
            PlaybackTransitionKind.Wipe => "WIPE",
            PlaybackTransitionKind.FadeCut => "FADECUT",
            PlaybackTransitionKind.CutFade => "CUTFADE",
            PlaybackTransitionKind.VFade => "VFADE",
            _ => throw new ArgumentOutOfRangeException(nameof(transition), transition.Kind, "Unsupported transition kind.")
        });

        if (transition.Duration is int duration)
        {
            parts.Add(duration.ToString(CultureInfo.InvariantCulture));
        }

        if (!string.IsNullOrWhiteSpace(transition.Tweener))
        {
            parts.Add(transition.Tweener);
        }

        if (transition.Direction is PlaybackTransitionDirection direction)
        {
            parts.Add(direction switch
            {
                PlaybackTransitionDirection.Left => "LEFT",
                PlaybackTransitionDirection.Right => "RIGHT",
                PlaybackTransitionDirection.Top => "TOP",
                PlaybackTransitionDirection.Bottom => "BOTTOM",
                _ => throw new ArgumentOutOfRangeException(nameof(transition), direction, "Unsupported transition direction.")
            });
        }
    }
}
