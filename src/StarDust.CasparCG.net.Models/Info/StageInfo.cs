using System.Collections.Generic;
using System.Xml.Serialization;

namespace StarDust.CasparCG.net.Models.Info
{
    /// <summary>
    /// Stage info
    /// </summary>

    [XmlRoot(ElementName = "stage")]
    public class StageInfo
    {
        /// <summary>
        /// Layer informations
        /// </summary>
        [XmlArray("layers")]
        [XmlArrayItem("layer")]
        public List<LayerInfo> Layers { get; set; }
    }
}
