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
        var offset = 0;
        var address = ReadOscString(packet, ref offset);
        var typeTag = ReadOscString(packet, ref offset);

        if (typeTag.Length == 0 || typeTag[0] != ',')
        {
            throw new FormatException("The OSC packet does not contain a valid type tag string.");
        }

        var arguments = ParseArguments(packet, ref offset, typeTag.AsSpan(1));
        return new OscMessage(address, arguments);
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
                'f' => ReadSingle(packet, ref offset),
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

    private static int AlignToFour(int length) => (length + 3) & ~3;
}
