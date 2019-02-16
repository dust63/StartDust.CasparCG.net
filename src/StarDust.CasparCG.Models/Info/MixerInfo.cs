using System.Xml.Serialization;

namespace StarDust.CasparCG.Models.Info
{
    [XmlRoot(ElementName = "mixer")]
    public class MixerInfo
    {
        [XmlElement(ElementName = "mix-time")]
        public string Mixtime { get; set; }
    }




}
