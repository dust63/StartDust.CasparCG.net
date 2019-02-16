using StarDust.CasparCG.Models.Info;
using System;
using System.Collections.Generic;

namespace StarDust.CasparCG.AmcpProtocol
{
    public class InfoEventArgs : EventArgs
    {
        public InfoEventArgs(List<ChannelInfo> channelsinfos)
        {
            this.ChannelsInfo = channelsinfos;
        }

        public List<ChannelInfo> ChannelsInfo { get; }


    }
}
