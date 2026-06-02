using StarDust.CasparCG.Events;
using StarDust.CasparCG.Osc;
using StarDust.CasparCG.Protocol.Osc;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class OscMessageParserTests
{
    [Fact]
    public void Parse_message_decodes_address_and_string_argument()
    {
        var packet = BuildOscPacket(
            "/channel/1/stage/layer/10/background/file/name",
            "TEST/GO1080P25");

        var message = OscPacketParser.Parse(packet);

        Assert.Equal("/channel/1/stage/layer/10/background/file/name", message.Address);
        Assert.Single(message.Arguments);
        Assert.Equal("TEST/GO1080P25", message.Arguments[0]);
    }

    [Fact]
    public void TryMap_background_file_name_to_playback_clip_changed()
    {
        var packet = BuildOscPacket(
            "/channel/1/stage/layer/10/background/file/name",
            "TEST/GO1080P25");

        var message = OscPacketParser.Parse(packet);
        var mapper = new DefaultOscMessageMapper();

        var mapped = mapper.TryMap(message.Address, message.Arguments, out var evt);

        Assert.True(mapped);
        var clipChanged = Assert.IsType<PlaybackClipChangedEvent>(evt);
        Assert.Equal("default", clipChanged.ClientName);
        Assert.Equal(1, clipChanged.Channel);
        Assert.Equal(10, clipChanged.Layer);
        Assert.Equal("TEST/GO1080P25", clipChanged.Clip);
    }

    [Fact]
    public void TryMap_foreground_file_name_to_playback_clip_changed()
    {
        var packet = BuildOscPacket(
            "/channel/1/stage/layer/10/foreground/file/name",
            "AMB");

        var message = OscPacketParser.Parse(packet);
        var mapper = new DefaultOscMessageMapper();

        var mapped = mapper.TryMap(message.Address, message.Arguments, out var evt);

        Assert.True(mapped);
        var clipChanged = Assert.IsType<PlaybackClipChangedEvent>(evt);
        Assert.Equal("default", clipChanged.ClientName);
        Assert.Equal(1, clipChanged.Channel);
        Assert.Equal(10, clipChanged.Layer);
        Assert.Equal("AMB", clipChanged.Clip);
    }

    [Fact]
    public void Parse_messages_decodes_osc_bundle()
    {
        var bundle = BuildOscBundlePacket(
            BuildOscPacket("/channel/1/stage/layer/10/background/file/name", "AMB"),
            BuildOscPacket("/channel/1/stage/layer/10/foreground/file/name", "MEDIA/FOO"));

        var messages = OscPacketParser.ParseMessages(bundle);

        Assert.Equal(2, messages.Count);
        Assert.Equal("/channel/1/stage/layer/10/background/file/name", messages[0].Address);
        Assert.Equal("AMB", messages[0].Arguments[0]);
        Assert.Equal("/channel/1/stage/layer/10/foreground/file/name", messages[1].Address);
        Assert.Equal("MEDIA/FOO", messages[1].Arguments[0]);
    }

    private static byte[] BuildOscPacket(string address, string stringArgument)
    {
        var bytes = new List<byte>();
        WriteOscString(bytes, address);
        WriteOscString(bytes, ",s");
        WriteOscString(bytes, stringArgument);
        return bytes.ToArray();
    }

    private static byte[] BuildOscBundlePacket(params byte[][] packets)
    {
        var bytes = new List<byte>();
        WriteOscString(bytes, "#bundle");
        bytes.AddRange(new byte[8]);

        foreach (var packet in packets)
        {
            WriteInt32BigEndian(bytes, packet.Length);
            bytes.AddRange(packet);
        }

        return bytes.ToArray();
    }

    private static void WriteInt32BigEndian(ICollection<byte> bytes, int value)
    {
        bytes.Add((byte)((value >> 24) & 0xFF));
        bytes.Add((byte)((value >> 16) & 0xFF));
        bytes.Add((byte)((value >> 8) & 0xFF));
        bytes.Add((byte)(value & 0xFF));
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
