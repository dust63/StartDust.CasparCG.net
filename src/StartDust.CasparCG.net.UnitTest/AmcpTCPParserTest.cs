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

       



        [Fact]
        public void Test_SendCommandAndGetResult()
        {
            Mock<IServerConnection> _mockServerConnection = MockServerConnection.PreConfigureServerConnection();
            _mockServerConnection
                .Setup(con => con.SendStringWithResult("VERSION", TimeSpan.FromSeconds(1)))
                .Returns(string.Concat("201 VERSION OK", "\r\n", "2.0.7.aecd9cf Stable"));


            var amcpParser = new AmcpTCPParser(_mockServerConnection.Object)
            {
                DefaultTimeoutInSecond = 1
            };
            var result = amcpParser.SendCommandAndGetResponse("VERSION");


            Assert.True(result.Data.Count == 1);
            Assert.True(result.Command == AMCPCommand.VERSION);
            Assert.True(result.Error == AMCPError.None);
            Assert.True(result.Data.First() == "2.0.7.aecd9cf Stable");



            var resultNull = amcpParser.SendCommandAndGetResponse("BAD COMMAND");
            Assert.True(resultNull == null);
        }


        [Fact]
        public void Test_ParsedEvent()
        {
            Mock<IServerConnection> _mockServerConnection =  MockServerConnection.PreConfigureServerConnection();
          

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
           

            Mock<IServerConnection> _mockServerConnection =  MockServerConnection.PreConfigureServerConnection();
          
            var amcpParser = new AmcpTCPParser(_mockServerConnection.Object)
            {
                DefaultTimeoutInSecond = 1
            };
            var result = amcpParser.SendCommandAndGetResponse("TLS");


            Assert.True(result.Data.Count == 9);
            Assert.True(result.Command == AMCPCommand.TLS);
            Assert.True(result.Error == AMCPError.None);
            Assert.True(result.Data.First() == "\"CasparCG_Flash_Templates_Example_Pack_1/ADVANCEDTEMPLATE1\" 30327 20170202153053");

        }



    }
}
