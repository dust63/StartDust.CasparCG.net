using System;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.net.Models
{
    [Serializable]
    public enum LockAction
    {
        [EnumMember]
        ACQUIRE,

        [EnumMember]
        RELEASE,

        [EnumMember]
        CLEAR
    }
}
