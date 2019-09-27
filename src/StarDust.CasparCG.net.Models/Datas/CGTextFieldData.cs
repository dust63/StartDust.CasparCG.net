using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;

namespace StarDust.CasparCG.net.Models
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


       
        public override string ToString()
        {
            return string.IsNullOrEmpty(this.Data) ? string.Empty : this.Data;
        }

        public XElement ToXml()
        {
            string str = (this.Data ?? string.Empty).Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;");
            return new XElement("data", new XAttribute("id", "text"), new XAttribute("value", str));
        }
    }
}
