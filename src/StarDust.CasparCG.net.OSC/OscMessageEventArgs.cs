using Rug.Osc;
using System;

namespace StarDust.CasparCG.net.OSC
{
    public class OscMessageEventArgs : EventArgs
    {

        public OscPacket OscPacket { get; }

        public OscMessageEventArgs(OscPacket oscPacket)
        {
            OscPacket = oscPacket;
        }

    }
}