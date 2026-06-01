namespace StarDust.CasparCG;

/// <summary>
/// Resolves configured <see cref="CasparClient"/> instances by name.
/// </summary>
public interface ICasparClientFactory
{
    /// <summary>
    /// Gets a configured client instance for the specified registration name.
    /// </summary>
    /// <param name="name">The client registration name.</param>
    /// <returns>The configured <see cref="CasparClient"/> instance.</returns>
    CasparClient GetClient(string name);
}
