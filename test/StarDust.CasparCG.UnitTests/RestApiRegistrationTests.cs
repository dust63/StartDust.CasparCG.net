using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StarDust.CasparCG.AspNetCore;
using StarDust.CasparCG.Hosting;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public sealed class RestApiRegistrationTests
{
    [Fact]
    public void AddCasparCGRestApi_registers_lifecycle_options_and_hosted_service()
    {
        var services = new ServiceCollection();

        services.AddCasparCG()
            .ConnectTo("127.0.0.1", 5250);

        services.AddCasparCGRestApi(options =>
        {
            options.WarmUpClientsOnStartup = true;
            options.MapHealthEndpoints = true;
        });

        using var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<CasparRestApiOptions>>().Value;

        Assert.True(options.WarmUpClientsOnStartup);
        Assert.True(options.MapHealthEndpoints);
        Assert.NotNull(provider.GetRequiredService<ICasparClientResolver>());

        var hostedServices = provider.GetServices<IHostedService>().ToArray();
        Assert.Contains(hostedServices, service => service.GetType().Name == "CasparRestApiHostedService");
    }
}
