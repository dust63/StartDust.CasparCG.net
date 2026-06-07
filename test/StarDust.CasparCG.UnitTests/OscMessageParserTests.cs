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
    public void Map_background_file_name_to_raw_state_and_playback_clip_changed()
    {
        var packet = BuildOscPacket(
            "/channel/1/stage/layer/10/background/file/name",
            "TEST/GO1080P25");

        var message = OscPacketParser.Parse(packet);
        var mapper = new DefaultOscMessageMapper();

        var events = mapper.Map(message.Address, message.Arguments);

        Assert.Equal(2, events.Count);
        var raw = Assert.IsType<OscStateChangedEvent>(events[0]);
        Assert.Equal("default", raw.ClientName);
        Assert.Equal(1, raw.Channel);
        Assert.Equal("stage/layer/10/background/file/name", raw.Path);
        Assert.Equal("TEST/GO1080P25", raw.Arguments[0]);
        var clipChanged = Assert.IsType<PlaybackClipChangedEvent>(events[1]);
        Assert.Equal("default", clipChanged.ClientName);
        Assert.Equal(1, clipChanged.Channel);
        Assert.Equal(10, clipChanged.Layer);
        Assert.Equal("TEST/GO1080P25", clipChanged.Clip);
    }

    [Fact]
    public void Map_foreground_file_name_to_raw_state_and_playback_clip_changed()
    {
        var packet = BuildOscPacket(
            "/channel/1/stage/layer/10/foreground/file/name",
            "AMB");

        var message = OscPacketParser.Parse(packet);
        var mapper = new DefaultOscMessageMapper();

        var events = mapper.Map(message.Address, message.Arguments);

        Assert.Equal(2, events.Count);
        var raw = Assert.IsType<OscStateChangedEvent>(events[0]);
        Assert.Equal("stage/layer/10/foreground/file/name", raw.Path);
        Assert.Equal("AMB", raw.Arguments[0]);
        var clipChanged = Assert.IsType<PlaybackClipChangedEvent>(events[1]);
        Assert.Equal("default", clipChanged.ClientName);
        Assert.Equal(1, clipChanged.Channel);
        Assert.Equal(10, clipChanged.Layer);
        Assert.Equal("AMB", clipChanged.Clip);
    }

    [Fact]
    public void Map_foreground_producer_to_raw_state_and_layer_producer_changed()
    {
        var packet = BuildOscPacket(
            "/channel/1/stage/layer/10/foreground/producer",
            "ffmpeg");

        var message = OscPacketParser.Parse(packet);
        var mapper = new DefaultOscMessageMapper("studio-a");

        var events = mapper.Map(message.Address, message.Arguments);

        Assert.Equal(2, events.Count);
        var raw = Assert.IsType<OscStateChangedEvent>(events[0]);
        Assert.Equal("studio-a", raw.ClientName);
        Assert.Equal(1, raw.Channel);
        Assert.Equal("stage/layer/10/foreground/producer", raw.Path);
        var producerChanged = Assert.IsType<LayerProducerChangedEvent>(events[1]);
        Assert.Equal("studio-a", producerChanged.ClientName);
        Assert.Equal(1, producerChanged.Channel);
        Assert.Equal(10, producerChanged.Layer);
        Assert.Equal("foreground", producerChanged.Slot);
        Assert.Equal("ffmpeg", producerChanged.Producer);
    }

    [Fact]
    public void Map_foreground_paused_to_raw_state_and_layer_paused_changed()
    {
        var packet = BuildOscPacket(
            "/channel/1/stage/layer/10/foreground/paused",
            ",T",
            _ => { });

        var message = OscPacketParser.Parse(packet);
        var mapper = new DefaultOscMessageMapper();

        var events = mapper.Map(message.Address, message.Arguments);

        Assert.Equal(2, events.Count);
        Assert.IsType<OscStateChangedEvent>(events[0]);
        var pausedChanged = Assert.IsType<LayerPausedChangedEvent>(events[1]);
        Assert.True(pausedChanged.Paused);
        Assert.Equal(10, pausedChanged.Layer);
    }

    [Fact]
    public void Map_file_time_to_raw_state_and_layer_progress_changed()
    {
        var packet = BuildOscPacket(
            "/channel/1/stage/layer/10/foreground/file/time",
            ",dd",
            bytes =>
            {
                WriteDoubleBigEndian(bytes, 12.5d);
                WriteDoubleBigEndian(bytes, 30d);
            });

        var message = OscPacketParser.Parse(packet);
        var mapper = new DefaultOscMessageMapper();

        var events = mapper.Map(message.Address, message.Arguments);

        Assert.Equal(2, events.Count);
        Assert.IsType<OscStateChangedEvent>(events[0]);
        var progressChanged = Assert.IsType<LayerProgressChangedEvent>(events[1]);
        Assert.Equal("foreground", progressChanged.Slot);
        Assert.Equal(12.5d, progressChanged.PositionSeconds);
        Assert.Equal(30d, progressChanged.DurationSeconds);
    }

    [Fact]
    public void Map_foreground_frames_left_to_raw_state_and_layer_frames_left_changed()
    {
        var packet = BuildOscPacket(
            "/channel/1/stage/layer/10/foreground/frames_left",
            ",h",
            bytes => WriteInt64BigEndian(bytes, 42));

        var message = OscPacketParser.Parse(packet);
        var mapper = new DefaultOscMessageMapper();

        var events = mapper.Map(message.Address, message.Arguments);

        Assert.Equal(2, events.Count);
        Assert.IsType<OscStateChangedEvent>(events[0]);
        var framesLeftChanged = Assert.IsType<LayerFramesLeftChangedEvent>(events[1]);
        Assert.Equal(42, framesLeftChanged.FramesLeft);
        Assert.Equal(10, framesLeftChanged.Layer);
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

    [Fact]
    public void Parse_messages_skips_unsupported_bundle_message_and_keeps_supported_messages()
    {
        var bundle = BuildOscBundlePacket(
            BuildOscPacket(
                "/channel/1/unsupported",
                ",c",
                bytes => WriteInt32BigEndian(bytes, 65)),
            BuildOscPacket("/channel/1/stage/layer/10/foreground/file/name", "MEDIA/FOO"));

        var messages = OscPacketParser.ParseMessages(bundle);

        var message = Assert.Single(messages);
        Assert.Equal("/channel/1/stage/layer/10/foreground/file/name", message.Address);
        Assert.Equal("MEDIA/FOO", message.Arguments[0]);
    }

    [Fact]
    public void Parse_message_decodes_int64_argument()
    {
        var packet = BuildOscPacket(
            "/channel/1/stage/layer/10/foreground/frames_left",
            ",h",
            bytes => WriteInt64BigEndian(bytes, 1234567890123L));

        var message = OscPacketParser.Parse(packet);

        Assert.Single(message.Arguments);
        var value = Assert.IsType<long>(message.Arguments[0]);
        Assert.Equal(1234567890123L, value);
    }

    [Fact]
    public void Parse_message_decodes_double_argument()
    {
        var packet = BuildOscPacket(
            "/channel/1/stage/layer/10/foreground/file/time",
            ",d",
            bytes => WriteDoubleBigEndian(bytes, 12.5d));

        var message = OscPacketParser.Parse(packet);

        Assert.Single(message.Arguments);
        var value = Assert.IsType<double>(message.Arguments[0]);
        Assert.Equal(12.5d, value);
    }

    [Fact]
    public void Osc_namespace_remains_available_after_merge()
    {
        var packets = StarDust.CasparCG.Protocol.Osc.OscPacketParser.ParseMessages(BuildOscPacket("/test", "ok"));

        Assert.Single(packets);
    }

    private static byte[] BuildOscPacket(string address, string stringArgument)
    {
        var bytes = new List<byte>();
        WriteOscString(bytes, address);
        WriteOscString(bytes, ",s");
        WriteOscString(bytes, stringArgument);
        return bytes.ToArray();
    }

    private static byte[] BuildOscPacket(string address, string typeTags, Action<ICollection<byte>> writeArguments)
    {
        var bytes = new List<byte>();
        WriteOscString(bytes, address);
        WriteOscString(bytes, typeTags);
        writeArguments(bytes);
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

    private static void WriteInt64BigEndian(ICollection<byte> bytes, long value)
    {
        for (var shift = 56; shift >= 0; shift -= 8)
        {
            bytes.Add((byte)((value >> shift) & 0xFF));
        }
    }

    private static void WriteDoubleBigEndian(ICollection<byte> bytes, double value)
    {
        WriteInt64BigEndian(bytes, BitConverter.DoubleToInt64Bits(value));
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
