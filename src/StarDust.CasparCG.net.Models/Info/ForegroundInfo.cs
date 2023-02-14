using System.Xml.Serialization;

namespace StarDust.CasparCG.net.Models.Info
{
    /// <summary>
    /// Foreground information
    /// </summary>
    [XmlRoot(ElementName = "foreground")]
    public class ForegroundInfo
    {
        /// <summary>
        /// Producer information
        /// </summary>
        [XmlElement(ElementName = "producer")]
        public ProducerInfo Producer { get; set; }
    }
}
