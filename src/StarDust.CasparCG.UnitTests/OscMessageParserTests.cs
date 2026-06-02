using StarDust.CasparCG.Events;
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
