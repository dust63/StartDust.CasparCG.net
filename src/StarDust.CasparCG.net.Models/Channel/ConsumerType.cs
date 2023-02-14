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
        /// Outputs the playing media on to DeckLink video cards 
        /// <see href="https://github.com/CasparCG/help/wiki/Decklink-Consumer"/>
        /// </summary>
        [AMCPCommandValue("DECKLINK")]
        [XmlEnum("decklink-consumer")]
        Decklink,

        /// <summary>
        /// Outputs the playing media on to video cards from Bluefish Technologies
        /// <see href="https://github.com/CasparCG/help/wiki/Bluefish-Consumer"/>
        /// </summary>
        [AMCPCommandValue("BLUEFISH")]
        [XmlEnum("bluefish-consumer")]
        Bluefish,

        /// <summary>
        ///The Screen Consumer outputs to either a window or fullscreen to one or several computer monitors attached directly to the hardware running the CasparCG Server software.
        /// <see href="https://github.com/CasparCG/help/wiki/Screen-Consumer"/>
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
        /// <see href="https://github.com/CasparCG/help/wiki/Image-Consumer"/>
        /// </summary>
        [AMCPCommandValue("IMAGE")]
        [XmlEnum("image-consumer")]
        Image,

        /// <summary>
        /// Sync to consumer
        /// <see href="https://github.com/CasparCG/help/wiki/FFmpeg-Consumer"/>
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
        /// <see href="https://github.com/CasparCG/help/wiki/FFmpeg-Consumer"/>
        /// </summary>
        [AMCPCommandValue("STREAM")]
        [XmlEnum("stream-consumer")]
        Stream,

        /// <summary>
        /// Ndi consumer
        /// <see href="https://github.com/CasparCG/help/wiki/NDI-Consumer"/>
        /// </summary>
        [AMCPCommandValue("STREAM")]
        [XmlEnum("ndi-consumer")]
        Ndi,
    }
}
