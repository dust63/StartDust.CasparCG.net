using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
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

    /// <summary>
    /// Registers CasparCG clients from a configuration section.
    /// </summary>
    /// <param name="services">The service collection to populate.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="sectionName">The configuration section name.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddCasparCGFromConfiguration(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName = "CasparCG")
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var clientsSection = configuration.GetSection(sectionName).GetSection("Clients");
        if (!clientsSection.Exists())
        {
            return services;
        }

        foreach (var clientSection in clientsSection.GetChildren())
        {
            var options = new CasparClientOptions { Name = clientSection.Key };
            clientSection.Bind(options);
            services.AddSingleton(options);
        }

        services.TryAddSingleton<ICasparClientFactory, NamedCasparClientFactory>();
        services.TryAddSingleton<CasparClient>(sp => sp.GetRequiredService<ICasparClientFactory>().GetClient("default"));

        return services;
    }
}
