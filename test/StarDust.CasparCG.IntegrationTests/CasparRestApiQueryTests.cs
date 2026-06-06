using System.Net.Http.Json;
using StarDust.CasparCG.AspNetCore.Contracts;
using Xunit;

namespace StarDust.CasparCG.IntegrationTests;

public sealed class CasparRestApiQueryTests
{
    [Fact]
    public async Task Get_server_version_returns_typed_payload()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
            .WithAmcpReply("VERSION SERVER", "201 VERSION OK\r\n2.4.0\r\n"));

        var response = await fixture.Client.GetAsync("/server/version");

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<ServerVersionResponse>();

        Assert.NotNull(payload);
        Assert.Equal("2.4.0", payload.Version);
    }
}
