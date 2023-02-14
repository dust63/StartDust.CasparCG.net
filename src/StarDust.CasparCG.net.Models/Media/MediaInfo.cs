using System;
using System.IO;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.net.Models.Media
{
    /// <summary>
    /// Informaiton about media
    /// </summary>
    [Serializable]
    public class MediaInfo
    {
        /// <summary>
        /// Instantiate an empty <see cref="MediaInfo"/>
        /// </summary>
        public MediaInfo()
        {
        }

        /// <summary>
        /// Instantiate an <see cref="MediaInfo"/>
        /// </summary>
        /// <param name="folder">folder of the media</param>
        /// <param name="name">file name of the media</param>
        /// <param name="type">type of media</param>
        /// <param name="size">file size</param>
        /// <param name="updated">last updated date</param>
        /// <param name="frames">number of frames</param>
        /// <param name="fps">frame per seconds</param>
        public MediaInfo(string folder, string name, MediaType type, long size, DateTime updated, long frames, decimal fps)
        {
            Folder = folder;
            Name = name;
            FullName = Path.Combine(folder, name);
            Size = size;
            LastUpdated = updated;
            Type = type;
            Frames = frames;
            Fps = fps;
        }

        /// <summary>
        /// Number of frames
        /// </summary>
        [DataMember]
        public long Frames { get; set; }

        /// <summary>
        /// Frame per seconds
        /// </summary>
        [DataMember]
        public decimal Fps { get; set; }

        /// <summary>
        /// Current folder of the media
        /// </summary>
        [DataMember]
        public string Folder { get; set; }

        /// <summary>
        /// File name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Full name
        /// </summary>
        [DataMember]
        public string FullName { get; set; }

        /// <summary>
        /// Type of media
        /// </summary>
        [DataMember]
        public MediaType Type { get; set; }

        /// <summary>
        /// File size
        /// </summary>
        [DataMember]
        public long Size { get; set; }

        /// <summary>
        /// Last updated date
        /// </summary>
        [DataMember]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// String representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FullName;
        }
    }
}
