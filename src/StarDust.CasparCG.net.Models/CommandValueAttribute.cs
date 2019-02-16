using System;
using System.Linq;
using System.Reflection;

namespace StarDust.CasparCG.net.Models
{
    /// <summary>
    /// Use this annotation to set a value for transform a enum to CapsarCG command value
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class AMCPCommandValue : Attribute
    {

        private readonly bool _isNotSupported;

        private string _command;


        public string Command
        {
            get
            {
                return _command;
            }
            set
            {
                _command = value;
            }
        }

        public AMCPCommandValue(string command, bool isNotSupported = false)
        {
            _isNotSupported = isNotSupported;
            Command = command;
        }

        public static AMCPCommandValue GetCommandValueAttribute(object enm)
        {
            AMCPCommandValue attr = null;
            if (enm != null)
            {
                MemberInfo[] mi = enm.GetType().GetMember(enm.ToString());
                attr = mi?.Any() ?? false ? GetCustomAttribute(mi.First(), typeof(AMCPCommandValue)) as AMCPCommandValue : null;                    
            }
            return attr;
        }
    }
}
