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
}
