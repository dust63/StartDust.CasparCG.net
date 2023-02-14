using System.Xml.Serialization;

namespace StarDust.CasparCG.net.Models.Info
{
    /// <summary>
    /// Paths configured
    /// </summary>
    [XmlRoot(ElementName = "paths")]
    public class PathsInfo
    {
        /// <summary>
        /// Media path on the server
        /// </summary>
        [XmlElement(ElementName = "media-path")]
        public string Mediapath { get; set; }

        /// <summary>
        /// Log path on the server
        /// </summary>
        [XmlElement(ElementName = "log-path")]
        public string Logpath { get; set; }

        /// <summary>
        /// Data path on the server
        /// </summary>
        [XmlElement(ElementName = "data-path")]
        public string Datapath { get; set; }

        /// <summary>
        /// Template path of the server
        /// </summary>
        [XmlElement(ElementName = "template-path")]
        public string Templatepath { get; set; }

        /// <summary>
        /// Thumbnail path of the server
        /// </summary>
        [XmlElement(ElementName = "thumbnails-path")]
        public string Thumbnailspath { get; set; }
    }
}
