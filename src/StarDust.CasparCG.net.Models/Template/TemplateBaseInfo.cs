using System;
using System.IO;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.net.Models
{
    /// <summary>
    /// Template information
    /// </summary>
    [Serializable]
    public class TemplateBaseInfo : ICloneable
    {
        /// <summary>
        /// Instantiate an empty <see cref="TemplateBaseInfo"/>
        /// </summary>
        public TemplateBaseInfo()
        {
        }

        /// <summary>
        /// Instantiate a <see cref="TemplateBaseInfo"/>
        /// </summary>
        /// <param name="fullPath">full path of the template</param>
        public TemplateBaseInfo(string fullPath)
        {
            Folder = Path.GetDirectoryName(fullPath);
            Name = Path.GetFileName(fullPath);
        }

        /// <summary>
        /// Instantiate a <see cref="TemplateBaseInfo"/>
        /// </summary>
        /// <param name="folder">folder of the template</param>
        /// <param name="name">file name of the template</param>
        /// <param name="size">file size of the template</param>
        /// <param name="updated">last udpated date</param>
        public TemplateBaseInfo(string folder, string name, long size, DateTime updated)
        {
            Folder = folder;
            Name = name;
            Size = size;
            LastUpdated = updated;
        }

        /// <summary>
        /// Folder
        /// </summary>
        [DataMember]
        public string Folder { get; set; }

        /// <summary>
        /// File name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// File size
        /// </summary>
        [DataMember]
        public long Size { get; set; }

        /// <summary>
        /// Laste updated date
        /// </summary>
        [DataMember]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Full path of the template
        /// </summary>
        public string FullName => this.Folder.Length > 0 ? Path.Combine(this.Folder, this.Name).Replace("\\", "/") : this.Name;

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Clone the <see cref="TemplateBaseInfo"/>
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new TemplateBaseInfo(this.Folder, this.Name, this.Size, this.LastUpdated);
        }
    }
}
