using System.Xml.Serialization;

namespace StarDust.CasparCG.net.Models.Info
{
    /// <summary>
    /// Consumer information
    /// </summary>
    [XmlRoot(ElementName = "consumer")]
    public class ConsumerInfo
    {
        private string _type;

        /// <summary>
        /// Channel id attached to
        /// </summary>
        public uint ChannelId { get; set; }

        /// <summary>
        /// Type of the consumer
        /// </summary>
        public ConsumerType ConsumerType { get; set; }

        
        /// <summary>
        /// Type of the consumer as string
        /// </summary>
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

        /// <summary>
        /// Is generating only video key
        /// </summary>
        [XmlElement(ElementName = "key-only")]
        public bool Keyonly { get; set; }

        /// <summary>
        /// Device type
        /// </summary>
        [XmlElement(ElementName = "device")]
        public string Device { get; set; }

        /// <summary>
        /// Is low latency
        /// </summary>
        [XmlElement(ElementName = "low-latency")]
        public bool Lowlatency { get; set; }

        /// <summary>
        /// Allow embeded audio
        /// </summary>
        [XmlElement(ElementName = "embedded-audio")]
        public bool Embeddedaudio { get; set; }

        /// <summary>
        /// Prensentation frame age
        /// </summary>
        [XmlElement(ElementName = "presentation-frame-age")]
        public string Presentationframeage { get; set; }

        /// <summary>
        /// Index of the consumer
        /// </summary>
        [XmlElement(ElementName = "index")]
        public int Index { get; set; }

        /// <summary>
        /// Is in window
        /// </summary>
        [XmlElement(ElementName = "windowed")]
        public bool Windowed { get; set; }

        /// <summary>
        /// Allow auto deinterlacing
        /// </summary>
        [XmlElement(ElementName = "auto-deinterlace")]
        public bool Autodeinterlace { get; set; }

    }
}