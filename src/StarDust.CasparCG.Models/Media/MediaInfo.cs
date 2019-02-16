using System;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.Models.Media
{
    [Serializable]
    public class MediaInfo
    {
        public MediaInfo()
        {
        }

        public MediaInfo(string folder, string name, MediaType type, long size, DateTime updated, long frames, Decimal fps)
        {
            this.Name = name;
            this.Size = size;
            this.LastUpdated = updated;
            this.Type = type;
            this.Frames = frames;
            this.Fps = fps;
        }

        [DataMember]
        public long Frames { get; set; }

        [DataMember]
        public decimal Fps { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string FullName { get; set; }

        [DataMember]
        public MediaType Type { get; set; }

        [DataMember]
        public long Size { get; set; }

        [DataMember]
        public DateTime LastUpdated { get; set; }

        public override string ToString()
        {
            return this.FullName;
        }
    }
}
