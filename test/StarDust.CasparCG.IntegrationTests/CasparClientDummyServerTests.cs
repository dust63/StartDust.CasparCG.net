using StarDust.CasparCG;
using Microsoft.Extensions.DependencyInjection;
using StarDust.CasparCG.Hosting;
using StarDust.CasparCG.Protocol.Amcp;
using StarDust.CasparCG.Query;
using StarDust.CasparCG.Testing.DummyServer;
using StarDust.CasparCG.Transport;
using Xunit;

namespace StarDust.CasparCG.IntegrationTests;

public class CasparClientDummyServerTests
{
    [Fact]
    public async Task Hosting_registration_uses_configured_amcp_endpoint()
    {
        await using var server = await DummyCasparServer.StartAsync(
            DummyScenario.Empty()
                .WithAmcpReply("VERSION SERVER", "201 VERSION OK\r\n2.5.0\r\n"),
            CancellationToken.None);

        var services = new ServiceCollection();
        services.AddCasparCG()
            .ConnectTo("127.0.0.1", server.AmcpPort);

        await using var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<CasparClient>();

        await client.ConnectAsync(CancellationToken.None);
        var version = await client.Server().VersionAsync(CancellationToken.None);

        Assert.Equal("2.5.0", version);
        Assert.Equal(["VERSION SERVER"], server.ReceivedCommands);
    }

    [Fact]
    public async Task Playback_scope_commands_are_sent_over_a_real_tcp_connection()
    {
        await using var server = await DummyCasparServer.StartAsync(
            DummyScenario.Empty()
                .WithAmcpReply("LOADBG 1-10 BG LOOP AUTO", "202 LOADBG OK\r\n")
                .WithAmcpReply("PLAY 1-10 AMB MIX 12 LOOP", "202 PLAY OK\r\n")
                .WithAmcpReply("PAUSE 1-10", "202 PAUSE OK\r\n")
                .WithAmcpReply("RESUME 1-10", "202 RESUME OK\r\n")
                .WithAmcpReply("STOP 1-10", "202 STOP OK\r\n"),
            CancellationToken.None);

        await using var transport = new TcpAmcpTransport("127.0.0.1", server.AmcpPort);
        var client = new CasparClient(transport);

        await client.ConnectAsync(CancellationToken.None);
        await client.Channel(1).Layer(10).LoadBg("BG").Loop().AutoPlay().SendAsync(CancellationToken.None);
        await client.Channel(1).Layer(10).Play("AMB").WithTransition(PlaybackTransition.Mix(12)).WithLoop().SendAsync(CancellationToken.None);
        await client.Channel(1).Layer(10).PauseAsync(CancellationToken.None);
        await client.Channel(1).Layer(10).ResumeAsync(CancellationToken.None);
        await client.Channel(1).Layer(10).StopAsync(CancellationToken.None);

        Assert.Equal(
            [
                "LOADBG 1-10 BG LOOP AUTO",
                "PLAY 1-10 AMB MIX 12 LOOP",
                "PAUSE 1-10",
                "RESUME 1-10",
                "STOP 1-10"
            ],
            server.ReceivedCommands);
    }

    [Fact]
    public async Task Data_scope_commands_round_trip_over_tcp()
    {
        await using var server = await DummyCasparServer.StartAsync(
            DummyScenario.Empty()
                .WithAmcpReply("DATA STORE foo bar", "202 DATA STORE OK\r\n")
                .WithAmcpReply("DATA RETRIEVE foo", "201 DATA RETRIEVE OK\r\npayload\r\n")
                .WithAmcpReply("DATA LIST clips", "200 DATA LIST OK\r\nitem-a\r\n\r\n")
                .WithAmcpReply("DATA REMOVE foo", "202 DATA REMOVE OK\r\n"),
            CancellationToken.None);

        await using var transport = new TcpAmcpTransport("127.0.0.1", server.AmcpPort);
        var client = new CasparClient(transport);

        await client.ConnectAsync(CancellationToken.None);
        var data = client.Data();
        var store = await data.StoreAsync("foo", "bar", CancellationToken.None);
        var retrieve = await data.RetrieveAsync("foo", CancellationToken.None);
        var list = await data.ListAsync("clips", CancellationToken.None);
        var remove = await data.RemoveAsync("foo", CancellationToken.None);

        Assert.Equal(202, store.StatusCode);
        Assert.Equal("payload", retrieve.Lines.Single());
        Assert.Equal(["item-a"], list.Lines);
        Assert.Equal(202, remove.StatusCode);
        Assert.Equal(
            [
                "DATA STORE foo bar",
                "DATA RETRIEVE foo",
                "DATA LIST clips",
                "DATA REMOVE foo"
            ],
            server.ReceivedCommands);
    }

