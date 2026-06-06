using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StarDust.CasparCG.Hosting;

namespace StarDust.CasparCG.AspNetCore.Internal;

internal sealed class CasparRestApiHostedService(
    ICasparClientFactory clientFactory,
    IEnumerable<CasparClientOptions> clientOptions,
    IOptions<CasparRestApiOptions> options) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!options.Value.WarmUpClientsOnStartup)
        {
            return;
        }

        foreach (var clientName in clientOptions.Select(client => client.Name).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            await clientFactory.GetClient(clientName).ConnectAsync(cancellationToken);
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var clientName in clientOptions.Select(client => client.Name).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            await clientFactory.GetClient(clientName).DisconnectAsync(cancellationToken);
        }
    }
}
