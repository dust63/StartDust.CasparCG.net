
using System;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.net.Models
{
    /// <summary>
    /// Transition directions
    /// </summary>
    [Serializable]
    public enum TransitionDirection
    {
        /// <summary>
        /// Right to Left
        /// </summary>
        [AMCPCommandValue("LEFT")]
        [DataMember]
        LEFT,

        /// <summary>
        /// Left to Right
        /// </summary>
        [AMCPCommandValue("RIGHT")]
        [DataMember]
        RIGHT
    }
}
