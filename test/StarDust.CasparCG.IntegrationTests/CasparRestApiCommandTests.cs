using System.Net;
using System.Net.Http.Json;
using StarDust.CasparCG.AspNetCore.Contracts;
using Xunit;

namespace StarDust.CasparCG.IntegrationTests;

public sealed class CasparRestApiCommandTests
{
    [Fact]
    public async Task Post_play_sends_expected_command()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
            .WithAmcpReply("PLAY 1-10 AMB", "202 PLAY OK\r\n"));

        var response = await fixture.Client.PostAsJsonAsync(
            "/channels/1/layers/10/play",
            new PlayRequest("AMB", false, null));

        response.EnsureSuccessStatusCode();
        Assert.Equal(["PLAY 1-10 AMB"], fixture.ReceivedCommands);
    }

    [Fact]
    public async Task Post_play_returns_bad_gateway_for_failed_amcp_response()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
            .WithAmcpReply("PLAY 1-10 FAIL", "404 PLAY FAILED\r\n"));

        var response = await fixture.Client.PostAsJsonAsync(
            "/channels/1/layers/10/play",
            new PlayRequest("FAIL", false, null));

        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);
    }

    [Fact]
    public async Task Representative_command_routes_cover_playback_mixer_data_and_admin()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
            .WithAmcpReply("LOADBG 1-10 BG LOOP AUTO", "202 LOADBG OK\r\n")
            .WithAmcpReply("PAUSE 1-10", "202 PAUSE OK\r\n")
            .WithAmcpReply("RESUME 1-10", "202 RESUME OK\r\n")
            .WithAmcpReply("STOP 1-10", "202 STOP OK\r\n")
            .WithAmcpReply("MIXER OPACITY 1-10 0.5", "202 MIXER OK\r\n")
            .WithAmcpReply("DATA STORE title hello", "202 DATA STORE OK\r\n")
            .WithAmcpReply("RESTART", "202 RESTART OK\r\n"));

        var loadBackgroundResponse = await fixture.Client.PostAsJsonAsync(
            "/channels/1/layers/10/loadbg",
            new LoadBackgroundRequest("BG", true, true));
        var pauseResponse = await fixture.Client.PostAsync("/channels/1/layers/10/pause", content: null);
        var resumeResponse = await fixture.Client.PostAsync("/channels/1/layers/10/resume", content: null);
        var stopResponse = await fixture.Client.PostAsync("/channels/1/layers/10/stop", content: null);
        var opacityResponse = await fixture.Client.PostAsJsonAsync(
            "/channels/1/layers/10/mixer/opacity",
            new MixerOpacityRequest(0.5));
        var putDataResponse = await fixture.Client.PutAsJsonAsync(
            "/data/title",
            new DataStoreRequest("hello"));
        var restartResponse = await fixture.Client.PostAsync("/admin/restart", content: null);

        loadBackgroundResponse.EnsureSuccessStatusCode();
        pauseResponse.EnsureSuccessStatusCode();
        resumeResponse.EnsureSuccessStatusCode();
        stopResponse.EnsureSuccessStatusCode();
        opacityResponse.EnsureSuccessStatusCode();
        putDataResponse.EnsureSuccessStatusCode();
        restartResponse.EnsureSuccessStatusCode();

        Assert.Equal(
            [
                "LOADBG 1-10 BG LOOP AUTO",
                "PAUSE 1-10",
                "RESUME 1-10",
                "STOP 1-10",
                "MIXER OPACITY 1-10 0.5",
                "DATA STORE title hello",
                "RESTART"
            ],
            fixture.ReceivedCommands);
    }
}
