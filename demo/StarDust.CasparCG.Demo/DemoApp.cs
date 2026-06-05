using System.Net.Sockets;
using Spectre.Console;
using StarDust.CasparCG;
using StarDust.CasparCG.Events;
using StarDust.CasparCG.Osc;
using StarDust.CasparCG.Protocol.Amcp;
using StarDust.CasparCG.Query;
using StarDust.CasparCG.State;
using StarDust.CasparCG.Transport;

namespace StarDust.CasparCG.Demo;

internal static class DemoApp
{
    private const string DefaultHost = "127.0.0.1";
    private const int DefaultAmcpPort = 5250;
    private const int DefaultOscPort = 6250;
    private const int DefaultChannel = 1;
    private const int DefaultLayer = 10;
    private const string DefaultClip = "AMB";
    private const int DefaultTimeoutSeconds = 8;

    private static readonly Style AccentStyle = new(Color.Green);
    public static async Task<int> RunAsync(string[] args)
    {
        var arguments = DemoArguments.Parse(args);
        if (arguments.ShowHelp)
        {
            RenderHelp();
            return arguments.ExitCode;
        }

        RenderBanner();

        try
        {
            var options = arguments.Options;
            if (arguments.Command == DemoCommand.Showcase)
            {
                var connection = ResolveShowcaseConnection(options);
                options = options with
                {
                    Host = connection.Host,
                    AmcpPort = connection.AmcpPort,
                    OscPort = connection.OscPort
                };
            }

            await using var session = new DemoSession(options);
            return await RunCommandAsync(arguments.Command, session, options);
        }
        catch (SocketException ex)
        {
            AnsiConsole.MarkupLine($"[red]Unable to connect:[/] {Markup.Escape(ex.Message)}");
            return 1;
        }
        catch (IOException ex)
        {
            AnsiConsole.MarkupLine($"[red]Transport error:[/] {Markup.Escape(ex.Message)}");
            return 1;
        }
        catch (AmcpCommandException ex)
        {
            RenderAmcpFailure("AMCP command failed", ex.Response);
            return 1;
        }
        catch (OperationCanceledException)
        {
            AnsiConsole.MarkupLine("[yellow]Demo timed out.[/]");
            return 1;
        }
    }

    private static async Task<int> RunCommandAsync(DemoCommand command, DemoSession session, DemoOptions options)
    {
        switch (command)
        {
            case DemoCommand.Showcase:
                await RunShowcaseAsync(session, options);
                break;
            case DemoCommand.Server:
                await RunServerAsync(session);
                break;
            case DemoCommand.Catalog:
                await RunCatalogAsync(session);
                break;
            case DemoCommand.Data:
                await RunDataAsync(session);
                break;
            case DemoCommand.Playback:
                await RunPlaybackAsync(session);
                break;
            case DemoCommand.Admin:
                await RunAdminAsync(session);
                break;
            case DemoCommand.Osc:
                await RunOscAsync(session, options);
                break;
        }

        return 0;
    }

    private static async Task RunShowcaseAsync(DemoSession session, DemoOptions options)
    {
        await session.ConnectAsync();
        RenderConnection(session);

        if (options.VerboseOsc)
        {
            session.Client.OscPacketReceived += packet =>
                AnsiConsole.MarkupLine($"[green]OSC packet:[/] {Markup.Escape(DescribeOscPacket(packet))}");
        }

        await session.StartOscAsync();
        await session.SubscribeOscAsync();

        var oscEvents = await ObserveOscEventsAsync(session, options.TimeoutSeconds, async () =>
        {
            await RunServerAsync(session);
            await RunCatalogAsync(session);
            await RunDataAsync(session);
            await RunPlaybackAsync(session);
            await RunAdminAsync(session);
        });

        await session.UnsubscribeOscAsync();
        RenderOscEventTable(oscEvents);
        RenderStateSnapshot(session.Client.State.GetSnapshot());
    }

