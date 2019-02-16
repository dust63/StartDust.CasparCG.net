using System.Xml.Serialization;

namespace StarDust.CasparCG.net.Models.Info
{
    [XmlRoot(ElementName = "producer")]
    public class ProducerInfo
    {

        public ProducerType ProducerType { get; set; }

        private string _type;

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


        [XmlElement(ElementName = "filename")]
        public string Filename { get; set; }
        [XmlElement(ElementName = "width")]
        public uint Width { get; set; }
        [XmlElement(ElementName = "height")]
        public uint Height { get; set; }
        [XmlElement(ElementName = "progressive")]
        public bool Progressive { get; set; }
        [XmlElement(ElementName = "fps")]
        public float Fps { get; set; }
        [XmlElement(ElementName = "loop")]
        public bool Loop { get; set; }
        [XmlElement(ElementName = "frame-number")]
        public int Framenumber { get; set; }
        [XmlElement(ElementName = "nb-frames")]
        public int Nbframes { get; set; }
        [XmlElement(ElementName = "file-frame-number")]
        public int Fileframenumber { get; set; }
        [XmlElement(ElementName = "file-nb-frames")]
        public int Filenbframes { get; set; }
    }
}
