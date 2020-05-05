using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public abstract class ChannelEventArgs : EventArgs
    {

        public ushort ChannelId { get;protected set; }

    }
}
