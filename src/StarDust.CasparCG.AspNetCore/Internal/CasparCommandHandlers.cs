using Microsoft.AspNetCore.Http;
using StarDust.CasparCG.AspNetCore.Contracts;

namespace StarDust.CasparCG.AspNetCore.Internal;

internal static class CasparCommandHandlers
{
    public static async Task<IResult> LoadBackgroundAsync(
        int channel,
        int layer,
        LoadBackgroundRequest request,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken)
    {
        try
        {
            var builder = clientResolver.ResolveDefaultClient()
                .Channel(channel)
                .Layer(layer)
                .LoadBg(request.Clip);

            if (request.Loop)
            {
                builder = builder.Loop();
            }

            if (request.AutoPlay)
            {
                builder = builder.AutoPlay();
            }

            await builder.SendAsync(cancellationToken);
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

            if (request.TransitionDuration is int transitionDuration)
            {
                var builder = client.Channel(channel).Layer(layer).Play(request.Clip).WithTransition().Mix(transitionDuration);
                if (request.Loop)
                {
                    builder = builder.WithLoop();
                }

                await builder.SendAsync(cancellationToken);
            }
            else
            {
                var builder = client.Channel(channel).Layer(layer).Play(request.Clip);
                if (request.Loop)
                {
                    builder = builder.WithLoop();
                }

                await builder.SendAsync(cancellationToken);
            }

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
        return Results.Problem(
            title: problem.Title,
            detail: problem.Detail,
            statusCode: problem.Status);
    }
}
