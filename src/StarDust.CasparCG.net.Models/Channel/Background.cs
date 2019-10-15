using System.Xml.Serialization;

namespace StarDust.CasparCG.net.Models.Info
{
    /// <summary>
    /// Background layer
    /// </summary>
    [XmlRoot(ElementName = "background")]
    public class Background
    {

        /// <summary>
        /// Producer of the background
        /// </summary>
        [XmlElement(ElementName = "producer")]
        public ProducerInfo Producer { get; set; }
    }
}
