using System.Xml.Serialization;

namespace StarDust.CasparCG.net.Models.Info
{
    /// <summary>
    /// Producer information
    /// </summary>
    [XmlRoot(ElementName = "producer")]
    public class ProducerInfo
    {
        private string _type;

        /// <summary>
        /// Type of the consumer
        /// </summary>
        [XmlElement(ElementName = "type")]
        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                ProducerType = _type.TryParseOrDefault(ProducerType.Unknown);
            }
        }

        /// <summary>
        /// Parsed type of the consumer
        /// </summary>
        [XmlIgnore]
        public ProducerType ProducerType { get; set; }

        /// <summary>
        /// Filename used by consumer
        /// </summary>
        [XmlElement(ElementName = "filename")]
        public string Filename { get; set; }

        /// <summary>
        /// Current width of the consumer
        /// </summary>
        [XmlElement(ElementName = "width")]
        public uint Width { get; set; }
        
        /// <summary>
        /// Current height of the consumer
        /// </summary>
        [XmlElement(ElementName = "height")]
        public uint Height { get; set; }
        
        /// <summary>
        /// Is progressive
        /// </summary>
        [XmlElement(ElementName = "progressive")]
        public bool Progressive { get; set; }
        
        /// <summary>
        /// Current FPS of the consumer
        /// </summary>
        [XmlElement(ElementName = "fps")]
        public float Fps { get; set; }
        
        /// <summary>
        /// Is looping
        /// </summary>
        [XmlElement(ElementName = "loop")]
        public bool Loop { get; set; }
        
        /// <summary>
        /// Frame number
        /// </summary>
        [XmlElement(ElementName = "frame-number")]
        public int Framenumber { get; set; }
        
        /// <summary>
        /// Total Number of frame
        /// </summary>
        [XmlElement(ElementName = "nb-frames")]
        public int Nbframes { get; set; }
        
        /// <summary>
        /// File frame number
        /// </summary>
        [XmlElement(ElementName = "file-frame-number")]
        public int Fileframenumber { get; set; }
        
        /// <summary>
        /// Total number of file frame
        /// </summary>
        [XmlElement(ElementName = "file-nb-frames")]
        public int Filenbframes { get; set; }
    }
}
