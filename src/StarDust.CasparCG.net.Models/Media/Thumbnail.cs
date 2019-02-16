using System;
using System.IO;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.net.Models.Media
{

    [Serializable]
    public class Thumbnail
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Folder { get; set; }

        [DataMember]
        public long Size { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        public string FullName
        {
            get
            {
                return this.Folder.Length > 0 ? Path.Combine(this.Folder, this.Name) : this.Name;
            }
        }

        [DataMember]
        public string Base64Image { get; set; }
    }
}
