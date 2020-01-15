using StarDust.CasparCG.net.Connection;
using StarDust.CasparCG.net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using StartDust.CasparCG.net.Crosscutting;


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
            ServerConnection.DataReceived += ServerConnection_DataReceived;
        }

        #endregion


        #region Public Methods





        /// <inheritdoc/>
        public AMCPEventArgs SendCommandAndGetResponse(AMCPCommand command, TimeSpan? timeout = null)
        {
            return SendCommandAndGetResponse(command.ToAmcpValue(), timeout.GetValueOrDefault(TimeSpan.FromSeconds(DefaultTimeoutInSecond)));
        }


        /// <inheritdoc/>
        public void SendCommandAndCheckError(AMCPCommand command)
        {
            var error = SendCommandAndGetStatus(command);
            if (error == AMCPError.None)
                return;

            throw new InvalidOperationException($"An error {error.ToString()} occured when sending the command to the server.");
        }

        /// <inheritdoc/>
        public void SendCommandAndCheckError(string command)
        {
            var error = SendCommandAndGetStatus(command);
            if (error == AMCPError.None)
                return;

            throw new InvalidOperationException($"An error {error.ToString()} occured when sending the command to the server.");
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

            var eventWaiter = new EventAwaiter<AMCPEventArgs>(
                h => ResponseParsed += h,
                h => ResponseParsed -= h);

            ServerConnection.SendString(command);
            var data = await eventWaiter.Task;

            return data?.Error ?? AMCPError.UndefinedError;
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

            //If we received multiple response we delimiter the response by block
            var responseBlocks = _regexBlockDelimiter.Split(data);

            foreach (var block in responseBlocks)
            {
                var parsingState = new AMCPParserState?(AMCPParserState.ExpectingHeader);
                var eventArgs = new AMCPEventArgs();
                var lines = _regexCommandDelimiter.Split(block);
                lines.Aggregate(parsingState, (current, line) => ParseLine(line, current, eventArgs));
                amcpParserEventArgsList.Add(eventArgs);
                OnResponseParsed(eventArgs);
            }

            return amcpParserEventArgsList;
        }

        /// <summary>
        /// Parse a line to know what type of line was received. Header, OneLine or Multiline
        /// </summary>
        /// <param name="line"></param>
        /// <param name="state"></param>
        /// <param name="nextParserEventArgs"></param>
        protected AMCPParserState? ParseLine(string line, AMCPParserState? state, AMCPEventArgs nextParserEventArgs)
        {
            if (state == null)
                return null;

            switch (state)
            {
                case AMCPParserState.ExpectingHeader:
                    return ParseHeader(line, nextParserEventArgs);
                case AMCPParserState.ExpectingOneLineData:
                    ParseOneLineData(line, nextParserEventArgs);
                    return null;
                case AMCPParserState.ExpectingMultilineData:
                    ParseMultilineData(line, nextParserEventArgs);
                    return AMCPParserState.ExpectingMultilineData;
                default:
                    throw new NotImplementedException($"{state.ToString()}");
            }

        }

        /// <summary>
        /// Parsing for the case we received only one line
        /// </summary>
        /// <param name="line"></param>
        /// <param name="eventArgs"></param>
        protected void ParseOneLineData(string line, AMCPEventArgs eventArgs)
        {
            eventArgs.Data.Add(line);
        }

        /// <summary>
        /// Parsing when we receiving multiline
        /// </summary>
        /// <param name="line"></param>
        protected void ParseMultilineData(string line, AMCPEventArgs eventArgs)
        {
            if (line.Length == 0)
                return;
            eventArgs.Data.Add(line);
        }

        /// <summary>
        /// Parse the header line to get return code and command type result
        /// </summary>
        /// <param name="line"></param>
        /// <param name="eventArgs"></param>
        protected AMCPParserState? ParseHeader(string line, AMCPEventArgs eventArgs)
        {
            if (string.IsNullOrEmpty(line))
                return null;

            var command = line;
            var code = _regexCode.Match(line).Value;

            if (string.IsNullOrEmpty(code))
                return null;

            //If we found a code we can remove him to line to get only the command
            if (!string.IsNullOrEmpty(code))
                command = line.Replace(code, "");


            //Removing extra chars that are returned when success
            command = command.Replace("OK", "").Trim();

            //Testing the code return 100+more Information, 200+more Success operation, 400+more error
            switch (code[0])
            {
                case '1':
                    return ParseInformationalHeader(code);
                case '2':
                    return ParseSuccessHeader(command, code, eventArgs);
                case '4':
                case '5':
                    ParseErrorHeader(command, code, eventArgs);
                    return null;
                default:
                    ParseRetrieveData(line, eventArgs);
                    return AMCPParserState.ExpectingMultilineData;
            }
        }

        /// <summary>
        /// Parsing for block containing data, like CLS or TLS command
        /// </summary>
        /// <param name="line"></param>
        protected void ParseRetrieveData(string line, AMCPEventArgs eventArgs)
        {
            eventArgs.Command = AMCPCommand.DATA_RETRIEVE;
            eventArgs.Data.Add(line.Replace("\\n", "\n"));
        }

        /// <summary>
        /// Parse code in the 200 range
        /// </summary>
        /// <param name="command"></param>
        /// <param name="code"></param>
        protected AMCPParserState? ParseSuccessHeader(string command, string code, AMCPEventArgs eventArgs)
        {
            eventArgs.Command = command.TryParseFromCommandValue(AMCPCommand.Undefined);

            if (!int.TryParse(code, out var returnCode))
                return null;

            switch (returnCode)
            {
                case 200:
                    return AMCPParserState.ExpectingMultilineData;
                case 201:
                    return AMCPParserState.ExpectingOneLineData;
                default:
                    return null;
            }

        }

        /// <summary>
        /// Parse error code
        /// </summary>
        /// <param name="command"></param>
        /// <param name="code"></param>
        protected void ParseErrorHeader(string command, string code, AMCPEventArgs eventArgs)
        {
            eventArgs.Error = code.ToAMCPError();
            eventArgs.Command = command.TryParseFromCommandValue(AMCPCommand.Undefined);
        }

        /// <summary>
        /// Parse code when range is 100
        /// </summary>
        /// <param name="code"></param>
        protected AMCPParserState? ParseInformationalHeader(string code)
        {
            return int.TryParse(code, out var returnCode) && returnCode == 101 ?
                    AMCPParserState.ExpectingOneLineData :
                    default(AMCPParserState?);
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
