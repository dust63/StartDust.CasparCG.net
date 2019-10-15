
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarDust.CasparCG.net.Models
{

    /// <summary>
    /// Provide enum extensions parsing
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Get the command value associate to an Enum
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToAmcpValue<TEnum>(this TEnum value) where TEnum : struct, IConvertible
        {
            return AMCPCommandValueAttribute.GetCommandValueAttribute(value)?.Command;
        }

        /// <summary>
        /// Find the right Enum from the command value passed
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="commandValueToParse"></param>
        /// <param name="defaultValue">Default value if no corresponding command value found</param>
        /// <returns></returns>
        public static TEnum TryParseFromCommandValue<TEnum>(this string commandValueToParse, TEnum defaultValue) where TEnum : struct, IConvertible
        {
            var enumDictionary = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToDictionary(e => e.ToAmcpValue() ?? e.ToString());
            return enumDictionary.ContainsKey(commandValueToParse) ? enumDictionary[commandValueToParse] : defaultValue;
        }

        /// <summary>
        /// Try parse string value to get enum value
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="valueToParse"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static TEnum TryParseOrDefault<TEnum>(this string valueToParse, TEnum defaultValue) where TEnum : struct, IConvertible
        {

            return Enum.TryParse(valueToParse, out TEnum cType) ? cType : defaultValue;

        }
    }
}
