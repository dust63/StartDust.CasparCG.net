using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace StarDust.CasparCG.net.Models
{
    [Serializable]
    public class CGDataPair
    {

        public CGDataPair()
        {
        }

        public CGDataPair(string name, ICGComponentData data)
        {
            this.Name = name;
            this.Data = data;
        }

        public CGDataPair(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Value
        {
            get
            {
                return this.Data != null ? this.Data.ToString() : string.Empty;
            }
            set
            {
                if (value == null)
                    this.Data = new CGTextFieldData(string.Empty);
                else
                    this.Data = new CGTextFieldData(value);
            }
        }

        public ICGComponentData Data { get; set; }

        private void ToEscapedXml(StringBuilder sb)
        {
            StringBuilder sb1 = new StringBuilder();
            this.Data.ToXml(sb1);
            sb1.Replace("\"", "\\\"");
            sb.Append("<componentData id=\\\"" + this.Name + "\\\">");
            sb.Append(sb1.ToString());
            sb.Append("</componentData>");
        }

        private void ToXml(StringBuilder sb)
        {
            if (this.Data == null)
                return;
            sb.Append("<componentData id=\"" + this.Name + "\">");
            this.Data.ToXml(sb);
            sb.Append("</componentData>");
        }

        public static string ToXml(IEnumerable<CGDataPair> pairs)
        {
            StringBuilder sb = new StringBuilder("<templateData>");
            foreach (CGDataPair pair in pairs)
                pair.ToXml(sb);
            sb.Append("</templateData>");
            return sb.ToString();
        }

        public static string ToEscapedXml(IEnumerable<CGDataPair> pairs)
        {
            StringBuilder sb = new StringBuilder("<templateData>");
            foreach (CGDataPair pair in pairs)
                pair.ToEscapedXml(sb);
            sb.Append("</templateData>");
            return sb.ToString();
        }
    }
}
