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

        return endpoints;
    }
}
