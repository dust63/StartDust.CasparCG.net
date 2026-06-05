using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StarDust.CasparCG.AspNetCore;
using StarDust.CasparCG.Hosting;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public sealed class RestApiRegistrationTests
{
    [Fact]
    public void AddCasparCGRestApi_registers_options_and_client_resolver()
    {
        var services = new ServiceCollection();

        services.AddCasparCG()
            .ConnectTo("127.0.0.1", 5250);

        services.AddCasparCGRestApi();

        using var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetRequiredService<IOptions<CasparRestApiOptions>>());
        Assert.NotNull(provider.GetRequiredService<ICasparClientResolver>());
    }
}
