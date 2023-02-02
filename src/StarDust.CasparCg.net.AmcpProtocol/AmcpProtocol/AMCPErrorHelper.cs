using System;

namespace StarDust.CasparCG.net.AmcpProtocol
{
    /// <summary>
    /// Helper to convert AMCP Error
    /// </summary>
    public static class AMCPErrorHelper
    {
        /// <summary>
        /// Try parse a string error and get the AMCP Error enum
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public static AMCPError ToAMCPError(this string errorCode)
        {
            return int.TryParse(errorCode, out var returnCode) ? returnCode.ToAMCPError() : AMCPError.UndefinedError;
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
