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
