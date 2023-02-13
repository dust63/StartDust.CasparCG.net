using System.Net.Sockets;

namespace StarDust.CasparCG.net.RestApi.Services;

/// <summary>
/// Used to check connectivity of CasparCG Server
/// </summary>
public class CasparCGConnectivityControl
{
    public static bool CheckAmqpResponse(string hostname, int port = 5250)
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