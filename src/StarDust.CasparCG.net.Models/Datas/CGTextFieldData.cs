using System;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;

namespace StarDust.CasparCG.net.Models
{
    /// <summary>
    /// Text field data
    /// </summary>
    [Serializable]
    public class CGTextFieldData : ICGComponentData
    {

        /// <summary>
        /// Initliaze empty text field data
        /// </summary>
        public CGTextFieldData()
        {
        }

        /// <summary>
        /// Initialize text field
        /// </summary>
        /// <param name="data">data to store</param>
        public CGTextFieldData(string data)
        {
            Data = data;
        }

        /// <summary>
        /// Data to store
        /// </summary>
        [DataMember]
        public string Data { get; set; }

        /// <summary>
        /// Display data
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.IsNullOrEmpty(Data) ? string.Empty : Data;
        }

        /// <summary>
        /// Serialize to Xml
        /// </summary>
        /// <returns></returns>
        public XElement ToXml()
        {
            string str = (Data ?? string.Empty).Replace("\"", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;");
            return new XElement("data", new XAttribute("id", "text"), new XAttribute("value", str));
        }
    }
}
