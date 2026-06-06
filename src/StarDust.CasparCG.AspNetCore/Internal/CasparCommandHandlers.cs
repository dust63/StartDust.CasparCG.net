using Microsoft.AspNetCore.Http;
using StarDust.CasparCG.AspNetCore.Contracts;

namespace StarDust.CasparCG.AspNetCore.Internal;

internal static class CasparCommandHandlers
{
    public static Task<IResult> LoadAsync(
        int channel,
        int layer,
        LoadRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.LoadAsync(channel, layer, request.Clip, request.Options, cancellationToken),
            clientResolver,
            cancellationToken);

    public static async Task<IResult> LoadBackgroundAsync(
        int channel,
        int layer,
        LoadBackgroundRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken)
    {
        try
        {
            await clientResolver.ResolveDefaultClient()
                .LoadBackgroundAsync(channel, layer, request.Clip, request.Options, cancellationToken);
            return Results.Ok();
        }
        catch (Exception exception)
        {
            return ToProblemResult(exception);
        }
    }

    public static async Task<IResult> PlayAsync(
        int channel,
        int layer,
        PlayRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = clientResolver.ResolveDefaultClient();
            await client.PlayAsync(channel, layer, request.Clip, request.Options, cancellationToken);

            return Results.Ok();
        }
        catch (Exception exception)
        {
            return ToProblemResult(exception);
        }
    }

    public static Task<IResult> PauseAsync(
        int channel,
        int layer,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).PauseAsync(cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> ResumeAsync(
        int channel,
        int layer,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).ResumeAsync(cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> StopAsync(
        int channel,
        int layer,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).StopAsync(cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> ClearAsync(
        int channel,
        int layer,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).ClearAsync(cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> ClearChannelAsync(
        int channel,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.ClearAsync(channel, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> CallAsync(
        int channel,
        int layer,
        CallRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.CallAsync(channel, layer, request.Arguments, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> CallBackgroundAsync(
        int channel,
        int layer,
        CallRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.CallBgAsync(channel, layer, request.Arguments, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> SwapAsync(
        int channel,
        int layer,
        SwapRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.SwapAsync(channel, layer, request.OtherChannel, request.OtherLayer, request.SwapTransforms, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> AddAsync(
        int channel,
        AddRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.AddAsync(channel, request.Consumer, request.Arguments, request.ConsumerIndex, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> RemoveAsync(
        int channel,
        RemoveRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        request.ConsumerIndex is int consumerIndex
            ? ExecuteAsync(
                client => client.RemoveAsync(channel, consumerIndex, cancellationToken),
                clientResolver,
                cancellationToken)
            : string.IsNullOrWhiteSpace(request.Arguments)
                ? Task.FromResult<IResult>(
                    Results.Problem(
                        title: "Invalid remove request",
                        detail: "Either consumerIndex or arguments must be provided.",
                        statusCode: StatusCodes.Status400BadRequest))
                : ExecuteAsync(
                    client => client.RemoveAsync(channel, request.Arguments!, cancellationToken),
                    clientResolver,
                    cancellationToken);

    public static Task<IResult> ApplyAsync(
        int channel,
        ApplyRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.ApplyAsync(channel, request.Layer, request.Arguments, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> PrintAsync(
        int channel,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.PrintAsync(channel, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> SetAsync(
        int channel,
        SetRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.SetAsync(channel, request.Key, request.Value, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> ClearAllAsync(
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.ClearAllAsync(cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> SetOpacityAsync(
        int channel,
        int layer,
        MixerOpacityRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).MixerOpacityAsync(request.Value, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> SetMixerKeyerAsync(
        int channel,
        int layer,
        BooleanValueRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).MixerKeyerAsync(request.Value, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> SetMixerInvertAsync(
        int channel,
        int layer,
        BooleanValueRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).MixerInvertAsync(request.Value, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> SetMixerBlendAsync(
        int channel,
        int layer,
        StringValueRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).MixerBlendAsync(request.Value, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> SetMixerBrightnessAsync(
        int channel,
        int layer,
        NumericValueRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).MixerBrightnessAsync(request.Value, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> SetMixerSaturationAsync(
        int channel,
        int layer,
        NumericValueRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).MixerSaturationAsync(request.Value, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> SetMixerContrastAsync(
        int channel,
        int layer,
        NumericValueRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).MixerContrastAsync(request.Value, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> SetMixerVolumeAsync(
        int channel,
        int layer,
        NumericValueRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).MixerVolumeAsync(request.Value, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> SetMixerMasterVolumeAsync(
        NumericValueRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.MixerMasterVolumeAsync(request.Value, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> SetMixerChromaAsync(
        int channel,
        int layer,
        ArgumentsRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).MixerChromaAsync(request.Arguments, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> SetMixerLevelsAsync(
        int channel,
        int layer,
        ArgumentsRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).MixerLevelsAsync(request.Arguments, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> SetMixerFillAsync(
        int channel,
        int layer,
        ArgumentsRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).MixerFillAsync(request.Arguments, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> SetMixerClipAsync(
        int channel,
        int layer,
        ArgumentsRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).MixerClipAsync(request.Arguments, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> SetMixerAnchorAsync(
        int channel,
        int layer,
        ArgumentsRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).MixerAnchorAsync(request.Arguments, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> SetMixerCropAsync(
        int channel,
        int layer,
        ArgumentsRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).MixerCropAsync(request.Arguments, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> SetMixerRotationAsync(
        int channel,
        int layer,
        ArgumentsRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).MixerRotationAsync(request.Arguments, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> SetMixerPerspectiveAsync(
        int channel,
        int layer,
        ArgumentsRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).MixerPerspectiveAsync(request.Arguments, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> SetMixerGridAsync(
        int channel,
        int layer,
        ArgumentsRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).MixerGridAsync(request.Arguments, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> CommitMixerAsync(
        int channel,
        int layer,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).MixerCommitAsync(cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> ClearMixerAsync(
        int channel,
        int layer,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).MixerClearAsync(cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> RunChannelGridAsync(
        int channel,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).GridAsync(cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> PutDataAsync(
        string key,
        DataStoreRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken)
    {
        try
        {
            return PutDataCoreAsync(key, request, clientResolver, cancellationToken);
        }
        catch (Exception exception)
        {
            return Task.FromResult(ToProblemResult(exception));
        }
    }

    public static Task<IResult> RestartAsync(
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Admin().RestartAsync(cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> RunServerGlGcAsync(
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Server().GlGcAsync(cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> RunServerDiagAsync(
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Server().DiagAsync(cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> ByeAsync(
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Admin().ByeAsync(cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> KillAsync(
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Admin().KillAsync(cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> SetAdminLogLevelAsync(
        StringValueRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Admin().SetLogLevelAsync(request.Value, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> AcquireLockAsync(
        int channel,
        LockAcquireRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Admin().AcquireLockAsync(channel, request.Phrase, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> ReleaseLockAsync(
        int channel,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Admin().ReleaseLockAsync(channel, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> ClearLockAsync(
        int channel,
        LockClearRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Admin().ClearLockAsync(channel, request.OverridePhrase, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> DeleteDataAsync(
        string key,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            async client => await client.Data().RemoveAsync(key, cancellationToken),
            clientResolver,
            cancellationToken);

    public static async Task<IResult> CgAddAsync(
        int channel,
        int layer,
        HttpRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken)
    {
        try
        {
            var payload = await CgPayloadTranslator.TranslateAddAsync(request, cancellationToken);
            await clientResolver.ResolveDefaultClient()
                .Channel(channel)
                .Layer(layer)
                .CgAddAsync(payload.Template!, payload.PlayOnLoad, payload.TemplateXml, cancellationToken);

            return Results.Ok();
        }
        catch (BadHttpRequestException exception)
        {
            return Results.Problem(
                title: "Invalid CG request",
                detail: exception.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
        catch (Exception exception)
        {
            return ToProblemResult(exception);
        }
    }

    public static Task<IResult> CgPlayAsync(
        int channel,
        int layer,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).CgPlayAsync(cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> CgStopAsync(
        int channel,
        int layer,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).CgStopAsync(cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> CgNextAsync(
        int channel,
        int layer,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).CgNextAsync(cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> CgRemoveAsync(
        int channel,
        int layer,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).CgRemoveAsync(cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> CgClearAsync(
        int channel,
        int layer,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).CgClearAsync(cancellationToken),
            clientResolver,
            cancellationToken);

    public static async Task<IResult> CgUpdateAsync(
        int channel,
        int layer,
        HttpRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken)
    {
        try
        {
            var payload = await CgPayloadTranslator.TranslateUpdateAsync(request, cancellationToken);
            await clientResolver.ResolveDefaultClient()
                .Channel(channel)
                .Layer(layer)
                .CgUpdateAsync(payload.TemplateXml, cancellationToken);

            return Results.Ok();
        }
        catch (BadHttpRequestException exception)
        {
            return Results.Problem(
                title: "Invalid CG request",
                detail: exception.Message,
                statusCode: StatusCodes.Status400BadRequest);
        }
        catch (Exception exception)
        {
            return ToProblemResult(exception);
        }
    }

    public static Task<IResult> CgInvokeAsync(
        int channel,
        int layer,
        CgInvokeRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Channel(channel).Layer(layer).CgInvokeAsync(request.Method, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> GenerateThumbnailAsync(
        string fileName,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Thumbnails().GenerateAsync(fileName, cancellationToken),
            clientResolver,
            cancellationToken);

    public static Task<IResult> GenerateAllThumbnailsAsync(
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken) =>
        ExecuteAsync(
            client => client.Thumbnails().GenerateAllAsync(cancellationToken),
            clientResolver,
            cancellationToken);

    private static async Task<IResult> ExecuteAsync(
        Func<CasparClient, ValueTask> action,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken)
    {
        try
        {
            await action(clientResolver.ResolveDefaultClient());
            return Results.Ok();
        }
        catch (Exception exception)
        {
            return ToProblemResult(exception);
        }
    }

    private static async Task<IResult> PutDataCoreAsync(
        string key,
        DataStoreRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken)
    {
        try
        {
            await clientResolver.ResolveDefaultClient().Data().StoreAsync(key, request.Value, cancellationToken);
            return Results.Ok();
        }
        catch (Exception exception)
        {
            return ToProblemResult(exception);
        }
    }

    private static IResult ToProblemResult(Exception exception)
    {
        var problem = CasparProblemDetailsFactory.FromException(exception);
        return TypedResults.Problem(
            title: problem.Title,
            detail: problem.Detail,
            statusCode: problem.Status,
            type: problem.Type,
            instance: problem.Instance,
            extensions: problem.Extensions);
    }
}
