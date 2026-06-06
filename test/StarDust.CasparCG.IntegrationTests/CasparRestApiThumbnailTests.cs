using System.Text;
using Xunit;

namespace StarDust.CasparCG.IntegrationTests;

public sealed class CasparRestApiThumbnailTests
{
    [Fact]
    public async Task Get_thumbnails_returns_structured_json()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
            .WithAmcpReply("THUMBNAIL LIST", "200 THUMBNAIL LIST OK\r\n\"AMB\" 42 20240101T120000\r\n\r\n"));

        var response = await fixture.Client.GetAsync("/thumbnails");

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"name\":\"AMB\"", payload, StringComparison.Ordinal);
    }

    [Fact]
    public async Task Get_thumbnail_returns_binary_payload()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
            .WithAmcpReply("THUMBNAIL RETRIEVE AMB", "201 THUMBNAIL RETRIEVE OK\r\nSGVsbG8=\r\n"));

        var response = await fixture.Client.GetAsync("/thumbnails/AMB");

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/octet-stream", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("Hello", Encoding.UTF8.GetString(await response.Content.ReadAsByteArrayAsync()));
    }

    [Fact]
    public async Task Post_generate_routes_send_expected_commands()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
            .WithAmcpReply("THUMBNAIL GENERATE AMB", "202 THUMBNAIL GENERATE OK\r\n")
            .WithAmcpReply("THUMBNAIL GENERATE_ALL", "202 THUMBNAIL GENERATE_ALL OK\r\n"));

        var generateResponse = await fixture.Client.PostAsync("/thumbnails/AMB/generate", content: null);
        var generateAllResponse = await fixture.Client.PostAsync("/thumbnails/generate-all", content: null);

        generateResponse.EnsureSuccessStatusCode();
        generateAllResponse.EnsureSuccessStatusCode();
        Assert.Equal(
            ["THUMBNAIL GENERATE AMB", "THUMBNAIL GENERATE_ALL"],
            fixture.ReceivedCommands);
    }
}
