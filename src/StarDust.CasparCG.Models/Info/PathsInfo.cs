using System.Xml.Serialization;

namespace StarDust.CasparCG.Models.Info
{
    [XmlRoot(ElementName = "paths")]
    public class PathsInfo
    {
        [XmlElement(ElementName = "media-path")]
        public string Mediapath { get; set; }
        [XmlElement(ElementName = "log-path")]
        public string Logpath { get; set; }
        [XmlElement(ElementName = "data-path")]
        public string Datapath { get; set; }
        [XmlElement(ElementName = "template-path")]
        public string Templatepath { get; set; }
        [XmlElement(ElementName = "thumbnails-path")]
        public string Thumbnailspath { get; set; }
    }
}
