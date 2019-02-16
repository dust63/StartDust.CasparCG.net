using System.Xml.Serialization;

namespace StarDust.CasparCG.Models.Info
{
    [XmlRoot(ElementName = "consumer")]
    public class ConsumerInfo
    {
        public uint ChannelId { get; set; }

        public ConsumerType ConsumerType { get; set; }

        private string _type;

        [XmlElement(ElementName = "type")]
        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                ConsumerType = _type.TryParseOrDefault(ConsumerType.Unknown);
            }
        }


        [XmlElement(ElementName = "key-only")]
        public bool Keyonly { get; set; }
        [XmlElement(ElementName = "device")]
        public string Device { get; set; }
        [XmlElement(ElementName = "low-latency")]
        public bool Lowlatency { get; set; }
        [XmlElement(ElementName = "embedded-audio")]
        public bool Embeddedaudio { get; set; }
        [XmlElement(ElementName = "presentation-frame-age")]
        public string Presentationframeage { get; set; }
        [XmlElement(ElementName = "index")]
        public int Index { get; set; }
        [XmlElement(ElementName = "windowed")]
        public bool Windowed { get; set; }
        [XmlElement(ElementName = "auto-deinterlace")]
        public bool Autodeinterlace { get; set; }

    }
}