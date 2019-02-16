using System.Collections.Generic;
using System.Linq;

namespace StarDust.CasparCG.Models
{
    public class TemplatesCollection
    {
        private Dictionary<string, List<TemplateBaseInfo>> _templates = new Dictionary<string, List<TemplateBaseInfo>>();


        public TemplatesCollection()
        {
        }

        public TemplatesCollection(List<TemplateBaseInfo> templates)
        {
            _templates = templates.GroupBy(x => x.Folder).ToDictionary(x=> x.Key, x=> x.ToList());   
            All = templates.ToList();
        }

        public List<TemplateBaseInfo> GetTemplatesInFolder(string folder)
        {
            return _templates[folder];
        }

        public List<TemplateBaseInfo> All { get; private set; } = new List<TemplateBaseInfo>();

        public ICollection<string> Folders
        {
            get
            {
                return _templates.Keys;
            }
        }

        public void Clear()
        {
            this._templates.Clear();
            All.Clear();
        }
    }
}
