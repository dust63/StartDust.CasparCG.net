using Microsoft.AspNetCore.Http;
using StarDust.CasparCG;

namespace StarDust.CasparCG.AspNetCore.Internal;

internal sealed class DefaultCasparClientResolver(
    CasparClient client,
    IHttpContextAccessor httpContextAccessor,
    ICasparClientFactory? clientFactory = null) : ICasparClientResolver
{
    public CasparClient ResolveDefaultClient() => ResolveCurrentClient();

    private CasparClient ResolveCurrentClient()
    {
        var serverName = httpContextAccessor.HttpContext?.Request.RouteValues["name"] as string;

        if (string.IsNullOrWhiteSpace(serverName) || string.Equals(serverName, "default", StringComparison.OrdinalIgnoreCase))
        {
            return client;
        }

        if (clientFactory is null)
        {
            throw new InvalidOperationException($"No named CasparCG client is registered for '{serverName}'.");
        }

        try
        {
            return clientFactory.GetClient(serverName);
        }
        catch (KeyNotFoundException exception)
        {
            throw new InvalidOperationException($"No named CasparCG client is registered for '{serverName}'.", exception);
        }
    }
}
