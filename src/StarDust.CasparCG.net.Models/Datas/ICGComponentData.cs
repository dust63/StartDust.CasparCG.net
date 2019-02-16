using System.Text;

namespace StarDust.CasparCG.net.Models
{
  public interface ICGComponentData
  {
    void ToAMCPEscapedXml(StringBuilder sb);

    void ToXml(StringBuilder sb);
  }
}
