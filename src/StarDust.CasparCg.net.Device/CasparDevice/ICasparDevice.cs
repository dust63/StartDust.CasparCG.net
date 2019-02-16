using  StarDust.CasparCG.net.AmcpProtocol;
using  StarDust.CasparCG.net.Connection;
using  StarDust.CasparCG.net.Models;
using  StarDust.CasparCG.net.Models.Diag;
using  StarDust.CasparCG.net.Models.Info;
using  StarDust.CasparCG.net.Models.Media;
using System;
using System.Collections.Generic;

namespace  StarDust.CasparCG.net.Device
{
    public interface ICasparDevice
    {

        /// <summary>
        /// Paths info of this server
        /// </summary>
        PathsInfo PathsInfo { get; }


        /// <summary>
        /// System info of this server
        /// </summary>
        SystemInfo SystemInfo { get; }


        /// <summary>
        /// List of the Channels available in the CasparCG Server
        /// </summary>
        IList<ChannelManager> Channels { get; }

        /// <summary>
        /// Tcp Connection to the server
        /// </summary>
        IServerConnection Connection { get; }


        /// <summary>
        /// Protocol interpreter
        /// </summary>
        IAMCPProtocolParser AMCProtocolParser { get; }

        /// <summary>
        /// Data that are store in the CasparCG Server
        /// </summary>
        IList<string> Datafiles { get; }

        /// <summary>
        /// List of Medias in the CasparCG Server
        /// </summary>
        IList<MediaInfo> Mediafiles { get; }


        /// <summary>
        /// Collection of templates in the CasparCG server
        /// </summary>
        TemplatesCollection Templates { get; }

        /// <summary>
        /// Indicated if the CasparCG server is connected
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Current Version of the Caspar CG Server
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Occurred when the server is connected or nto
        /// </summary>
        event EventHandler<ConnectionEventArgs> ConnectionStatusChanged;

        /// <summary>
        /// When data was retrieved
        /// </summary>
        event EventHandler<EventArgs> DataRetrieved;

        /// <summary>
        /// When a channel are add or remove or modify
        /// </summary>
        event EventHandler<EventArgs> ChannelsUpdated;

        /// <summary>
        /// When a template are add or remove
        /// </summary>
        event EventHandler<EventArgs> TemplatesUpdated;

        /// <summary>
        /// When a template are add or remove
        /// </summary>
        event EventHandler<EventArgs> MediafilesUpdated;

        /// <summary>
        /// When a data are add or remove
        /// </summary>
        event EventHandler<EventArgs> DatafilesUpdated;

        /// <summary>
        /// When a thumbnail are add or remove
        /// </summary>
        event EventHandler<EventArgs> ThumbnailsUpdated;

        /// <summary>
        /// Connect to the server
        /// </summary>
        /// <returns></returns>
        bool Connect();

        /// <summary>
        /// Disconnect from the server
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Get the version of CasparCG Server
        /// </summary>
        /// <returns></returns>
        string GetVersion();

        /// <summary>
        /// Ask to get the data list
        /// </summary>
        IList<string> GetDatalist();

        /// <summary>
        /// Ask to get media file list
        /// </summary>
        IList<MediaInfo> GetMediafiles();

        /// <summary>
        /// Ask to get the thumbnail list
        /// </summary>
        IList<Thumbnail> GetThumbnailList();

        /// <summary>
        /// Get info from the current server
        /// </summary>
        /// <returns></returns>
        IList<ChannelInfo> GetInfo();

        /// <summary>
        /// Ask to get a thumbnail for a specific file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>Base 64 image</returns>
        string GetThumbnail(string fileName);

        /// <summary>
        /// Ask to get template.
        /// </summary>
        TemplatesCollection GetTemplates();

        /// <summary>
        /// Get data from server
        /// </summary>
        /// <param name="name">Key name of the data</param>
        string GetData(string name);

        /// <summary>
        /// Delete a data sotre in CasparCG server
        /// </summary>
        /// <param name="name">Key name of the data</param>
        /// <returns></returns>
        bool DeleteData(string name);

        /// <summary>
        /// Store data to the server
        /// </summary>
        /// <param name="name">Key name of the data</param>
        /// <param name="data">Data to store in xml or json</param>
        bool StoreData(string name, string data);


        /// <summary>
        /// Regenerates a thumbnail for a file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        bool GenerateThumbnail(string filename);


        /// <summary>
        /// Regenerates all thumbnails.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        bool GenerateAllThumbnail(string filename);

        /// <summary>
        /// Opens a new channel and displays a grid with the contents of all the existing channels.
        /// <remarks> The element<channel-grid>true</channel-grid> must be present in casparcg.config for this to work correctly.</remarks>
        /// </summary>
        /// <returns></returns>
        bool ChannelGrid();

        /// <summary>
        /// Changes the log level of the server
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        bool SetLogLevel(LogLevel logLevel);


        /// <summary>
        /// Enables or disables the specified logging category
        /// </summary>
        /// <param name="logCategory"></param>
        /// <param name="enable"></param>
        /// <returns></returns>
        bool SetLogCategory(LogCategory logCategory, bool enable);



        /// <summary>
        /// Get info for a given template
        /// </summary>
        /// <param name="fullPath">File path of the template</param>
        /// <returns></returns>
        TemplateInfo GetInfoTemplate(string fullPath);

        /// <summary>
        /// Get info for a given template
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        TemplateInfo GetInfoTemplate(TemplateBaseInfo template);


        /// <summary>
        /// Get the system info where are hosted CasparCG Server
        /// </summary>
        /// <returns></returns>
        SystemInfo GetmInfoSystem();


        /// <summary>
        /// Return the paths info for this server
        /// </summary>
        /// <returns></returns>
        PathsInfo GetInfoPaths();


        /// <summary>
        /// Return the threads info
        /// </summary>
        /// <returns></returns>
        IList<ThreadsInfo> GetInfoThreads();


        /// <summary>
        /// Restart CasparCG Server
        /// </summary>
        /// <returns></returns>
        bool Restart();


        /// <summary>
        /// Get the Open GL Info
        /// </summary>
        /// <returns></returns>
        GLInfo GetGLInfo();


        /// <summary>
        /// Releases all the pooled OpenGL resources. May cause a pause on all video channels.
        /// </summary>
        /// <returns></returns>
        bool GLGc();

    }
}
