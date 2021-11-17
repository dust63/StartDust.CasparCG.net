using System.Xml.Serialization;

namespace StarDust.CasparCG.net.Models.Info
{
    [XmlRoot(ElementName = "layer")]
    public class LayerInfo
    {
        [XmlElement(ElementName = "status")]
        public string Status { get; set; }
        [XmlElement(ElementName = "auto_delta")]
        public string Auto_delta { get; set; }
        [XmlElement(ElementName = "frame-number")]
        public uint Framenumber { get; set; }
        [XmlElement(ElementName = "nb_frames")]
        public uint Nb_frames { get; set; }
        [XmlElement(ElementName = "frames-left")]
        public uint Framesleft { get; set; }
        [XmlElement(ElementName = "frame-age")]
        public uint Frameage { get; set; }
        [XmlElement(ElementName = "foreground")]
        public ForegroundInfo Foreground { get; set; }
        [XmlElement(ElementName = "background")]
        public Background Background { get; set; }
        [XmlElement(ElementName = "index")]
        public uint Index { get; set; }
    }
}
