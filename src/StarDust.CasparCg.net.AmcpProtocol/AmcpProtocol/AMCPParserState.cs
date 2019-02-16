namespace StarDust.CasparCG.net.AmcpProtocol
{
  public enum AMCPParserState
  {
    ExpectingHeader,
    ExpectingOneLineData,
    ExpectingMultilineData,
  }
}
