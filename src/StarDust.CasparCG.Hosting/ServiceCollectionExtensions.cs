using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StarDust.CasparCG;

namespace StarDust.CasparCG.Hosting;

/// <summary>
/// Provides dependency injection registration helpers for CasparCG services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a CasparCG client and returns a fluent builder for configuration.
    /// </summary>
    /// <param name="services">The service collection to populate.</param>
    /// <param name="name">The client registration name.</param>
    /// <returns>A fluent client registration builder.</returns>
    public static CasparClientRegistrationBuilder AddCasparCG(this IServiceCollection services, string name = "default")
    {
        var options = new CasparClientOptions { Name = name };
        services.AddSingleton(options);
        services.TryAddSingleton<ICasparClientFactory, NamedCasparClientFactory>();
        services.TryAddSingleton<CasparClient>(sp => sp.GetRequiredService<ICasparClientFactory>().GetClient("default"));
        return new CasparClientRegistrationBuilder(services, options);
    }
}
