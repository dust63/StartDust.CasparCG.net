using System;
using System.Linq;
using System.Reflection;

namespace StarDust.CasparCG.net.Models
{
    /// <summary>
    /// Use this annotation to set a value for transform a enum to CasparCG command value
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class AMCPCommandValue : Attribute
    {
        /// <summary>
        /// Command string
        /// </summary>
        public string Command { get; }


        public AMCPCommandValue(string command)
        {
            Command = command;
        }

        public static AMCPCommandValue GetCommandValueAttribute(object enm)
        {
            AMCPCommandValue attr = null;
            if (enm == null)
                return attr;
            var mi = enm.GetType().GetMember(enm.ToString());
            attr = mi?.Any() ?? false ? GetCustomAttribute(mi.First(), typeof(AMCPCommandValue)) as AMCPCommandValue : null;
            return attr;
        }
    }
}
