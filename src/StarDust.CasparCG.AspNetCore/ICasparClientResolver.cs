namespace StarDust.CasparCG.AspNetCore;

/// <summary>
/// Resolves the effective <see cref="CasparClient"/> used by the HTTP surface.
/// </summary>
public interface ICasparClientResolver
{
    /// <summary>
    /// Gets the default configured client instance.
    /// </summary>
    /// <returns>The resolved <see cref="CasparClient"/>.</returns>
    CasparClient ResolveDefaultClient();
}
