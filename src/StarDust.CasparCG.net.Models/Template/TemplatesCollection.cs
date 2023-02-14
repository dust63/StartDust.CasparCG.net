using System.Collections.Generic;
using System.Linq;

namespace StarDust.CasparCG.net.Models
{
    /// <summary>
    /// Collection of temaplates installed on server
    /// </summary>
    public class TemplatesCollection : Dictionary<string, List<TemplateBaseInfo>>
    {
        /// <summary>
        /// Instantiate an empty <see cref="TemplatesCollection"/>
        /// </summary>
        public TemplatesCollection()
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="templates"></param>
        public  TemplatesCollection(List<TemplateBaseInfo> templates) : base(templates.GroupBy(x => x.Folder).ToDictionary(x => x.Key, x => x.ToList())) { }

        /// <summary>
        /// Get templates in the given <paramref name="folder"/>
        /// </summary>
        /// <param name="folder">folder containing templates</param>
        /// <returns></returns>
        public List<TemplateBaseInfo> GetTemplatesInFolder(string folder) => base[folder].ToList();

        /// <summary>
        /// Get value as list
        /// </summary>
        public List<TemplateBaseInfo> All => this.SelectMany(x => x.Value).ToList();

        /// <summary>
        /// All folders path
        /// </summary>
        public ICollection<string> Folders => Keys;
        
    }
}
