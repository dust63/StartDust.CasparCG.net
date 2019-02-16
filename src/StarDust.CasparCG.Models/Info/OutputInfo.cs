using System.Collections.Generic;
using System.Xml.Serialization;

namespace StarDust.CasparCG.Models.Info
{
    [XmlRoot(ElementName = "output")]
    public class OutputInfo
    {
        [XmlArray("consumers")]
        [XmlArrayItem("consumer")]
        public List<ConsumerInfo> Consumers { get; set; }
    }
}