
using System;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.net.Models
{
    /// <summary>
    /// Kind of transitions
    /// </summary>
    [Serializable]
    public enum TransitionType
    {
        /// <summary>
        /// Cut transition
        /// </summary>
        [AMCPCommandValue("CUT")]
        [DataMember]
        CUT,

        /// <summary>
        /// Mix transition
        /// </summary>
        [AMCPCommandValue("MIX")]
        [DataMember]
        MIX,

        /// <summary>
        /// Push transition
        /// </summary>
        [AMCPCommandValue("PUSH")]
        [DataMember]
        PUSH,

        /// <summary>
        /// Slide transition
        /// </summary>
        [AMCPCommandValue("SLIDE")]
        [DataMember]
        SLIDE,

        /// <summary>
        /// Wipe transition
        /// </summary>
        [AMCPCommandValue("WIPE")]
        [DataMember]
        WIPE,
    }
}
