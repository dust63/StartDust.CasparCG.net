using StarDust.CasparCG.net.Connection;
using StarDust.CasparCG.net.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace StarDust.CasparCG.net.AmcpProtocol
{
    /// <summary>
    /// Class in charge to listen the ServerResponsed and transform datas received in AMCPEventArgs object
    /// </summary>
    public class AmcpTCPParser : IAMCPTcpParser
    {

        #region Fields

        const string ConstCommandDelimiter = "\r\n";
        const string ConstBlockDelimiter = "\r\n\r\n";

        private readonly Regex regexBlockDelimiter = new Regex(ConstBlockDelimiter, RegexOptions.Compiled);
        private readonly Regex regexCommandkDelimiter = new Regex(ConstCommandDelimiter, RegexOptions.Compiled);
        private readonly Regex regexCode = new Regex(@"[1-5][0][0-4]", RegexOptions.Compiled);
        private AMCPParserState ParsingState { get; set; } = AMCPParserState.ExpectingHeader;
        private AMCPEventArgs nextParserEventArgs = new AMCPEventArgs();

        #endregion


        #region Properties

        /// <inheritdoc cref=""/>
        public int DefaultTimeoutInSecond { get; set; } = 1;


        /// <inheritdoc cref=""/>
        public IServerConnection ServerConnection { get; private set; }
        /// <inheritdoc cref=""/>
        public string CommandDelimiter => ConstCommandDelimiter;
        /// <inheritdoc cref=""/>
        public string BlockDelimiter => ConstBlockDelimiter;
        /// <inheritdoc cref=""/>
        public event EventHandler<AMCPEventArgs> ResponseParsed;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="serverConnection">Caspar CG Tcp Server connection to listen</param>
        public AmcpTCPParser(IServerConnection serverConnection)
        {
            ServerConnection = serverConnection;
            ServerConnection.DatasReceived += ServerConnection_DatasReceived;
        }

        #endregion


        #region Public Methods



        /// <inheritdoc cref=""/>
        public AMCPEventArgs SendCommandAndParse(string command)
        {
            return Parse(ServerConnection.SendStringWithResult(command, TimeSpan.FromSeconds(DefaultTimeoutInSecond)))
                .FirstOrDefault(x =>
                    string.Equals(x.Command.ToString(), command.Split().First(), StringComparison.InvariantCultureIgnoreCase));
        }

        /// <inheritdoc cref=""/>
        public AMCPEventArgs SendCommandAndGetResponse(AMCPCommand command, TimeSpan? timeout)
        {
            return SendCommandAndGetResponse(command.ToAmcpValue(), timeout.GetValueOrDefault(TimeSpan.FromSeconds(DefaultTimeoutInSecond)));
        }


        /// <inheritdoc cref=""/>
        public bool SendCommand(AMCPCommand command)
        {
            return SendCommandAndGetStatus(command) == AMCPError.None;
        }

        /// <inheritdoc cref=""/>
        public bool SendCommand(string command)
        {
            return SendCommandAndGetStatus(command) == AMCPError.None;
        }

        /// <inheritdoc cref=""/>
        public AMCPError SendCommandAndGetStatus(AMCPCommand command)
        {
            return SendCommandAndGetStatus(command.ToAmcpValue());
        }

        /// <inheritdoc cref=""/>
        public AMCPError SendCommandAndGetStatus(string command)
        {
            var internalTimeout = TimeSpan.FromSeconds(DefaultTimeoutInSecond);
            AMCPEventArgs datas = null;
            void handler(object s, AMCPEventArgs e) => datas = e;
            ResponseParsed += handler;
            ServerConnection.SendString(command);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (datas == null && stopwatch.Elapsed < internalTimeout)
                Thread.Sleep(10);
            ResponseParsed -= handler;
            return datas?.Error ?? AMCPError.UndefinedError;
        }

        /// <inheritdoc cref=""/>
        public AMCPEventArgs SendCommandAndGetResponse(string command, TimeSpan? timeout)
        {
            var internalTimeout = timeout ?? TimeSpan.FromSeconds(DefaultTimeoutInSecond);
            AMCPEventArgs datas = null;
            void handler(object s, AMCPEventArgs e) => datas = e;
            ResponseParsed += handler;
            ServerConnection.SendString(command);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (datas == null && stopwatch.Elapsed < internalTimeout)
                Thread.Sleep(10);

            ResponseParsed -= handler;
            return datas;
        }

        #endregion


        #region Private Methods


        /// <summary>
        /// Handler to listen reponse from Server connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ServerConnection_DatasReceived(object sender, DatasReceivedEventArgs e)
        {
            Parse(e.Datas);
        }

        /// <summary>
        /// Get TCP Message, parse it to retrieve block for command send
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected IEnumerable<AMCPEventArgs> Parse(string data)
        {
            List<AMCPEventArgs> amcpParserEventArgsList = new List<AMCPEventArgs>();
            if (string.IsNullOrEmpty(data))
                return amcpParserEventArgsList;

            var splidatas = regexBlockDelimiter.Split(data);

            foreach (string input in splidatas)
            {
                nextParserEventArgs = new AMCPEventArgs();

                var strArray = regexCommandkDelimiter.Split(input);

                for (int index = 0; index < strArray.Length; ++index)
                    ParseLine(strArray[index], index + 1 == strArray.Length);

                amcpParserEventArgsList.Add(nextParserEventArgs);
            }

            return amcpParserEventArgsList;
        }

        /// <summary>
        /// Parse a line to know wich type of line was received. Header, OneLine or Multiline
        /// </summary>
        /// <param name="line"></param>
        /// <param name="isEndOfMessage"></param>
        protected void ParseLine(string line, bool isEndOfMessage)
        {
            switch (ParsingState)
            {
                case AMCPParserState.ExpectingHeader:
                    ParseHeader(line);
                    break;
                case AMCPParserState.ExpectingOneLineData:
                    ParseOneLineData(line);
                    break;
                case AMCPParserState.ExpectingMultilineData:
                    ParseMultilineData(line);
                    break;
            }
            if (!isEndOfMessage)
                return;
            OnResponseParsed(nextParserEventArgs);
            ParsingState = AMCPParserState.ExpectingHeader;
        }

        /// <summary>
        /// Parsing for the case we received only one line
        /// </summary>
        /// <param name="line"></param>
        protected void ParseOneLineData(string line)
        {
            nextParserEventArgs.Data.Add(line);
        }

        /// <summary>
        /// Parsing when we receiving multiline
        /// </summary>
        /// <param name="line"></param>
        protected void ParseMultilineData(string line)
        {
            if (line.Length == 0)
                return;
            nextParserEventArgs.Data.Add(line);
        }

        /// <summary>
        /// Parse the header line to get return code and command type result
        /// </summary>
        /// <param name="line"></param>
        protected void ParseHeader(string line)
        {
            if (string.IsNullOrEmpty(line))
                return;

            string command = line;
            string code = regexCode.Match(line).Value;

            if (string.IsNullOrEmpty(code))
                return;

            //If we found a code we can remove him to line to get only the command
            if (!string.IsNullOrEmpty(code))
                command = line.Replace(code, "");


            //Removing extra chars that are returned when success
            command = command.Replace("OK", "").Trim();

            ///Testing the code return 100+more Information, 200+more Success operation, 400+more error
            switch (code[0])
            {
                case '1':
                    ParseInformationalHeader(code);
                    break;
                case '2':
                    ParseSuccessHeader(command, code);
                    break;
                case '4':
                case '5':
                    ParseErrorHeader(command, code);
                    break;
                default:
                    ParseRetrieveData(line);
                    break;
            }
        }

        /// <summary>
        /// Parsing for block containing datas, like CLS or TLS command
        /// </summary>
        /// <param name="line"></param>
        protected void ParseRetrieveData(string line)
        {
            nextParserEventArgs.Command = AMCPCommand.DATA_RETRIEVE;
            nextParserEventArgs.Data.Add(line.Replace("\\n", "\n"));
        }


        protected void ParseSuccessHeader(string command, string code)
        {

            nextParserEventArgs.Command = command.TryParseFromCommandValue(AMCPCommand.Undefined);

            if (!int.TryParse(code, out int returnCode))
                return;

            switch (returnCode)
            {
                case 200:
                    ParsingState = AMCPParserState.ExpectingMultilineData;
                    return;
                case 201:
                    ParsingState = AMCPParserState.ExpectingOneLineData;
                    return;
                default:
                    return;
            }

        }

        protected void ParseErrorHeader(string command, string code)
        {
            nextParserEventArgs.Error = code.ToAMCPError();

            object commandParsed = AMCPCommand.Undefined;
            nextParserEventArgs.Command = command.TryParseFromCommandValue(AMCPCommand.Undefined);
        }

        protected void ParseInformationalHeader(string code)
        {

            if (!int.TryParse(code, out int returnCode))
                return;

            if (returnCode == 101)
                ParsingState = AMCPParserState.ExpectingOneLineData;
        }

        /// <summary>
        /// Call when a response is parsed completely
        /// </summary>
        /// <param name="args"></param>
        protected void OnResponseParsed(AMCPEventArgs args)
        {

            ResponseParsed?.Invoke(this, args);
        }



        #endregion
    }
}
