using System;
using System.Linq;
using System.Reflection;

namespace StarDust.CasparCG.net.Models
{
    /// <summary>
    /// Use this annotation to set a value for transform a enum to CasparCG command value
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class AMCPCommandValueAttribute : Attribute
    {
        /// <summary>
        /// Command string
        /// </summary>
        public string Command { get; }


        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="command"></param>
        public AMCPCommandValueAttribute(string command)
        {
            Command = command;
        }


        /// <summary>
        /// Get command AMCPCommandValueAttribute of a member
        /// </summary>
        /// <param name="enm"></param>
        /// <returns></returns>
        public static AMCPCommandValueAttribute GetCommandValueAttribute(object enm)
        {
            AMCPCommandValueAttribute attr = null;
            if (enm == null)
                return attr;
            var mi = enm.GetType().GetMember(enm.ToString());
            attr = mi.Any() ? GetCustomAttribute(mi.First(), typeof(AMCPCommandValueAttribute)) as AMCPCommandValueAttribute : null;
            return attr;
        }
    }
}
