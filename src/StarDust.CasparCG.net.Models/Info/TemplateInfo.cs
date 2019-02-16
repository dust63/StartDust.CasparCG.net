using System.Collections.Generic;
using System.Xml.Serialization;

namespace StarDust.CasparCG.net.Models.Info
{
    [XmlRoot(ElementName = "parameter")]
    public class Parameter
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "info")]
        public string Info { get; set; }
    }



    [XmlRoot(ElementName = "template")]
    public class TemplateInfo : TemplateBaseInfo
    {
        [XmlElement(ElementName = "components")]
        public string Components { get; set; }
        [XmlElement(ElementName = "keyframes")]
        public string Keyframes { get; set; }
        [XmlElement(ElementName = "instances")]
        public string Instances { get; set; }
        [XmlArray("parameters")]
        [XmlArrayItem("parameter")]
        public List<Parameter> Parameters { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "authorName")]
        public string AuthorName { get; set; }
        [XmlAttribute(AttributeName = "authorEmail")]
        public string AuthorEmail { get; set; }
        [XmlAttribute(AttributeName = "templateInfo")]
        public string TemplateInformation { get; set; }
        [XmlAttribute(AttributeName = "originalWidth")]
        public uint OriginalWidth { get; set; }
        [XmlAttribute(AttributeName = "originalHeight")]
        public uint OriginalHeight { get; set; }
        [XmlAttribute(AttributeName = "originalFrameRate")]
        public float OriginalFrameRate { get; set; }
    }
}
