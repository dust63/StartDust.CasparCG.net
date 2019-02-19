using StarDust.CasparCG.net.AmcpProtocol;
using StarDust.CasparCG.net.Models;
using Xunit;

namespace StartDust.CasparCG.net.UnitTest
{
    public class AMCPCommandTest
    {

        enum TestAcmpCommandEnum
        {

            Undefined,
            [AMCPCommandValue("My test command value")]
            TestCommand1

        }


        [Fact]
        public void Test_ToAMCPCommandValue()
        {
            var attendedValue = "My test command value";
            Assert.True(attendedValue == TestAcmpCommandEnum.TestCommand1.ToAmcpValue());

        }

        [Fact]
        public void Test_AmcpCommandParsing()
        {
            var value = "My test command value";
            var enumValue = value.TryParseFromCommandValue(TestAcmpCommandEnum.Undefined);
            Assert.True(enumValue == TestAcmpCommandEnum.TestCommand1);

            var badValue = "bad value command";
            var undefinedenumValue = badValue.TryParseFromCommandValue(TestAcmpCommandEnum.Undefined);

            Assert.True(undefinedenumValue == TestAcmpCommandEnum.Undefined);

        }



        [Fact]
        public void Test_AllCommandValue()
        {

            Assert.True(null == AMCPCommand.Undefined.ToAmcpValue());
            Assert.True(null == AMCPCommand.None.ToAmcpValue());
            Assert.True("LOAD" == AMCPCommand.LOAD.ToAmcpValue());
            Assert.True("LOADBG" == AMCPCommand.LOADBG.ToAmcpValue());
            Assert.True("PLAY" == AMCPCommand.PLAY.ToAmcpValue());
            Assert.True("STOP" == AMCPCommand.STOP.ToAmcpValue());
            Assert.True("DATA" == AMCPCommand.DATA.ToAmcpValue());
            Assert.True("CLEAR" == AMCPCommand.CLEAR.ToAmcpValue());
            Assert.True("SET" == AMCPCommand.SET.ToAmcpValue());
            Assert.True("MIXER" == AMCPCommand.MIXER.ToAmcpValue());
            Assert.True("CALL" == AMCPCommand.CALL.ToAmcpValue());
            Assert.True("REMOVE" == AMCPCommand.REMOVE.ToAmcpValue());
            Assert.True("ADD" == AMCPCommand.ADD.ToAmcpValue());
            Assert.True("SWAP" == AMCPCommand.SWAP.ToAmcpValue());
            Assert.True("THUMBNAIL GENERATE" == AMCPCommand.THUMBNAIL_GENERATE.ToAmcpValue());
            Assert.True("THUMBNAIL LIST" == AMCPCommand.THUMBNAIL_LIST.ToAmcpValue());
            Assert.True("THUMBNAIL RETRIEVE" == AMCPCommand.THUMBNAIL_RETRIEVE.ToAmcpValue());
            Assert.True("THUMBNAIL GENERATE_ALL" == AMCPCommand.THUMBNAIL_GENERATEALL.ToAmcpValue());
            Assert.True("CINF" == AMCPCommand.CINF.ToAmcpValue());
            Assert.True("CLS" == AMCPCommand.CLS.ToAmcpValue());
            Assert.True("FLS" == AMCPCommand.FLS.ToAmcpValue());
            Assert.True("TLS" == AMCPCommand.TLS.ToAmcpValue());
            Assert.True("STATUS" == AMCPCommand.STATUS.ToAmcpValue());
            Assert.True("INFO" == AMCPCommand.INFO.ToAmcpValue());
            Assert.True("INFO TEMPLATE" == AMCPCommand.INFO_TEMPLATE.ToAmcpValue());
            Assert.True("INFO CONFIG" == AMCPCommand.INFO_CONFIG.ToAmcpValue());
            Assert.True("INFO PATHS" == AMCPCommand.INFO_PATHS.ToAmcpValue());
            Assert.True("INFO SYSTEM" == AMCPCommand.INFO_SYSTEM.ToAmcpValue());
            Assert.True("INFO SERVER" == AMCPCommand.INFO_SERVER.ToAmcpValue());
            Assert.True("INFO QUEUES" == AMCPCommand.INFO_QUEUES.ToAmcpValue());
            Assert.True("INFO THREADS" == AMCPCommand.INFO_THREADS.ToAmcpValue());
            Assert.True("INFO DELAY" == AMCPCommand.INFO_DELAY.ToAmcpValue());
            Assert.True("DIAG" == AMCPCommand.DIAG.ToAmcpValue());
            Assert.True("GLGC" == AMCPCommand.GLGC.ToAmcpValue());
            Assert.True("BYE" == AMCPCommand.BYE.ToAmcpValue());
            Assert.True("KILL" == AMCPCommand.KILL.ToAmcpValue());
            Assert.True("RESTART" == AMCPCommand.RESTART.ToAmcpValue());
            Assert.True("HELP" == AMCPCommand.HELP.ToAmcpValue());
            Assert.True("HELP PRODUCER" == AMCPCommand.HELP_PRODUCER.ToAmcpValue());
            Assert.True("HELP CONSUMER" == AMCPCommand.HELP_CONSUMER.ToAmcpValue());
            Assert.True("CHANNEL_GRID" == AMCPCommand.CHANNEL_GRID.ToAmcpValue());
            Assert.True("MIXER COMMIT" == AMCPCommand.MIXER_COMMIT.ToAmcpValue());
            Assert.True("MIXER GRID" == AMCPCommand.MIXER_GRID.ToAmcpValue());
            Assert.True("MIXER STRAIGHT_ALPHA_OUTPUT" == AMCPCommand.MIXER_STRAIGHT_ALPHA_OUTPUT.ToAmcpValue());
            Assert.True("MIXER MASTERVOLUME" == AMCPCommand.MIXER_MASTERVOLUME.ToAmcpValue());
            Assert.True("MIXER VOLUME" == AMCPCommand.MIXER_VOLUME.ToAmcpValue());
            Assert.True("MIXER MIPMAP" == AMCPCommand.MIXER_MIPMAP.ToAmcpValue());
            Assert.True("MIXER ROTATION" == AMCPCommand.MIXER_ROTATION.ToAmcpValue());
            Assert.True("MIXER CROP" == AMCPCommand.MIXER_CROP.ToAmcpValue());
            Assert.True("MIXER PERSPECTIVE" == AMCPCommand.MIXER_PERSPECTIVE.ToAmcpValue());
            Assert.True("MIXER ANCHOR" == AMCPCommand.MIXER_ANCHOR.ToAmcpValue());
            Assert.True("MIXER CLIP" == AMCPCommand.MIXER_CLIP.ToAmcpValue());
            Assert.True("MIXER CLIP" == AMCPCommand.MIXER_CLIP.ToAmcpValue());
            Assert.True("MIXER FILL" == AMCPCommand.MIXER_FILL.ToAmcpValue());
            Assert.True("MIXER FILL" == AMCPCommand.MIXER_FILL.ToAmcpValue());
            Assert.True("MIXER CONTRAST" == AMCPCommand.MIXER_CONTRAST.ToAmcpValue());
            Assert.True("MIXER SATURATION" == AMCPCommand.MIXER_SATURATION.ToAmcpValue());
            Assert.True("MIXER BRIGHTNESS" == AMCPCommand.MIXER_BRIGHTNESS.ToAmcpValue());
            Assert.True("MIXER OPACITY" == AMCPCommand.MIXER_OPACITY.ToAmcpValue());
            Assert.True("MIXER BLEND" == AMCPCommand.MIXER_BLEND.ToAmcpValue());
            Assert.True("MIXER CHROMA" == AMCPCommand.MIXER_CHROMA.ToAmcpValue());
            Assert.True("MIXER KEYER" == AMCPCommand.MIXER_KEYER.ToAmcpValue());
            Assert.True("CG INFO" == AMCPCommand.CG_INFO.ToAmcpValue());
            Assert.True("CG INVOKE" == AMCPCommand.CG_INVOKE.ToAmcpValue());
            Assert.True("RESUME" == AMCPCommand.RESUME.ToAmcpValue());
            Assert.True("PAUSE" == AMCPCommand.PAUSE.ToAmcpValue());
            Assert.True("PRINT" == AMCPCommand.PRINT.ToAmcpValue());
            Assert.True("LOG LEVEL" == AMCPCommand.LOG_LEVEL.ToAmcpValue());
            Assert.True("CG UPDATE" == AMCPCommand.CG_UPDATE.ToAmcpValue());
            Assert.True("CG UPDATE" == AMCPCommand.CG_UPDATE.ToAmcpValue());
            Assert.True("CG CLEAR" == AMCPCommand.CG_CLEAR.ToAmcpValue());
            Assert.True("CG REMOVE" == AMCPCommand.CG_REMOVE.ToAmcpValue());
            Assert.True("CG NEXT" == AMCPCommand.CG_NEXT.ToAmcpValue());
            Assert.True("CG NEXT" == AMCPCommand.CG_NEXT.ToAmcpValue());
            Assert.True("CG STOP" == AMCPCommand.CG_STOP.ToAmcpValue());
            Assert.True("CG PLAY" == AMCPCommand.CG_PLAY.ToAmcpValue());
            Assert.True("CG ADD" == AMCPCommand.CG_ADD.ToAmcpValue());
            Assert.True("DATA REMOVE" == AMCPCommand.DATA_REMOVE.ToAmcpValue());
            Assert.True("DATA LIST" == AMCPCommand.DATA_LIST.ToAmcpValue());
            Assert.True("DATA RETRIEVE" == AMCPCommand.DATA_RETRIEVE.ToAmcpValue());
            Assert.True("LOCK" == AMCPCommand.LOCK.ToAmcpValue());
            Assert.True("LOG CATEGORY" == AMCPCommand.LOG_CATEGORY.ToAmcpValue());
            Assert.True("DATA STORE" == AMCPCommand.DATA_STORE.ToAmcpValue());
            Assert.True("MIXER CLEAR" == AMCPCommand.MIXER_CLEAR.ToAmcpValue());
            Assert.True("GL INFO" == AMCPCommand.GLINFO.ToAmcpValue());
        }








    }
}
