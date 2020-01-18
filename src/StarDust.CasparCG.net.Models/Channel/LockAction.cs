using System;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.net.Models
{
    /// <summary>
    /// Lock type. See LOCK part on <see href="https://github.com/CasparCG/help/wiki/AMCP-Protocol"/>
    /// </summary>
    [Serializable]
    public enum LockAction
    {
        /// <summary>
        /// Acquire
        /// </summary>
        [EnumMember]
        ACQUIRE,

        /// <summary>
        /// Release the lock
        /// </summary>
        [EnumMember]
        RELEASE,

        /// <summary>
        /// Clear the lock
        /// </summary>
        [EnumMember]
        CLEAR
    }
}
