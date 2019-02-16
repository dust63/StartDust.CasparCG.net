using System.Xml.Serialization;

namespace StarDust.CasparCG.net.Models
{
    public enum ProducerType
    {

        [AMCPCommandValue("Unknown")]
        [XmlEnum("")]
        Unknown,

        [AMCPCommandValue("EMPTY")]
        [XmlEnum("empty-producer")]
        Empty,

        [AMCPCommandValue("DECKLINK")]
        [XmlEnum("decklink-producer")]
        Decklink,

        [AMCPCommandValue("FFMPEG")]
        [XmlEnum("ffmpeg-producer")]
        FFMpeg,

        [AMCPCommandValue("COLOR")]
        [XmlEnum("color-producer")]
        Color,

        [AMCPCommandValue("AUDIO")]
        [XmlEnum("audio-producer")]
        Audio,

        [AMCPCommandValue("FLASH")]
        [XmlEnum("flash-producer")]
        Flash,

        [AMCPCommandValue("HTML")]
        [XmlEnum("html-producer")]
        Html,

        [AMCPCommandValue("IMAGE")]
        [XmlEnum("image-producer")]
        Image,

        [AMCPCommandValue("PHOTOSHOP")]
        [XmlEnum("photoshop-producer")]
        Photoshop,

        [AMCPCommandValue("ROUTE")]
        [XmlEnum("route-producer")]
        Route,


        [AMCPCommandValue("SCENE")]
        [XmlEnum("scene-producer")]
        Scene,
    }
}
