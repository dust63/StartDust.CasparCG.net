using System.Net;
using System.Net.Http.Json;
using StarDust.CasparCG.AspNetCore.Contracts;
using StarDust.CasparCG.Protocol.Amcp;
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
            new PlayRequest("AMB"));

        response.EnsureSuccessStatusCode();
        Assert.Equal(["PLAY 1-10 AMB"], fixture.ReceivedCommands);
    }

    [Fact]
    public async Task Post_play_supports_typed_playback_options()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
            .WithAmcpReply("PLAY 1-10 AMB SLIDE 10 linear LEFT LOOP SEEK 12 LENGTH 24 FILTER hflip CLEAR_ON_404", "202 PLAY OK\r\n"));

        var response = await fixture.Client.PostAsJsonAsync(
            "/channels/1/layers/10/play",
            new
            {
                clip = "AMB",
                options = new
                {
                    transition = new
                    {
                        kind = "slide",
                        duration = 10,
                        tweener = "linear",
                        direction = "left"
                    },
                    loop = true,
                    seek = 12,
                    length = 24,
                    filter = "hflip",
                    clearOn404 = true
                }
            });

        response.EnsureSuccessStatusCode();
        Assert.Equal(["PLAY 1-10 AMB SLIDE 10 linear LEFT LOOP SEEK 12 LENGTH 24 FILTER hflip CLEAR_ON_404"], fixture.ReceivedCommands);
    }

    [Fact]
    public async Task Post_play_returns_bad_gateway_for_failed_amcp_response()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
            .WithAmcpReply("PLAY 1-10 FAIL", "404 PLAY FAILED\r\n"));

        var response = await fixture.Client.PostAsJsonAsync(
            "/channels/1/layers/10/play",
            new PlayRequest("FAIL"));

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
            new LoadBackgroundRequest("BG", new LoadBackgroundOptions
            {
                Loop = true,
                AutoPlay = true
            }));
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

    [Fact]
    public async Task Load_and_loadbg_support_typed_playback_options()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
            .WithAmcpReply("LOAD 1-10 AMB MIX 10 linear SEEK 12 LENGTH 24 FILTER hflip CLEAR_ON_404", "202 LOAD OK\r\n")
            .WithAmcpReply("LOADBG 1-10 BG MIX 10 linear SEEK 100 LENGTH 200 FILTER hflip CLEAR_ON_404 AUTO", "202 LOADBG OK\r\n"));

        var loadResponse = await fixture.Client.PostAsJsonAsync(
            "/channels/1/layers/10/load",
            new LoadRequest("AMB", new PlaybackOptions
            {
                Transition = PlaybackTransition.Mix(10, "linear"),
                Seek = 12,
                Length = 24,
                Filter = "hflip",
                ClearOn404 = true
            }));
        var loadBgResponse = await fixture.Client.PostAsJsonAsync(
            "/channels/1/layers/10/loadbg",
            new LoadBackgroundRequest("BG", new LoadBackgroundOptions
            {
                Transition = PlaybackTransition.Mix(10, "linear"),
                Seek = 100,
                Length = 200,
                Filter = "hflip",
                ClearOn404 = true,
                AutoPlay = true
            }));

        loadResponse.EnsureSuccessStatusCode();
        loadBgResponse.EnsureSuccessStatusCode();

        Assert.Equal(
            [
                "LOAD 1-10 AMB MIX 10 linear SEEK 12 LENGTH 24 FILTER hflip CLEAR_ON_404",
                "LOADBG 1-10 BG MIX 10 linear SEEK 100 LENGTH 200 FILTER hflip CLEAR_ON_404 AUTO"
            ],
            fixture.ReceivedCommands);
    }

    [Fact]
    public async Task Query_slice_command_routes_cover_data_delete_and_gl_gc()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
            .WithAmcpReply("DATA REMOVE title", "202 DATA REMOVE OK\r\n")
            .WithAmcpReply("GL GC", "202 GL GC OK\r\n"));

        var deleteDataResponse = await fixture.Client.DeleteAsync("/data/title");
        var glGcResponse = await fixture.Client.PostAsync("/server/gl/gc", content: null);

        deleteDataResponse.EnsureSuccessStatusCode();
        glGcResponse.EnsureSuccessStatusCode();

        Assert.Equal(
            [
                "DATA REMOVE title",
                "GL GC"
            ],
            fixture.ReceivedCommands);
    }

    [Fact]
    public async Task Basic_control_routes_send_expected_commands()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
            .WithAmcpReply("LOAD 1-10 AMB", "202 LOAD OK\r\n")
            .WithAmcpReply("CLEAR 1", "202 CLEAR OK\r\n")
            .WithAmcpReply("CLEAR 1-10", "202 CLEAR OK\r\n")
            .WithAmcpReply("SWAP 1-10 2-20 TRANSFORMS", "202 SWAP OK\r\n")
            .WithAmcpReply("ADD 1-700 LOWER filename.mov", "202 ADD OK\r\n")
            .WithAmcpReply("REMOVE 1-700", "202 REMOVE OK\r\n")
            .WithAmcpReply("APPLY 1-10 LOWER", "202 APPLY OK\r\n")
            .WithAmcpReply("PRINT 1", "202 PRINT OK\r\n")
            .WithAmcpReply("SET 1 MODE FAST", "202 SET OK\r\n")
            .WithAmcpReply("CLEAR ALL", "202 CLEAR ALL OK\r\n"));

        var loadResponse = await fixture.Client.PostAsJsonAsync("/channels/1/layers/10/load", new { clip = "AMB" });
        var clearChannelResponse = await fixture.Client.PostAsync("/channels/1/clear", content: null);
        var clearResponse = await fixture.Client.PostAsync("/channels/1/layers/10/clear", content: null);
        var swapResponse = await fixture.Client.PostAsJsonAsync("/channels/1/layers/10/swap", new { otherChannel = 2, otherLayer = 20, swapTransforms = true });
        var addResponse = await fixture.Client.PostAsJsonAsync("/channels/1/add", new AddRequest("LOWER", "filename.mov", 700));
        var removeResponse = await fixture.Client.PostAsJsonAsync("/channels/1/remove", new RemoveRequest(ConsumerIndex: 700));
        var applyResponse = await fixture.Client.PostAsJsonAsync("/channels/1/apply", new ApplyRequest(10, "LOWER"));
        var printResponse = await fixture.Client.PostAsync("/channels/1/print", content: null);
        var setResponse = await fixture.Client.PostAsJsonAsync("/channels/1/set", new SetRequest("MODE", "FAST"));
        var clearAllResponse = await fixture.Client.PostAsync("/channels/clear-all", content: null);

        loadResponse.EnsureSuccessStatusCode();
        clearChannelResponse.EnsureSuccessStatusCode();
        clearResponse.EnsureSuccessStatusCode();
        swapResponse.EnsureSuccessStatusCode();
        addResponse.EnsureSuccessStatusCode();
        removeResponse.EnsureSuccessStatusCode();
        applyResponse.EnsureSuccessStatusCode();
        printResponse.EnsureSuccessStatusCode();
        setResponse.EnsureSuccessStatusCode();
        clearAllResponse.EnsureSuccessStatusCode();

        Assert.Equal(
            [
                "LOAD 1-10 AMB",
                "CLEAR 1",
                "CLEAR 1-10",
                "SWAP 1-10 2-20 TRANSFORMS",
                "ADD 1-700 LOWER filename.mov",
                "REMOVE 1-700",
                "APPLY 1-10 LOWER",
                "PRINT 1",
                "SET 1 MODE FAST",
                "CLEAR ALL"
            ],
            fixture.ReceivedCommands);
    }

    [Fact]
    public async Task First_mixer_routes_send_expected_commands()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
            .WithAmcpReply("MIXER KEYER 1-10 1", "202 MIXER KEYER OK\r\n")
            .WithAmcpReply("MIXER INVERT 1-10 0", "202 MIXER INVERT OK\r\n")
            .WithAmcpReply("MIXER BLEND 1-10 ADD", "202 MIXER BLEND OK\r\n")
            .WithAmcpReply("MIXER OPACITY 1-10 0.5", "202 MIXER OPACITY OK\r\n")
            .WithAmcpReply("MIXER BRIGHTNESS 1-10 0.6", "202 MIXER BRIGHTNESS OK\r\n")
            .WithAmcpReply("MIXER SATURATION 1-10 0.7", "202 MIXER SATURATION OK\r\n")
            .WithAmcpReply("MIXER CONTRAST 1-10 0.8", "202 MIXER CONTRAST OK\r\n")
            .WithAmcpReply("MIXER VOLUME 1-10 0.9", "202 MIXER VOLUME OK\r\n")
            .WithAmcpReply("MIXER MASTERVOLUME 0.4", "202 MIXER MASTERVOLUME OK\r\n")
            .WithAmcpReply("MIXER COMMIT 1-10", "202 MIXER COMMIT OK\r\n")
            .WithAmcpReply("MIXER CLEAR 1-10", "202 MIXER CLEAR OK\r\n")
            .WithAmcpReply("CHANNEL_GRID 1", "202 CHANNEL_GRID OK\r\n"));

        var keyerResponse = await fixture.Client.PostAsJsonAsync("/channels/1/layers/10/mixer/keyer", new { value = true });
        var invertResponse = await fixture.Client.PostAsJsonAsync("/channels/1/layers/10/mixer/invert", new { value = false });
        var blendResponse = await fixture.Client.PostAsJsonAsync("/channels/1/layers/10/mixer/blend", new { value = "ADD" });
        var opacityResponse = await fixture.Client.PostAsJsonAsync("/channels/1/layers/10/mixer/opacity", new { value = 0.5 });
        var brightnessResponse = await fixture.Client.PostAsJsonAsync("/channels/1/layers/10/mixer/brightness", new { value = 0.6 });
        var saturationResponse = await fixture.Client.PostAsJsonAsync("/channels/1/layers/10/mixer/saturation", new { value = 0.7 });
        var contrastResponse = await fixture.Client.PostAsJsonAsync("/channels/1/layers/10/mixer/contrast", new { value = 0.8 });
        var volumeResponse = await fixture.Client.PostAsJsonAsync("/channels/1/layers/10/mixer/volume", new { value = 0.9 });
        var masterVolumeResponse = await fixture.Client.PostAsJsonAsync("/mixer/master-volume", new { value = 0.4 });
        var commitResponse = await fixture.Client.PostAsync("/channels/1/layers/10/mixer/commit", content: null);
        var clearResponse = await fixture.Client.PostAsync("/channels/1/layers/10/mixer/clear", content: null);
        var gridResponse = await fixture.Client.PostAsync("/channels/1/grid", content: null);

        keyerResponse.EnsureSuccessStatusCode();
        invertResponse.EnsureSuccessStatusCode();
        blendResponse.EnsureSuccessStatusCode();
        opacityResponse.EnsureSuccessStatusCode();
        brightnessResponse.EnsureSuccessStatusCode();
        saturationResponse.EnsureSuccessStatusCode();
        contrastResponse.EnsureSuccessStatusCode();
        volumeResponse.EnsureSuccessStatusCode();
        masterVolumeResponse.EnsureSuccessStatusCode();
        commitResponse.EnsureSuccessStatusCode();
        clearResponse.EnsureSuccessStatusCode();
        gridResponse.EnsureSuccessStatusCode();

        Assert.Equal(
            [
                "MIXER KEYER 1-10 1",
                "MIXER INVERT 1-10 0",
                "MIXER BLEND 1-10 ADD",
                "MIXER OPACITY 1-10 0.5",
                "MIXER BRIGHTNESS 1-10 0.6",
                "MIXER SATURATION 1-10 0.7",
                "MIXER CONTRAST 1-10 0.8",
                "MIXER VOLUME 1-10 0.9",
                "MIXER MASTERVOLUME 0.4",
                "MIXER COMMIT 1-10",
                "MIXER CLEAR 1-10",
                "CHANNEL_GRID 1"
            ],
            fixture.ReceivedCommands);
    }

    [Fact]
    public async Task Free_form_mixer_routes_send_expected_commands()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
            .WithAmcpReply("MIXER CHROMA 1-10 0.1 0.2 0.3", "202 MIXER CHROMA OK\r\n")
            .WithAmcpReply("MIXER LEVELS 1-10 0 1 1 0 1", "202 MIXER LEVELS OK\r\n")
            .WithAmcpReply("MIXER FILL 1-10 0 0 1 1", "202 MIXER FILL OK\r\n")
            .WithAmcpReply("MIXER CLIP 1-10 0 0 1 1", "202 MIXER CLIP OK\r\n")
            .WithAmcpReply("MIXER ANCHOR 1-10 0.5 0.5", "202 MIXER ANCHOR OK\r\n")
            .WithAmcpReply("MIXER CROP 1-10 0 0 0 0", "202 MIXER CROP OK\r\n")
            .WithAmcpReply("MIXER ROTATION 1-10 45", "202 MIXER ROTATION OK\r\n")
            .WithAmcpReply("MIXER PERSPECTIVE 1-10 0 0 1 0 1 1 0 1", "202 MIXER PERSPECTIVE OK\r\n")
            .WithAmcpReply("MIXER GRID 1-10 2 2", "202 MIXER GRID OK\r\n"));

        var chromaResponse = await fixture.Client.PostAsJsonAsync("/channels/1/layers/10/mixer/chroma", new { arguments = "0.1 0.2 0.3" });
        var levelsResponse = await fixture.Client.PostAsJsonAsync("/channels/1/layers/10/mixer/levels", new { arguments = "0 1 1 0 1" });
        var fillResponse = await fixture.Client.PostAsJsonAsync("/channels/1/layers/10/mixer/fill", new { arguments = "0 0 1 1" });
        var clipResponse = await fixture.Client.PostAsJsonAsync("/channels/1/layers/10/mixer/clip", new { arguments = "0 0 1 1" });
        var anchorResponse = await fixture.Client.PostAsJsonAsync("/channels/1/layers/10/mixer/anchor", new { arguments = "0.5 0.5" });
        var cropResponse = await fixture.Client.PostAsJsonAsync("/channels/1/layers/10/mixer/crop", new { arguments = "0 0 0 0" });
        var rotationResponse = await fixture.Client.PostAsJsonAsync("/channels/1/layers/10/mixer/rotation", new { arguments = "45" });
        var perspectiveResponse = await fixture.Client.PostAsJsonAsync("/channels/1/layers/10/mixer/perspective", new { arguments = "0 0 1 0 1 1 0 1" });
        var gridResponse = await fixture.Client.PostAsJsonAsync("/channels/1/layers/10/mixer/grid", new { arguments = "2 2" });

        chromaResponse.EnsureSuccessStatusCode();
        levelsResponse.EnsureSuccessStatusCode();
        fillResponse.EnsureSuccessStatusCode();
        clipResponse.EnsureSuccessStatusCode();
        anchorResponse.EnsureSuccessStatusCode();
        cropResponse.EnsureSuccessStatusCode();
        rotationResponse.EnsureSuccessStatusCode();
        perspectiveResponse.EnsureSuccessStatusCode();
        gridResponse.EnsureSuccessStatusCode();

        Assert.Equal(
            [
                "MIXER CHROMA 1-10 0.1 0.2 0.3",
                "MIXER LEVELS 1-10 0 1 1 0 1",
                "MIXER FILL 1-10 0 0 1 1",
                "MIXER CLIP 1-10 0 0 1 1",
                "MIXER ANCHOR 1-10 0.5 0.5",
                "MIXER CROP 1-10 0 0 0 0",
                "MIXER ROTATION 1-10 45",
                "MIXER PERSPECTIVE 1-10 0 0 1 0 1 1 0 1",
                "MIXER GRID 1-10 2 2"
            ],
            fixture.ReceivedCommands);
    }

    [Fact]
    public async Task Admin_runtime_routes_send_expected_commands()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
            .WithAmcpReply("DIAG", "202 DIAG OK\r\n")
            .WithAmcpReply("BYE", "202 BYE OK\r\n")
            .WithAmcpReply("KILL", "202 KILL OK\r\n")
            .WithAmcpReply("LOG LEVEL", "201 LOG OK\r\nINFO\r\n")
            .WithAmcpReply("LOG LEVEL debug", "202 LOG OK\r\n")
            .WithAmcpReply("LOCK 1 ACQUIRE phrase", "202 LOCK ACQUIRE OK\r\n")
            .WithAmcpReply("LOCK 1 RELEASE", "202 LOCK RELEASE OK\r\n")
            .WithAmcpReply("LOCK 1 CLEAR override", "202 LOCK CLEAR OK\r\n"));

        var diagResponse = await fixture.Client.PostAsync("/server/diag", content: null);
        var byeResponse = await fixture.Client.PostAsync("/admin/bye", content: null);
        var killResponse = await fixture.Client.PostAsync("/admin/kill", content: null);
        var getLogLevelResponse = await fixture.Client.GetAsync("/admin/log-level");
        var putLogLevelResponse = await fixture.Client.PutAsJsonAsync("/admin/log-level", new StringValueRequest("debug"));
        var acquireLockResponse = await fixture.Client.PostAsJsonAsync("/admin/locks/1/acquire", new { phrase = "phrase" });
        var releaseLockResponse = await fixture.Client.PostAsync("/admin/locks/1/release", content: null);
        var clearLockResponse = await fixture.Client.PostAsJsonAsync("/admin/locks/1/clear", new { overridePhrase = "override" });

        diagResponse.EnsureSuccessStatusCode();
        byeResponse.EnsureSuccessStatusCode();
        killResponse.EnsureSuccessStatusCode();
        getLogLevelResponse.EnsureSuccessStatusCode();
        putLogLevelResponse.EnsureSuccessStatusCode();
        acquireLockResponse.EnsureSuccessStatusCode();
        releaseLockResponse.EnsureSuccessStatusCode();
        clearLockResponse.EnsureSuccessStatusCode();

        var logLevel = await getLogLevelResponse.Content.ReadFromJsonAsync<StringValueRequest>();

        Assert.NotNull(logLevel);
        Assert.Equal("INFO", logLevel.Value);
        Assert.Equal(
            [
                "DIAG",
                "BYE",
                "KILL",
                "LOG LEVEL",
                "LOG LEVEL debug",
                "LOCK 1 ACQUIRE phrase",
                "LOCK 1 RELEASE",
                "LOCK 1 CLEAR override"
            ],
            fixture.ReceivedCommands);
    }
}
