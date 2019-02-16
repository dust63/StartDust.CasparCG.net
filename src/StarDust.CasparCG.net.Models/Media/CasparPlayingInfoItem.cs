using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace StarDust.CasparCG.net.Models.Media
{
    [Serializable]
    public class CasparPlayingInfoItem
    {
        public CasparPlayingInfoItem()
        {
        }

        public CasparPlayingInfoItem(string clipname)
        {
            this.Clipname = clipname;
        }

        public CasparPlayingInfoItem(uint videoLayer, string clipname)
        {
            this.VideoLayer = videoLayer;
            this.Clipname = clipname;
        }

        public CasparPlayingInfoItem(string clipname, Transition transition)
        {
            this.Clipname = clipname;
            if (transition == null)
                return;
            Transition = transition;
        }

        public CasparPlayingInfoItem(uint videoLayer, string clipname, Transition transition)
        {
            this.VideoLayer = videoLayer;
            this.Clipname = clipname;
            if (transition == null)
                return;
            this.Transition.Type = transition.Type;
            this.Transition.Duration = transition.Duration;
        }

        public static CasparPlayingInfoItem Create(XmlReader reader)
        {
            CasparPlayingInfoItem casparItem = new CasparPlayingInfoItem();
            casparItem.ReadXml(reader);
            return casparItem;
        }

        [DataMember]
        public string Clipname { get; set; }

        [DataMember]
        public bool Loop { get; set; } = false;

        [DataMember]
        public uint VideoLayer { get; set; }

        [DataMember]
        public uint? Seek { get; set; }

        [DataMember]
        public uint? Length { get; set; }

        [DataMember]
        public Transition Transition { get; private set; }

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        public void ReadXml(XmlReader reader)
        {
            int content = (int)reader.MoveToContent();
            string str1 = reader["clipname"];
            this.Clipname = string.IsNullOrEmpty(str1) ? "" : str1;
            string s1 = reader["videoLayer"];
            if (!string.IsNullOrEmpty(s1))
                this.VideoLayer = uint.Parse(s1);
            string s2 = reader["seek"];
            if (!string.IsNullOrEmpty(s2))
                this.Seek = uint.Parse(s2);
            string s3 = reader["length"];
            if (!string.IsNullOrEmpty(s3))
                this.Length = uint.Parse(s3);
            string str2 = reader["loop"];
            bool result1 = false;
            bool.TryParse(str2, out result1);
            this.Loop = result1;
            reader.ReadStartElement();
            if (!(reader.Name == "transition"))
                return;
            int result2 = 0;
            string str3 = reader["type"];
            this.Transition = !int.TryParse(reader["duration"], out result2) || !Enum.IsDefined(typeof(TransitionType), str3.ToUpper()) ? new Transition() : new Transition((TransitionType)Enum.Parse(typeof(TransitionType), str3.ToUpper()), result2);
        }


    }
}
