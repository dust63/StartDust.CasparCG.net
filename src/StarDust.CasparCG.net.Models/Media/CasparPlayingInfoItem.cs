using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace StarDust.CasparCG.net.Models.Media
{
    /// <summary>
    /// Information about the current playing item
    /// </summary>
    [Serializable]
    public class CasparPlayingInfoItem
    {
        /// <summary>
        /// Instantiate an empty <see cref="CasparPlayingInfoItem"/>
        /// </summary>
        public CasparPlayingInfoItem()
        {
        }

        /// <summary>
        /// Instantitate a <see cref="CasparPlayingInfoItem"/>
        /// </summary>
        /// <param name="clipname"></param>
        public CasparPlayingInfoItem(string clipname)
        {
            Clipname = clipname;
        }

        /// <summary>
        /// Instantitate a <see cref="CasparPlayingInfoItem"/>
        /// </summary>
        /// <param name="videoLayer">video to play the clip</param>
        /// <param name="clipname">file name of the clip to play</param>
        public CasparPlayingInfoItem(uint videoLayer, string clipname)
        {
            VideoLayer = videoLayer;
            Clipname = clipname;
        }

        /// <summary>
        /// Instantitate a <see cref="CasparPlayingInfoItem"/>
        /// </summary>
        /// <param name="clipname">file name of the clip to play</param>
        /// <param name="transition">transition to apply when plaung</param>
        public CasparPlayingInfoItem(string clipname, Transition transition)
        {
            Clipname = clipname;
            if (transition == null)
                return;
            Transition = transition;
        }

        /// <summary>
        /// Instantitate a <see cref="CasparPlayingInfoItem"/>
        /// </summary>
        /// <param name="videoLayer">video to play the clip</param>
        /// <param name="clipname">file name of the clip to play</param>
        /// <param name="transition"></param>
        public CasparPlayingInfoItem(uint videoLayer, string clipname, Transition transition)
        {
            VideoLayer = videoLayer;
            Clipname = clipname;
            if (transition == null)
                return;
            Transition.Type = transition.Type;
            Transition.Duration = transition.Duration;
        }

        /// <summary>
        /// Deserialize xml to <see cref="CasparPlayingInfoItem"/>
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static CasparPlayingInfoItem Create(XmlReader reader)
        {
            var casparItem = new CasparPlayingInfoItem();
            casparItem.ReadXml(reader);
            return casparItem;
        }

        /// <summary>
        /// File name of the clip to play
        /// </summary>
        [DataMember]
        public string Clipname { get; set; }

        /// <summary>
        /// Play in loop
        /// </summary>
        [DataMember]
        public bool Loop { get; set; } = false;

        /// <summary>
        /// Video layer to play the clip
        /// </summary>
        [DataMember]
        public uint VideoLayer { get; set; }

        /// <summary>
        /// Frame to seek
        /// </summary>
        [DataMember]
        public uint? Seek { get; set; }

        /// <summary>
        /// Frame length of the clip
        /// </summary>
        [DataMember]
        public uint? Length { get; set; }

        /// <summary>
        /// Transition
        /// </summary>
        [DataMember]
        public Transition Transition { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public XmlSchema GetSchema()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Read xml to parse information
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            string str1 = reader["clipname"];
            Clipname = string.IsNullOrEmpty(str1) ? string.Empty : str1;
            string s1 = reader["videoLayer"];
            if (!string.IsNullOrEmpty(s1))
                VideoLayer = uint.Parse(s1);
            string s2 = reader["seek"];
            if (!string.IsNullOrEmpty(s2))
                Seek = uint.Parse(s2);
            string s3 = reader["length"];
            if (!string.IsNullOrEmpty(s3))
                Length = uint.Parse(s3);
            string str2 = reader["loop"];
            bool.TryParse(str2, out bool result1);
            Loop = result1;
            reader.ReadStartElement();
            if (reader.Name != "transition")
            {
                return;
            }

            string str3 = reader["type"];
            Transition = !int.TryParse(reader["duration"], out int result2) || !Enum.IsDefined(typeof(TransitionType), str3.ToUpper()) ? new Transition() : new Transition((TransitionType)Enum.Parse(typeof(TransitionType), str3.ToUpper()), result2);
        }


    }
}
