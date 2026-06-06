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

        MapCasparRoutes(group, string.Empty);
        MapCasparRoutes(group.MapGroup("/servers/{name}"), "/servers/{name}");

        if (options.EnableOpenApi)
        {
            var openApiPattern = string.IsNullOrWhiteSpace(pattern)
                ? "/swagger/{documentName}/swagger.json"
                : $"{pattern}/swagger/{{documentName}}/swagger.json";

            endpoints.MapGet(openApiPattern, OpenApiHandlers.GetDocumentAsync)
                .WithDisplayName("GET /swagger/{documentName}/swagger.json");
        }

        return endpoints;
    }

    private static void MapCasparRoutes(IEndpointRouteBuilder group, string displayPrefix)
    {
        static string Display(string displayPrefix, string route)
        {
            if (string.IsNullOrWhiteSpace(displayPrefix))
            {
                return route;
            }

            var separatorIndex = route.IndexOf(' ');
            if (separatorIndex < 0)
            {
                return $"{displayPrefix}{route}";
            }

            var method = route[..(separatorIndex + 1)];
            var path = route[(separatorIndex + 1)..];

            return path.StartsWith('/')
                ? $"{method}{displayPrefix}{path}"
                : $"{method}{displayPrefix}/{path}";
        }

        group.MapGet("/server/version", CasparQueryHandlers.GetServerVersionAsync)
            .WithDisplayName(Display(displayPrefix, "GET /server/version"));
        group.MapGet("/server/info", CasparQueryHandlers.GetServerInfoAsync)
            .WithDisplayName(Display(displayPrefix, "GET /server/info"));
        group.MapGet("/server/info/config", CasparQueryHandlers.GetServerInfoConfigAsync)
            .WithDisplayName(Display(displayPrefix, "GET /server/info/config"));
        group.MapGet("/server/info/paths", CasparQueryHandlers.GetServerInfoPathsAsync)
            .WithDisplayName(Display(displayPrefix, "GET /server/info/paths"));
        group.MapGet("/server/gl/info", CasparQueryHandlers.GetServerGlInfoAsync)
            .WithDisplayName(Display(displayPrefix, "GET /server/gl/info"));
        group.MapPost("/server/gl/gc", CasparCommandHandlers.RunServerGlGcAsync)
            .WithDisplayName(Display(displayPrefix, "POST /server/gl/gc"));
        group.MapPost("/server/diag", CasparCommandHandlers.RunServerDiagAsync)
            .WithDisplayName(Display(displayPrefix, "POST /server/diag"));
        group.MapGet("/admin/log-level", CasparQueryHandlers.GetAdminLogLevelAsync)
            .WithDisplayName(Display(displayPrefix, "GET /admin/log-level"));
        group.MapPut("/admin/log-level", CasparCommandHandlers.SetAdminLogLevelAsync)
            .WithDisplayName(Display(displayPrefix, "PUT /admin/log-level"));
        group.MapPost("/admin/bye", CasparCommandHandlers.ByeAsync)
            .WithDisplayName(Display(displayPrefix, "POST /admin/bye"));
        group.MapPost("/admin/kill", CasparCommandHandlers.KillAsync)
            .WithDisplayName(Display(displayPrefix, "POST /admin/kill"));
        group.MapPost("/admin/locks/{channel:int}/acquire", CasparCommandHandlers.AcquireLockAsync)
            .WithDisplayName(Display(displayPrefix, "POST /admin/locks/{channel}/acquire"));
        group.MapPost("/admin/locks/{channel:int}/release", CasparCommandHandlers.ReleaseLockAsync)
            .WithDisplayName(Display(displayPrefix, "POST /admin/locks/{channel}/release"));
        group.MapPost("/admin/locks/{channel:int}/clear", CasparCommandHandlers.ClearLockAsync)
            .WithDisplayName(Display(displayPrefix, "POST /admin/locks/{channel}/clear"));
        group.MapGet("/data", CasparQueryHandlers.GetDataListAsync)
            .WithDisplayName(Display(displayPrefix, "GET /data"));
        group.MapGet("/data/{key}", CasparQueryHandlers.GetDataAsync)
            .WithDisplayName(Display(displayPrefix, "GET /data/{key}"));
        group.MapGet("/media/files", CasparQueryHandlers.GetMediaFilesAsync)
            .WithDisplayName(Display(displayPrefix, "GET /media/files"));
        group.MapGet("/media/files/{fileName}", CasparQueryHandlers.GetMediaFileAsync)
            .WithDisplayName(Display(displayPrefix, "GET /media/files/{fileName}"));
        group.MapGet("/fonts", CasparQueryHandlers.GetFontsAsync)
            .WithDisplayName(Display(displayPrefix, "GET /fonts"));
        group.MapGet("/templates", CasparQueryHandlers.GetTemplatesAsync)
            .WithDisplayName(Display(displayPrefix, "GET /templates"));
        group.MapPost("/channels/clear-all", CasparCommandHandlers.ClearAllAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/clear-all"));
        group.MapPost("/channels/{channel:int}/clear", CasparCommandHandlers.ClearChannelAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/clear"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/load", CasparCommandHandlers.LoadAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/load"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/play", CasparCommandHandlers.PlayAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/play"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/loadbg", CasparCommandHandlers.LoadBackgroundAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/loadbg"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/pause", CasparCommandHandlers.PauseAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/pause"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/resume", CasparCommandHandlers.ResumeAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/resume"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/stop", CasparCommandHandlers.StopAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/stop"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/clear", CasparCommandHandlers.ClearAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/clear"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/call", CasparCommandHandlers.CallAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/call"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/callbg", CasparCommandHandlers.CallBackgroundAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/callbg"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/swap", CasparCommandHandlers.SwapAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/swap"));
        group.MapPost("/channels/{channel:int}/add", CasparCommandHandlers.AddAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/add"));
        group.MapPost("/channels/{channel:int}/remove", CasparCommandHandlers.RemoveAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/remove"));
        group.MapPost("/channels/{channel:int}/apply", CasparCommandHandlers.ApplyAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/apply"));
        group.MapPost("/channels/{channel:int}/print", CasparCommandHandlers.PrintAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/print"));
        group.MapPost("/channels/{channel:int}/set", CasparCommandHandlers.SetAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/set"));
        group.MapPost("/channels/{channel:int}/grid", CasparCommandHandlers.RunChannelGridAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/grid"));
        group.MapPost("/mixer/master-volume", CasparCommandHandlers.SetMixerMasterVolumeAsync)
            .WithDisplayName(Display(displayPrefix, "POST /mixer/master-volume"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/keyer", CasparCommandHandlers.SetMixerKeyerAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/mixer/keyer"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/invert", CasparCommandHandlers.SetMixerInvertAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/mixer/invert"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/blend", CasparCommandHandlers.SetMixerBlendAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/mixer/blend"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/chroma", CasparCommandHandlers.SetMixerChromaAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/mixer/chroma"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/opacity", CasparCommandHandlers.SetOpacityAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/mixer/opacity"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/brightness", CasparCommandHandlers.SetMixerBrightnessAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/mixer/brightness"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/saturation", CasparCommandHandlers.SetMixerSaturationAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/mixer/saturation"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/contrast", CasparCommandHandlers.SetMixerContrastAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/mixer/contrast"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/levels", CasparCommandHandlers.SetMixerLevelsAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/mixer/levels"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/fill", CasparCommandHandlers.SetMixerFillAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/mixer/fill"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/clip", CasparCommandHandlers.SetMixerClipAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/mixer/clip"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/anchor", CasparCommandHandlers.SetMixerAnchorAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/mixer/anchor"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/crop", CasparCommandHandlers.SetMixerCropAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/mixer/crop"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/rotation", CasparCommandHandlers.SetMixerRotationAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/mixer/rotation"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/perspective", CasparCommandHandlers.SetMixerPerspectiveAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/mixer/perspective"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/volume", CasparCommandHandlers.SetMixerVolumeAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/mixer/volume"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/grid", CasparCommandHandlers.SetMixerGridAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/mixer/grid"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/commit", CasparCommandHandlers.CommitMixerAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/mixer/commit"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/clear", CasparCommandHandlers.ClearMixerAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/mixer/clear"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/add", CasparCommandHandlers.CgAddAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/cg/add"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/play", CasparCommandHandlers.CgPlayAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/cg/play"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/stop", CasparCommandHandlers.CgStopAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/cg/stop"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/next", CasparCommandHandlers.CgNextAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/cg/next"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/remove", CasparCommandHandlers.CgRemoveAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/cg/remove"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/clear", CasparCommandHandlers.CgClearAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/cg/clear"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/update", CasparCommandHandlers.CgUpdateAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/cg/update"));
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/cg/invoke", CasparCommandHandlers.CgInvokeAsync)
            .WithDisplayName(Display(displayPrefix, "POST /channels/{channel}/layers/{layer}/cg/invoke"));
        group.MapPut("/data/{key}", CasparCommandHandlers.PutDataAsync)
            .WithDisplayName(Display(displayPrefix, "PUT /data/{key}"));
        group.MapDelete("/data/{key}", CasparCommandHandlers.DeleteDataAsync)
            .WithDisplayName(Display(displayPrefix, "DELETE /data/{key}"));
        group.MapPost("/admin/restart", CasparCommandHandlers.RestartAsync)
            .WithDisplayName(Display(displayPrefix, "POST /admin/restart"));
        group.MapGet("/thumbnails", CasparQueryHandlers.GetThumbnailsAsync)
            .WithDisplayName(Display(displayPrefix, "GET /thumbnails"));
        group.MapGet("/thumbnails/{fileName}", CasparQueryHandlers.GetThumbnailAsync)
            .WithDisplayName(Display(displayPrefix, "GET /thumbnails/{fileName}"));
        group.MapPost("/thumbnails/{fileName}/generate", CasparCommandHandlers.GenerateThumbnailAsync)
            .WithDisplayName(Display(displayPrefix, "POST /thumbnails/{fileName}/generate"));
        group.MapPost("/thumbnails/generate-all", CasparCommandHandlers.GenerateAllThumbnailsAsync)
            .WithDisplayName(Display(displayPrefix, "POST /thumbnails/generate-all"));

        group.MapGet("/events", CasparSseWriter.StreamAsync)
            .WithDisplayName(Display(displayPrefix, "GET /events"));
    }
}
