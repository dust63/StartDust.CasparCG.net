namespace StarDust.CasparCG.net.AmcpProtocol
{
    /// <summary>
    /// State of parsing
    /// </summary>
    public enum AMCPParserState
    {
        /// <summary>
        /// Expecting header
        /// </summary>
        ExpectingHeader,

        /// <summary>
        /// Expecting one line data
        /// </summary>
        ExpectingOneLineData,

        /// <summary>
        /// Expecting multiline data
        /// </summary>
        ExpectingMultilineData,
    }
}
