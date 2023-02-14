using System;
using System.IO;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.net.Models.Media
{
    /// <summary>
    /// Thumbnail
    /// </summary>
    [Serializable]
    public class Thumbnail
    {
        /// <summary>
        /// Name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Folder of the thumbnail
        /// </summary>
        [DataMember]
        public string Folder { get; set; }

        /// <summary>
        /// File Size
        /// </summary>
        [DataMember]
        public long Size { get; set; }

        /// <summary>
        /// Create on
        /// </summary>
        [DataMember]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Full path
        /// </summary>
        public string FullName
        {
            get
            {
                return Folder.Length > 0 ? Path.Combine(Folder, Name) : Name;
            }
        }

        /// <summary>
        /// Base64 encode image
        /// </summary>
        [DataMember]
        public string Base64Image { get; set; }
    }
}