    private static async Task RunServerAsync(DemoSession session)
    {
        AnsiConsole.MarkupLine("[bold green]Server[/]");
        RenderSimpleTable(
            "Server summary",
            ["Field", "Value"],
            [
                ("Version", await session.Client.Server().VersionAsync()),
                ("Server Info", "queried"),
                ("Config", "queried"),
                ("Paths", "queried"),
                ("GL", "queried")
            ]);

        RenderQueryTable("INFO", session.Client.Server().InfoAsync, "server-name", "channel-count");
        RenderQueryTable("INFO CONFIG", session.Client.Server().InfoConfigAsync, "config.log-level", "config.channels.count");
        RenderQueryTable("INFO PATHS", session.Client.Server().InfoPathsAsync, "paths.media-path", "paths.template-path");
        RenderQueryTable("GL INFO", session.Client.Server().GlInfoAsync, "gl.renderer", "gl.vendor");
        RenderCommandLog(session.Transport);
    }

    private static async Task RunCatalogAsync(DemoSession session)
    {
        AnsiConsole.MarkupLine("[bold green]Catalog[/]");
        var mediaFiles = await session.Client.Server().MediaFilesAsync();
        var mediaInfo = await session.Client.Server().MediaInfoAsync(session.Options.Clip);
        var fontFiles = await session.Client.Server().FontFilesAsync();
        var templateFiles = await session.Client.Server().TemplateFilesAsync();

        RenderMediaTable(mediaFiles, 12);
        RenderMediaInfoTable(mediaInfo);
        RenderSimpleTable("Fonts", ["Name", "Path"], fontFiles.Select(font => (font.Name, font.Path)));
        RenderSimpleTable("Templates", ["Name", "Value"], templateFiles.Select(template => (template.Name, string.Empty)));
        RenderCommandLog(session.Transport);
    }

    private static async Task RunDataAsync(DemoSession session)
    {
        AnsiConsole.MarkupLine("[bold green]Data[/]");
        var data = session.Client.Data();
        var key = "demo.note";
        var store = await data.StoreAsync(key, "Hello from StarDust");
        var retrieve = await data.RetrieveAsync(key);
        var list = await data.ListAsync(null);
        var remove = await data.RemoveAsync(key);

        RenderSimpleTable(
            "Data round trip",
            ["Operation", "Result"],
            [
                ("Store", store.StatusCode.ToString()),
                ("Retrieve", retrieve.Lines.SingleOrDefault() ?? string.Empty),
                ("List", list.Lines.Count.ToString()),
                ("Remove", remove.StatusCode.ToString())
            ]);
        RenderSimpleTable("Data list", ["Entry", "Value"], list.Lines.Select(line => (line, string.Empty)));
        RenderCommandLog(session.Transport);
    }

    private static async Task RunPlaybackAsync(DemoSession session)
    {
        AnsiConsole.MarkupLine("[bold green]Playback[/]");
        await session.Client.Channel(session.Options.Channel)
            .Layer(session.Options.Layer)
            .Sequence()
            .LoadBg(session.Options.Clip)
            .Loop()
            .AutoPlay()
            .Then()
            .Play(session.Options.Clip)
            .Then()
            .Pause()
            .SendAsync();

        await session.Client.ResumeAsync(session.Options.Channel, session.Options.Layer);
        await session.Client.StopAsync(session.Options.Channel, session.Options.Layer);

        await session.Client.Channel(session.Options.Channel)
            .Layer(session.Options.Layer)
            .CallBgAsync("BG");
        await session.Client.Channel(session.Options.Channel)
            .Layer(session.Options.Layer)
            .CallAsync(session.Options.Clip);
        await session.Client.Channel(session.Options.Channel)
            .Layer(session.Options.Layer)
            .ClearAsync();

        RenderSimpleTable(
            "Playback state",
            ["Channel", "Layer", "Clip"],
            RenderLayers(session.Client.State.GetSnapshot()));
        RenderCommandLog(session.Transport);
    }

