using System.Text.Json;
using StarDust.CasparCG.Events;
using Xunit;

namespace StarDust.CasparCG.IntegrationTests;

public sealed class CasparRestApiSseTests
{
    [Fact]
    public async Task Get_events_streams_named_sse_messages()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync();

        var responseTask = fixture.Client.GetAsync("/events", HttpCompletionOption.ResponseHeadersRead);
        await fixture.PublishEventAsync(new PlaybackClipChangedEvent("default", 1, 10, "AMB"));
        using var response = await responseTask;

        response.EnsureSuccessStatusCode();
        Assert.Equal("text/event-stream", response.Content.Headers.ContentType!.MediaType);

        await using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        var eventLine = await reader.ReadLineAsync();
        var dataLine = await reader.ReadLineAsync();

        Assert.Equal("event: playbackClipChanged", eventLine);
        Assert.NotNull(dataLine);
        Assert.StartsWith("data: ", dataLine, StringComparison.Ordinal);

        using var json = JsonDocument.Parse(dataLine["data: ".Length..]);
        Assert.Equal("playbackClipChanged", json.RootElement.GetProperty("type").GetString());
        Assert.Equal("default", json.RootElement.GetProperty("target").GetString());
    }

    [Theory]
    [MemberData(nameof(NewOscEventNames))]
    public async Task Get_events_streams_new_osc_event_names(CasparEvent evt, string expectedEventName)
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync();

        var responseTask = fixture.Client.GetAsync("/events", HttpCompletionOption.ResponseHeadersRead);
        await fixture.PublishEventAsync(evt);
        using var response = await responseTask;

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        var eventLine = await reader.ReadLineAsync();
        var dataLine = await reader.ReadLineAsync();

        Assert.Equal($"event: {expectedEventName}", eventLine);
        Assert.NotNull(dataLine);
        Assert.StartsWith("data: ", dataLine, StringComparison.Ordinal);

        using var json = JsonDocument.Parse(dataLine["data: ".Length..]);
        Assert.Equal(expectedEventName, json.RootElement.GetProperty("type").GetString());
        Assert.Equal(evt.ClientName, json.RootElement.GetProperty("target").GetString());
        Assert.True(json.RootElement.TryGetProperty("payload", out _));
    }

    public static IEnumerable<object[]> NewOscEventNames()
    {
        yield return
        [
            new OscStateChangedEvent("default", 1, "stage/layer/10/foreground/file/name", ["AMB"]),
            "oscStateChanged"
        ];
        yield return
        [
            new LayerProducerChangedEvent("default", 1, 10, "foreground", "ffmpeg"),
            "layerProducerChanged"
        ];
        yield return
        [
            new LayerPausedChangedEvent("default", 1, 10, true),
            "layerPausedChanged"
        ];
        yield return
        [
            new LayerProgressChangedEvent("default", 1, 10, "foreground", 12.5d, 30d),
            "layerProgressChanged"
        ];
        yield return
        [
            new LayerFramesLeftChangedEvent("default", 1, 10, 42),
            "layerFramesLeftChanged"
        ];
    }
}
