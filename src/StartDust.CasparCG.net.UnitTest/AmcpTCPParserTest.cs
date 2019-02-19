using Moq;
using StarDust.CasparCG.net.AmcpProtocol;
using StarDust.CasparCG.net.Connection;
using System;
using System.Linq;
using Xunit;

namespace StartDust.CasparCG.net.UnitTest
{
    public class AmcpTCPParserTest
    {





        [Fact]
        public void Test_SendCommandAndGetResult()
        {
            Mock<IServerConnection> _mockServerConnection = new Mock<IServerConnection>();
            _mockServerConnection.Setup(con => con.Connect())
                .Callback(() => _mockServerConnection.SetupGet(p => p.IsConnected).Returns(true));
            _mockServerConnection.Setup(con => con.Connect()).Raises(t => t.ConnectionStateChanged += null, new ConnectionEventArgs("null", 0, true));
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
            Assert.True(result.Data.First() == "2.0.7.aecd9cf Stable");
        }
    }
}
