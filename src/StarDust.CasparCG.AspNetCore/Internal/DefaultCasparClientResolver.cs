namespace StarDust.CasparCG.AspNetCore.Internal;

internal sealed class DefaultCasparClientResolver(CasparClient client) : ICasparClientResolver
{
    public CasparClient ResolveDefaultClient() => client;
}
