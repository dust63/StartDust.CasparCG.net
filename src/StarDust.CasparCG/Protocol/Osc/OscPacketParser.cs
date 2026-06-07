using System.Buffers.Binary;
using System.Text;

namespace StarDust.CasparCG.Protocol.Osc;

/// <summary>
/// Parses OSC datagrams into typed messages.
/// </summary>
public static class OscPacketParser
{
    /// <summary>
    /// Parses a single OSC message packet.
    /// </summary>
    /// <param name="packet">The OSC datagram bytes.</param>
    /// <returns>The parsed OSC message.</returns>
    public static OscMessage Parse(ReadOnlySpan<byte> packet)
    {
        var messages = ParseMessages(packet);
        if (messages.Count != 1)
        {
            throw new FormatException("The OSC packet is not a single message.");
        }

        return messages[0];
    }

    /// <summary>
    /// Parses an OSC packet or bundle into a flat list of OSC messages.
    /// </summary>
    /// <param name="packet">The OSC datagram bytes.</param>
    /// <returns>The parsed OSC messages.</returns>
    public static IReadOnlyList<OscMessage> ParseMessages(ReadOnlySpan<byte> packet)
    {
        var messages = new List<OscMessage>();
        ParsePacket(packet, messages);
        return messages;
    }

    private static void ParsePacket(ReadOnlySpan<byte> packet, ICollection<OscMessage> messages)
    {
        var offset = 0;
        var header = ReadOscString(packet, ref offset);

        if (header == "#bundle")
        {
            ParseBundle(packet, ref offset, messages);
            return;
        }

        var typeTag = ReadOscString(packet, ref offset);
        if (typeTag.Length == 0 || typeTag[0] != ',')
        {
            throw new FormatException("The OSC packet does not contain a valid type tag string.");
        }

        var arguments = ParseArguments(packet, ref offset, typeTag.AsSpan(1));
        messages.Add(new OscMessage(header, arguments));
    }

    private static void ParseBundle(ReadOnlySpan<byte> packet, ref int offset, ICollection<OscMessage> messages)
    {
        if (packet.Length - offset < 8)
        {
            throw new FormatException("The OSC bundle does not contain a timetag.");
        }

        offset += 8;

        while (offset < packet.Length)
        {
            if (packet.Length - offset < sizeof(int))
            {
                throw new FormatException("The OSC bundle does not contain a valid element size.");
            }

            var size = BinaryPrimitives.ReadInt32BigEndian(packet[offset..]);
            offset += sizeof(int);

            if (size < 0 || packet.Length - offset < size)
            {
                throw new FormatException("The OSC bundle contains an invalid element payload.");
            }

            try
            {
                ParsePacket(packet.Slice(offset, size), messages);
            }
            catch (NotSupportedException)
            {
            }

            offset += size;
        }
    }

    private static IReadOnlyList<object?> ParseArguments(ReadOnlySpan<byte> packet, ref int offset, ReadOnlySpan<char> typeTags)
    {
        var values = new object?[typeTags.Length];
        for (var i = 0; i < typeTags.Length; i++)
        {
            values[i] = typeTags[i] switch
            {
                's' => ReadOscString(packet, ref offset),
                'i' => ReadInt32(packet, ref offset),
                'h' => ReadInt64(packet, ref offset),
                'f' => ReadSingle(packet, ref offset),
                'd' => ReadDouble(packet, ref offset),
                'T' => true,
                'F' => false,
                _ => throw new NotSupportedException($"The OSC type tag '{typeTags[i]}' is not supported.")
            };
        }

        return Array.AsReadOnly(values);
    }

    private static string ReadOscString(ReadOnlySpan<byte> packet, ref int offset)
    {
        var slice = packet[offset..];
        var terminator = slice.IndexOf((byte)0);
        if (terminator < 0)
        {
            throw new FormatException("The OSC packet contains an unterminated string.");
        }

        var value = Encoding.UTF8.GetString(slice[..terminator]);
        offset += AlignToFour(terminator + 1);
        return value;
    }

    private static int ReadInt32(ReadOnlySpan<byte> packet, ref int offset)
    {
        if (packet.Length - offset < sizeof(int))
        {
            throw new FormatException("The OSC packet does not contain enough bytes for an int32.");
        }

        var value = BinaryPrimitives.ReadInt32BigEndian(packet[offset..]);
        offset += sizeof(int);
        return value;
    }

    private static float ReadSingle(ReadOnlySpan<byte> packet, ref int offset)
    {
        if (packet.Length - offset < sizeof(int))
        {
            throw new FormatException("The OSC packet does not contain enough bytes for a float32.");
        }

        var bits = BinaryPrimitives.ReadInt32BigEndian(packet[offset..]);
        offset += sizeof(int);
        return BitConverter.Int32BitsToSingle(bits);
    }

    private static long ReadInt64(ReadOnlySpan<byte> packet, ref int offset)
    {
        if (packet.Length - offset < sizeof(long))
        {
            throw new FormatException("The OSC packet does not contain enough bytes for an int64.");
        }

        var value = BinaryPrimitives.ReadInt64BigEndian(packet[offset..]);
        offset += sizeof(long);
        return value;
    }

    private static double ReadDouble(ReadOnlySpan<byte> packet, ref int offset)
    {
        if (packet.Length - offset < sizeof(long))
        {
            throw new FormatException("The OSC packet does not contain enough bytes for a float64.");
        }

        var bits = BinaryPrimitives.ReadInt64BigEndian(packet[offset..]);
        offset += sizeof(long);
        return BitConverter.Int64BitsToDouble(bits);
    }

    private static int AlignToFour(int length) => (length + 3) & ~3;
}
