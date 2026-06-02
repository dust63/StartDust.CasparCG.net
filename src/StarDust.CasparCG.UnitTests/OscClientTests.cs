using StarDust.CasparCG;
using StarDust.CasparCG.Events;
using StarDust.CasparCG.Osc;
using StarDust.CasparCG.Protocol.Osc;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class OscClientTests
{
    [Fact]
    public async Task Osc_packet_publishes_clip_changed_event_and_updates_state()
    {
        var amcpTransport = new RecordingAmcpTransport();
        var oscTransport = new InlineOscTransport();
        var client = new CasparClient(amcpTransport, oscTransport);
        await client.StartOscAsync(6250, CancellationToken.None);

        var received = new TaskCompletionSource<PlaybackClipChangedEvent>(TaskCreationOptions.RunContinuationsAsynchronously);
        _ = Task.Run(async () =>
        {
            await foreach (var evt in client.Events.OfType<PlaybackClipChangedEvent>().ReadAllAsync(CancellationToken.None))
            {
                received.TrySetResult(evt);
                break;
            }
        });

        await oscTransport.EmitAsync(BuildOscPacket("/channel/1/stage/layer/10/background/file/name", "TEST/GO1080P25"));

        var evt = await received.Task.WaitAsync(TimeSpan.FromSeconds(2));

        Assert.Equal("default", evt.ClientName);
        Assert.Equal(1, evt.Channel);
        Assert.Equal(10, evt.Layer);
        Assert.Equal("TEST/GO1080P25", evt.Clip);
        Assert.Equal("TEST/GO1080P25", client.State.GetSnapshot().Channels[1].Layers[10].Clip);
    }

    private sealed class RecordingAmcpTransport : IAmcpTransport
    {
        public ValueTask ConnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

        public ValueTask DisconnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

        public ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken) =>
            ValueTask.FromResult("202 OK\r\n");
    }

    private sealed class InlineOscTransport : IOscTransport
    {
        private Func<ReadOnlyMemory<byte>, CancellationToken, ValueTask>? _handler;

        public ValueTask StartAsync(int port, Func<ReadOnlyMemory<byte>, CancellationToken, ValueTask> packetHandler, CancellationToken cancellationToken)
        {
            _handler = packetHandler;
            return ValueTask.CompletedTask;
        }

        public ValueTask StopAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        public ValueTask EmitAsync(ReadOnlyMemory<byte> packet) =>
            _handler is null
                ? throw new InvalidOperationException("The OSC transport has not been started.")
                : _handler(packet, CancellationToken.None);
    }

    private static byte[] BuildOscPacket(string address, string stringArgument)
    {
        var bytes = new List<byte>();
        WriteOscString(bytes, address);
        WriteOscString(bytes, ",s");
        WriteOscString(bytes, stringArgument);
        return bytes.ToArray();
    }

    private static void WriteOscString(ICollection<byte> bytes, string value)
    {
        var textBytes = System.Text.Encoding.ASCII.GetBytes(value);
        foreach (var item in textBytes)
        {
            bytes.Add(item);
        }

        bytes.Add(0);
        while (bytes.Count % 4 != 0)
        {
            bytes.Add(0);
        }
    }
}
