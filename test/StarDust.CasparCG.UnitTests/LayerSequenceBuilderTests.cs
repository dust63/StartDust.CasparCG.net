using StarDust.CasparCG;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class LayerSequenceBuilderTests
{
    [Fact]
    public async Task Sequence_executes_commands_in_order_with_local_wait()
    {
        var transport = new RecordingAmcpTransport(
            "202 LOADBG OK\r\n",
            "202 PLAY OK\r\n");
        var client = new CasparClient(transport);

        await client
            .Channel(1)
            .Layer(10)
            .Sequence()
            .LoadBg("AMB")
            .Loop()
            .Then()
            .Wait(1)
            .Then()
            .Play("AMB")
            .SendAsync(CancellationToken.None);

        Assert.Equal(
            [
                "LOADBG 1-10 AMB LOOP\r\n",
                "PLAY 1-10 AMB\r\n"
            ],
            transport.SentCommands);
    }

    [Fact]
    public async Task Sequence_stops_when_a_command_fails()
    {
        var transport = new RecordingAmcpTransport(
            "404 LOADBG FAILED\r\n",
            "202 PLAY OK\r\n");
        var client = new CasparClient(transport);

        var exception = await Assert.ThrowsAsync<StarDust.CasparCG.Protocol.Amcp.AmcpCommandException>(
            () => client
                .Channel(1)
                .Layer(10)
                .Sequence()
                .LoadBg("AMB")
                .Then()
                .Play("AMB")
                .SendAsync(CancellationToken.None)
                .AsTask());

        Assert.Equal(404, exception.Response.StatusCode);
        Assert.Equal(["LOADBG 1-10 AMB\r\n"], transport.SentCommands);
    }

    [Fact]
    public async Task Sequence_supports_pause_steps_between_other_commands()
    {
        var transport = new RecordingAmcpTransport(
            "202 PLAY OK\r\n",
            "202 PAUSE OK\r\n",
            "202 CLEAR OK\r\n");
        var client = new CasparClient(transport);

        await client
            .Channel(1)
            .Layer(10)
            .Sequence()
            .Play("AMB")
            .Then()
            .Pause()
            .Then()
            .Clear()
            .SendAsync(CancellationToken.None);

        Assert.Equal(
            [
                "PLAY 1-10 AMB\r\n",
                "PAUSE 1-10\r\n",
                "CLEAR 1-10\r\n"
            ],
            transport.SentCommands);
    }

    [Fact]
    public async Task Sequence_supports_stop_steps_between_other_commands()
    {
        var transport = new RecordingAmcpTransport(
            "202 LOADBG OK\r\n",
            "202 STOP OK\r\n",
            "202 CLEAR OK\r\n");
        var client = new CasparClient(transport);

        await client
            .Channel(1)
            .Layer(10)
            .Sequence()
            .LoadBg("AMB")
            .Then()
            .Stop()
            .Then()
            .Clear()
            .SendAsync(CancellationToken.None);

        Assert.Equal(
            [
                "LOADBG 1-10 AMB\r\n",
                "STOP 1-10\r\n",
                "CLEAR 1-10\r\n"
            ],
            transport.SentCommands);
    }

    [Fact]
    public void Sequence_rejects_negative_wait_values()
    {
        var client = new CasparClient(new RecordingAmcpTransport());
        var sequence = client.Channel(1).Layer(10).Sequence();

        Action invalidMilliseconds = () => sequence.Wait(-1);
        Action invalidTimeSpan = () => sequence.Wait(TimeSpan.FromMilliseconds(-1));

        Assert.Throws<ArgumentOutOfRangeException>(invalidMilliseconds);
        Assert.Throws<ArgumentOutOfRangeException>(invalidTimeSpan);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Sequence_rejects_missing_clip_names(string clip)
    {
        var client = new CasparClient(new RecordingAmcpTransport());
        var sequence = client.Channel(1).Layer(10).Sequence();

        Assert.Throws<ArgumentException>(() => sequence.Play(clip));
        Assert.Throws<ArgumentException>(() => sequence.LoadBg(clip));
    }

    [Fact]
    public void Sequence_rejects_null_clip_names()
    {
        var client = new CasparClient(new RecordingAmcpTransport());
        var sequence = client.Channel(1).Layer(10).Sequence();

        Assert.Throws<ArgumentNullException>(() => sequence.Play(null!));
        Assert.Throws<ArgumentNullException>(() => sequence.LoadBg(null!));
    }

    private sealed class RecordingAmcpTransport : IAmcpTransport
    {
        private readonly Queue<string> _responses;

        public RecordingAmcpTransport(params string[] responses)
        {
            _responses = new Queue<string>(responses);
        }

        public List<string> SentCommands { get; } = [];

        public ValueTask ConnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

        public ValueTask DisconnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

        public ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken)
        {
            SentCommands.Add(commandText);
            return ValueTask.FromResult(_responses.Dequeue());
        }
    }
}
