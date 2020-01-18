using System.Xml.Serialization;

namespace StarDust.CasparCG.net.Models
{
    /// <summary>
    /// List of Producer type on Caspar CG. See https://github.com/CasparCG/help/wiki/Producers
    ///  Please see the AMCP commands section for a complete reference of the AMCP commands used to control this module.
    /// </summary>
    public enum ProducerType
    {

        [AMCPCommandValue("Unknown")]
        [XmlEnum("")]
        Unknown,



        [AMCPCommandValue("EMPTY")]
        [XmlEnum("empty-producer")]
        Empty,

        /// <summary>
        /// <see href="https://github.com/CasparCG/help/wiki/Decklink-Producer"/>
        /// The Decklink producer allows video sources to be input from BlackMagic Design cards and used as CasparCG Server layers so that CasparCG Server's Mixer can then be used to manipulate the layer. Note that the DeckLink Producer is separate from the DeckLink Consumer which can be used to output from Caspar as HD/SD SDI or HDMI.
        ///Please see the Table of supported video cards.
        /// </summary>
        [AMCPCommandValue("DECKLINK")]
        [XmlEnum("decklink-producer")]
        Decklink,

        /// <summary>
        /// <see href="https://github.com/CasparCG/help/wiki/FFMPEG-Producer"/>
        /// The FFmpeg Producer can play all media that FFmpeg can play, which includes many QuickTime video codec such as Animation, PNG, PhotoJPEG, MotionJPEG, as well as H.264, FLV, WMV and several audio codecs as well as uncompressed audio.
        ///The FFmpeg Producer can also play from direct show devices such as USB cameras connected to the server.
        /// </summary>
        [AMCPCommandValue("FFMPEG")]
        [XmlEnum("ffmpeg-producer")]
        FFMpeg,

        /// <summary>
        /// <see href="https://github.com/CasparCG/help/wiki/Color-Producer"/>
        /// The Color Producer generates a solid RGB color as fill and a grayscale value as key. The purpose of the producer is mainly to be used as an output test.
        /// </summary>
        [AMCPCommandValue("COLOR")]
        [XmlEnum("color-producer")]
        Color,

        /// <summary>
        /// <see href="https://github.com/CasparCG/help/wiki/Photoshop-Producer"/>
        /// The PSD Producer is a producer that can read the Adobe Photoshop .psd file format.
        ///This can be used to create templated graphics similar to the Flash Producer, but with a few big differences.
        /// </summary>
        [AMCPCommandValue("AUDIO")]
        [XmlEnum("audio-producer")]
        Audio,

        /// <summary>
        /// <see href="https://github.com/CasparCG/help/wiki/Flash-Producer"/>
        /// The Flash Producer uses Adobe’s Flash Player to play SWFs including full control over all dynamic content. You can even load multiple Flash files as layers that is stacked and composited by the CasparCG Server Mixer module and can then be controlled independently from a client program. Several Flash Player instances can also be loaded to get around the single-threaded nature of the current Flash Player.
        /// </summary>
        [AMCPCommandValue("FLASH")]
        [XmlEnum("flash-producer")]
        Flash,

        /// <summary>
        /// <see href="https://github.com/CasparCG/help/wiki/HTML-Producer"/>
        /// Html producer
        /// </summary>
        [AMCPCommandValue("HTML")]
        [XmlEnum("html-producer")]
        Html,

        /// <summary>
        ///  /// <see href="https://github.com/CasparCG/help/wiki/Image-Producer"/>
        /// The Image Producer displays bitmap images with and without alpha channel.
        /// </summary>
        [AMCPCommandValue("IMAGE")]
        [XmlEnum("image-producer")]
        Image,

        [AMCPCommandValue("PHOTOSHOP")]
        [XmlEnum("photoshop-producer")]
        Photoshop,

        /// <summary>
        /// <see href="https://github.com/CasparCG/help/wiki/Route-Producer"/>
        /// Provides a way of routing frames from other producers from one channel or layer to a layer on another channel.
        /// </summary>
        [AMCPCommandValue("ROUTE")]
        [XmlEnum("route-producer")]
        Route,

        /// <summary>
        /// <see href="https://github.com/CasparCG/help/wiki/Scene-Producer"/>
        /// Scene producer
        /// </summary>
        [AMCPCommandValue("SCENE")]
        [XmlEnum("scene-producer")]
        Scene
    }
}
