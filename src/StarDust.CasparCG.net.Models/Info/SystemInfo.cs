using System.Collections.Generic;
using System.Xml.Serialization;

namespace StarDust.CasparCG.net.Models.Info
{
    /// <summary>
    /// Windows OS informations
    /// </summary>
    [XmlRoot(ElementName = "windows")]
    public class WindowsInfo
    {
        /// <summary>
        /// Os name
        /// </summary>
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// OS Service pack
        /// </summary>
        [XmlElement(ElementName = "service-pack")]
        public string Servicepack { get; set; }
    }

    /// <summary>
    /// Decklink information
    /// </summary>
    [XmlRoot(ElementName = "decklink")]
    public class DecklinkServerInfo
    {
        /// <summary>
        /// Card list
        /// </summary>
        [XmlElement(ElementName = "device")]
        public List<string> DevicesNames { get; set; }
    }

    /// <summary>
    /// ffmpeg information
    /// </summary>
    [XmlRoot(ElementName = "ffmpeg")]
    public class FfmpegServerInfo
    {
        /// <summary>
        /// Av codec
        /// </summary>
        [XmlElement(ElementName = "avcodec")]
        public string Avcodec { get; set; }

        /// <summary>
        /// Av format
        /// </summary>
        [XmlElement(ElementName = "avformat")]
        public string Avformat { get; set; }
        
        /// <summary>
        /// Av filter
        /// </summary>
        [XmlElement(ElementName = "avfilter")]
        public string Avfilter { get; set; }
        
        /// <summary>
        /// Av util
        /// </summary>
        [XmlElement(ElementName = "avutil")]
        public string Avutil { get; set; }
        
        /// <summary>
        /// Sw scale
        /// </summary>
        [XmlElement(ElementName = "swscale")]
        public string Swscale { get; set; }
    }

    /// <summary>
    /// CasparCG information
    /// </summary>
    [XmlRoot(ElementName = "caspar")]
    public class CasparServerInfo
    {
        /// <summary>
        /// Decklink server information
        /// </summary>
        [XmlElement(ElementName = "decklink")]
        public DecklinkServerInfo Decklink { get; set; }
        
        /// <summary>
        /// Flash information
        /// </summary>
        [XmlElement(ElementName = "flash")]
        public string Flash { get; set; }
        
        /// <summary>
        /// Template host information
        /// </summary>
        [XmlElement(ElementName = "template-host")]
        public string Templatehost { get; set; }
        
        /// <summary>
        /// Free image information
        /// </summary>
        [XmlElement(ElementName = "free-image")]
        public string Freeimage { get; set; }
        
        /// <summary>
        /// ffmpeg information
        /// </summary>
        [XmlElement(ElementName = "ffmpeg")]
        public FfmpegServerInfo Ffmpeg { get; set; }
    }

    /// <summary>
    /// System information
    /// </summary>
    [XmlRoot(ElementName = "system")]
    public class SystemInfo
    {
        /// <summary>
        /// System hostname
        /// </summary>
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// OS Windows information
        /// </summary>
        [XmlElement(ElementName = "windows")]
        public WindowsInfo Windows { get; set; }

        /// <summary>
        /// Cpu installed on host
        /// </summary>
        [XmlElement(ElementName = "cpu")]
        public string Cpu { get; set; }

        /// <summary>
        /// CasparCG server information
        /// </summary>
        [XmlElement(ElementName = "caspar")]
        public CasparServerInfo Caspar { get; set; }
    }



}
