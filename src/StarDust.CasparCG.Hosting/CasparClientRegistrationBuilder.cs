using Microsoft.Extensions.DependencyInjection;
using StarDust.CasparCG;

namespace StarDust.CasparCG.Hosting;

/// <summary>
/// Provides a fluent API for configuring a CasparCG client registration.
/// </summary>
public sealed class CasparClientRegistrationBuilder
{
    internal CasparClientRegistrationBuilder(IServiceCollection services, CasparClientOptions options)
    {
        Services = services;
        Options = options;
    }

    internal IServiceCollection Services { get; }

    internal CasparClientOptions Options { get; }

    /// <summary>
    /// Configures the AMCP endpoint.
    /// </summary>
    /// <param name="host">The target host.</param>
    /// <param name="port">The target port.</param>
    /// <returns>The current registration builder.</returns>
    public CasparClientRegistrationBuilder ConnectTo(string host, int port)
    {
        Options.AmcpHost = host;
        Options.AmcpPort = port;
        return this;
    }

    /// <summary>
    /// Configures the OSC port.
    /// </summary>
    /// <param name="port">The OSC port.</param>
    /// <returns>The current registration builder.</returns>
    public CasparClientRegistrationBuilder ListenOscOn(int port)
    {
        Options.OscPort = port;
        return this;
    }

    /// <summary>
    /// Enables or disables auto reconnect.
    /// </summary>
    /// <param name="enabled">Whether auto reconnect should be enabled.</param>
    /// <returns>The current registration builder.</returns>
    public CasparClientRegistrationBuilder WithAutoReconnect(bool enabled = true)
    {
        Options.AutoReconnect = enabled;
        return this;
    }
}
