using System.Net.Http.Json;
using StarDust.CasparCG.AspNetCore.Contracts;
using StarDust.CasparCG.Query;
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

    [Fact]
    public async Task Get_server_queries_return_structured_payloads()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
            .WithAmcpReply("INFO", "200 INFO OK\r\nserver-name: dummy\r\nchannel-count: 1\r\n\r\n")
            .WithAmcpReply("INFO CONFIG", "200 INFO CONFIG OK\r\n<config><log-level>debug</log-level></config>\r\n\r\n")
            .WithAmcpReply("INFO PATHS", "200 INFO PATHS OK\r\n<paths><media-path>media/</media-path></paths>\r\n\r\n")
            .WithAmcpReply("GL INFO", "201 GL INFO OK\r\n<gl><renderer>opengl</renderer><vendor>casparcg</vendor></gl>\r\n"));

        var infoResponse = await fixture.Client.GetAsync("/server/info");
        var configResponse = await fixture.Client.GetAsync("/server/info/config");
        var pathsResponse = await fixture.Client.GetAsync("/server/info/paths");
        var glInfoResponse = await fixture.Client.GetAsync("/server/gl/info");

        infoResponse.EnsureSuccessStatusCode();
        configResponse.EnsureSuccessStatusCode();
        pathsResponse.EnsureSuccessStatusCode();
        glInfoResponse.EnsureSuccessStatusCode();

        var info = await infoResponse.Content.ReadFromJsonAsync<QueryDataMap>();
        var config = await configResponse.Content.ReadFromJsonAsync<QueryDataMap>();
        var paths = await pathsResponse.Content.ReadFromJsonAsync<QueryDataMap>();
        var glInfo = await glInfoResponse.Content.ReadFromJsonAsync<QueryDataMap>();

        Assert.NotNull(info);
        Assert.NotNull(config);
        Assert.NotNull(paths);
        Assert.NotNull(glInfo);
        Assert.Equal("dummy", info.Values["server-name"]);
        Assert.Equal("1", info.Values["channel-count"]);
        Assert.Equal("debug", config.Values["config.log-level"]);
        Assert.Equal("media/", paths.Values["paths.media-path"]);
        Assert.Equal("opengl", glInfo.Values["gl.renderer"]);
        Assert.Equal("casparcg", glInfo.Values["gl.vendor"]);
    }

    [Fact]
    public async Task Get_catalog_and_data_list_routes_return_payloads()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
            .WithAmcpReply("CINF AMB", "200 CINF OK\r\n\"AMB\" MOVIE 42 20240101120000 240 1/25 FIELD_A VALUE_A\r\n\r\n")
            .WithAmcpReply("FLS", "200 FLS OK\r\n\"Roboto\" \"fonts/Roboto Regular.ttf\"\r\n\r\n")
            .WithAmcpReply("TLS", "200 TLS OK\r\nLOWERTHIRD\r\nFULLFRAME\r\n\r\n")
            .WithAmcpReply("DATA LIST", "200 DATA LIST OK\r\nitem-a\r\nitem-b\r\n\r\n"));

        var mediaInfoResponse = await fixture.Client.GetAsync("/media/files/AMB");
        var fontsResponse = await fixture.Client.GetAsync("/fonts");
        var templatesResponse = await fixture.Client.GetAsync("/templates");
        var dataListResponse = await fixture.Client.GetAsync("/data");

        mediaInfoResponse.EnsureSuccessStatusCode();
        fontsResponse.EnsureSuccessStatusCode();
        templatesResponse.EnsureSuccessStatusCode();
        dataListResponse.EnsureSuccessStatusCode();

        var mediaInfo = await mediaInfoResponse.Content.ReadFromJsonAsync<MediaInfo>();
        var fonts = await fontsResponse.Content.ReadFromJsonAsync<FontFile[]>();
        var templates = await templatesResponse.Content.ReadFromJsonAsync<TemplateFile[]>();
        var dataItems = await dataListResponse.Content.ReadFromJsonAsync<string[]>();

        Assert.NotNull(mediaInfo);
        Assert.NotNull(fonts);
        Assert.NotNull(templates);
        Assert.NotNull(dataItems);
        Assert.Equal("AMB", mediaInfo.Name);
        Assert.Equal("VALUE_A", mediaInfo.Properties["FIELD_A"]);
        Assert.Equal("Roboto", Assert.Single(fonts).Name);
        Assert.Equal(["LOWERTHIRD", "FULLFRAME"], templates.Select(x => x.Name).ToArray());
        Assert.Equal(["item-a", "item-b"], dataItems);
    }
}
