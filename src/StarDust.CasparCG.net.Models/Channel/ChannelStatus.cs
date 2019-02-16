using System;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.net.Models
{
    [Serializable]
    public enum ChannelStatus
    {
        [EnumMember]
        Unknown,
        [EnumMember]
        Playing,
        [EnumMember]
        Stopped,
    }
}
