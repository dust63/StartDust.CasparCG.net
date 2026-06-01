using Microsoft.Extensions.DependencyInjection;
using StarDust.CasparCG;
using StarDust.CasparCG.Hosting;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class CasparClientBootstrapTests
{
    [Fact]
    public void AddCasparCg_registers_default_client_and_factory()
    {
        var services = new ServiceCollection();

        services.AddCasparCG();

        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<CasparClient>());
        Assert.NotNull(provider.GetRequiredService<ICasparClientFactory>());
    }
}
