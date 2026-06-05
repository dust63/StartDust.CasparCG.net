namespace StarDust.CasparCG.Fluent;

/// <summary>
/// Represents a fluent layer scope for command composition.
/// </summary>
public sealed class LayerScope(CasparClient client, int channel, int layer)
{
    /// <summary>
    /// Starts building a fluent play command.
    /// </summary>
    /// <param name="clip">The clip identifier.</param>
    /// <returns>A play command builder.</returns>
    public PlayCommandBuilder Play(string clip)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clip);
        return new(client, channel, layer, clip);
    }

    /// <summary>
    /// Starts building a fluent load background command.
    /// </summary>
    /// <param name="clip">The clip identifier.</param>
    /// <returns>A load background command builder.</returns>
    public LoadBackgroundCommandBuilder LoadBg(string clip)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clip);
        return new(client, channel, layer, clip);
    }

    /// <summary>
    /// Starts building a local layer sequence.
    /// </summary>
    /// <returns>A layer sequence builder.</returns>
    public LayerSequenceBuilder Sequence() => new(client, channel, layer);

    /// <summary>
    /// Sends a LOAD command for this layer.
    /// </summary>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask LoadAsync(string clip, CancellationToken cancellationToken = default) =>
        client.LoadAsync(channel, layer, clip, cancellationToken);

    /// <summary>
    /// Sends a PAUSE command for this layer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask PauseAsync(CancellationToken cancellationToken = default) =>
        client.PauseAsync(channel, layer, cancellationToken);

    /// <summary>
    /// Sends a RESUME command for this layer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ResumeAsync(CancellationToken cancellationToken = default) =>
        client.ResumeAsync(channel, layer, cancellationToken);

    /// <summary>
    /// Sends a STOP command for this layer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        client.StopAsync(channel, layer, cancellationToken);

    /// <summary>
    /// Sends a CLEAR command for this layer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ClearAsync(CancellationToken cancellationToken = default) =>
        client.ClearAsync(channel, layer, cancellationToken);

    /// <summary>
    /// Sends a CALL command for this layer.
    /// </summary>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CallAsync(string clip, CancellationToken cancellationToken = default) =>
        client.CallAsync(channel, layer, clip, cancellationToken);

    /// <summary>
    /// Sends a CALLBG command for this layer.
    /// </summary>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CallBgAsync(string clip, CancellationToken cancellationToken = default) =>
        client.CallBgAsync(channel, layer, clip, cancellationToken);

    /// <summary>
    /// Sends a SWAP command from this layer to another layer.
    /// </summary>
    /// <param name="otherChannel">The target channel.</param>
    /// <param name="otherLayer">The target layer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask SwapAsync(int otherChannel, int otherLayer, CancellationToken cancellationToken = default) =>
        client.SwapAsync(channel, layer, otherChannel, otherLayer, cancellationToken);

    /// <summary>
    /// Sends an ADD command for this layer.
    /// </summary>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask AddAsync(string clip, CancellationToken cancellationToken = default) =>
        client.AddAsync(channel, layer, clip, cancellationToken);

    /// <summary>
    /// Sends a REMOVE command for this layer.
    /// </summary>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask RemoveAsync(string clip, CancellationToken cancellationToken = default) =>
        client.RemoveAsync(channel, layer, clip, cancellationToken);

    /// <summary>
    /// Sends an APPLY command for this layer.
    /// </summary>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ApplyAsync(string clip, CancellationToken cancellationToken = default) =>
        client.ApplyAsync(channel, layer, clip, cancellationToken);

    /// <summary>
    /// Sends a PRINT command for this layer.
    /// </summary>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask PrintAsync(string clip, CancellationToken cancellationToken = default) =>
        client.PrintAsync(channel, layer, clip, cancellationToken);

    /// <summary>
    /// Sends a SET command for this layer.
    /// </summary>
    /// <param name="key">The property name.</param>
    /// <param name="value">The property value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask SetAsync(string key, string value, CancellationToken cancellationToken = default) =>
        client.SetAsync(channel, layer, key, value, cancellationToken);

    /// <summary>
    /// Sends a CG PLAY command for this layer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CgPlayAsync(CancellationToken cancellationToken = default) =>
        client.CgPlayAsync(channel, layer, cancellationToken);

    /// <summary>
    /// Sends a CG STOP command for this layer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CgStopAsync(CancellationToken cancellationToken = default) =>
        client.CgStopAsync(channel, layer, cancellationToken);

    /// <summary>
    /// Sends a CG NEXT command for this layer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CgNextAsync(CancellationToken cancellationToken = default) =>
        client.CgNextAsync(channel, layer, cancellationToken);

    /// <summary>
    /// Sends a CG REMOVE command for this layer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CgRemoveAsync(CancellationToken cancellationToken = default) =>
        client.CgRemoveAsync(channel, layer, cancellationToken);

    /// <summary>
    /// Sends a CG CLEAR command for this layer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CgClearAsync(CancellationToken cancellationToken = default) =>
        client.CgClearAsync(channel, layer, cancellationToken);

    /// <summary>
    /// Sends a CG INVOKE command for this layer.
    /// </summary>
    /// <param name="method">The method name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CgInvokeAsync(string method, CancellationToken cancellationToken = default) =>
        client.CgInvokeAsync(channel, layer, method, cancellationToken);

    /// <summary>
    /// Sends a CG ADD command for this layer.
    /// </summary>
    /// <param name="template">The template name.</param>
    /// <param name="playOnLoad">Whether the template should play on load.</param>
    /// <param name="data">The optional inline XML payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CgAddAsync(string template, bool playOnLoad, string? data, CancellationToken cancellationToken = default) =>
        client.CgAddAsync(channel, layer, template, playOnLoad, data, cancellationToken);

    /// <summary>
    /// Sends a CG UPDATE command for this layer.
    /// </summary>
    /// <param name="data">The inline XML payload.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The AMCP response.</returns>
    public ValueTask<Protocol.Amcp.AmcpResponse> CgUpdateAsync(string data, CancellationToken cancellationToken = default) =>
        client.CgUpdateAsync(channel, layer, data, cancellationToken);

    /// <summary>
    /// Sends a MIXER KEYER command for this layer.
    /// </summary>
    /// <param name="keyer">Whether the keyer is enabled.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerKeyerAsync(bool keyer, CancellationToken cancellationToken = default) =>
        client.MixerKeyerAsync(channel, layer, keyer, cancellationToken);

    /// <summary>
    /// Sends a MIXER INVERT command for this layer.
    /// </summary>
    /// <param name="invert">Whether colors are inverted.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerInvertAsync(bool invert, CancellationToken cancellationToken = default) =>
        client.MixerInvertAsync(channel, layer, invert, cancellationToken);

    /// <summary>
    /// Sends a MIXER BLEND command for this layer.
    /// </summary>
    /// <param name="blend">The blend mode.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerBlendAsync(string blend, CancellationToken cancellationToken = default) =>
        client.MixerBlendAsync(channel, layer, blend, cancellationToken);

    /// <summary>
    /// Sends a MIXER OPACITY command for this layer.
    /// </summary>
    /// <param name="opacity">The opacity value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerOpacityAsync(double opacity, CancellationToken cancellationToken = default) =>
        client.MixerOpacityAsync(channel, layer, opacity, cancellationToken);

    /// <summary>
    /// Sends a MIXER BRIGHTNESS command for this layer.
    /// </summary>
    /// <param name="brightness">The brightness value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerBrightnessAsync(double brightness, CancellationToken cancellationToken = default) =>
        client.MixerBrightnessAsync(channel, layer, brightness, cancellationToken);

    /// <summary>
    /// Sends a MIXER SATURATION command for this layer.
    /// </summary>
    /// <param name="saturation">The saturation value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerSaturationAsync(double saturation, CancellationToken cancellationToken = default) =>
        client.MixerSaturationAsync(channel, layer, saturation, cancellationToken);

    /// <summary>
    /// Sends a MIXER CONTRAST command for this layer.
    /// </summary>
    /// <param name="contrast">The contrast value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerContrastAsync(double contrast, CancellationToken cancellationToken = default) =>
        client.MixerContrastAsync(channel, layer, contrast, cancellationToken);

    /// <summary>
    /// Sends a MIXER VOLUME command for this layer.
    /// </summary>
    /// <param name="volume">The volume value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerVolumeAsync(double volume, CancellationToken cancellationToken = default) =>
        client.MixerVolumeAsync(channel, layer, volume, cancellationToken);

    /// <summary>
    /// Sends a MIXER CHROMA command for this layer.
    /// </summary>
    /// <param name="arguments">The raw command tail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerChromaAsync(string? arguments, CancellationToken cancellationToken = default) =>
        client.MixerChromaAsync(channel, layer, arguments, cancellationToken);

    /// <summary>
    /// Sends a MIXER LEVELS command for this layer.
    /// </summary>
    /// <param name="arguments">The raw command tail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerLevelsAsync(string? arguments, CancellationToken cancellationToken = default) =>
        client.MixerLevelsAsync(channel, layer, arguments, cancellationToken);

    /// <summary>
    /// Sends a MIXER FILL command for this layer.
    /// </summary>
    /// <param name="arguments">The raw command tail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerFillAsync(string? arguments, CancellationToken cancellationToken = default) =>
        client.MixerFillAsync(channel, layer, arguments, cancellationToken);

    /// <summary>
    /// Sends a MIXER CLIP command for this layer.
    /// </summary>
    /// <param name="arguments">The raw command tail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerClipAsync(string? arguments, CancellationToken cancellationToken = default) =>
        client.MixerClipAsync(channel, layer, arguments, cancellationToken);

    /// <summary>
    /// Sends a MIXER ANCHOR command for this layer.
    /// </summary>
    /// <param name="arguments">The raw command tail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerAnchorAsync(string? arguments, CancellationToken cancellationToken = default) =>
        client.MixerAnchorAsync(channel, layer, arguments, cancellationToken);

    /// <summary>
    /// Sends a MIXER CROP command for this layer.
    /// </summary>
    /// <param name="arguments">The raw command tail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerCropAsync(string? arguments, CancellationToken cancellationToken = default) =>
        client.MixerCropAsync(channel, layer, arguments, cancellationToken);

    /// <summary>
    /// Sends a MIXER ROTATION command for this layer.
    /// </summary>
    /// <param name="arguments">The raw command tail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerRotationAsync(string? arguments, CancellationToken cancellationToken = default) =>
        client.MixerRotationAsync(channel, layer, arguments, cancellationToken);

    /// <summary>
    /// Sends a MIXER PERSPECTIVE command for this layer.
    /// </summary>
    /// <param name="arguments">The raw command tail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerPerspectiveAsync(string? arguments, CancellationToken cancellationToken = default) =>
        client.MixerPerspectiveAsync(channel, layer, arguments, cancellationToken);

    /// <summary>
    /// Sends a MIXER GRID command for this layer.
    /// </summary>
    /// <param name="arguments">The raw command tail.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerGridAsync(string? arguments, CancellationToken cancellationToken = default) =>
        client.MixerGridAsync(channel, layer, arguments, cancellationToken);

    /// <summary>
    /// Sends a MIXER COMMIT command for this layer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerCommitAsync(CancellationToken cancellationToken = default) =>
        client.MixerCommitAsync(channel, layer, cancellationToken);

    /// <summary>
    /// Sends a MIXER CLEAR command for this layer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerClearAsync(CancellationToken cancellationToken = default) =>
        client.MixerClearAsync(channel, layer, cancellationToken);
}
