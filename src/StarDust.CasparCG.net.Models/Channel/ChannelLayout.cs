using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.net.Models
{
    /// <summary>
    /// Sound Layout configuration
    /// <see href="https://github.com/CasparCG/help/wiki/Media:-Audio"/>
    /// </summary>
    [Serializable]
    public enum ChannelLayout
    {
        /// <summary>
        /// Mono [1.0] Layout: [C]
        /// </summary>
        [Display(Name = "Mono")]
        [AMCPCommandValue("C")]
        [EnumMember]
        Mono,

        /// <summary>
        /// Stereo [2.0] Layout: [L R]
        /// </summary>
        [Display(Name = "Stereo")]
        [AMCPCommandValue("L R")]
        [EnumMember]
        Stereo,

        /// <summary>
        /// DTS [5.1] Layout: [ C L R Ls Rs LFE]
        /// </summary>
        [Display(Name = "DTS")]
        [AMCPCommandValue("C L R Ls Rs LFE")]
        [EnumMember]
        Dts,

        /// <summary>
        /// "Dolby E [5.1 + stereo mix] Layout: [L R C LFE Ls Rs Lmix Rmix]
        /// </summary>
        [Display(Name = "Dolby E")]
        [AMCPCommandValue("L R C LFE Ls Rs Lmix Rmix")]
        [EnumMember]
        DolbyE,

        /// <summary>
        /// Dolby Digital [5.1] Layout: [L C R Ls Rs LFE]
        /// </summary>
        [Display(Name = "Dolby Digital")]
        [AMCPCommandValue("L C R Ls Rs LFE")]
        [EnumMember]
        DolbyDigital,

        /// <summary>
        /// SMPTE [5.1] Layout: [L R C LFE Ls Rs]
        /// </summary>
        [Display(Name = "SMPTE")]
        [AMCPCommandValue("L R C LFE Ls Rs")]
        [EnumMember]
        Smpte,

        /// <summary>
        /// Pass Throught [16ch] Layout: [will just pass everything as is]
        /// </summary>
        [Display(Name = "Pass Throught")]
        [AMCPCommandValue("passthru")]
        [EnumMember]
        Passthrought
    }



}
