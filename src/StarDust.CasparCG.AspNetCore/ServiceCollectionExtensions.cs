using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StarDust.CasparCG.AspNetCore.Internal;

namespace StarDust.CasparCG.AspNetCore;

/// <summary>
/// Provides dependency injection registration helpers for the CasparCG REST addon.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers REST addon services for the configured CasparCG client.
    /// </summary>
    /// <param name="services">The service collection to populate.</param>
    /// <param name="configure">An optional delegate for REST API options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCasparCGRestApi(
        this IServiceCollection services,
        Action<CasparRestApiOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddOptions<CasparRestApiOptions>();

        if (configure is not null)
        {
            services.Configure(configure);
        }

        services.TryAddSingleton<ICasparClientResolver, DefaultCasparClientResolver>();

        return services;
    }
}
