using System.Net.Sockets;

namespace StarDust.CasparCG.net.RestApi.Services;

/// <summary>
/// Used to check connectivity of CasparCG Server
/// </summary>
public class CasparCGConnectivityChecker
{
    /// <summary>
    /// Check if CasparCG is running and listening for AMPQ port
    /// </summary>
    /// <param name="hostname"></param>
    /// <param name="port"></param>
    /// <returns></returns>
    public static bool IsAmqpListening(string hostname, int port = 5250)
    {
        try
        {
            using var client = new TcpClient(hostname, port);
            return true;
        }
        catch (SocketException)
        {
            return false;
        }
    }
}