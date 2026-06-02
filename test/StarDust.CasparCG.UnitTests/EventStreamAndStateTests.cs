using System.Threading.Channels;
using StarDust.CasparCG.Events;
using StarDust.CasparCG.State;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class EventStreamAndStateTests
{
    [Fact]
    public async Task Filtered_stream_returns_only_matching_events()
    {
        var source = Channel.CreateBounded<CasparEvent>(8);
        var stream = new CasparEventStream(source.Reader);

        await source.Writer.WriteAsync(new PlaybackClipChangedEvent("studio-a", 1, 10, "AMB"));
        await source.Writer.WriteAsync(new PlaybackClipChangedEvent("studio-a", 2, 5, "OTHER"));
        source.Writer.Complete();

        var result = new List<PlaybackClipChangedEvent>();
        await foreach (var evt in stream.ForChannel(1).OfType<PlaybackClipChangedEvent>().ReadAllAsync(CancellationToken.None))
        {
            result.Add(evt);
        }

        Assert.Single(result);
        Assert.Equal("AMB", result[0].Clip);
    }

    [Fact]
    public void State_store_tracks_last_clip_per_layer()
    {
        var state = new CasparStateStore();

        state.Apply(new PlaybackClipChangedEvent("studio-a", 1, 10, "AMB"));

        Assert.Equal("AMB", state.GetSnapshot().Channels[1].Layers[10].Clip);
    }
}
