using System.Xml.Serialization;

namespace StarDust.CasparCG.net.Models.Info
{
    [XmlRoot(ElementName = "background")]
    public class Background
    {

        [XmlElement(ElementName = "producer")]
        public ProducerInfo Producer { get; set; }
    }
}