    private static async Task RunAdminAsync(DemoSession session)
    {
        AnsiConsole.MarkupLine("[bold green]Admin[/]");
        var admin = session.Client.Admin();
        await session.Client.DiagAsync();
        var currentLogLevel = await admin.GetLogLevelAsync();
        await admin.SetLogLevelAsync("debug");
        await admin.AcquireLockAsync(session.Options.Channel, "phrase");
        await admin.ReleaseLockAsync(session.Options.Channel);
        await admin.ClearLockAsync(session.Options.Channel);

        var rows = new List<(string, string)>
        {
            ("Log level", currentLogLevel),
            ("Lock", "acquire/release/clear")
        };

        rows.Add(("Shutdown", "not executed in the demo"));

        RenderSimpleTable("Admin status", ["Operation", "Value"], rows);
        RenderCommandLog(session.Transport);
    }

    private static async Task RunOscAsync(DemoSession session, DemoOptions options)
    {
        AnsiConsole.MarkupLine("[bold green]OSC[/]");
        await session.StartOscAsync();
        await session.SubscribeOscAsync();

        var oscEvents = await ObserveOscEventsAsync(session, options.TimeoutSeconds, async () =>
        {
            await session.Client.PlayAsync(session.Options.Channel, session.Options.Layer, session.Options.Clip);
        });

        await session.Client.Osc().UnsubscribeAsync(session.Options.OscPort);
        RenderOscEventTable(oscEvents);
        RenderStateSnapshot(session.Client.State.GetSnapshot());
        RenderCommandLog(session.Transport);
    }

    internal static (string Host, int AmcpPort, int OscPort) ResolveShowcaseConnection(
        DemoOptions options,
        Func<string, string, string>? askString = null,
        Func<string, int, int>? askInt = null)
    {
        askString ??= PromptString;
        askInt ??= PromptInt;

        var host = options.Host == DefaultHost ? askString("host", DefaultHost) : options.Host;
        var amcpPort = options.AmcpPort == DefaultAmcpPort
            ? askInt("amcp-port", DefaultAmcpPort)
            : options.AmcpPort;
        var oscPort = options.OscPort == DefaultOscPort
            ? askInt("osc-port", DefaultOscPort)
            : options.OscPort;

        return (host, amcpPort, oscPort);
    }

    private static async Task<List<PlaybackClipChangedEvent>> ObserveOscEventsAsync(
        DemoSession session,
        int timeoutSeconds,
        Func<Task> action)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
        var events = new List<PlaybackClipChangedEvent>();
        var collector = CollectOscEventsAsync(session.Client.Events, events, cts.Token);

        await action();
        await Task.Delay(TimeSpan.FromSeconds(1));

