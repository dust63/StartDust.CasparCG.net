using System.Xml.Serialization;

namespace StarDust.CasparCG.net.Models.Info
{
    /// <summary>
    /// Mixer information
    /// </summary>
    [XmlRoot(ElementName = "mixer")]
    public class MixerInfo
    {
        /// <summary>
        /// Mix time
        /// </summary>
        [XmlElement(ElementName = "mix-time")]
        public string MixTime { get; set; }
    }
}
