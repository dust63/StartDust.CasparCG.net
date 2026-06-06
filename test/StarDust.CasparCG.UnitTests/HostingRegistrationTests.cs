using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using StarDust.CasparCG;
using StarDust.CasparCG.Hosting;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class HostingRegistrationTests
{
    [Fact]
    public void AddCasparCg_default_registration_builds_default_client()
    {
        var services = new ServiceCollection();

        services.AddCasparCG()
            .ConnectTo("127.0.0.1", 5250)
            .ListenOscOn(6250);

        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<CasparClient>());
    }

    [Fact]
    public void AddCasparCg_named_registration_is_resolved_from_factory()
    {
        var services = new ServiceCollection();

        services.AddCasparCG("studio-a")
            .ConnectTo("10.0.0.10", 5250)
            .ListenOscOn(6250);

        using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<ICasparClientFactory>();

        Assert.NotNull(factory.GetClient("studio-a"));
    }

    [Fact]
    public void AddCasparCg_from_configuration_registers_named_clients()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["CasparCG:Clients:default:AmcpHost"] = "127.0.0.1",
                ["CasparCG:Clients:default:AmcpPort"] = "5250",
                ["CasparCG:Clients:studio-a:AmcpHost"] = "10.0.0.10",
                ["CasparCG:Clients:studio-a:AmcpPort"] = "5251",
                ["CasparCG:Clients:studio-a:OscPort"] = "6251"
            })
            .Build();

        var services = new ServiceCollection();

        services.AddCasparCGFromConfiguration(configuration);

        using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<ICasparClientFactory>();

        Assert.NotNull(factory.GetClient("default"));
        Assert.NotNull(factory.GetClient("studio-a"));
    }
}
