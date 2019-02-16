using System.Xml.Serialization;

namespace StarDust.CasparCG.net.Models.Info
{
    [XmlRoot(ElementName = "foreground")]
    public class ForegroundInfo
    {
        [XmlElement(ElementName = "producer")]
        public ProducerInfo Producer { get; set; }
    }
}
