using ServiceStack.DataAnnotations;

namespace StarDust.CasparCG.net.RestApi.Models
{
    [Alias("CasparCgServer")]
    public class CasparCGServer
    {
        /// <summary>
        /// Create a new server model based on <paramref name="hostname"/> and <paramref name="name"/>
        /// </summary>
        /// <param name="hostname">hostname of the server</param>
        /// <param name="name">name of the server</param>
        public CasparCGServer(string hostname, string name)
        {
            Id = Guid.NewGuid();
            Hostname = hostname;
            Name = name;
        }

        /// <summary>
        /// Identifier of the server
        /// </summary>
        /// <value></value>
        [Required]
        [PrimaryKey]
        public Guid Id { get; set; }

        /// <summary>
        /// Hostname to contact the casparCG server
        /// </summary>
        /// <value></value>
        [Required]
        [Index(Unique = true)]
        public string Hostname { get; set; }

        /// <summary>
        /// Display name of the save
        /// </summary>
        /// <value></value>
        public string? Name { get; set; }
    }
}