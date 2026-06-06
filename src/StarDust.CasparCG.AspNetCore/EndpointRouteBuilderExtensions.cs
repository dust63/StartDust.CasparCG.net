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
        group.MapGet("/server/info", CasparQueryHandlers.GetServerInfoAsync)
            .WithDisplayName("GET /server/info");
        group.MapGet("/server/info/config", CasparQueryHandlers.GetServerInfoConfigAsync)
            .WithDisplayName("GET /server/info/config");
        group.MapGet("/server/info/paths", CasparQueryHandlers.GetServerInfoPathsAsync)
            .WithDisplayName("GET /server/info/paths");
        group.MapGet("/server/gl/info", CasparQueryHandlers.GetServerGlInfoAsync)
            .WithDisplayName("GET /server/gl/info");
        group.MapPost("/server/gl/gc", CasparCommandHandlers.RunServerGlGcAsync)
            .WithDisplayName("POST /server/gl/gc");
        group.MapPost("/server/diag", CasparCommandHandlers.RunServerDiagAsync)
            .WithDisplayName("POST /server/diag");
        group.MapGet("/admin/log-level", CasparQueryHandlers.GetAdminLogLevelAsync)
            .WithDisplayName("GET /admin/log-level");
        group.MapPut("/admin/log-level", CasparCommandHandlers.SetAdminLogLevelAsync)
            .WithDisplayName("PUT /admin/log-level");
        group.MapPost("/admin/bye", CasparCommandHandlers.ByeAsync)
            .WithDisplayName("POST /admin/bye");
        group.MapPost("/admin/kill", CasparCommandHandlers.KillAsync)
            .WithDisplayName("POST /admin/kill");
        group.MapPost("/admin/locks/{channel:int}/acquire", CasparCommandHandlers.AcquireLockAsync)
            .WithDisplayName("POST /admin/locks/{channel}/acquire");
        group.MapPost("/admin/locks/{channel:int}/release", CasparCommandHandlers.ReleaseLockAsync)
            .WithDisplayName("POST /admin/locks/{channel}/release");
        group.MapPost("/admin/locks/{channel:int}/clear", CasparCommandHandlers.ClearLockAsync)
            .WithDisplayName("POST /admin/locks/{channel}/clear");
        group.MapGet("/data", CasparQueryHandlers.GetDataListAsync)
            .WithDisplayName("GET /data");
        group.MapGet("/data/{key}", CasparQueryHandlers.GetDataAsync)
            .WithDisplayName("GET /data/{key}");
        group.MapGet("/media/files", CasparQueryHandlers.GetMediaFilesAsync)
            .WithDisplayName("GET /media/files");
        group.MapGet("/media/files/{fileName}", CasparQueryHandlers.GetMediaFileAsync)
            .WithDisplayName("GET /media/files/{fileName}");
        group.MapGet("/fonts", CasparQueryHandlers.GetFontsAsync)
            .WithDisplayName("GET /fonts");
        group.MapGet("/templates", CasparQueryHandlers.GetTemplatesAsync)
            .WithDisplayName("GET /templates");
        group.MapPost("/channels/clear-all", CasparCommandHandlers.ClearAllAsync)
            .WithDisplayName("POST /channels/clear-all");
        group.MapPost("/channels/{channel:int}/clear", CasparCommandHandlers.ClearChannelAsync)
            .WithDisplayName("POST /channels/{channel}/clear");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/load", CasparCommandHandlers.LoadAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/load");
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
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/clear", CasparCommandHandlers.ClearAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/clear");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/call", CasparCommandHandlers.CallAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/call");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/callbg", CasparCommandHandlers.CallBackgroundAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/callbg");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/swap", CasparCommandHandlers.SwapAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/swap");
        group.MapPost("/channels/{channel:int}/add", CasparCommandHandlers.AddAsync)
            .WithDisplayName("POST /channels/{channel}/add");
        group.MapPost("/channels/{channel:int}/remove", CasparCommandHandlers.RemoveAsync)
            .WithDisplayName("POST /channels/{channel}/remove");
        group.MapPost("/channels/{channel:int}/apply", CasparCommandHandlers.ApplyAsync)
            .WithDisplayName("POST /channels/{channel}/apply");
        group.MapPost("/channels/{channel:int}/print", CasparCommandHandlers.PrintAsync)
            .WithDisplayName("POST /channels/{channel}/print");
        group.MapPost("/channels/{channel:int}/set", CasparCommandHandlers.SetAsync)
            .WithDisplayName("POST /channels/{channel}/set");
        group.MapPost("/channels/{channel:int}/grid", CasparCommandHandlers.RunChannelGridAsync)
            .WithDisplayName("POST /channels/{channel}/grid");
        group.MapPost("/mixer/master-volume", CasparCommandHandlers.SetMixerMasterVolumeAsync)
            .WithDisplayName("POST /mixer/master-volume");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/keyer", CasparCommandHandlers.SetMixerKeyerAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/mixer/keyer");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/invert", CasparCommandHandlers.SetMixerInvertAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/mixer/invert");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/blend", CasparCommandHandlers.SetMixerBlendAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/mixer/blend");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/chroma", CasparCommandHandlers.SetMixerChromaAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/mixer/chroma");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/opacity", CasparCommandHandlers.SetOpacityAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/mixer/opacity");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/brightness", CasparCommandHandlers.SetMixerBrightnessAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/mixer/brightness");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/saturation", CasparCommandHandlers.SetMixerSaturationAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/mixer/saturation");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/contrast", CasparCommandHandlers.SetMixerContrastAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/mixer/contrast");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/levels", CasparCommandHandlers.SetMixerLevelsAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/mixer/levels");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/fill", CasparCommandHandlers.SetMixerFillAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/mixer/fill");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/clip", CasparCommandHandlers.SetMixerClipAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/mixer/clip");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/anchor", CasparCommandHandlers.SetMixerAnchorAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/mixer/anchor");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/crop", CasparCommandHandlers.SetMixerCropAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/mixer/crop");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/rotation", CasparCommandHandlers.SetMixerRotationAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/mixer/rotation");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/perspective", CasparCommandHandlers.SetMixerPerspectiveAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/mixer/perspective");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/volume", CasparCommandHandlers.SetMixerVolumeAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/mixer/volume");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/grid", CasparCommandHandlers.SetMixerGridAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/mixer/grid");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/commit", CasparCommandHandlers.CommitMixerAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/mixer/commit");
        group.MapPost("/channels/{channel:int}/layers/{layer:int}/mixer/clear", CasparCommandHandlers.ClearMixerAsync)
            .WithDisplayName("POST /channels/{channel}/layers/{layer}/mixer/clear");
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
        group.MapDelete("/data/{key}", CasparCommandHandlers.DeleteDataAsync)
            .WithDisplayName("DELETE /data/{key}");
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