        cts.Cancel();
        await collector;
        return events;
    }

    private static async Task CollectOscEventsAsync(
        CasparEventStream events,
        ICollection<PlaybackClipChangedEvent> collected,
        CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var evt in events.ReadAllAsync(cancellationToken))
            {
                if (evt is PlaybackClipChangedEvent playback)
                {
                    collected.Add(playback);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }

    private static void RenderOscEventTable(IReadOnlyList<PlaybackClipChangedEvent> events)
    {
        RenderSimpleTable(
            "OSC events",
            ["Channel", "Layer", "Clip"],
            events.Count == 0
                ? new[] { ("-", "-", "(none)") }
                : events.Select(evt => (evt.Channel.ToString(), evt.Layer.ToString(), evt.Clip)).ToArray());
    }

    private static void RenderStateSnapshot(CasparStateSnapshot snapshot)
    {
        var rows = snapshot.Channels
            .SelectMany(channel => channel.Value.Layers.Select(layer => (channel.Key.ToString(), layer.Key.ToString(), layer.Value.Clip ?? string.Empty)))
            .ToArray();

        RenderSimpleTable(
            "Tracked layers",
            ["Channel", "Layer", "Clip"],
            rows.Length == 0 ? new[] { ("-", "-", "(none)") } : rows);
    }

    private static void RenderMediaTable(IReadOnlyList<MediaFile> mediaFiles, int limit)
    {
        AnsiConsole.MarkupLine("[bold green]Media[/]");
        var rows = mediaFiles
            .Take(limit)
            .Select(media => (
                media.Name,
                media.Kind.ToString(),
                media.SizeBytes.ToString(),
                media.LastModified?.ToString("u") ?? string.Empty,
                media.FrameCount?.ToString() ?? string.Empty,
                media.FrameRateOrDuration ?? string.Empty))
            .ToArray();

        var table = CreateTable(["Name", "Kind", "Size", "Modified", "Frames", "Rate/Duration"]);
        foreach (var row in rows)
        {
            table.AddRow(row.Item1, row.Item2, row.Item3, row.Item4, row.Item5, row.Item6);
        }

        if (rows.Length == 0)
        {
            table.AddRow("[grey](none)[/]", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        AnsiConsole.Write(table);
        Console.WriteLine();
    }

    private static void RenderMediaInfoTable(MediaInfo mediaInfo)
    {
        var rows = new List<(string, string)>
        {
            ("Name", mediaInfo.Name),
            ("Kind", mediaInfo.Kind.ToString()),
            ("Size", mediaInfo.SizeBytes?.ToString() ?? string.Empty),
            ("Modified", mediaInfo.LastModified?.ToString("u") ?? string.Empty),
            ("Frames", mediaInfo.FrameCount?.ToString() ?? string.Empty),
            ("Rate/Duration", mediaInfo.FrameRateOrDuration ?? string.Empty)
        };

        rows.AddRange(mediaInfo.Properties.Select(property => ($"Property {property.Key}", property.Value)));
        RenderSimpleTable("Media info", ["Field", "Value"], rows);
    }

    private static void RenderQueryTable(string title, Func<CancellationToken, ValueTask<QueryDataMap>> query, params string[] interestingKeys)
    {
        var data = query(CancellationToken.None).GetAwaiter().GetResult();
        var rows = new List<(string, string)>();
        foreach (var key in interestingKeys)
        {
            if (data.Values.TryGetValue(key, out var value))
            {
                rows.Add((key, value));
            }
        }

        if (rows.Count == 0)
        {
            rows.Add(("Payload", "no mapped values"));
        }

        RenderSimpleTable(title, ["Field", "Value"], rows);
    }

    private static void RenderCommandLog(RecordingAmcpTransport transport)
    {
        var commands = transport.SentCommands.ToArray();
        if (commands.Length == 0)
        {
            return;
        }

        AnsiConsole.MarkupLine("[bold green]Commands[/]");
        RenderSimpleTable(
            "Commands",
            ["#", "Wire"],
            commands.Select((command, index) => ((index + 1).ToString(), command)).ToArray());
        transport.SentCommands.Clear();
    }

    private static void RenderSimpleTable(string title, string[] columns, IEnumerable<(string, string)> rows)
    {
        AnsiConsole.MarkupLine($"[bold green]{Markup.Escape(title)}[/]");
        var table = CreateTable(columns);
        var rowArray = rows.ToArray();
        if (rowArray.Length == 0)
        {
            table.AddRow("[grey](none)[/]", string.Empty);
        }
        else
        {
            foreach (var row in rowArray)
            {
                table.AddRow(EscapeCell(row.Item1), EscapeCell(row.Item2));
            }
        }

        AnsiConsole.Write(table);
        Console.WriteLine();
    }

    private static void RenderSimpleTable(string title, string[] columns, IEnumerable<(string, string, string)> rows)
    {
        AnsiConsole.MarkupLine($"[bold green]{Markup.Escape(title)}[/]");
        var table = CreateTable(columns);
        var rowArray = rows.ToArray();
        if (rowArray.Length == 0)
        {
            table.AddRow("[grey](none)[/]", string.Empty, string.Empty);
        }
        else
        {
            foreach (var row in rowArray)
            {
                table.AddRow(EscapeCell(row.Item1), EscapeCell(row.Item2), EscapeCell(row.Item3));
            }
        }

        AnsiConsole.Write(table);
        Console.WriteLine();
    }

    private static void RenderSimpleTable(string title, string[] columns, IEnumerable<(string, string, string, string, string, string)> rows)
    {
        AnsiConsole.MarkupLine($"[bold green]{Markup.Escape(title)}[/]");
        var table = CreateTable(columns);
        var rowArray = rows.ToArray();
        if (rowArray.Length == 0)
        {
            table.AddRow("[grey](none)[/]", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        }
        else
        {
            foreach (var row in rowArray)
            {
                table.AddRow(
                    EscapeCell(row.Item1),
                    EscapeCell(row.Item2),
                    EscapeCell(row.Item3),
                    EscapeCell(row.Item4),
                    EscapeCell(row.Item5),
                    EscapeCell(row.Item6));
            }
        }

        AnsiConsole.Write(table);
        Console.WriteLine();
    }

    private static Table CreateTable(string[] columns)
    {
        var table = new Table
        {
            Border = TableBorder.Rounded,
            BorderStyle = AccentStyle
        };

        foreach (var column in columns)
        {
            table.AddColumn($"[green]{Markup.Escape(column)}[/]");
        }

        return table;
    }

    private static string EscapeCell(string value) => Markup.Escape(value);

    private static void RenderConnection(DemoSession session)
    {
        AnsiConsole.MarkupLine($"[green]Connecting to[/] {Markup.Escape(session.Options.Host)}:[green]{session.Options.AmcpPort}[/]");
        AnsiConsole.MarkupLine($"[green]OSC port[/] [green]{session.Options.OscPort}[/]");
        Console.WriteLine();
    }

    private static void RenderBanner()
    {
        var banner = new FigletText("CASPARCG")
        {
            Color = Color.Green,
            Justification = Justify.Center
        };

        AnsiConsole.Write(banner);
        AnsiConsole.MarkupLine("[bold green]StarDust.CasparCG demo[/]");
        AnsiConsole.MarkupLine("[green]Modern AMCP, OSC, and state showcase[/]");
        Console.WriteLine();
    }

    private static void RenderHelp()
    {
        RenderBanner();

        AnsiConsole.MarkupLine("[bold green]Usage[/]");
        AnsiConsole.MarkupLine($"[green]demo[/] [grey]<command>[/] [grey]{Markup.Escape("[options]")}[/]");
        Console.WriteLine();

        RenderSimpleTable(
            "Commands",
            ["Command", "Purpose"],
            [
                ("showcase", "Run every showcase scenario"),
                ("server", "Display server-level AMCP information"),
                ("catalog", "Display media, font, and template catalog data"),
                ("data", "Demonstrate DATA commands"),
                ("playback", "Demonstrate playback and layer commands"),
                ("admin", "Demonstrate log and lock commands"),
                ("osc", "Demonstrate OSC state updates")
            ]);

        RenderSimpleTable(
            "Options",
            ["Option", "Default"],
            [
                ("--host", DefaultHost),
                ("--amcp-port", DefaultAmcpPort.ToString()),
                ("--osc-port", DefaultOscPort.ToString()),
                ("--channel", DefaultChannel.ToString()),
                ("--layer", DefaultLayer.ToString()),
                ("--clip", DefaultClip),
                ("--timeout", DefaultTimeoutSeconds.ToString()),
                ("--verbose-osc", "false")
            ]);
    }

    private static void RenderAmcpFailure(string title, AmcpResponse response)
    {
        AnsiConsole.MarkupLine($"[red]{Markup.Escape(title)}[/]");
        RenderSimpleTable(
            "AMCP failure",
            ["Field", "Value"],
            [
                ("Status", response.StatusCode.ToString()),
                ("Command", response.CommandText),
                ("Status line", response.StatusLine)
            ]);
    }

    private static string DescribeOscPacket(ReadOnlyMemory<byte> packet)
    {
        var span = packet.Span;
        if (span.Length >= 8 && span[..8].SequenceEqual("#bundle\0"u8))
        {
            return $"bundle {packet.Length} bytes";
        }

        var previewLength = Math.Min(packet.Length, 32);
        var preview = Convert.ToHexString(packet.Span[..previewLength]);
        return packet.Length == previewLength ? $"{packet.Length} bytes {preview}" : $"{packet.Length} bytes {preview}...";
    }

    private static IEnumerable<(string, string, string)> RenderLayers(CasparStateSnapshot snapshot)
    {
        foreach (var channel in snapshot.Channels)
        {
            foreach (var layer in channel.Value.Layers)
            {
                yield return (channel.Key.ToString(), layer.Key.ToString(), layer.Value.Clip ?? string.Empty);
            }
        }
    }

    private sealed class DemoSession : IAsyncDisposable
    {
        private readonly RecordingAmcpTransport _transport;
        private readonly UdpOscTransport _oscTransport;
        private bool _oscStarted;

        public DemoSession(DemoOptions options)
        {
            Options = options;
            _transport = new RecordingAmcpTransport(options.Host, options.AmcpPort);
            _oscTransport = new UdpOscTransport();
            Client = new CasparClient(
                _transport,
                _oscTransport,
                new DefaultOscMessageMapper("demo-probe"),
                "demo-probe");
        }

        public DemoOptions Options { get; }

        public CasparClient Client { get; }

        public RecordingAmcpTransport Transport => _transport;

        public ValueTask ConnectAsync(CancellationToken cancellationToken = default) =>
            Client.ConnectAsync(cancellationToken);

        public async ValueTask StartOscAsync(CancellationToken cancellationToken = default)
        {
            if (_oscStarted)
            {
                return;
            }

            await Client.StartOscAsync(Options.OscPort, cancellationToken);
            _oscStarted = true;
        }

        public ValueTask SubscribeOscAsync(CancellationToken cancellationToken = default) =>
            Client.Osc().SubscribeAsync(Options.OscPort, cancellationToken);

        public ValueTask UnsubscribeOscAsync(CancellationToken cancellationToken = default) =>
            Client.Osc().UnsubscribeAsync(Options.OscPort, cancellationToken);

        public async ValueTask DisposeAsync()
        {
            if (_oscStarted)
            {
                await Client.StopOscAsync(CancellationToken.None);
            }

            await _oscTransport.DisposeAsync();
            await _transport.DisposeAsync();
        }
    }

    private sealed class RecordingAmcpTransport : IAmcpTransport, IAsyncDisposable
    {
        private readonly TcpAmcpTransport _inner;

        public RecordingAmcpTransport(string host, int port)
        {
            _inner = new TcpAmcpTransport(host, port);
        }

        public List<string> SentCommands { get; } = [];

        public ValueTask ConnectAsync(CancellationToken cancellationToken = default) =>
            _inner.ConnectAsync(cancellationToken);

        public ValueTask DisconnectAsync(CancellationToken cancellationToken = default) =>
            _inner.DisconnectAsync(cancellationToken);

        public async ValueTask<string> SendAsync(string commandText, CancellationToken cancellationToken = default)
        {
            SentCommands.Add(commandText.TrimEnd('\r', '\n'));
            return await _inner.SendAsync(commandText, cancellationToken);
        }

        public ValueTask DisposeAsync() => _inner.DisposeAsync();
    }

    internal sealed record DemoOptions(
        string Host,
        int AmcpPort,
        int OscPort,
        int Channel,
        int Layer,
        string Clip,
        int TimeoutSeconds,
        bool VerboseOsc);

    private enum DemoCommand
    {
        Showcase,
        Server,
        Catalog,
        Data,
        Playback,
        Admin,
        Osc
    }

    private sealed record DemoArguments(DemoCommand Command, DemoOptions Options, bool ShowHelp, int ExitCode)
    {
        public static DemoArguments Parse(string[] args)
        {
            if (args.Length == 0)
            {
                return new DemoArguments(DemoCommand.Showcase, CreateOptions(Array.Empty<string>()), ShowHelp: false, ExitCode: 0);
            }

            if (args.Any(IsHelp))
            {
                return new DemoArguments(DemoCommand.Showcase, CreateOptions(Array.Empty<string>()), ShowHelp: true, ExitCode: 0);
            }

            var commandName = args[0];
            if (commandName.StartsWith("-", StringComparison.Ordinal))
            {
                return new DemoArguments(DemoCommand.Showcase, CreateOptions(args), ShowHelp: false, ExitCode: 0);
            }

            var command = ParseCommand(commandName, out var valid);
            if (!valid)
            {
                return new DemoArguments(DemoCommand.Showcase, CreateOptions(Array.Empty<string>()), ShowHelp: true, ExitCode: 1);
            }

            return new DemoArguments(command, CreateOptions(args.Skip(1).ToArray()), ShowHelp: false, ExitCode: 0);
        }

        private static DemoOptions CreateOptions(string[] args)
        {
            var options = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["host"] = DefaultHost,
                ["amcp-port"] = DefaultAmcpPort.ToString(),
                ["osc-port"] = DefaultOscPort.ToString(),
                ["channel"] = DefaultChannel.ToString(),
                ["layer"] = DefaultLayer.ToString(),
                ["clip"] = DefaultClip,
                ["timeout"] = DefaultTimeoutSeconds.ToString(),
                ["verbose-osc"] = bool.FalseString
            };

            for (var index = 0; index < args.Length; index++)
            {
                var token = args[index];
                if (IsHelp(token))
                {
                    return new DemoOptions(DefaultHost, DefaultAmcpPort, DefaultOscPort, DefaultChannel, DefaultLayer, DefaultClip, DefaultTimeoutSeconds, false);
                }

                if (!token.StartsWith("-", StringComparison.Ordinal))
                {
                    continue;
                }

                var (key, value, consumed) = ParseOption(args, index);
                if (key is null)
                {
                    continue;
                }

                if (string.Equals(key, "verbose-osc", StringComparison.OrdinalIgnoreCase))
                {
                    options[key] = bool.TrueString;
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(value))
                {
                    options[key] = value;
                }

                index += consumed;
            }

            return new DemoOptions(
                options["host"],
                ParseInt(options["amcp-port"], DefaultAmcpPort),
                ParseInt(options["osc-port"], DefaultOscPort),
                ParseInt(options["channel"], DefaultChannel),
                ParseInt(options["layer"], DefaultLayer),
                options["clip"],
                ParseInt(options["timeout"], DefaultTimeoutSeconds),
                bool.Parse(options["verbose-osc"]));
        }

        private static DemoCommand ParseCommand(string value, out bool valid)
        {
            valid = true;
            switch (value.ToLowerInvariant())
            {
                case "showcase":
                    return DemoCommand.Showcase;
                case "server":
                    return DemoCommand.Server;
                case "catalog":
                    return DemoCommand.Catalog;
                case "data":
                    return DemoCommand.Data;
                case "playback":
                    return DemoCommand.Playback;
                case "admin":
                    return DemoCommand.Admin;
                case "osc":
                    return DemoCommand.Osc;
                default:
                    valid = false;
                    return DemoCommand.Showcase;
            }
        }

        private static bool IsHelp(string value) =>
            string.Equals(value, "-h", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(value, "--help", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(value, "help", StringComparison.OrdinalIgnoreCase);

        private static (string? Key, string? Value, int Consumed) ParseOption(string[] args, int index)
        {
            var token = args[index];
            var raw = token.TrimStart('-');
            if (raw.Length == 0)
            {
                return (null, null, 0);
            }

            if (raw.Contains('='))
            {
                var parts = raw.Split('=', 2);
                return (NormalizeKey(parts[0]), parts[1], 0);
            }

            if (string.Equals(raw, "verbose-osc", StringComparison.OrdinalIgnoreCase))
            {
                return (NormalizeKey(raw), null, 0);
            }

            if (index + 1 >= args.Length)
            {
                return (NormalizeKey(raw), string.Empty, 0);
            }

            return (NormalizeKey(raw), args[index + 1], 1);
        }

        private static string NormalizeKey(string value) => value.TrimStart('-').ToLowerInvariant();

        private static int ParseInt(string value, int fallback) =>
            int.TryParse(value, out var parsed) ? parsed : fallback;
    }

    private static string PromptString(string key, string defaultValue) =>
        AnsiConsole.Prompt(
            new TextPrompt<string>($"[green]{Markup.Escape(key)}[/]")
                .DefaultValue(defaultValue)
                .DefaultValueStyle(new Style(Color.Green, decoration: Decoration.Italic)));

    private static int PromptInt(string key, int defaultValue) =>
        AnsiConsole.Prompt(
            new TextPrompt<int>($"[green]{Markup.Escape(key)}[/]")
                .DefaultValue(defaultValue)
                .DefaultValueStyle(new Style(Color.Green, decoration: Decoration.Italic)));
}
