using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.net.Models
{
    [Serializable]
    public enum ChannelLayout
    {
        [Display(Name = "Mono")]
        [AMCPCommandValue("C")]
        [EnumMember]
        Mono,


        [Display(Name = "Stereo")]
        [AMCPCommandValue("L R")]
        [EnumMember]
        Stereo,

        [Display(Name = "DTS")]
        [AMCPCommandValue("C L R Ls Rs LFE")]
        [EnumMember]
        Dts,

        [Display(Name = "DolbyE")]
        [AMCPCommandValue("L R C LFE Ls Rs Lmix Rmix")]
        [EnumMember]
        DolbyE,

        [Display(Name = "DolbyDigital")]
        [AMCPCommandValue("L C R Ls Rs LFE")]
        [EnumMember]
        DolbyDigital,

        [Display(Name = "SMPTE")]
        [AMCPCommandValue("L R C LFE Ls Rs")]
        [EnumMember]
        Smpte,

        [Display(Name = "PassThrought")]
        [AMCPCommandValue("passthru")]
        [EnumMember]
        Passthrought
    }



}
