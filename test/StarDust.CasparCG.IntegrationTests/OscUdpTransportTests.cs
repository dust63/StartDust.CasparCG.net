using System.Net;
using System.Net.Sockets;
using StarDust.CasparCG;
using StarDust.CasparCG.Events;
using StarDust.CasparCG.Osc;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.IntegrationTests;

public class OscUdpTransportTests
{
    [Fact]
    public async Task Udp_osc_packet_is_published_as_domain_event()
    {
        var client = new CasparClient(new RecordingAmcpTransport(), new UdpOscTransport());
        var port = GetFreeUdpPort();
        await client.StartOscAsync(port, CancellationToken.None);

        var received = new TaskCompletionSource<PlaybackClipChangedEvent>(TaskCreationOptions.RunContinuationsAsynchronously);
        _ = Task.Run(async () =>
        {
            await foreach (var evt in client.Events.OfType<PlaybackClipChangedEvent>().ReadAllAsync(CancellationToken.None))
            {
                received.TrySetResult(evt);
                break;
            }
        });

        await SendOscPacketAsync(port, "/channel/1/stage/layer/10/background/file/name", "TEST/GO1080P25");

        var evt = await received.Task.WaitAsync(TimeSpan.FromSeconds(2));

        Assert.Equal(1, evt.Channel);
        Assert.Equal(10, evt.Layer);
        Assert.Equal("TEST/GO1080P25", evt.Clip);
        Assert.Equal("TEST/GO1080P25", client.State.GetSnapshot().Channels[1].Layers[10].Clip);

        await client.StopOscAsync(CancellationToken.None);
    }

    private sealed class RecordingAmcpTransport : IAmcpTransport
    {
        public ValueTask ConnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

        public ValueTask DisconnectAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

        public ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken) =>
            ValueTask.FromResult("202 OK\r\n");
    }

    private static int GetFreeUdpPort()
    {
        using var udp = new UdpClient(0);
        return ((IPEndPoint)udp.Client.LocalEndPoint!).Port;
    }

    private static async Task SendOscPacketAsync(int port, string address, string value)
    {
        using var udp = new UdpClient();
        var payload = BuildOscPacket(address, value);
        await udp.SendAsync(payload, payload.Length, new IPEndPoint(IPAddress.Loopback, port));
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
