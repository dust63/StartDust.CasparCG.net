using System.Xml.Serialization;

namespace StarDust.CasparCG.net.Models.Info
{
    /// <summary>
    /// Layer information
    /// </summary>
    [XmlRoot(ElementName = "layer")]
    public class LayerInfo
    {
        /// <summary>
        /// Status
        /// </summary>
        [XmlElement(ElementName = "status")]
        public string Status { get; set; }
        
        /// <summary>
        /// Auto delta
        /// </summary>
        [XmlElement(ElementName = "auto_delta")]
        public string Auto_delta { get; set; }
        
        /// <summary>
        /// Frame number
        /// </summary>
        [XmlElement(ElementName = "frame-number")]
        public uint Framenumber { get; set; }
        
        /// <summary>
        /// Number of frame
        /// </summary>
        [XmlElement(ElementName = "nb_frames")]
        public uint NbFrames { get; set; }
        
        /// <summary>
        /// Framew left
        /// </summary>
        [XmlElement(ElementName = "frames-left")]
        public uint FramesLeft { get; set; }
        
        /// <summary>
        /// Frame age
        /// </summary>
        [XmlElement(ElementName = "frame-age")]
        public uint FrameAge { get; set; }
        
        /// <summary>
        /// Foreground information
        /// </summary>
        [XmlElement(ElementName = "foreground")]
        public ForegroundInfo Foreground { get; set; }
        
        /// <summary>
        /// Background information
        /// </summary>
        [XmlElement(ElementName = "background")]
        public Background Background { get; set; }
        
        /// <summary>
        /// Index of the layer
        /// </summary>
        [XmlElement(ElementName = "index")]
        public uint Index { get; set; }
    }
}
