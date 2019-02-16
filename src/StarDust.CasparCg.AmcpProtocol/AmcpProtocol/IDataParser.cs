using StarDust.CasparCG.Models;
using StarDust.CasparCG.Models.Info;
using StarDust.CasparCG.Models.Media;

namespace StarDust.CasparCG.AmcpProtocol
{
    public interface IDataParser
    {
        string DateParseFormat { get; }

        /// <summary>
        /// Get a string received parse it to get Media info object
        /// </summary>
        /// <param name="stringData"></param>
        /// <returns></returns>
        MediaInfo ParseClipData(string stringData);

        /// <summary>
        /// Get a string received parse it to get template info object
        /// </summary>
        /// <param name="stringData"></param>
        /// <returns></returns>
        TemplateBaseInfo ParseTemplate(string stringData);

        /// <summary>
        /// Get a string received parse it to get thumbnail info object
        /// </summary>
        /// <param name="stringData"></param>
        /// <returns></returns>
        Thumbnail ParseThumbnailDatas(string stringData);


        /// <summary>
        /// Get a string parse it to get channel info
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        ChannelInfo ParseChannelInfo(string str);


        /// <summary>
        /// Get a string parse it to get template info
        /// </summary>
        /// <param name="stringData"></param>
        /// <returns></returns>
        TemplateInfo ParseTemplateInfo(string stringData);


        /// <summary>
        /// Get a string parse it to get System info
        /// </summary>
        /// <param name="stringData"></param>
        /// <returns></returns>
        SystemInfo ParseInfoSystem(string stringData);


        /// <summary>
        /// Get a string parse it to get Paths info
        /// </summary>
        /// <param name="stringData"></param>
        /// <returns></returns>
        PathsInfo ParseInfoPaths(string stringData);


        /// <summary>
        /// Get a string parse it to get Threads info
        /// </summary>
        /// <param name="stringData"></param>
        /// <returns></returns>
        ThreadsInfo ParseInfoThreads(string stringData);



        GLInfo ParseGLInfo(string stringData);
    }
}