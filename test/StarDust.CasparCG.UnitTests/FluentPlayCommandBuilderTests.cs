using StarDust.CasparCG;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class FluentPlayCommandBuilderTests
{
    [Fact]
    public async Task Fluent_play_builder_serializes_transition_and_loop()
    {
        var transport = new RecordingAmcpTransport();
        var client = new CasparClient(transport);

        await client
            .Channel(1)
            .Layer(10)
            .Play("AMB")
            .WithTransition().Mix(12)
            .WithLoop()
            .SendAsync(CancellationToken.None);

        Assert.Equal("PLAY 1-10 AMB MIX 12 LOOP\r\n", transport.LastCommandText);
    }

    [Fact]
    public async Task Fluent_loadbg_builder_serializes_loop_and_autoplay()
    {
        var transport = new RecordingAmcpTransport();
        var client = new CasparClient(transport);

        await client
            .Channel(1)
            .Layer(10)
            .LoadBg("AMB")
            .Loop()
            .AutoPlay()
            .SendAsync(CancellationToken.None);

        Assert.Equal("LOADBG 1-10 AMB LOOP AUTO\r\n", transport.LastCommandText);
    }

    [Fact]
    public async Task Fluent_scopes_remain_compatible_for_direct_play()
    {
        var transport = new RecordingAmcpTransport();
        var client = new CasparClient(transport);

        await client
            .Channel(2)
            .Layer(20)
            .Play("CLIP")
            .SendAsync(CancellationToken.None);

        Assert.Equal("PLAY 2-20 CLIP\r\n", transport.LastCommandText);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Fluent_play_and_loadbg_builders_reject_missing_clip_names(string clip)
    {
        var client = new CasparClient(new RecordingAmcpTransport());
        var layer = client.Channel(1).Layer(10);

        Assert.Throws<ArgumentException>(() => layer.Play(clip));
        Assert.Throws<ArgumentException>(() => layer.LoadBg(clip));
    }

    [Fact]
    public void Fluent_play_and_loadbg_builders_reject_null_clip_names()
    {
        var client = new CasparClient(new RecordingAmcpTransport());
        var layer = client.Channel(1).Layer(10);

        Assert.Throws<ArgumentNullException>(() => layer.Play(null!));
        Assert.Throws<ArgumentNullException>(() => layer.LoadBg(null!));
    }

    private sealed class RecordingAmcpTransport : IAmcpTransport
    {
        public string? LastCommandText { get; private set; }

        public ValueTask ConnectAsync(CancellationToken cancellationToken = default) => ValueTask.CompletedTask;

        public ValueTask DisconnectAsync(CancellationToken cancellationToken = default) => ValueTask.CompletedTask;

        public ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken = default)
        {
            LastCommandText = commandText;
            return ValueTask.FromResult("202 PLAY OK");
        }
    }
}
