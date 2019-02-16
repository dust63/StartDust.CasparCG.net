using System;

namespace StarDust.CasparCG.AmcpProtocol
{
    public enum AMCPError
    {
        UndefinedError = 0,
        None = 1,
        InvalidCommand = 400,
        InvalidChannel = 401,
        MissingParameter = 402,
        InvalidParameter = 403,
        FileNotFound = 404,
        InternalServerError = 500,
        InvalidFile = 502,
    }


    
    public static class AMCPErrorHelper
    {
        /// <summary>
        /// Try parse a string error and get the AMCP Error enum
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public static AMCPError ToAMCPError(this string errorCode)
        {
            if (int.TryParse(errorCode, out int returnCode))
                return returnCode.ToAMCPError();

            return AMCPError.UndefinedError;
        }

        /// <summary>
        /// Try parse a int error and get the AMCP Error enum
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public static AMCPError ToAMCPError(this int errorCode)
        {
            if (Enum.IsDefined(typeof(AMCPError), errorCode))
                return (AMCPError)errorCode;

            return AMCPError.UndefinedError;
        }
    }
}
