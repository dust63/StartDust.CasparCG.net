using System;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.net.Models.Media
{
    /// <summary>
    /// Media type
    /// </summary>
    [Serializable]
    public enum MediaType
    {
        /// <summary>
        /// Still (fixed image)
        /// </summary>
        [DataMember]
        STILL,

        /// <summary>
        /// Movie (video file)
        /// </summary>
        [DataMember]
        MOVIE,

        /// <summary>
        /// Audio file
        /// </summary>
        [DataMember]
        AUDIO,
    }
}
