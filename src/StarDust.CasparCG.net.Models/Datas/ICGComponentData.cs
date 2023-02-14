using System.Text;
using System.Xml.Linq;

namespace StarDust.CasparCG.net.Models
{
    /// <summary>
    /// Represent a data
    /// </summary>
    public interface ICGComponentData
    {
        /// <summary>
        /// Serialize to Xml
        /// </summary>
        /// <returns></returns>
        XElement ToXml();
    }
}
