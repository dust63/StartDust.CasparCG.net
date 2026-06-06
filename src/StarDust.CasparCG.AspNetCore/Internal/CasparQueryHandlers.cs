using Microsoft.AspNetCore.Http;
using StarDust.CasparCG.AspNetCore.Contracts;

namespace StarDust.CasparCG.AspNetCore.Internal;

internal static class CasparQueryHandlers
{
    public static async Task<IResult> GetServerVersionAsync(
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken)
    {
        var version = await clientResolver.ResolveDefaultClient()
            .Server()
            .VersionAsync(cancellationToken);

        return Results.Ok(new ServerVersionResponse(version));
    }

    public static async Task<IResult> GetServerInfoAsync(
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken)
    {
        var info = await clientResolver.ResolveDefaultClient()
            .Server()
            .InfoAsync(cancellationToken);

        return Results.Ok(info);
    }

    public static async Task<IResult> GetServerInfoConfigAsync(
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken)
    {
        var info = await clientResolver.ResolveDefaultClient()
            .Server()
            .InfoConfigAsync(cancellationToken);

        return Results.Ok(info);
    }

    public static async Task<IResult> GetServerInfoPathsAsync(
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken)
    {
        var info = await clientResolver.ResolveDefaultClient()
            .Server()
            .InfoPathsAsync(cancellationToken);

        return Results.Ok(info);
    }

    public static async Task<IResult> GetServerGlInfoAsync(
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken)
    {
        var info = await clientResolver.ResolveDefaultClient()
            .Server()
            .GlInfoAsync(cancellationToken);

        return Results.Ok(info);
    }

    public static async Task<IResult> GetAdminLogLevelAsync(
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken)
    {
        var level = await clientResolver.ResolveDefaultClient()
            .Admin()
            .GetLogLevelAsync(cancellationToken);

        return Results.Ok(new StringValueRequest(level));
    }

    public static async Task<IResult> GetDataListAsync(
        string? subDirectory,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken)
    {
        var response = await clientResolver.ResolveDefaultClient()
            .Data()
            .ListAsync(subDirectory, cancellationToken);

        return Results.Ok(response.Lines);
    }

    public static async Task<IResult> GetDataAsync(
        string key,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken)
    {
        var response = await clientResolver.ResolveDefaultClient()
            .Data()
            .RetrieveAsync(key, cancellationToken);

        return Results.Ok(new DataValueResponse(key, response.Lines));
    }

    public static async Task<IResult> GetMediaFilesAsync(
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken)
    {
        var mediaFiles = await clientResolver.ResolveDefaultClient()
            .Server()
            .MediaFilesAsync(cancellationToken);

        var payload = mediaFiles
            .Select(MediaFileResponse.FromDomain)
            .ToArray();

        return Results.Ok(payload);
    }

    public static async Task<IResult> GetMediaFileAsync(
        string fileName,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken)
    {
        var mediaInfo = await clientResolver.ResolveDefaultClient()
            .Server()
            .MediaInfoAsync(fileName, cancellationToken);

        return Results.Ok(mediaInfo);
    }

    public static async Task<IResult> GetFontsAsync(
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken)
    {
        var fonts = await clientResolver.ResolveDefaultClient()
            .Server()
            .FontFilesAsync(cancellationToken);

        return Results.Ok(fonts);
    }

    public static async Task<IResult> GetTemplatesAsync(
        string? subDirectory,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken)
    {
        var templates = await clientResolver.ResolveDefaultClient()
            .Server()
            .TemplateFilesAsync(subDirectory, cancellationToken);

        return Results.Ok(templates);
    }

    public static async Task<IResult> GetThumbnailsAsync(
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await clientResolver.ResolveDefaultClient()
                .Thumbnails()
                .ListAsync(null, cancellationToken);

            return Results.Ok(ThumbnailResponseParser.ParseList(response.Raw));
        }
        catch (Exception exception)
        {
            return ToProblemResult(exception);
        }
    }

    public static async Task<IResult> GetThumbnailAsync(
        string fileName,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await clientResolver.ResolveDefaultClient()
                .Thumbnails()
                .RetrieveAsync(fileName, cancellationToken);

            var bytes = ThumbnailResponseParser.ParseBinary(response.Raw);
            return Results.File(bytes, "application/octet-stream");
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
