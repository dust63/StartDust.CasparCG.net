using System;
using System.Runtime.Serialization;
using System.Text;

namespace StarDust.CasparCG.Models
{
    [Serializable]
    public class CGTextFieldData : ICGComponentData
    {


        public CGTextFieldData()
        {
        }

        public CGTextFieldData(string data)
        {
            this.Data = data;
        }

        [DataMember]
        public string Data { get; set; }

        public void ToAMCPEscapedXml(StringBuilder sb)
        {
            sb.Append("<data id=\\\"text\\\" value=\\\"");
            string str = string.IsNullOrEmpty(this.Data) ? string.Empty : this.Data.Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\\", "\\\\");
            sb.Append(str);
            sb.Append("\\\" />");
        }

        public void ToXml(StringBuilder sb)
        {
            string str = (this.Data ?? string.Empty).Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;");
            sb.Append("<data id=\"text\" value=\"" + str + "\" />");
        }

        public override string ToString()
        {
            return string.IsNullOrEmpty(this.Data) ? string.Empty : this.Data;
        }
    }
}
