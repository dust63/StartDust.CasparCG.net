namespace StarDust.CasparCG.net.Models.Diag
{
    /// <summary>
    /// Category of log
    /// </summary>
    public enum LogCategory
    {
        /// <summary>
        /// call trace
        /// </summary>
        [AMCPCommandValue("calltrace")]
        CALLTRACE,

        /// <summary>
        /// Communication
        /// </summary>
        [AMCPCommandValue("communication")]
        COMMUNICATION,

    }
}
