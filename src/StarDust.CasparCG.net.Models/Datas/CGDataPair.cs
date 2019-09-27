using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;

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

     

        private string ToXml()
        {
            if (this.Data == null)
                return null;
            var xmlElement = new XElement("componentData", new XAttribute("id", Name), this.Data.ToXml());
            return xmlElement.ToString();
        }

    }
}
