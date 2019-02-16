namespace StarDust.CasparCG.AmcpProtocol
{
  public interface ICGDataContainer
  {
    string ToXml();

    string ToAMCPEscapedXml();
  }
}
