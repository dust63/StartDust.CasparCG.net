using System;
using System.IO;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.Models
{
    [Serializable]
    public class TemplateBaseInfo : ICloneable
    {
        public TemplateBaseInfo()
        {
        }

        public TemplateBaseInfo(string fullPath)
        {
            Folder = Path.GetDirectoryName(fullPath);
            Name = Path.GetFileName(fullPath);
        }

        public TemplateBaseInfo(string folder, string name, long size, DateTime updated)
        {
            this.Folder = folder;
            this.Name = name;
            this.Size = size;
            this.LastUpdated = updated;
        }

        [DataMember]
        public string Folder { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public long Size { get; set; }

        [DataMember]
        public DateTime LastUpdated { get; set; }

        public string FullName
        {
            get
            {
                return this.Folder.Length > 0 ? Path.Combine(this.Folder, this.Name) : this.Name;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }

        public object Clone()
        {
            return new TemplateBaseInfo(this.Folder, this.Name, this.Size, this.LastUpdated);
        }
    }
}
