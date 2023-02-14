using System.Xml.Serialization;

namespace StarDust.CasparCG.net.Models
{

    /// <summary>
    /// Consumer type configuration
    /// <see href="https://github.com/CasparCG/help/wiki/Server:-Configuration"/>
    /// </summary>
    public enum ConsumerType
    {
        /// <summary>
        /// Cannot determine the consumer type
        /// </summary>
        [AMCPCommandValue("Unknown")]
        [XmlEnum("")]
        Unknown,

        /// <summary>
        /// Blackmagic decklink card
        /// </summary>
        [AMCPCommandValue("DECKLINK")]
        [XmlEnum("decklink-consumer")]
        Decklink,

        /// <summary>
        /// Bluefish card
        /// </summary>
        [AMCPCommandValue("BLUEFISH")]
        [XmlEnum("bluefish-consumer")]
        Bluefish,

        /// <summary>
        /// UI window
        /// </summary>
        [AMCPCommandValue("SCREEN")]
        [XmlEnum("screen-consumer")]
        Screen,

        /// <summary>
        /// System audio card
        /// </summary>
        [AMCPCommandValue("AUDIO")]
        [XmlEnum("audio-consumer")]
        Audio,

        /// <summary>
        /// Image consumer
        /// </summary>
        [AMCPCommandValue("IMAGE")]
        [XmlEnum("image-consumer")]
        Image,

        /// <summary>
        /// Sync to consumer
        /// </summary>
        [AMCPCommandValue("SYNCTO")]
        [XmlEnum("syncto-consumer")]
        Syncto,

        /// <summary>
        /// File consumer
        /// </summary>
        [AMCPCommandValue("FILE")]
        [XmlEnum("file-consumer")]
        File,

        /// <summary>
        /// Stream consumer
        /// </summary>
        [AMCPCommandValue("STREAM")]
        [XmlEnum("stream-consumer")]
        Stream
    }
}
