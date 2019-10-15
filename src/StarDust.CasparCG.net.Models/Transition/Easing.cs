
using System;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.net.Models
{
    /// <summary>
    /// List of easing use on transition
    /// </summary>
    [Serializable]
    public enum Easing
    {
        /// <summary>
        /// None
        /// </summary>
        [AMCPCommandValue("None")]
        [DataMember]
        None,

        /// <summary>
        /// Linear
        /// </summary>
        [AMCPCommandValue("Linear")]
        [DataMember]
        Linear,

        /// <summary>
        /// Ease none
        /// </summary>
        [AMCPCommandValue("EaseNone")]
        [DataMember]
        EaseNone,

        /// <summary>
        /// Ease in quad
        /// </summary>
        [AMCPCommandValue("EaseInQuad")]
        [DataMember]
        EaseInQuad,

        /// <summary>
        /// Ease out quad
        /// </summary>
        [AMCPCommandValue("EaseOutQuad")]
        [DataMember]
        EaseOutQuad,

        /// <summary>
        /// Ease in out quad
        /// </summary>
        [AMCPCommandValue("EaseInOutQuad")]
        [DataMember]
        EaseInOutQuad,

        /// <summary>
        /// Ease out in quad
        /// </summary>
        [AMCPCommandValue("EaseOutInQuad")]
        [DataMember]
        EaseOutInQuad,

        /// <summary>
        /// Ease in cubic
        /// </summary>
        [AMCPCommandValue("EaseInCubic")]
        [DataMember]
        EaseInCubic,

        /// <summary>
        /// Ease out cubic
        /// </summary>
        [AMCPCommandValue("EaseOutCubic")]
        [DataMember]
        EaseOutCubic,

        /// <summary>
        /// Ease in out cubic
        /// </summary>
        [AMCPCommandValue("EaseInOutCubic")]
        [DataMember]
        EaseInOutCubic,

        /// <summary>
        /// Ease out in cubic
        /// </summary>
        [AMCPCommandValue("EaseOutInCubic")]
        [DataMember]
        EaseOutInCubic,

        /// <summary>
        /// Ease in quart
        /// </summary>
        [AMCPCommandValue("EaseInQuart")]
        [DataMember]
        EaseInQuart,

        /// <summary>
        /// Ease out quart
        /// </summary>
        [AMCPCommandValue("EaseOutQuart")]
        [DataMember]
        EaseOutQuart,

        /// <summary>
        /// Ease in out quart
        /// </summary>
        [AMCPCommandValue("EaseInOutQuart")]
        [DataMember]
        EaseInOutQuart,

        /// <summary>
        /// Ease out in quart
        /// </summary>
        [AMCPCommandValue("EaseOutInQuart")]
        [DataMember]
        EaseOutInQuart,

        /// <summary>
        /// Ease in quint
        /// </summary>
        [AMCPCommandValue("EaseInQuint")]
        [DataMember]
        EaseInQuint,

        /// <summary>
        /// Ease out quint
        /// </summary>
        [AMCPCommandValue("EaseOutQuint")]
        [DataMember]
        EaseOutQuint,

        /// <summary>
        /// Ease in out quint
        /// </summary>
        [AMCPCommandValue("EaseInOutQuint")]
        [DataMember]
        EaseInOutQuint,

        /// <summary>
        /// Ease out in quint
        /// </summary>
        [AMCPCommandValue("EaseOutInQuint")]
        [DataMember]
        EaseOutInQuint,

        /// <summary>
        /// Ease in sine
        /// </summary>
        [AMCPCommandValue("EaseInSine")]
        [DataMember]
        EaseInSine,

        /// <summary>
        /// Ease out sine
        /// </summary>
        [AMCPCommandValue("EaseOutSine")]
        [DataMember]
        EaseOutSine,

        /// <summary>
        /// Ease in out sine
        /// </summary>
        [AMCPCommandValue("EaseInOutSine")]
        [DataMember]
        EaseInOutSine,

        /// <summary>
        /// Ease out in sine
        /// </summary>
        [AMCPCommandValue("EaseOutInSine")]
        [DataMember]
        EaseOutInSine,

        /// <summary>
        /// Ease in expo
        /// </summary>
        [AMCPCommandValue("EaseInExpo")]
        [DataMember]
        EaseInExpo,

        /// <summary>
        /// Ease out expo
        /// </summary>
        [AMCPCommandValue("EaseOutExpo")]
        [DataMember]
        EaseOutExpo,

        /// <summary>
        /// Ease ou expo
        /// </summary>
        [AMCPCommandValue("EaseOutExpo")]
        [DataMember]
        EaseInOutExpo,

        /// <summary>
        /// Ease out in expo
        /// </summary>
        [AMCPCommandValue("EaseOutInExpo")]
        [DataMember]
        EaseOutInExpo,

        /// <summary>
        /// Ease in circle
        /// </summary>
        [AMCPCommandValue("EaseInCirc")]
        [DataMember]
        EaseInCirc,

        /// <summary>
        /// Ease out circle
        /// </summary>
        [AMCPCommandValue("EaseOutCirc")]
        [DataMember]
        EaseOutCirc,

        /// <summary>
        /// Ease in out circle
        /// </summary>
        [AMCPCommandValue("EaseInOutCirc")]
        [DataMember]
        EaseInOutCirc,

        /// <summary>
        /// Ease out in circle
        /// </summary>
        [AMCPCommandValue("EaseOutInCirc")]
        [DataMember]
        EaseOutInCirc,

        /// <summary>
        /// Ease in elastic
        /// </summary>
        [AMCPCommandValue("EaseInElastic")]
        [DataMember]
        EaseInElastic,

        /// <summary>
        /// Ease out elastic
        /// </summary>
        [AMCPCommandValue("EaseOutElastic")]
        [DataMember]
        EaseOutElastic,

        /// <summary>
        /// Ease in out elastic
        /// </summary>
        [AMCPCommandValue("EaseInOutElastic")]
        [DataMember]
        EaseInOutElastic,

        /// <summary>
        /// Ease out in elastic
        /// </summary>
        [AMCPCommandValue("EaseOutInElastic")]
        [DataMember]
        EaseOutInElastic,

        /// <summary>
        /// Ease in back
        /// </summary>
        [AMCPCommandValue("EaseInBack")]
        [DataMember]
        EaseInBack,

        /// <summary>
        /// Ease out back
        /// </summary>
        [AMCPCommandValue("EaseOutBack")]
        [DataMember]
        EaseOutBack,

        /// <summary>
        /// Ease in out back
        /// </summary>
        [AMCPCommandValue("EaseInOutBack")]
        [DataMember]
        EaseInOutBack,

        /// <summary>
        /// Ease out in back
        /// </summary>
        [AMCPCommandValue("EaseOutInBack")]
        [DataMember]
        EaseOutInBack,

        /// <summary>
        /// Ease out bounce
        /// </summary>
        [AMCPCommandValue("EaseOutBounce")]
        [DataMember]
        EaseOutBounce,

        /// <summary>
        /// Ease in bounce
        /// </summary>
        [AMCPCommandValue("EaseInBounce")]
        [DataMember]
        EaseInBounce,

        /// <summary>
        /// Ease in out bounce
        /// </summary>
        [AMCPCommandValue("EaseInOutBounce")]
        [DataMember]
        EaseInOutBounce,

        /// <summary>
        /// Ease out in bounce
        /// </summary>
        [AMCPCommandValue("EaseOutInBounce")]
        [DataMember]
        EaseOutInBounce,
    }
}
