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
    public class CasparCGDatasParser : IDataParser
    {
        const string ConstDateParseFormat = "yyyyMMddHHmmss";
        public string DateParseFormat { get { return ConstDateParseFormat; } }

        private const string ConstThumbnailDateFormat = "yyyyMMddTHHmmss";
        public string ThumbnailDateParseFormat { get { return ConstThumbnailDateFormat; } }

        /// <summary>
        /// Static use to split datas string
        /// </summary>
        private static readonly Regex RegexParser = new Regex(@"((""((?<token>.*?)(?<!\\)"")|(?<token>[\w]+))(\s)*)", RegexOptions.Compiled);


        /// <inheritdoc cref=""/>
        public MediaInfo ParseClipData(string stringData)
        {
            long fileSize = 0;
            string fullPath = string.Empty;
            string fileName = null;

            DateTime lastUpdate = DateTime.MinValue;
            MediaType mediaType = MediaType.MOVIE;
            long duration = 0;
            decimal fps = 0;

            var splited = SplitValues(stringData);

            if (splited.Count >= 1)
            {
                fullPath = splited[0].Replace("\"", "");
                fileName = Path.GetFileName(fullPath);
            }

            if (splited.Count >= 2)
                Enum.TryParse(splited[1], true, out mediaType);

            if (splited.Count >= 3)
                long.TryParse(splited[2], out fileSize);

            if (splited.Count >= 4)
                DateTime.TryParseExact(splited[3], DateParseFormat, null, DateTimeStyles.AssumeLocal, out lastUpdate);

            if (splited.Count >= 5)
                long.TryParse(splited[4], out duration);

            if (splited.Count >= 7)
                decimal.TryParse(splited[6], out fps);



            return new MediaInfo { Fps = fps, LastUpdated = lastUpdate, Frames = duration, Size = fileSize, Type = mediaType, Name = fileName, FullName = fullPath };
        }

        /// <inheritdoc cref=""/>
        public TemplateBaseInfo ParseTemplate(string stringData)
        {
            long fileSize = 0;
            string fullPath = string.Empty;
            DateTime lastUpdate = DateTime.MinValue;
            string folder = null;
            string fileName = null;

            var splited = SplitValues(stringData);

            if (splited.Count >= 1)
            {
                fullPath = splited[0].Replace("\"", "");
                fileName = Path.GetFileName(fullPath);
                folder = fullPath.LastIndexOf("/") > 0 ? fullPath.Replace(fileName, "").Remove(fullPath.LastIndexOf("/")) : string.Empty;
            }


            if (splited.Count >= 2)
                long.TryParse(splited[1], out fileSize);

            if (splited.Count >= 3)
                DateTime.TryParseExact(splited[2], DateParseFormat, null, DateTimeStyles.AssumeLocal, out lastUpdate);



            return new TemplateBaseInfo { Folder = folder, Name = fileName, LastUpdated = lastUpdate, Size = fileSize };
        }

        /// <inheritdoc cref=""/>
        public Thumbnail ParseThumbnailDatas(string stringData)
        {

            int fileSize = 0;
            string fileName = null;
            string folder = null;
            string fullPath = string.Empty;
            DateTime lastUpdate = DateTime.MinValue;

            var splited = SplitValues(stringData);

            if (splited.Count >= 1)
            {
                fullPath = splited[0].Replace("\"", "");
                fileName = Path.GetFileName(fullPath);
                folder = Path.GetDirectoryName(fullPath);
            }


            if (splited.Count >= 2)
                DateTime.TryParseExact(splited[1], ThumbnailDateParseFormat, null, DateTimeStyles.AssumeLocal, out lastUpdate);

            if (splited.Count >= 3)
                int.TryParse(splited[2], out fileSize);



            return new Thumbnail { Size = fileSize, Name = Path.GetFileName(fullPath), Folder = folder, CreatedOn = lastUpdate };
        }

        /// <inheritdoc cref=""/>
        public ChannelInfo ParseChannelInfo(string stringData)
        {

            if (IsValidXml(stringData))
            {
                return GetXml<ChannelInfo>(stringData);
            }

            else
            {

                var splitDatas = SplitValues(stringData);
                if (splitDatas.Count < 3)
                {
                    return null;
                }

                uint.TryParse(splitDatas[0], out uint id);
                var videoMode = splitDatas[1].TryParseFromCommandValue(VideoMode.Unknown);
                var status = splitDatas[2].TryParseFromCommandValue(ChannelStatus.Stopped);
                return new ChannelInfo(id, videoMode, status, "");
            }
        }

        /// <inheritdoc cref=""/>
        public TemplateInfo ParseTemplateInfo(string stringData)
        {
            if (!IsValidXml(stringData))
            {
                return null;
            }
            return GetXml<TemplateInfo>(stringData);

        }


        /// <inheritdoc cref=""/>
        public GLInfo ParseGLInfo(string stringData)
        {
            if (!IsValidXml(stringData))
            {
                return null;
            }


            var glInfo = GetXml<GLInfo>(stringData);
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
            return RegexParser.Matches(data).Cast<Match>().Where(m => m.Groups["token"].Success).Select(x => x.Value.Trim()).ToList();

        }




        /// <inheritdoc cref=""/>
        public SystemInfo ParseInfoSystem(string stringData)
        {
            if (!IsValidXml(stringData))
                return null;

            return GetXml<SystemInfo>(stringData);
        }

        /// <inheritdoc cref=""/>
        public PathsInfo ParseInfoPaths(string stringData)
        {

            if (!IsValidXml(stringData))
                return null;

            return GetXml<PathsInfo>(stringData);
        }



        public ThreadsInfo ParseInfoThreads(string stringData)
        {
            string processName = null;
            int id = 0;
            var splited = stringData.Split('\t');

            if (splited.Length == 0)
                return null;

            if (splited.Length >= 1)
                int.TryParse(splited.FirstOrDefault(), out id);


            if (splited.Length >= 2)
                processName = splited[1];

            return new ThreadsInfo { Id = id == 0 ? default(int?) : id, ProcesssName = processName };


        }


        /// <summary>
        /// Convert XML string to object of T
        /// </summary>
        /// <typeparam name="T">Type of the object that you want to deserialize</typeparam>
        /// <param name="xmlString">Xml data in string format</param>
        /// <returns></returns>
        private T GetXml<T>(string xmlString)
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
            if (string.IsNullOrEmpty(stringData) || !stringData.StartsWith("<?xml"))
            {
                return false;
            }

            return true;
        }
    }
}
