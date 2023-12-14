using Moq;
using StarDust.CasparCG.net.Connection;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StartDust.CasparCG.net.UnitTest
{
    public static class MockServerConnection
    {
        public static Mock<IServerConnection> PreConfigureServerConnection()
        {
            Mock<IServerConnection> _mockServerConnection = new Mock<IServerConnection>();
            _mockServerConnection.Setup(con => con.Connect())
                .Callback(() => _mockServerConnection.SetupGet(p => p.IsConnected).Returns(true));
            _mockServerConnection.Setup(con => con.Connect())
                .Raises(t => t.ConnectionStateChanged += null, new ConnectionEventArgs("null", 0, true));

            //Mocking version command
            _mockServerConnection
                .Setup(con => con.SendStringAsync("VERSION", It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Raises(con => con.DataReceived += null, new DatasReceivedEventArgs(string.Concat("201 VERSION OK", "\r\n", "2.0.7.aecd9cf Stable")));


            //Mocking info command
            _mockServerConnection
                .Setup(con => con.SendStringAsync("INFO", It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Raises(con => con.DataReceived += null, new DatasReceivedEventArgs(InfoReturnData()));

            //Mock TLS Command
            _mockServerConnection
                .Setup(con => con.SendStringWithResult("TLS", TimeSpan.FromSeconds(1)))
                .Returns(TemplateInfo());

            _mockServerConnection
                .Setup(con => con.SendStringWithResult("CLS", TimeSpan.FromSeconds(1)))
                .Returns(ClsData());


            return _mockServerConnection;
        }


        private static string ClsData()
        {
            var sb = new StringBuilder();
            sb.AppendLine("200 CLS OK");
            sb.AppendLine("\"4K\\ALL_STAR_GAME_PART1_2015_XAVCINTRA_[MCS-UHD03]\"  MOVIE  4211098292 20160107101950 91521 1/50");
            sb.AppendLine("\"4K\\ALL_STAR_GAME_PART3_2015_XAVCINTRA_[MCS-UHD03]\"  MOVIE  8767794540 20160109100518 190605 1/50");
            sb.AppendLine("\"4K\\AMERICA_[MCS-UHD03WTLOGO]\"  MOVIE  12424663756 20160223113510 270125 1/50");
            sb.AppendLine("\"4K\\FLAMENCO FLATLAND_[MCS-UHD03]\"  MOVIE  471068028 20160415153545 0 1/50");
            sb.AppendLine("\"4K\\MYANMAR_[MCS-UHD03WTLOGO]ENGLISH\"  MOVIE  9634663668 20160122033826 209455 1/50");
            sb.AppendLine("\"4K\\RED BULL RAMPAGE_[MCS-UHD03]\"  MOVIE  451063324 20160415153631 0 1/50");
            sb.AppendLine("\"4K\\ROCKY MOUNTAINS_[MCS-UHD03]\"  MOVIE  226106472 20151207180346 4867 1/50");
            sb.AppendLine("\"4K\\WHITE_PASSION_[MCS-UHD03]\"  MOVIE  292941976 20151208033658 6321 1/50");
            sb.AppendLine("\"AMB\"  MOVIE  6445960 20150804193335 268 1/25");
            sb.AppendLine("\"BATMANTHEKILLINGJOKE-TLR1_H1080P\"  MOVIE  100769272 20160718124417 2049 1000000/23976023");
            sb.AppendLine("\"BIG_BUCK_BUNNY_1080P_H264\"  MOVIE  725106140 20150815113124 14315 1/24");
            sb.AppendLine("\"CG1080I50\"  MOVIE  6159792 20150804193335 264 1/25");
            sb.AppendLine("\"CG1080I50_A\"  MOVIE  10298115 20150804193336 260 1/25");
            sb.AppendLine("\"FILENAME\"  MOVIE  62954230 20170201091206 429 1/25");
            sb.AppendLine("\"FILENAME2\"  MOVIE  242649738 20170201091411 1636 1/25");
            sb.AppendLine("\"GO1080P25\"  MOVIE  16694084 20150804193336 445 1/25");
            sb.AppendLine("\"INTERSTELLAR-TLR_1B-5.1CH-1080P-HDTN\"  MOVIE  84692561 20160718152545 2428 1/24");
            sb.AppendLine("\"JASONBOURNE-UK-TLR1_H1080P\"  MOVIE  166711269 20160718124012 3353 1/25");
            sb.AppendLine("\"KUBOANDTHE2STRINGS-TLR5_H1080P\"  MOVIE  72704107 20160718124440 1446 1000000/23976023");
            sb.AppendLine("\"MIRE-18DBFS\"  MOVIE  3829614 20150819230013 1574 1/25");
            sb.AppendLine("\"MIRE-24DBFS\"  MOVIE  3830699 20150820194836 1574 1/25");
            sb.AppendLine("\"MIRE-30DBFS\"  MOVIE  3830201 20150820194027 1574 1/25");
            sb.AppendLine("\"MIRE-36DBFS\"  MOVIE  3830948 20150820195433 1574 1/25");
            sb.AppendLine("\"MIRE-6DBFS\"  MOVIE  3829952 20150819231047 1574 1/25");
            sb.AppendLine("\"MIRE-XXDBFS\"  MOVIE  3830450 20150820194526 1574 1/25");
            sb.AppendLine("\"N23HD003\"  MOVIE  775326021 20141009124917 3013 1/25");
            sb.AppendLine("\"SPLIT\"  STILL  6220854 20150804193337 0 0/1");
            sb.AppendLine("\"S?QUENCE 01\"  MOVIE  112583 20150820194746 40 1/25");
            sb.AppendLine("\"TESTPATTERNS\\1080I5000_TEST\"  MOVIE  1053771 20150804193337 50 1/25");
            sb.AppendLine("\"TESTPATTERNS\\1080I5000_TEST_A\"  MOVIE  1340796 20150804193337 50 1/25");
            sb.AppendLine("\"TESTPATTERNS\\1080I5994_TEST\"  MOVIE  1250970 20150804193337 60 1/30");
            sb.AppendLine("\"TESTPATTERNS\\1080I5994_TEST_A\"  MOVIE  1592759 20150804193337 60 1/30");
            sb.AppendLine("\"TESTPATTERNS\\1080I6000_TEST\"  MOVIE  1268605 20150804193337 59 125/3747");
            sb.AppendLine("\"TESTPATTERNS\\1080I6000_TEST_A\"  MOVIE  1606243 20150804193337 59 125/3747");
            sb.AppendLine("\"TESTPATTERNS\\1080P2398_TEST\"  MOVIE  608718 20150804193337 47 1000000/23976023");
            sb.AppendLine("\"TESTPATTERNS\\1080P2398_TEST_A\"  MOVIE  776850 20150804193337 47 1000000/23976023");
            sb.AppendLine("\"TESTPATTERNS\\1080P2400_TEST\"  MOVIE  607748 20150804193337 48 1/24");
            sb.AppendLine("\"TESTPATTERNS\\1080P2400_TEST_A\"  MOVIE  777272 20150804193337 48 1/24");
            sb.AppendLine("\"TESTPATTERNS\\1080P2500_TEST\"  MOVIE  634626 20150804193337 50 1/25");
            sb.AppendLine("\"TESTPATTERNS\\1080P2500_TEST_A\"  MOVIE  809223 20150804193337 50 1/25");
            sb.AppendLine("\"TESTPATTERNS\\1080P2997_TEST\"  MOVIE  760365 20150804193337 59 125/3747");
            sb.AppendLine("\"TESTPATTERNS\\1080P2997_TEST_A\"  MOVIE  970665 20150804193338 59 125/3747");
            sb.AppendLine("\"TESTPATTERNS\\1080P3000_TEST\"  MOVIE  759386 20150804193338 60 1/30");
            sb.AppendLine("\"TESTPATTERNS\\1080P3000_TEST_A\"  MOVIE  971151 20150804193338 60 1/30");
            sb.AppendLine("\"TESTPATTERNS\\1080P5000_TEST\"  MOVIE  1267714 20150804193338 100 1/50");
            sb.AppendLine("\"TESTPATTERNS\\1080P5000_TEST_A\"  MOVIE  1615895 20150804193338 100 1/50");
            sb.AppendLine("\"TESTPATTERNS\\1080P5994_TEST\"  MOVIE  817311 20150804193338 59 125/3747");
            sb.AppendLine("\"TESTPATTERNS\\1080P5994_TEST_A\"  MOVIE  1004809 20150804193338 59 125/3747");
            sb.AppendLine("\"TESTPATTERNS\\1080P6000_TEST\"  MOVIE  1517716 20150804193338 120 1/60");
            sb.AppendLine("\"TESTPATTERNS\\1080P6000_TEST_A\"  MOVIE  1939533 20150804193338 120 1/60");
            sb.AppendLine("\"TESTPATTERNS\\1080_TEST\"  STILL  8294444 20150804193338 0 0/1");
            sb.AppendLine("\"TESTPATTERNS\\2160P2398_TEST\"  MOVIE  725131 20150804193338 47 1000000/23976023");
            sb.AppendLine("\"TESTPATTERNS\\2160P2398_TEST_A\"  MOVIE  798765 20150804193338 47 1000000/23976023");
            sb.AppendLine("\"TESTPATTERNS\\2160P2400_TEST\"  MOVIE  724935 20150804193338 48 1/24");
            sb.AppendLine("\"TESTPATTERNS\\2160P2400_TEST_A\"  MOVIE  798572 20150804193338 48 1/24");
            sb.AppendLine("\"TESTPATTERNS\\2160P2500_TEST\"  MOVIE  755704 20150804193338 50 1/25");
            sb.AppendLine("\"TESTPATTERNS\\2160P2500_TEST_A\"  MOVIE  831973 20150804193338 50 1/25");
            sb.AppendLine("\"TESTPATTERNS\\2160P2997_TEST\"  MOVIE  905285 20150804193338 59 125/3747");

            return sb.ToString();
        }

        private static string InfoReturnData()
        {
            var sb = new StringBuilder();
            sb.AppendLine("200 INFO OK");
            sb.Append("1 PAL PLAYING");

            return sb.ToString();
        }

        private static string TemplateInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine("200 TLS OK");
            sb.AppendLine("\"CasparCG_Flash_Templates_Example_Pack_1/ADVANCEDTEMPLATE1\" 30327 20170202153053");
            sb.AppendLine("\"CasparCG_Flash_Templates_Example_Pack_1/ADVANCEDTEMPLATE2\" 49578 20170202153053");
            sb.AppendLine("\"CasparCG_Flash_Templates_Example_Pack_1/SIMPLETEMPLATE1\" 18606 20170202153053");
            sb.AppendLine("\"CasparCG_Flash_Templates_Example_Pack_1/SIMPLETEMPLATE2\" 1751565 20170202153054");
            sb.AppendLine("\"CASPAR_TEXT\" 19920 20170202153053");
            sb.AppendLine("\"FRAME\" 244156 20170202153053");
            sb.AppendLine("\"NTSC-TEST-30\" 37275 20170202153053");
            sb.AppendLine("\"NTSC-TEST-60\" 37274 20170202153053");
            sb.AppendLine("\"PHONE\" 1442360 20170202153053");


            return sb.ToString();
        }
    }
}
