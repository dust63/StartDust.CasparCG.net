
using System;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.net.Models
{
    [Serializable]
    public enum TransitionType
    {
        [AMCPCommandValue("CUT")]
        [DataMember]
        CUT,
        [AMCPCommandValue("MIX")]
        [DataMember]
        MIX,
        [AMCPCommandValue("PUSH")]
        [DataMember]
        PUSH,
        [AMCPCommandValue("SLIDE")]
        [DataMember]
        SLIDE,
        [AMCPCommandValue("WIPE")]
        [DataMember]
        WIPE,
    }
}
