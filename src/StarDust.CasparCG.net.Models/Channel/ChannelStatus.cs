using System;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.net.Models
{
    /// <summary>
    /// Indicate if the channel is playing or not
    /// </summary>
    [Serializable]
    public enum ChannelStatus
    {
        /// <summary>
        /// We can't get any value from CasparCG
        /// </summary>
        [EnumMember]
        Unknown,

        /// <summary>
        /// The channel is currently playing something
        /// </summary>
        [EnumMember]
        [AMCPCommandValue("PLAYING")]
        Playing,

        /// <summary>
        /// The channel is stopped
        /// </summary>
        [EnumMember]
        [AMCPCommandValue("STOPPED")]
        Stopped,
    }
}
