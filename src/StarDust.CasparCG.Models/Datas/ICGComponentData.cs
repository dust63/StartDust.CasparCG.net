using System.Text;

namespace StarDust.CasparCG.Models
{
  public interface ICGComponentData
  {
    void ToAMCPEscapedXml(StringBuilder sb);

    void ToXml(StringBuilder sb);
  }
}
