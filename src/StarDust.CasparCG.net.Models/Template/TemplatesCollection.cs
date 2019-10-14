using System.Collections.Generic;
using System.Linq;

namespace StarDust.CasparCG.net.Models
{
    public class TemplatesCollection : Dictionary<string, List<TemplateBaseInfo>>
    {
        
        public TemplatesCollection()
        {
        }
        
        public  TemplatesCollection(List<TemplateBaseInfo> templates) : base(templates.GroupBy(x => x.Folder).ToDictionary(x => x.Key, x => x.ToList())) { }
        public List<TemplateBaseInfo> GetTemplatesInFolder(string folder)
        {
            return base[folder].ToList();
        }

        public List<TemplateBaseInfo> All => this.SelectMany(x => x.Value).ToList();

        public ICollection<string> Folders => Keys;

        public void Clear()
        {
            Clear();
        }
    }
}
