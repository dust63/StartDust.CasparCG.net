using System;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.Models
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
