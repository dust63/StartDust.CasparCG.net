namespace StarDust.CasparCG.net.AmcpProtocol
{
    /// <summary>
    /// AMCP Error code that can be sent by the server
    /// </summary>
    public enum AMCPError
    {
        /// <summary>
        /// UNDEFINED
        /// </summary>
        UndefinedError = 0,

        /// <summary>
        /// NONE
        /// </summary>
        None = 1,

        /// <summary>
        /// Invalid command received
        /// </summary>
        InvalidCommand = 400,

        /// <summary>
        /// Invalid Channel
        /// </summary>
        InvalidChannel = 401,

        /// <summary>
        /// Missing parameter in the command
        /// </summary>
        MissingParameter = 402,

        /// <summary>
        /// Invalid parameter
        /// </summary>
        InvalidParameter = 403,

        /// <summary>
        /// File not found
        /// </summary>
        FileNotFound = 404,

        /// <summary>
        /// Internal Server Error
        /// </summary>
        InternalServerError = 500,

        /// <summary>
        /// Invalid File
        /// </summary>
        InvalidFile = 502,
    }
}
