namespace StarDust.CasparCG.AmcpProtocol
{
  public enum AMCPParserState
  {
    ExpectingHeader,
    ExpectingOneLineData,
    ExpectingMultilineData,
  }
}
