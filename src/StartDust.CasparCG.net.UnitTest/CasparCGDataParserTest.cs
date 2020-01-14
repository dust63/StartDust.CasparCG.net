using StarDust.CasparCG.net.AmcpProtocol;
using StarDust.CasparCG.net.Models;
using System;
using System.Linq;
using System.Text;
using Xunit;

namespace StartDust.CasparCG.net.UnitTest
{
    public class CasparCGDataParserTest
    {


        [Fact]
        public void Test_ParsingCls()
        {
            var stringData = "\"TESTPATTERNS\\1080I5000_TEST\"  MOVIE  1053771 20150804193337 50 1/25";

            var parser = new CasparCGDataParser();
            var clip = parser.ParseClipData(stringData);

            Assert.True(clip != null);
            Assert.True(clip.FullName == "TESTPATTERNS\\1080I5000_TEST");
            Assert.True(clip.Fps == 25);
            Assert.True(clip.Frames == 50);
            Assert.True(clip.LastUpdated == new DateTime(2015, 08, 04, 19, 33, 37));
            Assert.True(clip.Size == 1053771);
        }


        [Fact]
        public void Test_ParsingTls()
        {
            var stringData = "\"CasparCG_Flash_Templates_Example_Pack_1/ADVANCEDTEMPLATE1\" 30327 20170202153053";

            var parser = new CasparCGDataParser();
            var template = parser.ParseTemplate(stringData);

            Assert.True(template.Folder == "CasparCG_Flash_Templates_Example_Pack_1");
            Assert.True(template.Name == "ADVANCEDTEMPLATE1");
            Assert.True(template.Size == 30327);
            Assert.True(template.LastUpdated == new DateTime(2017, 02, 02, 15, 30, 53));
            Assert.True(template.FullName == "CasparCG_Flash_Templates_Example_Pack_1/ADVANCEDTEMPLATE1");

        }

        [Fact]
        public void Test_ParsingThumbnail()
        {
            var stringData = "\"TESTPATTERNS\\1080I5994_TEST_A\" 20150804T193337 2305";

            var parser = new CasparCGDataParser();
            var thumbnail = parser.ParseThumbnailData(stringData);

            Assert.True(thumbnail.Size == 2305);
            Assert.True(thumbnail.FullName == "TESTPATTERNS\\1080I5994_TEST_A");
            Assert.True(thumbnail.Name == "1080I5994_TEST_A");
            Assert.True(thumbnail.Folder == "TESTPATTERNS");
            Assert.True(thumbnail.CreatedOn == new DateTime(2015, 08, 04, 19, 33, 37));

        }

        [Fact]
        public void Test_ParsingChannelInfo()
        {

            var strinData = "1 PAL PLAYING";
            var parser = new CasparCGDataParser();
            var channelInfo = parser.ParseChannelInfo(strinData);

            Assert.True(channelInfo.VideoMode == VideoMode.PAL);
            Assert.True(channelInfo.ID == 1);
            Assert.True(channelInfo.Status == ChannelStatus.Playing);



        }



        [Fact]
        public void Test_ParsingChannelInfoFromXml()
        {
            StringBuilder stringData = new StringBuilder();

            stringData.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            stringData.Append("<channel>");
            stringData.Append("   <video-mode>PAL</video-mode>");
            stringData.Append("   <stage/>");
            stringData.Append("   <mixer>");
            stringData.Append("      <mix-time>2</mix-time>");
            stringData.Append("   </mixer>");
            stringData.Append("   <output>");
            stringData.Append("      <consumers>");
            stringData.Append("         <consumer>");
            stringData.Append("            <type>decklink-consumer</type>");
            stringData.Append("            <key-only>false</key-only>");
            stringData.Append("            <device>1</device>");
            stringData.Append("            <low-latency>false</low-latency>");
            stringData.Append("            <embedded-audio>false</embedded-audio>");
            stringData.Append("            <presentation-frame-age>358</presentation-frame-age>");
            stringData.Append("            <index>301</index>");
            stringData.Append("         </consumer>");
            stringData.Append("         <consumer>");
            stringData.Append("            <type>oal-consumer</type>");
            stringData.Append("            <index>500</index>");
            stringData.Append("         </consumer>");
            stringData.Append("         <consumer>");
            stringData.Append("            <type>ogl-consumer</type>");
            stringData.Append("            <key-only>false</key-only>");
            stringData.Append("            <windowed>true</windowed>");
            stringData.Append("            <auto-deinterlace>true</auto-deinterlace>");
            stringData.Append("            <index>600</index>");
            stringData.Append("         </consumer>");
            stringData.Append("      </consumers>");
            stringData.Append("   </output>");
            stringData.Append("   <index>0</index>");
            stringData.Append("</channel>");



            var parser = new CasparCGDataParser();
            var channelInfo = parser.ParseChannelInfo(stringData.ToString());

            Assert.True(channelInfo.VideoMode == VideoMode.PAL);
            Assert.True(channelInfo.ID == 1);
            Assert.True(channelInfo.Output != null);
            Assert.True(channelInfo.Output.Consumers != null);
            Assert.True(channelInfo.Output.Consumers.Count == 3);

            var consumer = channelInfo.Output.Consumers[2];

            Assert.True(consumer.Type == "ogl-consumer");
            Assert.True(consumer.Keyonly == false);
            Assert.True(consumer.Windowed == true);
            Assert.True(consumer.Autodeinterlace == true);
            Assert.True(consumer.Index == 600);

        }


        [Fact]
        public void Test_ParseTemplateInfo()
        {
            var stringData = new StringBuilder();

            stringData.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            stringData.AppendLine("<template version=\"1.8.0\" authorName=\"Peter Karlsson\" authorEmail=\"peter.p.karlsson@svt.se\" templateInfo=\"\" originalWidth=\"1920\" originalHeight=\"1080\" originalFrameRate=\"30\">");
            stringData.AppendLine("   <components>");
            stringData.AppendLine("      <component name=\"CasparTextField\">");
            stringData.AppendLine("         <property name=\"text\" type=\"string\" info=\"String data\"/>");
            stringData.AppendLine("      </component>");
            stringData.AppendLine("      <component name=\"CasparTextField 2\">");
            stringData.AppendLine("         <property name=\"text\" type=\"string\" info=\"String data\"/>");
            stringData.AppendLine("      </component>");
            stringData.AppendLine("   </components>");
            stringData.AppendLine("   <keyframes/>");
            stringData.AppendLine("   <instances>");
            stringData.AppendLine("      <instance name=\"f0\" type=\"CasparTextField\"/>");
            stringData.AppendLine("   </instances>");
            stringData.AppendLine("   <parameters/>");
            stringData.AppendLine("</template>");



            var parser = new CasparCGDataParser();
            var templateInfo = parser.ParseTemplateInfo(stringData.ToString());

            Assert.True(templateInfo.Version == "1.8.0");
            Assert.True(templateInfo.OriginalWidth == 1920);
            Assert.True(templateInfo.OriginalHeight == 1080);
            Assert.True(templateInfo.OriginalFrameRate == 30);
            Assert.True(templateInfo.AuthorEmail == "peter.p.karlsson@svt.se");
            Assert.True(templateInfo.AuthorName == "Peter Karlsson");
            Assert.True(templateInfo.Components != null && templateInfo.Components.Count == 2);
            Assert.True(templateInfo.Components.First().Name == "CasparTextField");
        }

    }
}
