using System.Text;
using System.Xml.Linq;

namespace StarDust.CasparCG.net.Models
{
    public interface ICGComponentData
    {
        XElement ToXml();
    }
}
