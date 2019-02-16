using System.Xml.Serialization;

namespace StarDust.CasparCG.net.Models
{
    public enum ConsumerType
    {

        [AMCPCommandValue("Unknown")]
        [XmlEnum("")]
        Unknown,

        [AMCPCommandValue("DECKLINK")]
        [XmlEnum("decklink-consumer")]
        Decklink,

        [AMCPCommandValue("BLUEFISH")]
        [XmlEnum("bluefish-consumer")]
        Bluefish,

        [AMCPCommandValue("SCREEN")]
        [XmlEnum("screen-consumer")]
        Screen,

        [AMCPCommandValue("AUDIO")]
        [XmlEnum("audio-consumer")]
        Audio,

        [AMCPCommandValue("IMAGE")]
        [XmlEnum("image-consumer")]
        Image,

        [AMCPCommandValue("SYNCTO")]
        [XmlEnum("syncto-consumer")]
        Syncto,

        [AMCPCommandValue("FILE")]
        [XmlEnum("file-consumer")]
        File,

        [AMCPCommandValue("STREAM")]
        [XmlEnum("stream-consumer")]
        Stream,

    }
}
