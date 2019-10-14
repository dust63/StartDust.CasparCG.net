using StarDust.CasparCG.net.Connection;
using StarDust.CasparCG.net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


namespace StarDust.CasparCG.net.AmcpProtocol
{
    /// <summary>
    /// Class in charge to listen the Server response and transform data received in AMCPEventArgs object
    /// </summary>
    public class AmcpTCPParser : IAMCPTcpParser
    {

        #region Fields

        private const string ConstCommandDelimiter = "\r\n";
        private const string ConstBlockDelimiter = "\r\n\r\n";

        private readonly Regex _regexBlockDelimiter = new Regex(ConstBlockDelimiter, RegexOptions.Compiled);
        private readonly Regex _regexCommandDelimiter = new Regex(ConstCommandDelimiter, RegexOptions.Compiled);
        private readonly Regex _regexCode = new Regex(@"[1-5][0][0-4]", RegexOptions.Compiled);
        private AMCPParserState ParsingState { get; set; } = AMCPParserState.ExpectingHeader;
        private AMCPEventArgs _nextParserEventArgs = new AMCPEventArgs();

        #endregion


        #region Properties

        /// <inheritdoc/>
        public int DefaultTimeoutInSecond { get; set; } = 1;


        /// <inheritdoc/>
        public IServerConnection ServerConnection { get; private set; }
        /// <inheritdoc/>
        public string CommandDelimiter => ConstCommandDelimiter;
        /// <inheritdoc/>
        public string BlockDelimiter => ConstBlockDelimiter;
        /// <inheritdoc/>
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
            ServerConnection.DatasReceived += ServerConnection_DataReceived;
        }

        #endregion


        #region Public Methods





        /// <inheritdoc/>
        public AMCPEventArgs SendCommandAndGetResponse(AMCPCommand command, TimeSpan? timeout = null)
        {
            return SendCommandAndGetResponse(command.ToAmcpValue(), timeout.GetValueOrDefault(TimeSpan.FromSeconds(DefaultTimeoutInSecond)));
        }


        /// <inheritdoc/>
        public bool SendCommand(AMCPCommand command)
        {
            return SendCommandAndGetStatus(command) == AMCPError.None;
        }

        /// <inheritdoc/>
        public bool SendCommand(string command)
        {
            return SendCommandAndGetStatus(command) == AMCPError.None;
        }

        /// <inheritdoc/>
        public AMCPError SendCommandAndGetStatus(AMCPCommand command)
        {
            return SendCommandAndGetStatus(command.ToAmcpValue());
        }

        /// <inheritdoc/>
        public AMCPError SendCommandAndGetStatus(string command)
        {
            return SendCommandAndGetStatusAsync(command).GetAwaiter().GetResult();
        }

        public async Task<AMCPError> SendCommandAndGetStatusAsync(string command)
        {
            AMCPEventArgs data = null;
            using (var signal = new SemaphoreSlim(0, 1))
            {
                void Handler(object s, AMCPEventArgs e)
                {
                    data = e;
                    signal.Release();
                }

                ResponseParsed += Handler;

                ServerConnection.SendString(command);

                await signal.WaitAsync(TimeSpan.FromSeconds(DefaultTimeoutInSecond));
                ResponseParsed -= Handler;

                return data?.Error ?? AMCPError.UndefinedError;
            }
        }

        /// <inheritdoc/>
        public AMCPEventArgs SendCommandAndGetResponse(string command, TimeSpan? timeout = null)
        {
            var message = ServerConnection.SendStringWithResult(command, TimeSpan.FromSeconds(DefaultTimeoutInSecond));
            var data = Parse(message).FirstOrDefault();
            return data;
        }

        #endregion


        #region Private Methods


        /// <summary>
        /// Handler to listen response from Server connection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ServerConnection_DataReceived(object sender, DatasReceivedEventArgs e)
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
            var amcpParserEventArgsList = new List<AMCPEventArgs>();
            if (string.IsNullOrEmpty(data))
                return amcpParserEventArgsList;

            var splitData = _regexBlockDelimiter.Split(data);

            foreach (var input in splitData)
            {
                _nextParserEventArgs = new AMCPEventArgs();

                var strArray = _regexCommandDelimiter.Split(input);

                for (var index = 0; index < strArray.Length; ++index)
                    ParseLine(strArray[index], index + 1 == strArray.Length);

                amcpParserEventArgsList.Add(_nextParserEventArgs);
            }

            return amcpParserEventArgsList;
        }

        /// <summary>
        /// Parse a line to know what type of line was received. Header, OneLine or Multiline
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
            OnResponseParsed(_nextParserEventArgs);
            ParsingState = AMCPParserState.ExpectingHeader;
        }

        /// <summary>
        /// Parsing for the case we received only one line
        /// </summary>
        /// <param name="line"></param>
        protected void ParseOneLineData(string line)
        {
            _nextParserEventArgs.Data.Add(line);
        }

        /// <summary>
        /// Parsing when we receiving multiline
        /// </summary>
        /// <param name="line"></param>
        protected void ParseMultilineData(string line)
        {
            if (line.Length == 0)
                return;
            _nextParserEventArgs.Data.Add(line);
        }

        /// <summary>
        /// Parse the header line to get return code and command type result
        /// </summary>
        /// <param name="line"></param>
        protected void ParseHeader(string line)
        {
            if (string.IsNullOrEmpty(line))
                return;

            var command = line;
            var code = _regexCode.Match(line).Value;

            if (string.IsNullOrEmpty(code))
                return;

            //If we found a code we can remove him to line to get only the command
            if (!string.IsNullOrEmpty(code))
                command = line.Replace(code, "");


            //Removing extra chars that are returned when success
            command = command.Replace("OK", "").Trim();

            //Testing the code return 100+more Information, 200+more Success operation, 400+more error
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
        /// Parsing for block containing data, like CLS or TLS command
        /// </summary>
        /// <param name="line"></param>
        protected void ParseRetrieveData(string line)
        {
            _nextParserEventArgs.Command = AMCPCommand.DATA_RETRIEVE;
            _nextParserEventArgs.Data.Add(line.Replace("\\n", "\n"));
        }

        /// <summary>
        /// Parse code in the 200 range
        /// </summary>
        /// <param name="command"></param>
        /// <param name="code"></param>
        protected void ParseSuccessHeader(string command, string code)
        {

            _nextParserEventArgs.Command = command.TryParseFromCommandValue(AMCPCommand.Undefined);

            if (!int.TryParse(code, out var returnCode))
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

        /// <summary>
        /// Parse error code
        /// </summary>
        /// <param name="command"></param>
        /// <param name="code"></param>
        protected void ParseErrorHeader(string command, string code)
        {
            _nextParserEventArgs.Error = code.ToAMCPError();
            _nextParserEventArgs.Command = command.TryParseFromCommandValue(AMCPCommand.Undefined);
        }

        /// <summary>
        /// Parse code when range is 100
        /// </summary>
        /// <param name="code"></param>
        protected void ParseInformationalHeader(string code)
        {

            if (!int.TryParse(code, out var returnCode))
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
