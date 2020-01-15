using StarDust.CasparCG.net.Models;
using StarDust.CasparCG.net.Models.Info;
using StarDust.CasparCG.net.Models.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace StarDust.CasparCG.net.AmcpProtocol
{
    /// <inheritdoc />
    public class CasparCGDataParser : IDataParser
    {
        private const string ConstDateParseFormat = "yyyyMMddHHmmss";

        /// <inheritdoc />
        public string DateParseFormat => ConstDateParseFormat;

        private const string ConstThumbnailDateFormat = "yyyyMMddTHHmmss";

        /// <summary>
        /// Date format for thumbnail
        /// </summary>
        public string ThumbnailDateParseFormat => ConstThumbnailDateFormat;

        /// <summary>
        /// Static use to split data string
        /// </summary>
        private static readonly Regex RegexParser = new Regex(@"[\""].+?[\""]|[^ ]+", RegexOptions.Compiled);


        /// <inheritdoc />
        public MediaInfo ParseClipData(string stringData)
        {
            long fileSize = 0;
            var fullPath = string.Empty;
            string fileName = null;

            var lastUpdate = DateTime.MinValue;
            var mediaType = MediaType.MOVIE;
            long duration = 0;
            decimal fps = 0;

            var splitData = SplitValues(stringData);

            if (splitData.Count >= 1)
            {
                fullPath = splitData[0].Replace("\"", "");
                fileName = Path.GetFileName(fullPath);
            }

            if (splitData.Count >= 2)
                Enum.TryParse(splitData[1], true, out mediaType);

            if (splitData.Count >= 3)
                long.TryParse(splitData[2], out fileSize);

            if (splitData.Count >= 4)
                DateTime.TryParseExact(splitData[3], DateParseFormat, null, DateTimeStyles.AssumeLocal, out lastUpdate);

            if (splitData.Count >= 5)
                long.TryParse(splitData[4], out duration);

            if (splitData.Count >= 6)
                decimal.TryParse(splitData[5].Split('/').Last(), out fps);



            return new MediaInfo { Fps = fps, LastUpdated = lastUpdate, Frames = duration, Size = fileSize, Type = mediaType, Name = fileName, FullName = fullPath };
        }

        /// <inheritdoc />
        public TemplateBaseInfo ParseTemplate(string stringData)
        {
            long fileSize = 0;
            var lastUpdate = DateTime.MinValue;
            string folder = null;
            string fileName = null;

            var splitData = SplitValues(stringData);

            if (splitData.Count >= 1)
            {
                var fullPath = splitData[0].Replace("\"", "");
                fullPath = fullPath.Replace("/", "\\");
                fileName = Path.GetFileName(fullPath);
                folder = Path.GetDirectoryName(fullPath); ;
            }


            if (splitData.Count >= 2)
                long.TryParse(splitData[1], out fileSize);

            if (splitData.Count >= 3)
                DateTime.TryParseExact(splitData[2], DateParseFormat, null, DateTimeStyles.AssumeLocal, out lastUpdate);



            return new TemplateBaseInfo { Folder = folder, Name = fileName, LastUpdated = lastUpdate, Size = fileSize };
        }

        /// <inheritdoc />
        public Thumbnail ParseThumbnailData(string stringData)
        {

            var fileSize = 0;
            string folder = null;
            var fullPath = string.Empty;
            var lastUpdate = DateTime.MinValue;

            var splitData = SplitValues(stringData);

            if (splitData.Count >= 1)
            {
                fullPath = splitData[0].Replace("\"", "");
                folder = Path.GetDirectoryName(fullPath);
            }


            if (splitData.Count >= 2)
                DateTime.TryParseExact(splitData[1], ThumbnailDateParseFormat, null, DateTimeStyles.AssumeLocal, out lastUpdate);

            if (splitData.Count >= 3)
                int.TryParse(splitData[2], out fileSize);



            return new Thumbnail { Size = fileSize, Name = Path.GetFileName(fullPath), Folder = folder, CreatedOn = lastUpdate };
        }

        /// <inheritdoc />
        public ChannelInfo ParseChannelInfo(string stringData)
        {

            if (IsValidXml(stringData))
            {
                return DeserializeFromXml<ChannelInfo>(stringData);
            }

            var splitData = SplitValues(stringData);
            if (splitData.Count < 3)
            {
                return null;
            }

            uint.TryParse(splitData[0], out var id);
            var videoMode = splitData[1].TryParseFromCommandValue(VideoMode.Unknown);
            var status = splitData[2].TryParseFromCommandValue(ChannelStatus.Stopped);
            return new ChannelInfo(id, videoMode, status, "");
        }

        /// <inheritdoc />
        public TemplateInfo ParseTemplateInfo(string stringData)
        {
            return !IsValidXml(stringData) ? null : DeserializeFromXml<TemplateInfo>(stringData);
        }


        /// <inheritdoc />
        public GLInfo ParseGLInfo(string stringData)
        {
            if (!IsValidXml(stringData))
            {
                return null;
            }
            
            var glInfo = DeserializeFromXml<GLInfo>(stringData);
            glInfo = glInfo ?? new GLInfo();
            glInfo.Xml = stringData;

            return glInfo;
        }


        /// <summary>
        /// Pass data to regex to get split value
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static List<string> SplitValues(string data)
        {
            return RegexParser.Matches(data).Cast<Match>()
                .Select(m => m.Value).ToList();
        }




        /// <inheritdoc />
        public SystemInfo ParseInfoSystem(string stringData)
        {
            return !IsValidXml(stringData) ? null : DeserializeFromXml<SystemInfo>(stringData);
        }

        /// <inheritdoc />
        public PathsInfo ParseInfoPaths(string stringData)
        {
            return !IsValidXml(stringData) ? null : DeserializeFromXml<PathsInfo>(stringData);
        }


        /// <inheritdoc />
        public ThreadsInfo ParseInfoThreads(string stringData)
        {
            string processName = null;
            var id = 0;
            var splitData = stringData.Split('\t');

            if (splitData.Length == 0)
                return null;

            if (splitData.Length >= 1)
                int.TryParse(splitData.FirstOrDefault(), out id);


            if (splitData.Length >= 2)
                processName = splitData[1];

            return new ThreadsInfo { Id = id == 0 ? default(int?) : id, ProcesssName = processName };


        }


        /// <summary>
        /// Convert XML string to object of T
        /// </summary>
        /// <typeparam name="T">Type of the object that you want to deserialize</typeparam>
        /// <param name="xmlString">Xml data in string format</param>
        /// <returns></returns>
        private static T DeserializeFromXml<T>(string xmlString)
        {
            var serializer = new XmlSerializer(typeof(T));
            var deserializeObject = (T)serializer.Deserialize(new StringReader(xmlString));
            return deserializeObject;
        }

        /// <summary>
        /// Retrun true if the xml string is valid.
        /// </summary>
        /// <param name="stringData"></param>
        /// <returns></returns>
        private static bool IsValidXml(string stringData)
        {
            return !string.IsNullOrEmpty(stringData) && stringData.StartsWith("<?xml");
        }
    }
}
