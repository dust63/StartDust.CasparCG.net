using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StarDust.CasparCG.AspNetCore.Internal;

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
        group.MapPut("/data/{key}", CasparCommandHandlers.PutDataAsync)
            .WithDisplayName("PUT /data/{key}");
        group.MapPost("/admin/restart", CasparCommandHandlers.RestartAsync)
            .WithDisplayName("POST /admin/restart");

        return endpoints;
    }
}
