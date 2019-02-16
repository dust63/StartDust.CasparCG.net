namespace StarDust.CasparCG.Models.Diag
{
    public enum LogLevel
    {

        [AMCPCommandValue("trace")]
        TRACE,

        [AMCPCommandValue("debug")]
        DEBUG,


        [AMCPCommandValue("info")]
        INFO,

        [AMCPCommandValue("warning")]
        WARNING,

        [AMCPCommandValue("error")]
        ERROR,


        [AMCPCommandValue("fatal")]
        FATAL

    }
}
