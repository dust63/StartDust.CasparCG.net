using Moq;
using StarDust.CasparCG.net.AmcpProtocol;
using StarDust.CasparCG.net.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace StartDust.CasparCG.net.UnitTest
{
    public class AmcpTCPParserTest
    {

        public Mock<IServerConnection> PreConfigureServerConnection()
        {
            Mock<IServerConnection> _mockServerConnection = new Mock<IServerConnection>();
            _mockServerConnection.Setup(con => con.Connect())
                .Callback(() => _mockServerConnection.SetupGet(p => p.IsConnected).Returns(true));
            _mockServerConnection.Setup(con => con.Connect())
                .Raises(t => t.ConnectionStateChanged += null, new ConnectionEventArgs("null", 0, true));
            return _mockServerConnection;
        }



        [Fact]
        public void Test_SendCommandAndGetResult()
        {
            Mock<IServerConnection> _mockServerConnection = PreConfigureServerConnection();
            _mockServerConnection
                .Setup(con => con.SendStringWithResult("VERSION", TimeSpan.FromSeconds(1)))
                .Returns(string.Concat("201 VERSION OK", "\r\n", "2.0.7.aecd9cf Stable"));


            var amcpParser = new AmcpTCPParser(_mockServerConnection.Object)
            {
                DefaultTimeoutInSecond = 1
            };
            var result = amcpParser.SendCommandAndParse("VERSION");


            Assert.True(result.Data.Count == 1);
            Assert.True(result.Command == AMCPCommand.VERSION);
            Assert.True(result.Error == AMCPError.None);
            Assert.True(result.Data.First() == "2.0.7.aecd9cf Stable");



            var resultNull = amcpParser.SendCommandAndParse("BAD COMMAND");
            Assert.True(resultNull == null);
        }


        [Fact]
        public void Test_ParsedEvent()
        {
            Mock<IServerConnection> _mockServerConnection = PreConfigureServerConnection();
            _mockServerConnection
                .Setup(con => con.SendString("VERSION"))
                .Raises(con => con.DatasReceived += null, new DatasReceivedEventArgs(string.Concat("201 VERSION OK", "\r\n", "2.0.7.aecd9cf Stable")));

            _mockServerConnection
                .Setup(con => con.SendString("INFO"))
                .Raises(con => con.DatasReceived += null, new DatasReceivedEventArgs(string.Concat("201 INFO OK", "\r\n", "1 PAL PLAYING")));
             


            var amcpParser = new AmcpTCPParser(_mockServerConnection.Object)
            {
                DefaultTimeoutInSecond = 1
            };




            var results = new List<AMCPEventArgs>();
            amcpParser.ResponseParsed += (s, e) =>
            {
                results.Add(e);
            };
            amcpParser.SendCommand("VERSION");


            Assert.True(results.Count == 1);
            Assert.True(results.Single().Command == AMCPCommand.VERSION);
            Assert.True(results.Single().Error == AMCPError.None);
            Assert.True(results.Single().Data.First() == "2.0.7.aecd9cf Stable");

            amcpParser.SendCommand("INFO");
            Assert.True(results.Count == 2);
            Assert.True(results.Last().Command == AMCPCommand.INFO);
            Assert.True(results.Last().Error == AMCPError.None);
            Assert.True(results.Last().Data.First() == "1 PAL PLAYING");

        }




        [Fact]
        public void Test_MultiLineDatasParsing()
        {
            var sb = new StringBuilder();
            sb.Append("\"CasparCG_Flash_Templates_Example_Pack_1/ADVANCEDTEMPLATE1\" 30327 20170202153053");
            sb.Append("\r\n");
            sb.Append("\"CasparCG_Flash_Templates_Example_Pack_1/ADVANCEDTEMPLATE2\" 49578 20170202153053");
            sb.Append("\r\n");
            sb.Append("\"CasparCG_Flash_Templates_Example_Pack_1/SIMPLETEMPLATE1\" 18606 20170202153053");
            sb.Append("\r\n");
            sb.Append("\"CasparCG_Flash_Templates_Example_Pack_1/SIMPLETEMPLATE2\" 1751565 20170202153054");
            sb.Append("\r\n");
            sb.Append("\"CASPAR_TEXT\" 19920 20170202153053");
            sb.Append("\r\n");
            sb.Append("\"FRAME\" 244156 20170202153053");
            sb.Append("\r\n");
            sb.Append("\"NTSC-TEST-30\" 37275 20170202153053");
            sb.Append("\r\n");
            sb.Append("\"NTSC-TEST-60\" 37274 20170202153053");
            sb.Append("\r\n");
            sb.Append("\"PHONE\" 1442360 20170202153053");
            sb.Append("\r\n");

            Mock<IServerConnection> _mockServerConnection = PreConfigureServerConnection();
            _mockServerConnection
                .Setup(con => con.SendStringWithResult("TLS", TimeSpan.FromSeconds(1)))
                .Returns(string.Concat("200 TLS OK", "\r\n", sb.ToString()));



            var amcpParser = new AmcpTCPParser(_mockServerConnection.Object)
            {
                DefaultTimeoutInSecond = 1
            };
            var result = amcpParser.SendCommandAndParse("TLS");


            Assert.True(result.Data.Count == 9);
            Assert.True(result.Command == AMCPCommand.TLS);
            Assert.True(result.Error == AMCPError.None);
            Assert.True(result.Data.First() == "\"CasparCG_Flash_Templates_Example_Pack_1/ADVANCEDTEMPLATE1\" 30327 20170202153053");

        }



    }
}
