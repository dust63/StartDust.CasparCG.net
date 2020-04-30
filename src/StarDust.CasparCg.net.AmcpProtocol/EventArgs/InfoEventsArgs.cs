using StarDust.CasparCG.net.Models.Info;
using System;
using System.Collections.Generic;

namespace StarDust.CasparCG.net.AmcpProtocol
{
    /// <summary>
    /// Channels Info received from the server
    /// </summary>
    public class InfoEventArgs : EventArgs
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="channelsInfos"></param>
        public InfoEventArgs(List<ChannelInfo> channelsInfos)
        {
            this.ChannelsInfo = channelsInfos ?? new List<ChannelInfo>();
        }

        /// <summary>
        /// List of channel info
        /// </summary>
        public List<ChannelInfo> ChannelsInfo { get; }


    }
}
