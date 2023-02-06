using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StarDust.CasparCG.net.Device;

namespace StarDust.CasparCG.net.RestApi.Models
{
    public class CasparCGServer
    {
        public CasparCGServer(KeyValuePair<Guid, ICasparDevice> keyValue)
        {
            Id = keyValue.Key;
            Hostname = keyValue.Value.ConnectionSettings.Hostname;
        }

        public Guid Id { get; }
        public string Hostname { get; }
    }
}