using System.Collections.Generic;
using System.Xml.Serialization;

namespace StarDust.CasparCG.Models.Info
{

    [XmlRoot(ElementName = "windows")]
    public class WindowsInfo
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "service-pack")]
        public string Servicepack { get; set; }
    }

    [XmlRoot(ElementName = "decklink")]
    public class DecklinkServerInfo
    {
        [XmlElement(ElementName = "device")]
        public List<string> DevicesNames { get; set; }
    }

    [XmlRoot(ElementName = "ffmpeg")]
    public class FfmpegServerInfo
    {
        [XmlElement(ElementName = "avcodec")]
        public string Avcodec { get; set; }
        [XmlElement(ElementName = "avformat")]
        public string Avformat { get; set; }
        [XmlElement(ElementName = "avfilter")]
        public string Avfilter { get; set; }
        [XmlElement(ElementName = "avutil")]
        public string Avutil { get; set; }
        [XmlElement(ElementName = "swscale")]
        public string Swscale { get; set; }
    }

    [XmlRoot(ElementName = "caspar")]
    public class CasparServerInfo
    {
        [XmlElement(ElementName = "decklink")]
        public DecklinkServerInfo Decklink { get; set; }
        [XmlElement(ElementName = "flash")]
        public string Flash { get; set; }
        [XmlElement(ElementName = "template-host")]
        public string Templatehost { get; set; }
        [XmlElement(ElementName = "free-image")]
        public string Freeimage { get; set; }
        [XmlElement(ElementName = "ffmpeg")]
        public FfmpegServerInfo Ffmpeg { get; set; }
    }

    [XmlRoot(ElementName = "system")]
    public class SystemInfo
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "windows")]
        public WindowsInfo Windows { get; set; }
        [XmlElement(ElementName = "cpu")]
        public string Cpu { get; set; }
        [XmlElement(ElementName = "caspar")]
        public CasparServerInfo Caspar { get; set; }
    }



}