    [Fact]
    public async Task Query_scope_commands_parse_structured_payloads_over_tcp()
    {
        await using var server = await DummyCasparServer.StartAsync(
            DummyScenario.Empty()
                .WithAmcpReply("VERSION SERVER", "201 VERSION OK\r\n2.5.0\r\n")
                .WithAmcpReply("INFO", "200 INFO OK\r\nserver-name: dummy\r\nchannel-count: 1\r\n\r\n")
                .WithAmcpReply("INFO CONFIG", "200 INFO CONFIG OK\r\n<config><log-level>debug</log-level><channels><count>1</count></channels></config>\r\n\r\n")
                .WithAmcpReply("INFO PATHS", "200 INFO PATHS OK\r\n<paths><media-path>media/</media-path><template-path>templates/</template-path></paths>\r\n\r\n")
                .WithAmcpReply("GL INFO", "201 GL INFO OK\r\n<gl><renderer>opengl</renderer><vendor>casparcg</vendor></gl>\r\n"),
            CancellationToken.None);

        await using var transport = new TcpAmcpTransport("127.0.0.1", server.AmcpPort);
        var client = new CasparClient(transport);

        await client.ConnectAsync(CancellationToken.None);
        var version = await client.Server().VersionAsync(CancellationToken.None);
        var info = await client.Server().InfoAsync(CancellationToken.None);
        var config = await client.Server().InfoConfigAsync(CancellationToken.None);
        var paths = await client.Server().InfoPathsAsync(CancellationToken.None);
        var glInfo = await client.Server().GlInfoAsync(CancellationToken.None);

        Assert.Equal("2.5.0", version);
        Assert.Equal("dummy", info.Values["server-name"]);
        Assert.Equal("1", info.Values["channel-count"]);
        Assert.Equal("debug", config.Values["config.log-level"]);
        Assert.Equal("1", config.Values["config.channels.count"]);
        Assert.Equal("media/", paths.Values["paths.media-path"]);
        Assert.Equal("templates/", paths.Values["paths.template-path"]);
        Assert.Equal("opengl", glInfo.Values["gl.renderer"]);
        Assert.Equal("casparcg", glInfo.Values["gl.vendor"]);
        Assert.Equal(
            [
                "VERSION SERVER",
                "INFO",
                "INFO CONFIG",
                "INFO PATHS",
                "GL INFO"
            ],
            server.ReceivedCommands);
    }

    [Fact]
    public async Task Catalog_queries_parse_realistic_payloads_over_tcp()
    {
        await using var server = await DummyCasparServer.StartAsync(
            DummyScenario.Empty()
                .WithAmcpReply(
                    "CLS",
                    "200 CLS OK\r\n\"AMB\" MOVIE 42 20240101120000 240 1/25\r\n\"PROMO\" MOVIE 13 20240101120001 120 1/25\r\n\r\n")
                .WithAmcpReply(
                    "CINF AMB",
                    "200 CINF OK\r\n\"AMB\" MOVIE 42 20240101120000 240 1/25 FIELD_A VALUE_A\r\n\r\n")
                .WithAmcpReply(
                    "FLS",
                    "200 FLS OK\r\n\"Roboto\" \"fonts/Roboto Regular.ttf\"\r\n\r\n")
                .WithAmcpReply(
                    "TLS",
                    "200 TLS OK\r\nLOWERTHIRD\r\nFULLFRAME\r\n\r\n"),
            CancellationToken.None);

        await using var transport = new TcpAmcpTransport("127.0.0.1", server.AmcpPort);
        var client = new CasparClient(transport);

        await client.ConnectAsync(CancellationToken.None);
        var mediaFiles = await client.Server().MediaFilesAsync(CancellationToken.None);
        var mediaInfo = await client.Server().MediaInfoAsync("AMB", CancellationToken.None);
        var fontFiles = await client.Server().FontFilesAsync(CancellationToken.None);
        var templateFiles = await client.Server().TemplateFilesAsync(CancellationToken.None);

        Assert.Equal(2, mediaFiles.Count);
        Assert.Equal("AMB", mediaFiles[0].Name);
        Assert.Equal(MediaFileKind.Movie, mediaFiles[0].Kind);
        Assert.Equal("PROMO", mediaFiles[1].Name);
        Assert.Equal("AMB", mediaInfo.Name);
        Assert.Equal("VALUE_A", mediaInfo.Properties["FIELD_A"]);
        Assert.Equal("Roboto", fontFiles.Single().Name);
        Assert.Equal("fonts/Roboto Regular.ttf", fontFiles.Single().Path);
        Assert.Equal(["LOWERTHIRD", "FULLFRAME"], templateFiles.Select(x => x.Name).ToArray());
        Assert.Equal(
            [
                "CLS",
                "CINF AMB",
                "FLS",
                "TLS"
            ],
            server.ReceivedCommands);
    }

