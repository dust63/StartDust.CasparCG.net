using System.Xml.Serialization;

namespace StarDust.CasparCG.net.Models.Info
{
    [XmlRoot(ElementName = "mixer")]
    public class MixerInfo
    {
        [XmlElement(ElementName = "mix-time")]
        public string Mixtime { get; set; }
    }




}
