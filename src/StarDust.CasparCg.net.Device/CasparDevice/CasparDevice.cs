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
    public class CasparDevice : ICasparDevice
    {
        #region Fields

        const int MaxWaitTimeInSec = 5;
        private readonly object lockObject = new object();
        #endregion

        #region Properties

        /// <summary>
        /// Paths info of this server
        /// </summary>
        public PathsInfo PathsInfo { get; private set; }


        /// <summary>
        /// System info of this server
        /// </summary>
        public SystemInfo SystemInfo { get; private set; }

        /// <summary>
        /// Tcp connection to the CasparCG Server
        /// </summary>
        public IServerConnection Connection { get; private set; }


        /// <summary>
        /// Procotol parser
        /// </summary>
        public IAMCPProtocolParser AMCProtocolParser { get; }

        /// <summary>
        /// Channels declared in the CasparCG Server
        /// </summary>
        public IList<ChannelManager> Channels { get; private set; }

        /// <summary>
        /// Templates store in the CasparCG Server
        /// </summary>
        public TemplatesCollection Templates { get; private set; }

        /// <summary>
        /// Mediafiles store in the CasparCG Server
        /// </summary>
        public IList<MediaInfo> Mediafiles { get; private set; }


        /// <summary>
        /// Thumbnails store in the CasparCG Server
        /// </summary>
        public IList<Thumbnail> Thumbnails { get; private set; }

        /// <summary>
        /// Data files store in the CasparCG Server
        /// </summary>
        public IList<string> Datafiles { get; private set; }

        /// <summary>
        /// Current version of the server
        /// </summary>
        public string Version { get; private set; }

        /// <summary>
        /// Is the CasparCg is connected
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return Connection != null && Connection.IsConnected;
            }
        }

        public List<string> Fonts { get; private set; }


        #endregion

        #region Events
        public event EventHandler<ConnectionEventArgs> ConnectionStatusChanged;
        public event EventHandler<EventArgs> DataRetrieved;
        public event EventHandler<EventArgs> ChannelsUpdated;
        public event EventHandler<EventArgs> TemplatesUpdated;
        public event EventHandler<EventArgs> MediafilesUpdated;
        public event EventHandler<EventArgs> DatafilesUpdated;
        public event EventHandler<EventArgs> ThumbnailsUpdated;
        #endregion

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

            Connection.ConnectionStateChanged += new EventHandler<ConnectionEventArgs>(Server__ConnectionStateChanged);
        }



        private async void Server__ConnectionStateChanged(object sender, ConnectionEventArgs e)
        {

            if (e.Connected)
            {
                ConnectionStatusChanged?.Invoke(this, e);
                try
                {
                    await Task.Factory.StartNew(() => GetVersion());
                    await Task.Factory.StartNew(() => GetInfo());
                    await Task.Factory.StartNew(() => GetThumbnailList());
                    await Task.Factory.StartNew(() => GetDatalist());
                    await Task.Factory.StartNew(() => GetTemplates());
                    await Task.Factory.StartNew(() => GetMediafiles());
                    await Task.Factory.StartNew(() => GetInfoPaths());
                    await Task.Factory.StartNew(() => GetInfoSystem());

                }
                catch
                {
                    //We don't want to crash the connection
                }

            }

        }



        /// <inheritdoc cref=""/>
        public bool GLGc()
        {
            if (!IsConnected)
                return false;

            return AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus(AMCPCommand.GLGC) == AMCPError.None;
        }

        /// <inheritdoc cref=""/>
        public bool Restart()
        {
            if (!IsConnected)
                return false;

            return AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus(AMCPCommand.RESTART) == AMCPError.None;
        }


        /// <inheritdoc cref=""/>
        public bool ChannelGrid()
        {
            if (!IsConnected)
                return false;

            return AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus(AMCPCommand.CHANNEL_GRID) == AMCPError.None;
        }


        #region Diagnostics

        /// <inheritdoc cref=""/>
        public bool SetLogLevel(LogLevel logLevel)
        {
            return AMCProtocolParser.AmcpTcpParser.SendCommand($"{AMCPCommand.LOG_LEVEL.ToAmcpValue()} {logLevel.ToAmcpValue()}");
        }


        /// <inheritdoc cref=""/>
        public bool SetLogCategory(LogCategory logCategory, bool enable)
        {
            return AMCProtocolParser.AmcpTcpParser.SendCommand($"{AMCPCommand.LOG_CATEGORY.ToAmcpValue()} {logCategory.ToAmcpValue()} {(enable ? "1" : "0")}");
        }

        #endregion

        #region Query

        /// <inheritdoc cref=""/>
        public GLInfo GetGLInfo()
        {
            GLInfo glInfo = null;
            lock (lockObject)
            {
                if (!IsConnected)
                    return null;

                var raised = false;

                void handler(object o, GLInfoEventArgs e)
                {
                    glInfo = e.GLInfo; raised = true;
                }
                AMCProtocolParser.GlInfoReceived += handler;
                AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus(AMCPCommand.GLINFO);

                SpinWait.SpinUntil(() => !raised, TimeSpan.FromSeconds(MaxWaitTimeInSec));

                AMCProtocolParser.GlInfoReceived -= handler;
                return glInfo;
            }
        }

        /// <inheritdoc cref=""/>
        public IList<ChannelInfo> GetInfo()
        {
            lock (lockObject)
            {
                if (!IsConnected)
                    return null;
                var raised = false;

                void handler(object o, InfoEventArgs e)
                {
                    OnUpdatedChannelInfo(o, e); raised = true;
                }
                AMCProtocolParser.InfoReceived += handler;
                AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus(AMCPCommand.INFO);

                SpinWait.SpinUntil(() => !raised, TimeSpan.FromSeconds(MaxWaitTimeInSec));

                AMCProtocolParser.InfoReceived -= handler;
                return Channels.OfType<ChannelInfo>().ToList();
            }

        }

        /// <inheritdoc cref=""/>
        public IList<ThreadsInfo> GetInfoThreads()
        {

            var threadsInfos = new List<ThreadsInfo>();
            lock (lockObject)
            {
                if (!IsConnected)
                    return threadsInfos;

                var raised = false;
                void handler(object o, InfoThreadsEventArgs e)
                {
                    threadsInfos = e.ThreadsInfo;
                    raised = true;
                }
                AMCProtocolParser.InfoThreadsReceive += handler;
                AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus(AMCPCommand.INFO_THREADS);

                SpinWait.SpinUntil(() => !raised, TimeSpan.FromSeconds(MaxWaitTimeInSec));

                AMCProtocolParser.InfoThreadsReceive -= handler;
            }

            return threadsInfos;
        }

        /// <inheritdoc cref=""/>
        public SystemInfo GetInfoSystem()
        {
            lock (lockObject)
            {
                if (!IsConnected)
                    return null;
                var raised = false;
                void handler(object o, InfoSystemEventArgs e)
                {
                    SystemInfo = e.SystemInfo;
                    raised = true;
                }
                AMCProtocolParser.InfoSystemReceived += handler;
                AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus(AMCPCommand.INFO_SYSTEM);

                SpinWait.SpinUntil(() => !raised, TimeSpan.FromSeconds(MaxWaitTimeInSec));

                AMCProtocolParser.InfoSystemReceived -= handler;
                return SystemInfo;
            }
        }

        /// <inheritdoc cref=""/>
        public PathsInfo GetInfoPaths()
        {
            lock (lockObject)
            {
                if (!IsConnected)
                    return null;
                var raised = false;
                void handler(object o, InfoPathsEventArgs e)
                {
                    PathsInfo = e.PathsInfo;
                    raised = true;
                }
                AMCProtocolParser.InfoPathsReceived += handler;
                AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus(AMCPCommand.INFO_PATHS);

                SpinWait.SpinUntil(() => !raised, TimeSpan.FromSeconds(MaxWaitTimeInSec));

                AMCProtocolParser.InfoPathsReceived -= handler;
                return PathsInfo;
            }
        }

        /// <inheritdoc cref=""/>
        public TemplateInfo GetInfoTemplate(string templateFilePath)
        {
            return GetInfoTemplate(new TemplateBaseInfo(templateFilePath));
        }

        /// <inheritdoc cref=""/>
        public TemplateInfo GetInfoTemplate(TemplateBaseInfo template)
        {
            TemplateInfo info = null;
            lock (lockObject)
            {
                if (!IsConnected)
                    return null;
                var raised = false;
                void handler(object o, TemplateInfoEventArgs e)
                {
                    info = e.TemplateInfo;
                    info.Folder = template.Folder;
                    info.Name = template.Name;
                    info.LastUpdated = template.LastUpdated;
                    raised = true;
                }
                AMCProtocolParser.InfoTemplateReceived += handler;
                AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus($"{AMCPCommand.INFO_TEMPLATE.ToAmcpValue()} {template.FullName}");

                SpinWait.SpinUntil(() => !raised, TimeSpan.FromSeconds(MaxWaitTimeInSec));

                AMCProtocolParser.InfoTemplateReceived -= handler;
                return info;
            }

        }

        /// <inheritdoc cref=""/>
        public string GetVersion()
        {
            lock (lockObject)
            {
                if (!IsConnected)
                    return null;
                Version = AMCProtocolParser.AmcpTcpParser.SendCommandAndGetResponse(AMCPCommand.VERSION, null)?.Data
                    .FirstOrDefault();
            }

            return Version;

        }

        /// <inheritdoc cref=""/>
        public IList<MediaInfo> GetMediafiles()
        {
            lock (lockObject)
            {
                if (!IsConnected)
                    return new List<MediaInfo>();

                var raised = false;

                void handler(object o, CLSEventArgs e)
                {
                    OnUpdatedMediafiles(o, e);
                    raised = true;
                }

                AMCProtocolParser.CLSReceived += handler;
                AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus(AMCPCommand.CLS);

                SpinWait.SpinUntil(() => !raised, TimeSpan.FromSeconds(MaxWaitTimeInSec));

                AMCProtocolParser.CLSReceived -= handler;
            }

            return Mediafiles;

        }

        /// <inheritdoc cref=""/>
        public TemplatesCollection GetTemplates()
        {
            lock (lockObject)
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
                AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus(AMCPCommand.TLS);

                SpinWait.SpinUntil(() => !raised, TimeSpan.FromSeconds(MaxWaitTimeInSec));

                AMCProtocolParser.TLSReceived -= handler;
            }

            return Templates;

        }

        /// <inheritdoc cref=""/>
        public IList<string> GetFonts()
        {
            lock (lockObject)
            {
                if (!IsConnected)
                    return Fonts;

                var raised = false;
                EventHandler<AMCPEventArgs> handler = (o, e) =>
                {
                    Fonts = e.Data;
                    raised = true;
                };
                AMCProtocolParser.FlsReceived += handler;
                AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus(AMCPCommand.TLS);

                SpinWait.SpinUntil(() => !raised, TimeSpan.FromSeconds(MaxWaitTimeInSec));

                AMCProtocolParser.FlsReceived -= handler;
            }

            return Fonts;
        }




        #endregion


        #region Data
        public IList<string> GetDatalist()
        {
            lock (lockObject)
            {
                if (!IsConnected)
                    return new List<string>();

                var raised = false;
                var counter = 0;

                void handler(object o, DataListEventArgs e)
                {
                    OnUpdatedDataList(o, e);
                    raised = true;
                }

                AMCProtocolParser.DataListUpdated += handler;
                AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus(AMCPCommand.DATA_LIST);

                while (!raised && counter < MaxWaitTimeInSec)
                {
                    Thread.Sleep(10);
                    counter++;
                }

                AMCProtocolParser.DataListUpdated -= handler;
            }

            return Datafiles;
        }

        public bool DeleteData(string name)
        {
            if (!IsConnected)
                return false;

            return AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus($"{AMCPCommand.DATA_REMOVE.ToAmcpValue()} {name}") == AMCPError.None;
        }
        public bool StoreData(string name, string data)
        {
            if (!IsConnected)
                return false;

            return AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus($"{AMCPCommand.DATA_STORE.ToAmcpValue()} \"{name}\" \"{data}\"") == AMCPError.None;
        }
        public string GetData(string name)
        {

            if (!IsConnected)
                return null;

            var raised = false;
            var counter = 0;
            string data = null;

            lock (lockObject)
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
        public IList<Thumbnail> GetThumbnailList()
        {
            if (!IsConnected)
                return new List<Thumbnail>();

            lock (lockObject)
            {
                var raised = false;

                void handler(object o, ThumbnailsListEventArgs e)
                {
                    OnUpdatedThumbnailList(o, e);
                    raised = true;
                }

                AMCProtocolParser.ThumbnailsListReceived += handler;
                AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus(AMCPCommand.THUMBNAIL_LIST);

                SpinWait.SpinUntil(() => !raised, TimeSpan.FromSeconds(MaxWaitTimeInSec));

                AMCProtocolParser.ThumbnailsListReceived -= handler;
            }

            return Thumbnails;
        }
        public string GetThumbnail(string filename)
        {
            if (!IsConnected)
                return null;

            var raised = false;
            string data = null;
            void handler(object o, ThumbnailsRetreiveEventArgs e) { data = e.Base64Image; raised = true; }
            AMCProtocolParser.ThumbnailsRetrievedReceived += handler;

            AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus($"{AMCPCommand.THUMBNAIL_RETRIEVE.ToAmcpValue()} {filename}");

            SpinWait.SpinUntil(() => !raised, TimeSpan.FromSeconds(MaxWaitTimeInSec));

            AMCProtocolParser.ThumbnailsRetrievedReceived -= handler;
            return data;
        }
        public bool GenerateThumbnail(string filename)
        {
            if (!IsConnected)
                return false;

            return AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus($"{AMCPCommand.THUMBNAIL_GENERATE.ToAmcpValue()} {filename}") == AMCPError.None;
        }
        public bool GenerateAllThumbnail(string filename)
        {
            if (!IsConnected)
                return false;

            return AMCProtocolParser.AmcpTcpParser.SendCommandAndGetStatus(AMCPCommand.THUMBNAIL_GENERATEALL) == AMCPError.None;
        }

        #endregion

        public bool Connect()
        {

            if (IsConnected)
                return false;
            lock (lockObject)
            {
                Connection.Connect();
            }

            return true;
        }

        public void Disconnect()
        {
            lock (lockObject)
            {
                Connection.Disconnect();
            }
        }

        private void OnUpdatedChannelInfo(object sender, InfoEventArgs e)
        {
            Channels = Channels ?? new List<ChannelManager>();
            foreach (ChannelInfo channelInfo in e.ChannelsInfo)
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

        private void OnVersion(object sender, VersionEventArgs e)
        {
            Version = e.Version;
        }

        private void OnUpdatedDataList(object sender, DataListEventArgs e)
        {
            Datafiles = e.Datas;
            DatafilesUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void OnDataRetrieved(object sender, DataRetrieveEventArgs e)
        {
            DataRetrieved?.Invoke(this, EventArgs.Empty);
        }

        private void OnUpdatedThumbnailList(object sender, ThumbnailsListEventArgs e)
        {
            Thumbnails = e.Thumbnails;
            ThumbnailsUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
