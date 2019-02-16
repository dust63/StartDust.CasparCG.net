namespace StarDust.CasparCG.net.AmcpProtocol
{
  public interface ICGDataContainer
  {
    string ToXml();

    string ToAMCPEscapedXml();
  }
}
