using StarDust.CasparCG.net.Connection;
using System;


namespace StarDust.CasparCG.net.AmcpProtocol
{

    /// <summary>
    /// In charge to get data received in TCP, parse them to get command info and data
    /// </summary>
    public interface IAmcpTcpParser
    {
        /// <summary>
        /// Default timeout when we want to send a command and get result
        /// </summary>
        int DefaultTimeoutInSecond { get; set; }

        /// <summary>
        /// A tcp connection to CasparCG Server
        /// </summary>
        IServerConnection ServerConnection { get; }

        /// <summary>
        /// Identifier for each command
        /// </summary>
        string CommandDelimiter { get; }

        /// <summary>
        /// Identifier to extract data block
        /// </summary>
        string BlockDelimiter { get; }


        /// <summary>
        /// When a data was parsed successfully
        /// </summary>
        event EventHandler<AMCPEventArgs> ResponseParsed;


        /// <summary>
        /// Send command and check if we found an error. if yes throw an exception
        /// </summary>
        /// <param name="command"></param>
        void SendCommandAndCheckError(AMCPCommand command);


        /// <summary>
        /// Send command and check if we found an error. if yes throw an exception
        /// </summary>
        /// <param name="command"></param>
        void SendCommandAndCheckError(string command);

        /// <summary>
        /// Send a command and get error status. If true then success
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        bool SendCommand(AMCPCommand command);

        /// <summary>
        /// Send a command and get error status. If true then success
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        bool SendCommand(string command);
        

        /// <summary>
        /// Send a command and get error status. If none then success
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        AMCPError SendCommandAndGetStatus(AMCPCommand command);

        /// <summary>
        /// Send a command and get error status. If none then success
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        AMCPError SendCommandAndGetStatus(string command);


        /// <summary>
        ///  Send a command and get error status and get response parsed
        /// </summary>
        /// <param name="command"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        AMCPEventArgs SendCommandAndGetResponse(AMCPCommand command, TimeSpan? timeout = null);


        /// <summary>
        ///  Send a command and get error status and get response parsed
        /// </summary>
        /// <param name="command"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        AMCPEventArgs SendCommandAndGetResponse(string command, TimeSpan? timeout = null);


    }
}
