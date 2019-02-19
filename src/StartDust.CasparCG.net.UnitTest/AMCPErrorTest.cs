using StarDust.CasparCG.net.AmcpProtocol;
using Xunit;

namespace StartDust.CasparCG.net.UnitTest
{
    public class AMCPErrorTest
    {
        [Fact]
        public void Test_ToAMCPError_FromString()
        {

            var codeError = "400";
   

            Assert.True(codeError.ToAMCPError() == AMCPError.InvalidCommand);

            codeError = "0";
            Assert.True(codeError.ToAMCPError() == AMCPError.UndefinedError);


            codeError = "1";
            Assert.True(codeError.ToAMCPError() == AMCPError.None);


            codeError = "401";
            Assert.True(codeError.ToAMCPError() == AMCPError.InvalidChannel);

            codeError = "402";
            Assert.True(codeError.ToAMCPError() == AMCPError.MissingParameter);


            codeError = "403";
            Assert.True(codeError.ToAMCPError() == AMCPError.InvalidParameter);


            codeError = "404";
            Assert.True(codeError.ToAMCPError() == AMCPError.FileNotFound);


            codeError = "500";
            Assert.True(codeError.ToAMCPError() == AMCPError.InternalServerError);

            codeError = "502";
            Assert.True(codeError.ToAMCPError() == AMCPError.InvalidFile);

        }



        [Fact]
        public void Test_ToAMCPError_FromInt()
        {

            var codeError = 400;


            Assert.True(codeError.ToAMCPError() == AMCPError.InvalidCommand);

            codeError = 0;
            Assert.True(codeError.ToAMCPError() == AMCPError.UndefinedError);

            codeError = 1;
            Assert.True(codeError.ToAMCPError() == AMCPError.None);


            codeError = 401;
            Assert.True(codeError.ToAMCPError() == AMCPError.InvalidChannel);

            codeError = 402;
            Assert.True(codeError.ToAMCPError() == AMCPError.MissingParameter);


            codeError = 403;
            Assert.True(codeError.ToAMCPError() == AMCPError.InvalidParameter);


            codeError = 404;
            Assert.True(codeError.ToAMCPError() == AMCPError.FileNotFound);


            codeError = 500;
            Assert.True(codeError.ToAMCPError() == AMCPError.InternalServerError);

            codeError = 502;
            Assert.True(codeError.ToAMCPError() == AMCPError.InvalidFile);
        }


    }
}
