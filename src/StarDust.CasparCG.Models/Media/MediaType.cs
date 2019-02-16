using System;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.Models.Media
{
    [Serializable]
    public enum MediaType
    {

        [DataMember]
        STILL,

        [DataMember]
        MOVIE,

        [DataMember]
        AUDIO,
    }
}
