namespace StarDust.CasparCG.net.Models.Diag
{
    /// <summary>
    /// Log level definition
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// Trace
        /// </summary>
        [AMCPCommandValue("trace")]
        TRACE,

        /// <summary>
        /// Debug
        /// </summary>
        [AMCPCommandValue("debug")]
        DEBUG,

        /// <summary>
        /// Information
        /// </summary>
        [AMCPCommandValue("info")]
        INFO,

        /// <summary>
        /// Warning
        /// </summary>
        [AMCPCommandValue("warning")]
        WARNING,

        /// <summary>
        /// Error
        /// </summary>
        [AMCPCommandValue("error")]
        ERROR,

        /// <summary>
        /// Fatal
        /// </summary>
        [AMCPCommandValue("fatal")]
        FATAL
    }
}
