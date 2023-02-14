using System.Xml.Linq;

namespace StarDust.CasparCG.net.AmcpProtocol
{
    /// <summary>
    /// Data container
    /// </summary>
    public interface ICGDataContainer
    {
        /// <summary>
        /// Serialize to xml string
        /// </summary>
        /// <returns></returns>
        string ToXml();
    }
}
