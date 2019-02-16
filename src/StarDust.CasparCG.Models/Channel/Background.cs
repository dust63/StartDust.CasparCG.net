using System.Xml.Serialization;

namespace StarDust.CasparCG.Models.Info
{
    [XmlRoot(ElementName = "background")]
    public class Background
    {

        [XmlElement(ElementName = "producer")]
        public ProducerInfo Producer { get; set; }
    }
}
