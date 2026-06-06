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
}
