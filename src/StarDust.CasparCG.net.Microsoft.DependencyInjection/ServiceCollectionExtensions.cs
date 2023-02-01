using Microsoft.Extensions.DependencyInjection;
using StarDust.CasparCG.net.Device;
using StarDust.CasparCG.net.AmcpProtocol;
using StarDust.CasparCG.net.Connection;

namespace StarDust.CasparCG.net.Microsoft.DependencyInjections;

/// <summary>
/// Extension to configure dependencies of <see cref="CasparDevice"/>
/// </summary>
public static class ServiceCollectionExtensions
{ 
     public static IServiceCollection AddCasparCGDevice(this IServiceCollection services)
     {
        services.AddTransient<ICasparDevice, CasparDevice>();
        services.AddTransient<IServerConnection, ServerConnection>();
        services.AddTransient<IAMCPTcpParser, AmcpTCPParser>();
        services.AddSingleton<IDataParser, CasparCGDataParser>();
        services.AddSingleton<IAMCPProtocolParser, AMCPProtocolParser>();
        
        return services;
    }
}
