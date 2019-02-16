
using System;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.net.Models
{
    [Serializable]
    public enum TransitionDirection
    {
        [AMCPCommandValue("LEFT")]
        [DataMember]
        LEFT,
        [AMCPCommandValue("RIGHT")]
        [DataMember]
        RIGHT
    }
}
