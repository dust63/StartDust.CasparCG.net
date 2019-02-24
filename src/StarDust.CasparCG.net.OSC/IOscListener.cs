using System;
using System.Threading.Tasks;

namespace StarDust.CasparCG.net.OSC
{
    public interface IOscListener
    {
        string OscServerIp { get; }
        int OscServerPort { get; }

        event EventHandler<OscMessageEventArgs> OscMessageReceived;

        Task StartListening(string ipAddress, int port);
        void StopListening();
    }
}