using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;
using StarDust.CasparCG.net.Device;

namespace StarDust.CasparCG.net.RestApi.Models
{
    [Alias("CasparCgServer")]
    public class CasparCGServer
    {
        public CasparCGServer(string hostname, string name)
        {
            Id = Guid.NewGuid();
            Hostname = hostname;
            Name = name;
        }

        public CasparCGServer(KeyValuePair<Guid, ICasparDevice> keyValue)
        {
            Id = keyValue.Key;
            Hostname = keyValue.Value.ConnectionSettings.Hostname;
        }

        [Required]
        public Guid Id { get;set; }

        [Required]
        public string Hostname { get;set; }
        public string? Name{get;set;}
    }
}