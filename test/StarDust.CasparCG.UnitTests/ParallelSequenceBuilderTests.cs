using StarDust.CasparCG;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class ParallelSequenceBuilderTests
{
    [Fact]
    public async Task Parallel_executes_sequences_concurrently_while_preserving_sequence_order()
    {
        var secondCommandStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var firstCommandReleased = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var transport = new CoordinatedAmcpTransport(async commandText =>
        {
            if (commandText == "LOADBG 1-10 AMB\r\n")
            {
                secondCommandStarted.TrySetResult();
                await firstCommandReleased.Task;
                return "202 LOADBG OK\r\n";
            }

            if (commandText == "PLAY 2-20 BMB\r\n")
            {
                secondCommandStarted.TrySetResult();
                return "202 PLAY OK\r\n";
            }

            return commandText switch
            {
                "PLAY 1-10 AMB\r\n" => "202 PLAY OK\r\n",
                "LOADBG 2-20 BMB\r\n" => "202 LOADBG OK\r\n",
                _ => throw new InvalidOperationException($"Unexpected command: {commandText}")
            };
        });
        var client = new CasparClient(transport);

        var sendTask = client
            .Parallel(
                client.Channel(1).Layer(10).Sequence()
                    .LoadBg("AMB")
                    .Then()
                    .Play("AMB"),
                client.Channel(2).Layer(20).Sequence()
                    .Play("BMB")
                    .Then()
                    .LoadBg("BMB")
                    .Then())
            .SendAsync(CancellationToken.None)
            .AsTask();

        await secondCommandStarted.Task.WaitAsync(TimeSpan.FromSeconds(1));
        firstCommandReleased.TrySetResult();
        await sendTask;

        Assert.Equal(
            [
                "LOADBG 1-10 AMB\r\n",
                "PLAY 2-20 BMB\r\n",
                "LOADBG 2-20 BMB\r\n",
                "PLAY 1-10 AMB\r\n"
            ],
            transport.SentCommands);
    }

    [Fact]
    public async Task Parallel_fails_when_any_sequence_fails()
    {
        var transport = new RecordingAmcpTransport(
            "202 LOADBG OK\r\n",
            "404 PLAY FAILED\r\n");
        var client = new CasparClient(transport);

        var exception = await Assert.ThrowsAsync<StarDust.CasparCG.Protocol.Amcp.AmcpCommandException>(
            () => client
                .Parallel(
                    client.Channel(1).Layer(10).Sequence()
                        .LoadBg("AMB")
                        .Then(),
                    client.Channel(2).Layer(20).Sequence()
                        .Play("BMB"))
                .SendAsync(CancellationToken.None)
                .AsTask());

        Assert.Equal(404, exception.Response.StatusCode);
        Assert.Equal(
            [
                "LOADBG 1-10 AMB\r\n",
                "PLAY 2-20 BMB\r\n"
            ],
            transport.SentCommands);
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

    private sealed class CoordinatedAmcpTransport(Func<string, Task<string>> sendAsync) : IAmcpTransport
    {
        private readonly Func<string, Task<string>> _sendAsync = sendAsync;

        public List<string> SentCommands { get; } = [];

        public ValueTask ConnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

        public ValueTask DisconnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

        public async ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken)
        {
            SentCommands.Add(commandText);
            return await _sendAsync(commandText);
        }
    }
}
