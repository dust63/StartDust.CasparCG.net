using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarDust.CasparCG.net.OSC.EventHub.Events
{
    public abstract class StageEventArgs : ChannelEventArgs
    {
        
        public ushort LayerId { get; protected set; }
    }
}
