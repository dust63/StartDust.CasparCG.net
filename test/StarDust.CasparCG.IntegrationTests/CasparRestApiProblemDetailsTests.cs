using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using StarDust.CasparCG.AspNetCore.Contracts;
using StarDust.CasparCG.Testing.DummyServer;
using Xunit;

namespace StarDust.CasparCG.IntegrationTests;

public sealed class CasparRestApiProblemDetailsTests
{
    [Fact]
    public async Task Rest_problem_details_include_amcp_metadata_in_json()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
            .WithAmcpReply("PLAY 1-10 FAIL", "404 PLAY FAILED\r\n"));

        var response = await fixture.Client.PostAsJsonAsync(
            "/channels/1/layers/10/play",
            new PlayRequest("FAIL"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;

        Assert.Equal(404, root.GetProperty("status").GetInt32());
        Assert.Equal(404, root.GetProperty("amcpStatusCode").GetInt32());
        Assert.Equal("404 PLAY FAILED", root.GetProperty("amcpStatusLine").GetString());
        Assert.Equal("PLAY FAILED", root.GetProperty("amcpCommandText").GetString());
        Assert.Equal("ClientError", root.GetProperty("amcpCategory").GetString());
    }
}
