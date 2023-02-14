using System.Collections.Generic;
using System.Xml.Serialization;

namespace StarDust.CasparCG.net.Models.Info
{
    /// <summary>
    /// Output information
    /// </summary>
    [XmlRoot(ElementName = "output")]
    public class OutputInfo
    {
        /// <summary>
        /// All consumers informations
        /// </summary>
        [XmlArray("consumers")]
        [XmlArrayItem("consumer")]
        public List<ConsumerInfo> Consumers { get; set; }
    }
}