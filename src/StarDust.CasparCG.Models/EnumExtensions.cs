
using System;
using System.Collections.Generic;

namespace StarDust.CasparCG.Models
{
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
            return AMCPCommandValue.GetCommandValueAttribute(value)?.Command;
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

            var enumDictionnary = new Dictionary<string, TEnum>();
            foreach (TEnum e in Enum.GetValues(typeof(TEnum)))
            {
                enumDictionnary.Add(e.ToAmcpValue() ?? e.ToString(), e);
            }

            return enumDictionnary.ContainsKey(commandValueToParse) ? enumDictionnary[commandValueToParse] : defaultValue;
        }


        public static TEnum TryParseOrDefault<TEnum>(this string valueToParse, TEnum defaultValue) where TEnum : struct, IConvertible
        {

            return Enum.TryParse<TEnum>(valueToParse, out TEnum cType) ? cType : defaultValue;

        }
    }
}
