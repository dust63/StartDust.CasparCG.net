using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace StarDust.CasparCG.Models.Info
{
    [Serializable]
    [XmlRoot("channel")]
    public class ChannelInfo
    {

        public ChannelInfo()
        {
        }

        public ChannelInfo(uint id, VideoMode vm, ChannelStatus cs, string activeClip)
        {
            this.ID = id;
            this.VideoMode = vm;
            this.Status = cs;
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


        private string _videoModeXml;


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



        [XmlElement(ElementName = "stage")]
        public StageInfo Stage { get; set; }

        [XmlElement(ElementName = "mixer")]
        public MixerInfo Mixer { get; set; }

        [XmlElement(ElementName = "output")]
        public OutputInfo Output { get; set; }

        private uint _index;
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


        public override string ToString()
        {
            return $"{ID} - {VideoMode} - {Status}";
        }
    }
}
