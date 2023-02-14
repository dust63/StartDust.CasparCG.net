using System.Collections.Generic;
using System.Xml.Serialization;

namespace StarDust.CasparCG.net.Models.Info
{
    /// <summary>
    /// Parameters
    /// </summary>
    [XmlRoot(ElementName = "parameter")]
    public class Parameter
    {
        /// <summary>
        /// Id
        /// </summary>
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Type of the parameter
        /// </summary>
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Information
        /// </summary>
        [XmlAttribute(AttributeName = "info")]
        public string Info { get; set; }
    }

    /// <summary>
    /// Property
    /// </summary>
    [XmlRoot(ElementName = "property")]
    public class Property
    {
        /// <summary>
        /// Name of the property
        /// </summary>
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Type of the property
        /// </summary>
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// Information about the property
        /// </summary>
        [XmlAttribute(AttributeName = "info")]
        public string Info { get; set; }
    }

    /// <summary>
    /// Component
    /// </summary>
    [XmlRoot(ElementName = "component")]
    public class Component
    {
        /// <summary>
        /// Name
        /// </summary>
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Property
        /// </summary>
        [XmlElement(ElementName = "property")]
        public Property Property { get; set; }
    }

    /// <summary>
    /// Instance
    /// </summary>
    [XmlRoot(ElementName = "instance")]
    public class Instance
    {
        /// <summary>
        /// Name
        /// </summary>
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Template
    /// </summary>
    [XmlRoot(ElementName = "template")]
    public class TemplateInfo : TemplateBaseInfo
    {
        /// <summary>
        /// List of component
        /// </summary>
        [XmlArray(ElementName = "components")]
        [XmlArrayItem(ElementName = "component")]
        public List<Component> Components { get; set; }
        
        /// <summary>
        /// Key frames
        /// </summary>
        [XmlElement(ElementName = "keyframes")]
        public string Keyframes { get; set; }

        /// <summary>
        /// List of instances
        /// </summary>
        [XmlArray(ElementName = "instances")]
        [XmlArrayItem(ElementName = "instance")]
        public List<Instance> Instances { get; set; }
        
        /// <summary>
        /// List of parameter
        /// </summary>
        [XmlArray("parameters")]
        [XmlArrayItem("parameter")]
        public List<Parameter> Parameters { get; set; }
        
        /// <summary>
        /// Version
        /// </summary>
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        
        /// <summary>
        /// Name of the author
        /// </summary>
        [XmlAttribute(AttributeName = "authorName")]
        public string AuthorName { get; set; }
        
        /// <summary>
        /// Email of the author
        /// </summary>
        [XmlAttribute(AttributeName = "authorEmail")]
        public string AuthorEmail { get; set; }
        
        /// <summary>
        /// Template information
        /// </summary>
        [XmlAttribute(AttributeName = "templateInfo")]
        public string TemplateInformation { get; set; }
        
        /// <summary>
        /// Original width
        /// </summary>
        [XmlAttribute(AttributeName = "originalWidth")]
        public uint OriginalWidth { get; set; }
        
        /// <summary>
        /// Original height
        /// </summary>
        [XmlAttribute(AttributeName = "originalHeight")]
        public uint OriginalHeight { get; set; }
        
        /// <summary>
        /// Original frame rate
        /// </summary>
        [XmlAttribute(AttributeName = "originalFrameRate")]
        public float OriginalFrameRate { get; set; }
    }
}
