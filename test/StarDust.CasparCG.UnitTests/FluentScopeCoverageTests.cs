using StarDust.CasparCG;
using StarDust.CasparCG.Osc;
using StarDust.CasparCG.Protocol.Amcp;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class FluentScopeCoverageTests
{
    [Fact]
    public async Task ServerScope_version_forwards_to_client_query()
    {
        var transport = new RecordingAmcpTransport("201 VERSION OK\r\n2.5.0\r\n");
        var client = new CasparClient(transport);

        var version = await client.Server().VersionAsync(CancellationToken.None);

        Assert.Equal("2.5.0", version);
        Assert.Equal("VERSION SERVER\r\n", transport.SentCommands.Single());
    }

    [Fact]
    public async Task ServerScope_query_methods_forward_to_client_queries()
    {
        var transport = new RecordingAmcpTransport(
            "200 CLS OK\r\n\"AMB\" MOVIE 42 20240101120000 240 1/25\r\n\r\n",
            "201 INFO OK\r\nserver-info\r\n",
            "201 INFO CONFIG OK\r\nconfig-info\r\n",
            "201 INFO PATHS OK\r\npaths-info\r\n",
            "202 DIAG OK\r\n");
        var client = new CasparClient(transport);

        var mediaFiles = await client.Server().MediaFilesAsync(CancellationToken.None);
        var info = await client.Server().InfoAsync(CancellationToken.None);
        var config = await client.Server().InfoConfigAsync(CancellationToken.None);
        var paths = await client.Server().InfoPathsAsync(CancellationToken.None);
        await client.Server().DiagAsync(CancellationToken.None);

        Assert.Equal("\"AMB\" MOVIE 42 20240101120000 240 1/25", mediaFiles.Single());
        Assert.Equal("server-info", info.Lines.Single());
        Assert.Equal("config-info", config.Lines.Single());
        Assert.Equal("paths-info", paths.Lines.Single());
        Assert.Equal(
            [
                "CLS\r\n",
                "INFO\r\n",
                "INFO CONFIG\r\n",
                "INFO PATHS\r\n",
                "DIAG\r\n"
            ],
            transport.SentCommands);
    }

    [Fact]
    public async Task AdminScope_restart_forwards_to_restart_command()
    {
        var transport = new RecordingAmcpTransport("202 RESTART OK\r\n");
        var client = new CasparClient(transport);

        await client.Admin().RestartAsync(CancellationToken.None);

        Assert.Equal("RESTART\r\n", transport.SentCommands.Single());
    }

    [Fact]
    public async Task AdminScope_dangerous_commands_are_kept_behind_admin_scope()
    {
        var transport = new RecordingAmcpTransport(
            "202 BYE OK\r\n",
            "202 KILL OK\r\n",
            "202 RESTART OK\r\n",
            "202 LOCK OK\r\n");
        var client = new CasparClient(transport);
        var admin = client.Admin();

        await admin.ByeAsync(CancellationToken.None);
        await admin.KillAsync(CancellationToken.None);
        await admin.RestartAsync(CancellationToken.None);
        await admin.LockAsync(CancellationToken.None);

        Assert.Equal(
            [
                "BYE\r\n",
                "KILL\r\n",
                "RESTART\r\n",
                "LOCK\r\n"
            ],
            transport.SentCommands);
    }

    [Fact]
    public async Task DataScope_methods_forward_to_data_commands()
    {
        var transport = new RecordingAmcpTransport(
            "202 DATA STORE OK\r\n",
            "201 DATA RETRIEVE OK\r\npayload\r\n",
            "201 DATA LIST OK\r\nitem-a\r\nitem-b\r\n",
            "202 DATA REMOVE OK\r\n");
        var client = new CasparClient(transport);
        var data = client.Data();

        var store = await data.StoreAsync("foo", "bar", CancellationToken.None);
        var retrieve = await data.RetrieveAsync("foo", CancellationToken.None);
        var list = await data.ListAsync("sub", CancellationToken.None);
        var remove = await data.RemoveAsync("foo", CancellationToken.None);

        Assert.Equal(202, store.StatusCode);
        Assert.Equal("payload", retrieve.Lines.Single());
        Assert.Equal(["item-a", "item-b"], list.Lines);
        Assert.Equal(202, remove.StatusCode);
        Assert.Equal(
            [
                "DATA STORE foo bar\r\n",
                "DATA RETRIEVE foo\r\n",
                "DATA LIST sub\r\n",
                "DATA REMOVE foo\r\n"
            ],
            transport.SentCommands);
    }

    [Fact]
    public async Task ThumbnailScope_methods_forward_to_thumbnail_commands()
    {
        var transport = new RecordingAmcpTransport(
            "201 THUMBNAIL LIST OK\r\nAMB\r\n",
            "201 THUMBNAIL RETRIEVE OK\r\nimage-data\r\n",
            "202 THUMBNAIL GENERATE OK\r\n",
            "202 THUMBNAIL GENERATE_ALL OK\r\n");
        var client = new CasparClient(transport);
        var thumbnails = client.Thumbnails();

        var list = await thumbnails.ListAsync("clips", CancellationToken.None);
        var retrieve = await thumbnails.RetrieveAsync("AMB", CancellationToken.None);
        await thumbnails.GenerateAsync("AMB", CancellationToken.None);
        await thumbnails.GenerateAllAsync(CancellationToken.None);

        Assert.Equal("AMB", list.Lines.Single());
        Assert.Equal("image-data", retrieve.Lines.Single());
        Assert.Equal(
            [
                "THUMBNAIL LIST clips\r\n",
                "THUMBNAIL RETRIEVE AMB\r\n",
                "THUMBNAIL GENERATE AMB\r\n",
                "THUMBNAIL GENERATE_ALL\r\n"
            ],
            transport.SentCommands);
    }

    [Fact]
    public async Task OscScope_methods_forward_to_osc_operations()
    {
        var amcpTransport = new RecordingAmcpTransport(
            "202 OSC SUBSCRIBE OK\r\n",
            "202 OSC UNSUBSCRIBE OK\r\n");
        var oscTransport = new RecordingOscTransport();
        var client = new CasparClient(amcpTransport, oscTransport, new NoOpOscMessageMapper());
        var osc = client.Osc();

        await osc.StartAsync(6250, CancellationToken.None);
        await osc.SubscribeAsync(6250, CancellationToken.None);
        await osc.UnsubscribeAsync(5253, CancellationToken.None);
        await osc.StopAsync(CancellationToken.None);

        Assert.Equal(6250, oscTransport.StartedPort);
        Assert.True(oscTransport.Stopped);
        Assert.Equal(
            [
                "OSC SUBSCRIBE 6250\r\n",
                "OSC UNSUBSCRIBE 5253\r\n"
            ],
            amcpTransport.SentCommands);
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

    private sealed class RecordingOscTransport : IOscTransport
    {
        public int? StartedPort { get; private set; }

        public bool Stopped { get; private set; }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        public ValueTask StartAsync(
            int port,
            Func<ReadOnlyMemory<byte>, CancellationToken, ValueTask> packetHandler,
            CancellationToken cancellationToken)
        {
            StartedPort = port;
            return ValueTask.CompletedTask;
        }

        public ValueTask StopAsync(CancellationToken cancellationToken)
        {
            Stopped = true;
            return ValueTask.CompletedTask;
        }
    }

    private sealed class NoOpOscMessageMapper : IOscMessageMapper
    {
        public bool TryMap(string address, IReadOnlyList<object?> arguments, out Events.CasparEvent? evt)
        {
            evt = null;
            return false;
        }
    }
}
