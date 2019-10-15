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
                await Task.Factory.StartNew(GetVersion)
                    .ContinueWith(t => GetInfoAsync())
                    .ContinueWith(t => GetThumbnailList())
                    .ContinueWith(t => GetDatalist())
                    .ContinueWith(t => GetTemplates())
                    .ContinueWith(t => GetMediafiles())
                    .ContinueWith(t => GetInfoPaths())
                    .ContinueWith(t => GetInfoSystem());
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
        public async Task<GLInfo> GetGLInfoAsync()
        {
            GLInfo glInfo = null;

            if (!IsConnected)
                return null;

            using (var signal = new SemaphoreSlim(0, 1))
            {
                void Handler(object o, GLInfoEventArgs e)
                {
                    glInfo = e.GLInfo;
                    signal.Release();
                }

                AMCProtocolParser.GlInfoReceived += Handler;
                AMCProtocolParser.AmcpTcpParser.SendCommand(AMCPCommand.GLINFO);

                await signal.WaitAsync(TimeSpan.FromSeconds(MaxWaitTimeInSec));
                AMCProtocolParser.GlInfoReceived -= Handler;

                return glInfo;
            }
        }

        /// <inheritdoc/>
        public GLInfo GetGLInfo()
        {
            return GetGLInfoAsync().GetAwaiter().GetResult();
        }

        /// <inheritdoc/>
        public IList<ChannelInfo> GetInfo()
        {
            return GetInfoAsync().GetAwaiter().GetResult();
        }

        /// <inheritdoc/>
        public async Task<IList<ChannelInfo>> GetInfoAsync()
        {
            if (!IsConnected)
                return null;

            using (var signal = new SemaphoreSlim(0, 1))
            {
                void Handler(object o, InfoEventArgs e)
                {
                    OnUpdatedChannelInfo(e);
                    signal.Release();
                }

                AMCProtocolParser.InfoReceived += Handler;
                AMCProtocolParser.AmcpTcpParser.SendCommand(AMCPCommand.INFO);


                await signal.WaitAsync(TimeSpan.FromSeconds(MaxWaitTimeInSec));
                AMCProtocolParser.InfoReceived -= Handler;

                return Channels.OfType<ChannelInfo>().ToList();
            }
        }



        /// <inheritdoc/>
        public async Task<IList<ThreadsInfo>> GetInfoThreadsAsync()
        {
            var threadsInfos = new List<ThreadsInfo>();

            if (!IsConnected)
                return threadsInfos;

            using (var signal = new SemaphoreSlim(0, 1))
            {
                void Handler(object o, InfoThreadsEventArgs e)
                {
                    threadsInfos = e.ThreadsInfo;
                    signal.Release();
                }

                AMCProtocolParser.InfoThreadsReceive += Handler;
                AMCProtocolParser.AmcpTcpParser.SendCommand(AMCPCommand.INFO_THREADS);

                await signal.WaitAsync(TimeSpan.FromSeconds(MaxWaitTimeInSec));
                AMCProtocolParser.InfoThreadsReceive -= Handler;

                return threadsInfos;
            }
        }


        /// <inheritdoc/>
        public IList<ThreadsInfo> GetInfoThreads()
        {
            return GetInfoThreadsAsync().GetAwaiter().GetResult();
        }


        /// <inheritdoc/>
        public async Task<SystemInfo> GetInfoSystemAsync()
        {
            if (!IsConnected)
                return null;

            using (var signal = new SemaphoreSlim(0, 1))
            {
                void Handler(object o, InfoSystemEventArgs e)
                {
                    SystemInfo = e.SystemInfo;
                    signal.Release();
                }

                AMCProtocolParser.InfoSystemReceived += Handler;
                AMCProtocolParser.AmcpTcpParser.SendCommand(AMCPCommand.INFO_SYSTEM);

                await signal.WaitAsync(TimeSpan.FromSeconds(MaxWaitTimeInSec));
                AMCProtocolParser.InfoSystemReceived -= Handler;

                return SystemInfo;
            }
        }

        /// <inheritdoc/>
        public SystemInfo GetInfoSystem()
        {
            return GetInfoSystemAsync().GetAwaiter().GetResult();
        }

        /// <inheritdoc/>
        public async Task<PathsInfo> GetInfoPathsAsync()
        {
            if (!IsConnected)
                return null;
            using (var signal = new SemaphoreSlim(0, 1))
            {
                void Handler(object o, InfoPathsEventArgs e)
                {
                    PathsInfo = e.PathsInfo;
                    signal.Release();
                }

                AMCProtocolParser.InfoPathsReceived += Handler;
                AMCProtocolParser.AmcpTcpParser.SendCommand(AMCPCommand.INFO_PATHS);
                await signal.WaitAsync(TimeSpan.FromSeconds(MaxWaitTimeInSec));
                AMCProtocolParser.InfoPathsReceived -= Handler;

                return PathsInfo;
            }
        }

        /// <inheritdoc/>
        public PathsInfo GetInfoPaths()
        {
            return GetInfoPathsAsync().GetAwaiter().GetResult();
        }

        /// <inheritdoc/>
        public TemplateInfo GetInfoTemplate(string templateFilePath)
        {
            return GetInfoTemplateAsync(new TemplateBaseInfo(templateFilePath)).GetAwaiter().GetResult();
        }

        /// <inheritdoc/>
        public TemplateInfo GetInfoTemplate(TemplateBaseInfo template)
        {
            return GetInfoTemplateAsync(template).GetAwaiter().GetResult();
        }

        /// <inheritdoc/>
        public Task<TemplateInfo> GetInfoTemplateAsync(string templateFilePath)
        {
            return GetInfoTemplateAsync(new TemplateBaseInfo(templateFilePath));
        }

        /// <inheritdoc/>
        public async Task<TemplateInfo> GetInfoTemplateAsync(TemplateBaseInfo template)
        {
            TemplateInfo info = null;

            if (!IsConnected)
                return null;

            using (var signal = new SemaphoreSlim(0, 1))
            {
                void Handler(object o, TemplateInfoEventArgs e)
                {
                    info = e.TemplateInfo;

                    if (info != null)
                    {
                        info.Folder = template.Folder;
                        info.Name = template.Name;
                        info.LastUpdated = template.LastUpdated;
                    }

                    signal.Release();
                }

                AMCProtocolParser.InfoTemplateReceived += Handler;
                AMCProtocolParser.AmcpTcpParser.SendCommand($"{AMCPCommand.INFO_TEMPLATE} {template.FullName}");
                await signal.WaitAsync(TimeSpan.FromSeconds(MaxWaitTimeInSec));
                AMCProtocolParser.InfoTemplateReceived -= Handler;

                return info;
            }
        }

        /// <inheritdoc/>
        public string GetVersion()
        {
            if (!IsConnected)
                return null;
            Version = AMCProtocolParser.AmcpTcpParser.SendCommandAndGetResponse(AMCPCommand.VERSION)?.Data
                .FirstOrDefault();

            return Version;
        }

        /// <inheritdoc/>
        public async Task<IList<MediaInfo>> GetMediafilesAsync()
        {

            if (!IsConnected)
                return new List<MediaInfo>();

            using (var signal = new SemaphoreSlim(0, 1))
            {
                void Handler(object o, CLSEventArgs e)
                {
                    OnUpdatedMediafiles(e);
                    signal.Release();
                }

                AMCProtocolParser.CLSReceived += Handler;
                AMCProtocolParser.AmcpTcpParser.SendCommand(AMCPCommand.CLS);

                await signal.WaitAsync(TimeSpan.FromSeconds(MaxWaitTimeInSec));
                AMCProtocolParser.CLSReceived -= Handler;

                return Mediafiles;
            }
        }

        /// <inheritdoc/>
        public IList<MediaInfo> GetMediafiles()
        {
            return GetMediafilesAsync().GetAwaiter().GetResult();

        }

        /// <inheritdoc/>
        public async Task<TemplatesCollection> GetTemplatesAsync()
        {
            if (!IsConnected)
                return new TemplatesCollection();

            using (var signal = new SemaphoreSlim(0, 1))
            {
                void Handler(object o, TLSEventArgs e)
                {
                    OnUpdatedTemplatesList(e);
                    signal.Release();
                }

                AMCProtocolParser.TLSReceived += Handler;
                AMCProtocolParser.AmcpTcpParser.SendCommand(AMCPCommand.TLS);

                await signal.WaitAsync(TimeSpan.FromSeconds(MaxWaitTimeInSec));

                AMCProtocolParser.TLSReceived -= Handler;

                return Templates;
            }
        }


        /// <inheritdoc/>
        public TemplatesCollection GetTemplates()
        {
            return GetTemplatesAsync().GetAwaiter().GetResult();
        }

        /// <inheritdoc/>
        public async Task<IList<string>> GetFontsAsync()
        {

            if (!IsConnected)
                return Fonts;

            using (var signal = new SemaphoreSlim(0, 1))
            {
                void Handler(object o, AMCPEventArgs e)
                {
                    Fonts = e.Data;
                    signal.Release();
                }

                AMCProtocolParser.FlsReceived += Handler;
                AMCProtocolParser.AmcpTcpParser.SendCommand(AMCPCommand.TLS);

                await signal.WaitAsync(TimeSpan.FromSeconds(MaxWaitTimeInSec));
                AMCProtocolParser.FlsReceived -= Handler;

                return Fonts;
            }
        }

        /// <inheritdoc/>
        public IList<string> GetFonts()
        {
            return GetFontsAsync().GetAwaiter().GetResult();
        }




        #endregion


        #region Data

        ///<inheritdoc />
        public async Task<IList<string>> GetDatalistAsync()
        {

            if (!IsConnected)
                return new List<string>();

            using (var signal = new SemaphoreSlim(0, 1))
            {
                void Handler(object o, DataListEventArgs e)
                {
                    OnUpdatedDataList(e);
                    signal.Release();
                }

                AMCProtocolParser.DataListUpdated += Handler;
                AMCProtocolParser.AmcpTcpParser.SendCommand(AMCPCommand.DATA_LIST);

                await signal.WaitAsync(TimeSpan.FromSeconds(MaxWaitTimeInSec));
                AMCProtocolParser.DataListUpdated -= Handler;

                return Datafiles;
            }
        }

        ///<inheritdoc />
        public IList<string> GetDatalist()
        {
            return GetDatalistAsync().GetAwaiter().GetResult();
        }

        ///<inheritdoc />
        public bool DeleteData(string name)
        {
            if (!IsConnected)
                return false;

            return AMCProtocolParser.AmcpTcpParser.SendCommand($"{AMCPCommand.DATA_REMOVE.ToAmcpValue()} {name}");
        }

        ///<inheritdoc />
        public bool StoreData(string name, string data)
        {
            if (!IsConnected)
                return false;

            return AMCProtocolParser.AmcpTcpParser.SendCommand($"{AMCPCommand.DATA_STORE.ToAmcpValue()} \"{name}\" \"{data}\"");
        }

        ///<inheritdoc />
        public async Task<string> GetDataAsync(string name)
        {

            if (!IsConnected)
                return null;


            string data = null;
            using (var signal = new SemaphoreSlim(0, 1))
            {
                void Handler(object o, DataRetrieveEventArgs e)
                {
                    data = e.Data;
                    signal.Release();
                }

                AMCProtocolParser.DataRetrieved += Handler;

                AMCProtocolParser.AmcpTcpParser.SendCommand(
                    $"{AMCPCommand.DATA_RETRIEVE.ToAmcpValue()} \"{name}\"");

                await signal.WaitAsync(TimeSpan.FromSeconds(MaxWaitTimeInSec));
                AMCProtocolParser.DataRetrieved -= Handler;

                return data;
            }
        }

        ///<inheritdoc />
        public string GetData(string name)
        {
            return GetDataAsync(name).GetAwaiter().GetResult();
        }
        #endregion

        #region Thumbnail

        ///<inheritdoc />
        public async Task<IList<Thumbnail>> GetThumbnailListAsync()
        {
            if (!IsConnected)
                return new List<Thumbnail>();


            using (var signal = new SemaphoreSlim(0, 1))
            {
                void Handler(object o, ThumbnailsListEventArgs e)
                {
                    OnUpdatedThumbnailList(e);
                    signal.Release();
                }

                AMCProtocolParser.ThumbnailsListReceived += Handler;
                AMCProtocolParser.AmcpTcpParser.SendCommand(AMCPCommand.THUMBNAIL_LIST);

                await signal.WaitAsync(TimeSpan.FromSeconds(MaxWaitTimeInSec));
                AMCProtocolParser.ThumbnailsListReceived -= Handler;

                return Thumbnails;
            }
        }

        ///<inheritdoc />
        public IList<Thumbnail> GetThumbnailList()
        {
            return GetThumbnailListAsync().GetAwaiter().GetResult();
        }

        ///<inheritdoc />
        public async Task<string> GetThumbnailAsync(string filename)
        {
            if (!IsConnected)
                return null;


            string data = null;
            using (var signal = new SemaphoreSlim(0, 1))
            {
                void Handler(object o, ThumbnailsRetreiveEventArgs e)
                {
                    data = e.Base64Image;
                    signal.Release();
                }

                AMCProtocolParser.ThumbnailsRetrievedReceived += Handler;

                AMCProtocolParser.AmcpTcpParser.SendCommand(
                    $"{AMCPCommand.THUMBNAIL_RETRIEVE.ToAmcpValue()} {filename}");

                await signal.WaitAsync(TimeSpan.FromSeconds(MaxWaitTimeInSec));
                AMCProtocolParser.ThumbnailsRetrievedReceived -= Handler;

                return data;
            }
        }

        ///<inheritdoc />
        public string GetThumbnail(string filename)
        {
            return GetThumbnailAsync(filename).GetAwaiter().GetResult();
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

            Connection.Connect();
            return true;
        }

        ///<inheritdoc />
        public void Disconnect()
        {
            Connection.Disconnect();
        }


        private void OnUpdatedChannelInfo(InfoEventArgs e)
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

        private void OnUpdatedTemplatesList(TLSEventArgs e)
        {
            Templates = new TemplatesCollection(e.Templates);
            TemplatesUpdated?.Invoke(this, EventArgs.Empty);
        }

        private void OnUpdatedMediafiles(CLSEventArgs e)
        {
            Mediafiles = e.Medias;
            MediafilesUpdated?.Invoke(this, EventArgs.Empty);
        }



        private void OnUpdatedDataList(DataListEventArgs e)
        {
            Datafiles = e.Datas;
            DatafilesUpdated?.Invoke(this, EventArgs.Empty);
        }


        private void OnUpdatedThumbnailList(ThumbnailsListEventArgs e)
        {
            Thumbnails = e.Thumbnails;
            ThumbnailsUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
