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
    public PlayCommandBuilder Play(string clip) => new(client, channel, layer, clip);

    /// <summary>
    /// Sends a PAUSE command for this layer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask PauseAsync(CancellationToken cancellationToken) =>
        client.PauseAsync(channel, layer, cancellationToken);

    /// <summary>
    /// Sends a RESUME command for this layer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ResumeAsync(CancellationToken cancellationToken) =>
        client.ResumeAsync(channel, layer, cancellationToken);

    /// <summary>
    /// Sends a STOP command for this layer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask StopAsync(CancellationToken cancellationToken) =>
        client.StopAsync(channel, layer, cancellationToken);

    /// <summary>
    /// Sends a CLEAR command for this layer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ClearAsync(CancellationToken cancellationToken) =>
        client.ClearAsync(channel, layer, cancellationToken);

    /// <summary>
    /// Sends a CALL command for this layer.
    /// </summary>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CallAsync(string clip, CancellationToken cancellationToken) =>
        client.CallAsync(channel, layer, clip, cancellationToken);

    /// <summary>
    /// Sends a CALLBG command for this layer.
    /// </summary>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CallBgAsync(string clip, CancellationToken cancellationToken) =>
        client.CallBgAsync(channel, layer, clip, cancellationToken);

    /// <summary>
    /// Sends a SWAP command from this layer to another layer.
    /// </summary>
    /// <param name="otherChannel">The target channel.</param>
    /// <param name="otherLayer">The target layer.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask SwapAsync(int otherChannel, int otherLayer, CancellationToken cancellationToken) =>
        client.SwapAsync(channel, layer, otherChannel, otherLayer, cancellationToken);

    /// <summary>
    /// Sends an ADD command for this layer.
    /// </summary>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask AddAsync(string clip, CancellationToken cancellationToken) =>
        client.AddAsync(channel, layer, clip, cancellationToken);

    /// <summary>
    /// Sends a REMOVE command for this layer.
    /// </summary>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask RemoveAsync(string clip, CancellationToken cancellationToken) =>
        client.RemoveAsync(channel, layer, clip, cancellationToken);

    /// <summary>
    /// Sends an APPLY command for this layer.
    /// </summary>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask ApplyAsync(string clip, CancellationToken cancellationToken) =>
        client.ApplyAsync(channel, layer, clip, cancellationToken);

    /// <summary>
    /// Sends a PRINT command for this layer.
    /// </summary>
    /// <param name="clip">The clip identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask PrintAsync(string clip, CancellationToken cancellationToken) =>
        client.PrintAsync(channel, layer, clip, cancellationToken);

    /// <summary>
    /// Sends a SET command for this layer.
    /// </summary>
    /// <param name="key">The property name.</param>
    /// <param name="value">The property value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask SetAsync(string key, string value, CancellationToken cancellationToken) =>
        client.SetAsync(channel, layer, key, value, cancellationToken);

    /// <summary>
    /// Sends a CG PLAY command for this layer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CgPlayAsync(CancellationToken cancellationToken) =>
        client.CgPlayAsync(channel, layer, cancellationToken);

    /// <summary>
    /// Sends a CG STOP command for this layer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CgStopAsync(CancellationToken cancellationToken) =>
        client.CgStopAsync(channel, layer, cancellationToken);

    /// <summary>
    /// Sends a CG NEXT command for this layer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CgNextAsync(CancellationToken cancellationToken) =>
        client.CgNextAsync(channel, layer, cancellationToken);

    /// <summary>
    /// Sends a CG REMOVE command for this layer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CgRemoveAsync(CancellationToken cancellationToken) =>
        client.CgRemoveAsync(channel, layer, cancellationToken);

    /// <summary>
    /// Sends a CG CLEAR command for this layer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CgClearAsync(CancellationToken cancellationToken) =>
        client.CgClearAsync(channel, layer, cancellationToken);

    /// <summary>
    /// Sends a CG INVOKE command for this layer.
    /// </summary>
    /// <param name="method">The method name.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask CgInvokeAsync(string method, CancellationToken cancellationToken) =>
        client.CgInvokeAsync(channel, layer, method, cancellationToken);

    /// <summary>
    /// Sends a MIXER KEYER command for this layer.
    /// </summary>
    /// <param name="keyer">Whether the keyer is enabled.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerKeyerAsync(bool keyer, CancellationToken cancellationToken) =>
        client.MixerKeyerAsync(channel, layer, keyer, cancellationToken);

    /// <summary>
    /// Sends a MIXER INVERT command for this layer.
    /// </summary>
    /// <param name="invert">Whether colors are inverted.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerInvertAsync(bool invert, CancellationToken cancellationToken) =>
        client.MixerInvertAsync(channel, layer, invert, cancellationToken);

    /// <summary>
    /// Sends a MIXER BLEND command for this layer.
    /// </summary>
    /// <param name="blend">The blend mode.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerBlendAsync(string blend, CancellationToken cancellationToken) =>
        client.MixerBlendAsync(channel, layer, blend, cancellationToken);

    /// <summary>
    /// Sends a MIXER OPACITY command for this layer.
    /// </summary>
    /// <param name="opacity">The opacity value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerOpacityAsync(double opacity, CancellationToken cancellationToken) =>
        client.MixerOpacityAsync(channel, layer, opacity, cancellationToken);

    /// <summary>
    /// Sends a MIXER BRIGHTNESS command for this layer.
    /// </summary>
    /// <param name="brightness">The brightness value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerBrightnessAsync(double brightness, CancellationToken cancellationToken) =>
        client.MixerBrightnessAsync(channel, layer, brightness, cancellationToken);

    /// <summary>
    /// Sends a MIXER SATURATION command for this layer.
    /// </summary>
    /// <param name="saturation">The saturation value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerSaturationAsync(double saturation, CancellationToken cancellationToken) =>
        client.MixerSaturationAsync(channel, layer, saturation, cancellationToken);

    /// <summary>
    /// Sends a MIXER CONTRAST command for this layer.
    /// </summary>
    /// <param name="contrast">The contrast value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerContrastAsync(double contrast, CancellationToken cancellationToken) =>
        client.MixerContrastAsync(channel, layer, contrast, cancellationToken);

    /// <summary>
    /// Sends a MIXER VOLUME command for this layer.
    /// </summary>
    /// <param name="volume">The volume value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerVolumeAsync(double volume, CancellationToken cancellationToken) =>
        client.MixerVolumeAsync(channel, layer, volume, cancellationToken);

    /// <summary>
    /// Sends a MIXER COMMIT command for this layer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerCommitAsync(CancellationToken cancellationToken) =>
        client.MixerCommitAsync(channel, layer, cancellationToken);

    /// <summary>
    /// Sends a MIXER CLEAR command for this layer.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous send operation.</returns>
    public ValueTask MixerClearAsync(CancellationToken cancellationToken) =>
        client.MixerClearAsync(channel, layer, cancellationToken);
}
