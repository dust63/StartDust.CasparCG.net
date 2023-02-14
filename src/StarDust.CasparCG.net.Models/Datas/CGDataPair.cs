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
        /// <summary>
        /// Create empty <see cref="CGDataPair"/>
        /// </summary>
        public CGDataPair()
        {
        }

        /// <summary>
        /// Create <see cref="CGDataPair"/> whith name and data
        /// <paramref name="name"/>
        /// <paramref name="data"/>
        /// </summary>
        public CGDataPair(string name, ICGComponentData data)
        {
            this.Name = name;
            this.Data = data;
        }

        /// <summary>
        /// Create <see cref="CGDataPair"/> whith name and string value
        /// </summary>
        /// <param name="name">name of data</param>
        /// <param name="value">string value</param>
        public CGDataPair(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Name of the data store
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Value stored
        /// </summary>
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

        /// <summary>
        /// Data stored
        /// </summary>
        public ICGComponentData Data { get; set; }

     
        /// <summary>
        /// Serialize to xml
        /// </summary>
        /// <returns></returns>
        public string ToXml()
        {
            if (this.Data == null)
                return null;
            var xmlElement = new XElement("componentData", new XAttribute("id", Name), this.Data.ToXml());
            return xmlElement.ToString();
        }

    }
}
