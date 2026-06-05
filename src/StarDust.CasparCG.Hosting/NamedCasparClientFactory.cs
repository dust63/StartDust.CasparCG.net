using StarDust.CasparCG;

namespace StarDust.CasparCG.Hosting;

internal sealed class NamedCasparClientFactory : ICasparClientFactory
{
    private readonly IReadOnlyDictionary<string, CasparClient> _clients;

    public NamedCasparClientFactory(IEnumerable<CasparClientOptions> options)
    {
        _clients = options.ToDictionary(
            x => x.Name,
            _ => new CasparClient(new NoOpAmcpTransport()));
    }

    public CasparClient GetClient(string name) => _clients[name];
}
