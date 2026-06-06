using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StarDust.CasparCG.AspNetCore.Internal;
using StarDust.CasparCG.AspNetCore.Sse;

namespace StarDust.CasparCG.AspNetCore;

/// <summary>
/// Provides endpoint mapping helpers for the CasparCG REST addon.
/// </summary>
public static class EndpointRouteBuilderExtensions
{
    /// <summary>
    /// Maps the REST API surface for the configured CasparCG client.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <returns>The updated endpoint route builder.</returns>
    public static IEndpointRouteBuilder MapCasparCGApi(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var options = endpoints.ServiceProvider
            .GetRequiredService<IOptions<CasparRestApiOptions>>()
            .Value;

        var prefix = options.RoutePrefix.Trim('/');
        var pattern = string.IsNullOrWhiteSpace(prefix) ? string.Empty : $"/{prefix}";
        var group = endpoints.MapGroup(pattern);

        group.MapGet("/server/version", CasparQueryHandlers.GetServerVersionAsync)
            .WithDisplayName("GET /server/version");
        group.MapGet("/data/{key}", CasparQueryHandlers.GetDataAsync)
            .WithDisplayName("GET /data/{key}");
        group.MapGet("/media/files", CasparQueryHandlers.GetMediaFilesAsync)
            .WithDisplayName("GET /media/files");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/play", CasparCommandHandlers.PlayAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/play");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/loadbg", CasparCommandHandlers.LoadBackgroundAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/loadbg");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/pause", CasparCommandHandlers.PauseAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/pause");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/resume", CasparCommandHandlers.ResumeAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/resume");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/stop", CasparCommandHandlers.StopAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/stop");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/opacity", CasparCommandHandlers.SetOpacityAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/mixer/opacity");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/add", CasparCommandHandlers.CgAddAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/cg/add");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/play", CasparCommandHandlers.CgPlayAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/cg/play");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/stop", CasparCommandHandlers.CgStopAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/cg/stop");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/next", CasparCommandHandlers.CgNextAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/cg/next");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/remove", CasparCommandHandlers.CgRemoveAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/cg/remove");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/clear", CasparCommandHandlers.CgClearAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/cg/clear");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/update", CasparCommandHandlers.CgUpdateAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/cg/update");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/invoke", CasparCommandHandlers.CgInvokeAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/cg/invoke");
        group.MapPut("/data/{key}", CasparCommandHandlers.PutDataAsync)
            .WithDisplayName("PUT /data/{key}");
        group.MapPost("/admin/restart", CasparCommandHandlers.RestartAsync)
            .WithDisplayName("POST /admin/restart");
        group.MapGet("/thumbnails", CasparQueryHandlers.GetThumbnailsAsync)
            .WithDisplayName("GET /thumbnails");
        group.MapGet("/thumbnails/{fileName}", CasparQueryHandlers.GetThumbnailAsync)
            .WithDisplayName("GET /thumbnails/{fileName}");
        group.MapPost("/thumbnails/{fileName}/generate", CasparCommandHandlers.GenerateThumbnailAsync)
            .WithDisplayName("POST /thumbnails/{fileName}/generate");
        group.MapPost("/thumbnails/generate-all", CasparCommandHandlers.GenerateAllThumbnailsAsync)
            .WithDisplayName("POST /thumbnails/generate-all");

        if (options.EnableSse)
        {
            group.MapGet("/events", CasparSseWriter.StreamAsync)
                .WithDisplayName("GET /events");
        }

        return endpoints;
    }
}
