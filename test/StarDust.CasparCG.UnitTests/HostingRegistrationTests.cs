using Microsoft.Extensions.DependencyInjection;
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
}
