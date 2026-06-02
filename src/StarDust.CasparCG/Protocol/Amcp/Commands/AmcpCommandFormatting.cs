namespace StarDust.CasparCG.Protocol.Amcp.Commands;

internal static class AmcpCommandFormatting
{
    internal static string Channel(int channel) => $"{channel}";

    internal static string ChannelLayer(int channel, int layer) => $"{channel}-{layer}";
}
