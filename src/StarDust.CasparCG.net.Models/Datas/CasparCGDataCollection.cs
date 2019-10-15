using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using StarDust.CasparCG.net.AmcpProtocol;
using StarDust.CasparCG.net.Models;

namespace StarDust.CasparCG.net.Datas
{

    /// <summary>
    /// Collection of key value pair of data to sent to template.
    /// </summary>
    public class CasparCGDataCollection : Dictionary<string, ICGComponentData>, ICGDataContainer
    {
        /// <summary>
        /// Add a text filed data
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value">text to sent</param>
        public void Add(string name, string value)
        {
            Add(name, new CGTextFieldData(value));
        }


        /// <summary>
        /// Get value if exists if not return null.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new ICGComponentData this[string key]
        {
            get
            {
                if (!string.IsNullOrEmpty(key) && ContainsKey(key))
                    return base[key];

                return null;
            }
            set => base[key] = value;
        }    

        /// <summary>
        /// Transform the collection to xml data to sent to the server
        /// </summary>
        /// <returns></returns>
        public string ToXml()
        {
            var xml = new XElement("templateData");                                    
            
            foreach (var compData in Keys.Select(key => new XElement("componentData", new XAttribute("id", key), this[key].ToXml())))
            {
                xml.Add(compData);
            }           

            //Amcp support only inline xml and quote prefix by backslash
            return xml.ToString()
                .Replace("\r\n","")
                 .Replace("\t", "")
                .Replace("\"", "\\\"");
        }


    }
}
