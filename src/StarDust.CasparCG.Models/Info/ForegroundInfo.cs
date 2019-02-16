using System.Xml.Serialization;

namespace StarDust.CasparCG.Models.Info
{
    [XmlRoot(ElementName = "foreground")]
    public class ForegroundInfo
    {
        [XmlElement(ElementName = "producer")]
        public ProducerInfo Producer { get; set; }
    }
}
