using StarDust.CasparCG;

using StarDust.CasparCG.Osc;
using StarDust.CasparCG.Transport;

namespace StarDust.CasparCG.Hosting;

internal sealed class NamedCasparClientFactory : ICasparClientFactory
{
    private readonly IReadOnlyDictionary<string, CasparClient> _clients;

    public NamedCasparClientFactory(IEnumerable<CasparClientOptions> options)
    {
        _clients = options
            .GroupBy(x => x.Name)
            .ToDictionary(
                group => group.Key,
                group => CreateClient(group.Last()));
    }

    public CasparClient GetClient(string name) => _clients[name];

    private static CasparClient CreateClient(CasparClientOptions options)
    {
        var amcpTransport = new TcpAmcpTransport(options.AmcpHost, options.AmcpPort);
        IOscTransport? oscTransport = options.OscPort > 0 ? new UdpOscTransport() : null;
        var oscMessageMapper = new DefaultOscMessageMapper(options.Name);

        return new CasparClient(amcpTransport, oscTransport, oscMessageMapper, options.Name);
    }
}
