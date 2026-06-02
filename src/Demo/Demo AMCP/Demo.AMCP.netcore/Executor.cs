using StarDust.CasparCG;
using StarDust.CasparCG.Events;
using StarDust.CasparCG.Osc;
using StarDust.CasparCG.Protocol.Amcp;
using StarDust.CasparCG.Transport;
using System.Net.Sockets;

namespace StarDust.Demo.AMCP.netcore;

internal sealed class Executor
{
    private const string DefaultHost = "127.0.0.1";
    private const int DefaultAmcpPort = 5250;
    private const int DefaultOscPort = 6250;
    private const int DefaultChannel = 1;
    private const int DefaultLayer = 10;
    private const string DefaultClip = "AMB";

    public static async Task RunAsync(string[] args)
    {
        var options = ParseOptions(args);
        await using var amcpTransport = new TcpAmcpTransport(options.Host, options.AmcpPort);
        await using var oscTransport = new UdpOscTransport();
        var client = new CasparClient(
            amcpTransport,
            oscTransport,
            new DefaultOscMessageMapper("demo-probe"),
            "demo-probe");
        client.OscPacketReceived += packet => PrintOscPacket(packet);

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(options.TimeoutSeconds));
        var eventTask = ObserveEventsAsync(client.Events, cts.Token);
        var oscStarted = false;

        try
        {
            Console.WriteLine($"Starting OSC listener on UDP {options.OscPort}...");
            await client.StartOscAsync(options.OscPort, cts.Token);
            oscStarted = true;

            Console.WriteLine($"Connecting to {options.Host}:{options.AmcpPort}...");
            await client.ConnectAsync(cts.Token);
            Console.WriteLine($"Subscribing OSC to UDP {options.OscPort}...");
            await client.SubscribeOscAsync(options.OscPort, cts.Token);

            await PrintServerInfoAsync(client, cts.Token);

            Console.WriteLine($"Triggering AMB on channel {options.Channel}, layer {options.Layer}...");
            await client.LoadBackgroundAsync(options.Channel, options.Layer, options.Clip, cts.Token);
            await client.PlayAsync(options.Channel, options.Layer, options.Clip, cts.Token);

            Console.WriteLine($"Waiting up to {options.TimeoutSeconds}s for OSC events...");
            try
            {
                await eventTask;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("OSC observation timed out.");
            }

            var snapshot = client.State.GetSnapshot();
            Console.WriteLine();
            Console.WriteLine("State snapshot:");
            DumpSnapshot(snapshot);
        }
        catch (SocketException ex)
        {
            Console.Error.WriteLine($"Unable to connect to {options.Host}:{options.AmcpPort}: {ex.Message}");
            Environment.ExitCode = 1;
        }
        finally
        {
            cts.Cancel();

            if (oscStarted)
            {
                await client.StopOscAsync(CancellationToken.None);
            }
        }
    }

    private static async Task ObserveEventsAsync(CasparEventStream events, CancellationToken cancellationToken)
    {
        await foreach (var evt in events.ReadAllAsync(cancellationToken))
        {
            Console.WriteLine($"OSC event: {evt}");
        }
    }

    private static void PrintOscPacket(ReadOnlyMemory<byte> packet)
    {
        var hex = Convert.ToHexString(packet.Span);
        Console.WriteLine($"OSC packet ({packet.Length} bytes): {hex}");
    }

    private static async Task PrintServerInfoAsync(CasparClient client, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine("AMCP version:");
            Console.WriteLine(await client.GetVersionAsync(cancellationToken));
        }
        catch (AmcpCommandException ex)
        {
            Console.WriteLine($"VERSION failed: {ex.Response.StatusCode} {ex.Response.CommandText}");
        }

        Console.WriteLine();

        try
        {
            var medias = await client.GetMediaFilesAsync(cancellationToken);
            Console.WriteLine($"Media files ({medias.Count}):");
            foreach (var media in medias)
            {
                Console.WriteLine(media);
            }
        }
        catch (AmcpCommandException ex)
        {
            Console.WriteLine($"CLS failed: {ex.Response.StatusCode} {ex.Response.CommandText}");
        }

        Console.WriteLine();
    }

    private static void DumpSnapshot(StarDust.CasparCG.State.CasparStateSnapshot snapshot)
    {
        if (snapshot.Channels.Count == 0)
        {
            Console.WriteLine("  No tracked channels yet.");
            return;
        }

        foreach (var channel in snapshot.Channels)
        {
            Console.WriteLine($"  Channel {channel.Key}:");
            foreach (var layer in channel.Value.Layers)
            {
                Console.WriteLine($"    Layer {layer.Key}: {layer.Value.Clip}");
            }
        }
    }

    private static ProbeOptions ParseOptions(string[] args)
    {
        return new ProbeOptions
        {
            Host = args.ElementAtOrDefault(0) ?? DefaultHost,
            AmcpPort = ParseInt(args, 1, DefaultAmcpPort),
            OscPort = ParseInt(args, 2, DefaultOscPort),
            Channel = ParseInt(args, 3, DefaultChannel),
            Layer = ParseInt(args, 4, DefaultLayer),
            Clip = args.ElementAtOrDefault(5) ?? DefaultClip
        };
    }

    private static int ParseInt(string[] args, int index, int fallback) =>
        int.TryParse(args.ElementAtOrDefault(index), out var value) ? value : fallback;

    private sealed class ProbeOptions
    {
        public string Host { get; init; } = DefaultHost;

        public int AmcpPort { get; init; } = DefaultAmcpPort;

        public int OscPort { get; init; } = DefaultOscPort;

        public int Channel { get; init; } = DefaultChannel;

        public int Layer { get; init; } = DefaultLayer;

        public string Clip { get; init; } = DefaultClip;

        public int TimeoutSeconds { get; init; } = 8;
    }
}
