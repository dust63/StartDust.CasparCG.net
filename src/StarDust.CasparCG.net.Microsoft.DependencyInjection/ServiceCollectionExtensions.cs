using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using StarDust.CasparCG.net.AmcpProtocol;
using StarDust.CasparCG.net.Connection;
using StarDust.CasparCG.net.Device;

namespace StarDust.CasparCG.net.Microsoft.DependencyInjections;

/// <summary>
/// Extension to configure dependencies of <see cref="CasparDevice"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add dependencies to interact with CasparCG.
    /// To interact with CasparCG play with the interface <see cref="ICasparDevice"/>
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddCasparCG(this IServiceCollection services)
    {
        services.TryAddTransient<ICasparDevice, CasparDevice>();
        services.TryAddTransient<IServerConnection, ServerConnection>();
        services.TryAddTransient<IAmcpTcpParser, AmcpTcpParser>();
        services.TryAddSingleton<IDataParser, CasparCGDataParser>();
        services.TryAddSingleton<IAMCPProtocolParser, AMCPProtocolParser>();

        return services;
    }
}
