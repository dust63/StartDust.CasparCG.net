using System;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.net.Models.Media
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
