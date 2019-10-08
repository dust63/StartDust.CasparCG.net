using StarDust.CasparCG.net.AmcpProtocol;
using StarDust.CasparCG.net.Connection;
using StarDust.CasparCG.net.Models;
using StarDust.CasparCG.net.Models.Diag;
using StarDust.CasparCG.net.Models.Info;
using StarDust.CasparCG.net.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StarDust.CasparCG.net.Device
{
    ///<inheritdoc/>
    public class CasparDevice : ICasparDevice
    {
        #region Fields

        private const int MaxWaitTimeInSec = 5;
        private readonly object _lockObject = new object();
        #endregion

        #region Properties

        ///<inheritdoc />
        public PathsInfo PathsInfo { get; private set; }


        ///<inheritdoc />
        public SystemInfo SystemInfo { get; private set; }

        ///<inheritdoc />
        public IServerConnection Connection { get; }


        ///<inheritdoc />
        public IAMCPProtocolParser AMCProtocolParser { get; }

        ///<inheritdoc />
        public IList<ChannelManager> Channels { get; private set; }

        ///<inheritdoc />
        public TemplatesCollection Templates { get; private set; }

        ///<inheritdoc />
        public IList<MediaInfo> Mediafiles { get; private set; }


        ///<inheritdoc />
        public IList<Thumbnail> Thumbnails { get; private set; }

        ///<inheritdoc />
        public IList<string> Datafiles { get; private set; }

        ///<inheritdoc />
        public string Version { get; private set; }

        ///<inheritdoc />
        public bool IsConnected => Connection != null && Connection.IsConnected;

        ///<inheritdoc />
        public IList<string> Fonts { get; private set; }


        #endregion

        #region Events
        ///<inheritdoc />
        public event EventHandler<ConnectionEventArgs> ConnectionStatusChanged;

        ///<inheritdoc />
        public event EventHandler<EventArgs> DataRetrieved;

        ///<inheritdoc />
        public event EventHandler<EventArgs> ChannelsUpdated;

        ///<inheritdoc />
        public event EventHandler<EventArgs> TemplatesUpdated;

        ///<inheritdoc />
        public event EventHandler<EventArgs> MediafilesUpdated;

        ///<inheritdoc />
        public event EventHandler<EventArgs> DatafilesUpdated;

        ///<inheritdoc />
        public event EventHandler<EventArgs> ThumbnailsUpdated;
        #endregion

        ///<inheritdoc/>
        public CasparDevice(IServerConnection connection, IAMCPProtocolParser amcpProtocolParser)
        {

            Connection = connection;
            AMCProtocolParser = amcpProtocolParser;


            Channels = new List<ChannelManager>();
            Templates = new TemplatesCollection();
            Mediafiles = new List<MediaInfo>();
            Datafiles = new List<string>();
            Fonts = new List<string>();
            Version = "unknown";

            Connection.ConnectionStateChanged += Server__ConnectionStateChanged;
        }



        private async void Server__ConnectionStateChanged(object sender, ConnectionEventArgs e)
        {
            if (!e.Connected)
                return;

            ConnectionStatusChanged?.Invoke(this, e);
            try
            {
                await Task.Factory.StartNew(GetVersion);
                await Task.Factory.StartNew( GetInfo);
                await Task.Factory.StartNew(GetThumbnailList);
                await Task.Factory.StartNew( GetDatalist);
                await Task.Factory.StartNew(GetTemplates);
                await Task.Factory.StartNew( GetMediafiles);
                await Task.Factory.StartNew(GetInfoPaths);
                await Task.Factory.StartNew(GetInfoSystem);

            }
            catch
            {
                //We don't want to crash the connection
            }

        }



        /// <inheritdoc/>
        public bool GLGc()
        {
            return IsConnected && AMCProtocolParser.AmcpTcpParser.SendCommand(AMCPCommand.GLGC);
        }

       /// <inheritdoc/>
        public bool Restart()
        {
            return IsConnected && AMCProtocolParser.AmcpTcpParser.SendCommand(AMCPCommand.RESTART);
        }


       /// <inheritdoc/>
        public bool ChannelGrid()
        {
            return IsConnected && AMCProtocolParser.AmcpTcpParser.SendCommand(AMCPCommand.CHANNEL_GRID);
        }


        #region Diagnostics

       /// <inheritdoc/>
        public bool SetLogLevel(LogLevel logLevel)
        {
            return AMCProtocolParser.AmcpTcpParser.SendCommand($"{AMCPCommand.LOG_LEVEL.ToAmcpValue()} {logLevel.ToAmcpValue()}");
        }


       /// <inheritdoc/>
        public bool SetLogCategory(LogCategory logCategory, bool enable)
        {
            return AMCProtocolParser.AmcpTcpParser.SendCommand($"{AMCPCommand.LOG_CATEGORY.ToAmcpValue()} {logCategory.ToAmcpValue()} {(enable ? "1" : "0")}");
        }

        #endregion

        #region Query

       /// <inheritdoc/>
        public GLInfo GetGLInfo()
        {
            GLInfo glInfo = null;
            lock (_lockObject)
            {
                if (!IsConnected)
                    return null;

                var raised = false;

                void Handler(object o, GLInfoEventArgs e)
                {
                    glInfo = e.GLInfo; raised = true;
                }
                AMCProtocolParser.GlInfoReceived += Handler;
                AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus(AMCPCommand.GLINFO);

                SpinWait.SpinUntil(() => !raised, TimeSpan.FromSeconds(MaxWaitTimeInSec));

                AMCProtocolParser.GlInfoReceived -= Handler;
                return glInfo;
            }
        }

       /// <inheritdoc/>
        public IList<ChannelInfo> GetInfo()
        {
            lock (_lockObject)
            {
                if (!IsConnected)
                    return null;
                var raised = false;

                void Handler(object o, InfoEventArgs e)
                {
                    OnUpdatedChannelInfo(o, e); raised = true;
                }
                AMCProtocolParser.InfoReceived += Handler;
                AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus(AMCPCommand.INFO);

                SpinWait.SpinUntil(() => !raised, TimeSpan.FromSeconds(MaxWaitTimeInSec));

                AMCProtocolParser.InfoReceived -= Handler;
                return Channels.OfType<ChannelInfo>().ToList();
            }

        }

       /// <inheritdoc/>
        public IList<ThreadsInfo> GetInfoThreads()
        {

            var threadsInfos = new List<ThreadsInfo>();
            lock (_lockObject)
            {
                if (!IsConnected)
                    return threadsInfos;

                var raised = false;
                void Handler(object o, InfoThreadsEventArgs e)
                {
                    threadsInfos = e.ThreadsInfo;
                    raised = true;
                }
                AMCProtocolParser.InfoThreadsReceive += Handler;
                AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus(AMCPCommand.INFO_THREADS);

                SpinWait.SpinUntil(() => !raised, TimeSpan.FromSeconds(MaxWaitTimeInSec));

                AMCProtocolParser.InfoThreadsReceive -= Handler;
            }

            return threadsInfos;
        }

        /// <inheritdoc/>
        public SystemInfo GetInfoSystem()
        {
            lock (_lockObject)
            {
                if (!IsConnected)
                    return null;
                var raised = false;
                void Handler(object o, InfoSystemEventArgs e)
                {
                    SystemInfo = e.SystemInfo;
                    raised = true;
                }
                AMCProtocolParser.InfoSystemReceived += Handler;
                AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus(AMCPCommand.INFO_SYSTEM);

                SpinWait.SpinUntil(() => !raised, TimeSpan.FromSeconds(MaxWaitTimeInSec));

                AMCProtocolParser.InfoSystemReceived -= Handler;
                return SystemInfo;
            }
        }

       /// <inheritdoc/>
        public PathsInfo GetInfoPaths()
        {
            lock (_lockObject)
            {
                if (!IsConnected)
                    return null;
                var raised = false;
                void Handler(object o, InfoPathsEventArgs e)
                {
                    PathsInfo = e.PathsInfo;
                    raised = true;
                }
                AMCProtocolParser.InfoPathsReceived += Handler;
                AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus(AMCPCommand.INFO_PATHS);

                SpinWait.SpinUntil(() => !raised, TimeSpan.FromSeconds(MaxWaitTimeInSec));

                AMCProtocolParser.InfoPathsReceived -= Handler;
                return PathsInfo;
            }
        }

       /// <inheritdoc/>
        public TemplateInfo GetInfoTemplate(string templateFilePath)
        {
            return GetInfoTemplate(new TemplateBaseInfo(templateFilePath));
        }

       /// <inheritdoc/>
        public TemplateInfo GetInfoTemplate(TemplateBaseInfo template)
        {
            TemplateInfo info = null;
            lock (_lockObject)
            {
                if (!IsConnected)
                    return null;
                var raised = false;
                void Handler(object o, TemplateInfoEventArgs e)
                {
                    info = e.TemplateInfo;
                    info.Folder = template.Folder;
                    info.Name = template.Name;
                    info.LastUpdated = template.LastUpdated;
                    raised = true;
                }
                AMCProtocolParser.InfoTemplateReceived += Handler;
                AMCProtocolParser.AmcpTcpParser.SendCommand($"{AMCPCommand.INFO_TEMPLATE} {template.FullName}");

                SpinWait.SpinUntil(() => !raised, TimeSpan.FromSeconds(MaxWaitTimeInSec));

                AMCProtocolParser.InfoTemplateReceived -= Handler;
                return info;
            }

        }

       /// <inheritdoc/>
        public string GetVersion()
        {
            lock (_lockObject)
            {
                if (!IsConnected)
                    return null;
                Version = AMCProtocolParser.AmcpTcpParser.SendCommandAndGetResponse(AMCPCommand.VERSION)?.Data
                    .FirstOrDefault();
            }

            return Version;

        }

       /// <inheritdoc/>
        public IList<MediaInfo> GetMediafiles()
        {
            lock (_lockObject)
            {
                if (!IsConnected)
                    return new List<MediaInfo>();

                var raised = false;

                void Handler(object o, CLSEventArgs e)
                {
                    OnUpdatedMediafiles(o, e);
                    raised = true;
                }

                AMCProtocolParser.CLSReceived += Handler;
                AMCProtocolParser.AmcpTcpParser.SendCommand(AMCPCommand.CLS);

                SpinWait.SpinUntil(() => !raised, TimeSpan.FromSeconds(MaxWaitTimeInSec));

                AMCProtocolParser.CLSReceived -= Handler;
            }

            return Mediafiles;

        }

       /// <inheritdoc/>
        public TemplatesCollection GetTemplates()
        {
            lock (_lockObject)
            {
                if (!IsConnected)
                    return new TemplatesCollection();

                var raised = false;
                EventHandler<TLSEventArgs> handler = (o, e) =>
                {
                    OnUpdatedTemplatesList(o, e);
                    raised = true;
                };
                AMCProtocolParser.TLSReceived += handler;
                AMCProtocolParser.AmcpTcpParser.SendCommand(AMCPCommand.TLS);

                SpinWait.SpinUntil(() => !raised, TimeSpan.FromSeconds(MaxWaitTimeInSec));

                AMCProtocolParser.TLSReceived -= handler;
            }

            return Templates;

        }

       /// <inheritdoc/>
        public IList<string> GetFonts()
        {
            lock (_lockObject)
            {
                if (!IsConnected)
                    return Fonts;

                var raised = false;

                void Handler(object o, AMCPEventArgs e)
                {
                    Fonts = e.Data;
                    raised = true;
                }

                AMCProtocolParser.FlsReceived += Handler;
                AMCProtocolParser.AmcpTcpParser.SendCommand(AMCPCommand.TLS);

                SpinWait.SpinUntil(() => !raised, TimeSpan.FromSeconds(MaxWaitTimeInSec));

                AMCProtocolParser.FlsReceived -= Handler;
            }

            return Fonts;
        }




        #endregion


        #region Data

        ///<inheritdoc />
        public IList<string> GetDatalist()
        {
            lock (_lockObject)
            {
                if (!IsConnected)
                    return new List<string>();

                var raised = false;
                var counter = 0;

                void Handler(object o, DataListEventArgs e)
                {
                    OnUpdatedDataList(o, e);
                    raised = true;
                }

                AMCProtocolParser.DataListUpdated += Handler;
                AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus(AMCPCommand.DATA_LIST);

                while (!raised && counter < MaxWaitTimeInSec)
                {
                    Thread.Sleep(10);
                    counter++;
                }

                AMCProtocolParser.DataListUpdated -= Handler;
            }

            return Datafiles;
        }

        ///<inheritdoc />
        public bool DeleteData(string name)
        {
            if (!IsConnected)
                return false;

            return AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus($"{AMCPCommand.DATA_REMOVE.ToAmcpValue()} {name}") == AMCPError.None;
        }

        ///<inheritdoc />
        public bool StoreData(string name, string data)
        {
            if (!IsConnected)
                return false;

            return AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus($"{AMCPCommand.DATA_STORE.ToAmcpValue()} \"{name}\" \"{data}\"") == AMCPError.None;
        }

        ///<inheritdoc />
        public string GetData(string name)
        {

            if (!IsConnected)
                return null;

            var raised = false;
            var counter = 0;
            string data = null;

            lock (_lockObject)
            {
                void handler(object o, DataRetrieveEventArgs e)
                {
                    data = e.Data;
                    raised = true;
                }

                AMCProtocolParser.DataRetrieved += handler;

                AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus(
                    $"{AMCPCommand.DATA_RETRIEVE.ToAmcpValue()} \"{name}\"");

                while (!raised && counter < MaxWaitTimeInSec)
                {
                    Thread.Sleep(10);
                    counter++;
                }

                AMCProtocolParser.DataRetrieved -= handler;
            }

            return data;
        }
        #endregion

        #region Thumbnail
        ///<inheritdoc />
        public IList<Thumbnail> GetThumbnailList()
        {
            if (!IsConnected)
                return new List<Thumbnail>();

            lock (_lockObject)
            {
                var raised = false;

                void handler(object o, ThumbnailsListEventArgs e)
                {
                    OnUpdatedThumbnailList(o, e);
                    raised = true;
                }

                AMCProtocolParser.ThumbnailsListReceived += handler;
                AMCProtocolParser.AmcpTcpParser.SendCommand(AMCPCommand.THUMBNAIL_LIST);

                SpinWait.SpinUntil(() => !raised, TimeSpan.FromSeconds(MaxWaitTimeInSec));

                AMCProtocolParser.ThumbnailsListReceived -= handler;
            }

            return Thumbnails;
        }

        ///<inheritdoc />
        public string GetThumbnail(string filename)
        {
            if (!IsConnected)
                return null;

            var raised = false;
            string data = null;
            void handler(object o, ThumbnailsRetreiveEventArgs e) { data = e.Base64Image; raised = true; }
            AMCProtocolParser.ThumbnailsRetrievedReceived += handler;

            AMCProtocolParser.AmcpTcpParser.SendCommand($"{AMCPCommand.THUMBNAIL_RETRIEVE.ToAmcpValue()} {filename}");

            SpinWait.SpinUntil(() => !raised, TimeSpan.FromSeconds(MaxWaitTimeInSec));

            AMCProtocolParser.ThumbnailsRetrievedReceived -= handler;
            return data;
        }

        ///<inheritdoc />
        public bool GenerateThumbnail(string filename)
        {
            return IsConnected && AMCProtocolParser.AmcpTcpParser.SendCommand($"{AMCPCommand.THUMBNAIL_GENERATE.ToAmcpValue()} {filename}");
        }

        ///<inheritdoc />
        public bool GenerateAllThumbnail(string filename)
        {
            return IsConnected && AMCProtocolParser.AmcpTcpParser.SendCommand(AMCPCommand.THUMBNAIL_GENERATEALL);
        }

        #endregion
        ///<inheritdoc />
        public bool Connect()
        {
            if (IsConnected)
                return false;

            lock (_lockObject)
            {
                Connection.Connect();
            }

            return true;
        }

        ///<inheritdoc />
        public void Disconnect()
        {
            lock (_lockObject)
            {
                Connection.Disconnect();
            }
        }


        private void OnUpdatedChannelInfo(object sender, InfoEventArgs e)
        {
            Channels = Channels ?? new List<ChannelManager>();
            foreach (var channelInfo in e.ChannelsInfo)
            {
                var channel = Channels.FirstOrDefault(x => x.ID == channelInfo.ID);
                if (channel != null)
                    channel.VideoMode = channelInfo.VideoMode;
                else
                    Channels.Add(new ChannelManager(AMCProtocolParser, channelInfo.ID, channelInfo.VideoMode));
            }
            ChannelsUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void OnUpdatedTemplatesList(object sender, TLSEventArgs e)
        {
            Templates = new TemplatesCollection(e.Templates);
            TemplatesUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void OnUpdatedMediafiles(object sender, CLSEventArgs e)
        {
            Mediafiles = e.Medias;
            MediafilesUpdated?.Invoke(this, EventArgs.Empty);
        }



        private void OnUpdatedDataList(object sender, DataListEventArgs e)
        {
            Datafiles = e.Datas;
            DatafilesUpdated?.Invoke(this, EventArgs.Empty);
        }

    
        private void OnUpdatedThumbnailList(object sender, ThumbnailsListEventArgs e)
        {
            Thumbnails = e.Thumbnails;
            ThumbnailsUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
