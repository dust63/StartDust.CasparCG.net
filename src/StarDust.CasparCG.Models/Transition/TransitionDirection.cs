
using System;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.Models
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
