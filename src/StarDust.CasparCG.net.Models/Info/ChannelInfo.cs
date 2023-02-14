using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace StarDust.CasparCG.net.Models.Info
{
    /// <summary>
    /// CasparCG Channel information
    /// </summary>
    [Serializable]
    [XmlRoot("channel")]
    public class ChannelInfo
    {
        private string _videoModeXml; 
        private uint _index;

        /// <summary>
        /// Create emtpy channel info
        /// </summary>
        public ChannelInfo()
        {
        }

        /// <summary>
        /// Create channel information
        /// </summary>
        /// <param name="id">index of the channel</param>
        /// <param name="videoMode">video mode</param>
        /// <param name="channelStatus">channel status</param>
        /// <param name="activeClip">current active clips</param>
        public ChannelInfo(uint id, VideoMode videoMode, ChannelStatus channelStatus, string activeClip)
        {
            this.ID = id;
            this.VideoMode = videoMode;
            this.Status = channelStatus;
            this.ActiveClip = activeClip;
        }

        /// <summary>
        /// Id of the Channel
        /// </summary>
        [DataMember]
        public uint ID { get; set; }

        /// <summary>
        /// Video resolution and fps of the Channel
        /// </summary>
        [DataMember]
        [XmlIgnore]
        public VideoMode VideoMode { get; set; }

        /// <summary>
        /// Video mode of the channel
        /// </summary>
        [XmlElement(ElementName = "video-mode")]
        public string VideoModelXml
        {
            get { return _videoModeXml; }
            set
            {
                _videoModeXml = value;
                VideoMode = _videoModeXml.TryParseOrDefault(VideoMode.Unknown);
            }
        }

        /// <summary>
        /// If the Channel is playing or not
        /// </summary>
        [DataMember]
        public ChannelStatus Status { get; set; }

        /// <summary>
        /// Clip that is On Air
        /// </summary>
        [DataMember]
        public string ActiveClip { get; set; }

        /// <summary>
        /// Stage information
        /// </summary>
        [XmlElement(ElementName = "stage")]
        public StageInfo Stage { get; set; }

        /// <summary>
        /// Mixer information
        /// </summary>
        [XmlElement(ElementName = "mixer")]
        public MixerInfo Mixer { get; set; }

        /// <summary>
        /// Output information
        /// </summary>
        [XmlElement(ElementName = "output")]
        public OutputInfo Output { get; set; }

        /// <summary>
        /// Index of the channel
        /// </summary>
        [XmlElement(ElementName = "index")]
        public uint Index
        {
            get { return _index; }
            set
            {
                _index = value;
                ID = _index + 1;
            }
        }

        /// <summary>
        /// Display channel info to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{ID} - {VideoMode} - {Status}";
        }
    }
}