    [Fact]
    public async Task Admin_scope_commands_cover_log_and_lock_variants_over_tcp()
    {
        await using var server = await DummyCasparServer.StartAsync(
            DummyScenario.Empty()
                .WithAmcpReply("DIAG", "202 DIAG OK\r\n")
                .WithAmcpReply("BYE", "202 BYE OK\r\n")
                .WithAmcpReply("KILL", "202 KILL OK\r\n")
                .WithAmcpReply("RESTART", "202 RESTART OK\r\n")
                .WithAmcpReply("LOG LEVEL", "201 LOG OK\r\nINFO\r\n")
                .WithAmcpReply("LOG LEVEL debug", "202 LOG OK\r\n")
                .WithAmcpReply("LOCK 1 ACQUIRE phrase", "202 LOCK ACQUIRE OK\r\n")
                .WithAmcpReply("LOCK 1 RELEASE", "202 LOCK RELEASE OK\r\n")
                .WithAmcpReply("LOCK 1 CLEAR", "202 LOCK CLEAR OK\r\n")
                .WithAmcpReply("LOCK 1 CLEAR override", "202 LOCK CLEAR OK\r\n"),
            CancellationToken.None);

        await using var transport = new TcpAmcpTransport("127.0.0.1", server.AmcpPort);
        var client = new CasparClient(transport);

        await client.ConnectAsync(CancellationToken.None);
        var admin = client.Admin();

        await client.DiagAsync(CancellationToken.None);
        await admin.ByeAsync(CancellationToken.None);
        await admin.KillAsync(CancellationToken.None);
        await admin.RestartAsync(CancellationToken.None);
        var currentLogLevel = await admin.GetLogLevelAsync(CancellationToken.None);
        await admin.SetLogLevelAsync("debug", CancellationToken.None);
        await admin.AcquireLockAsync(1, "phrase", CancellationToken.None);
        await admin.ReleaseLockAsync(1, CancellationToken.None);
        await admin.ClearLockAsync(1, cancellationToken: CancellationToken.None);
        await admin.ClearLockAsync(1, "override", CancellationToken.None);

        Assert.Equal("INFO", currentLogLevel);
        Assert.Equal(
            [
                "DIAG",
                "BYE",
                "KILL",
                "RESTART",
                "LOG LEVEL",
                "LOG LEVEL debug",
                "LOCK 1 ACQUIRE phrase",
                "LOCK 1 RELEASE",
                "LOCK 1 CLEAR",
                "LOCK 1 CLEAR override"
            ],
            server.ReceivedCommands);
    }

    [Fact]
    public async Task Non_success_reply_raises_amcp_command_exception()
    {
        await using var server = await DummyCasparServer.StartAsync(
            DummyScenario.Empty().WithAmcpReply("PLAY 1-10 FAIL", "404 PLAY FAILED\r\n"),
            CancellationToken.None);

        await using var transport = new TcpAmcpTransport("127.0.0.1", server.AmcpPort);
        var client = new CasparClient(transport);

        await client.ConnectAsync(CancellationToken.None);

        var exception = await Assert.ThrowsAsync<AmcpCommandException>(
            () => client.PlayAsync(1, 10, "FAIL", CancellationToken.None).AsTask());

        Assert.Equal(404, exception.Response.StatusCode);
        Assert.Equal("PLAY FAILED", exception.Response.CommandText);
        Assert.Equal(["PLAY 1-10 FAIL"], server.ReceivedCommands);
    }
}
