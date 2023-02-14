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
using StartDust.CasparCG.net.Crosscutting;

namespace StarDust.CasparCG.net.Device
{
    ///<inheritdoc/>
    public class CasparDevice : ICasparDevice
    {
        #region Fields

        /// <summary>
        /// Max wait time before re attempt
        /// </summary>
        protected const int MaxWaitTimeInSec = 5;
        private IList<ChannelManager> _channels;
        private TemplatesCollection _templates;
        private IList<MediaInfo> _mediaFiles;
        private IList<Thumbnail> _thumbnails;
        private IList<string> _dataList;

        #endregion

        #region Properties

        ///<inheritdoc />
        public PathsInfo PathsInfo { get; private set; }


        ///<inheritdoc />
        public SystemInfo SystemInfo { get; private set; }

        ///<inheritdoc />
        public IServerConnection Connection => AMCProtocolParser.AmcpTcpParser.ServerConnection;


        ///<inheritdoc />
        public IAMCPProtocolParser AMCProtocolParser { get; }

        ///<inheritdoc />
        public IList<ChannelManager> Channels
        {
            get
            {
                if (_channels == null || !_channels.Any())
                    GetInfo();
                return _channels;
            }
        }

        ///<inheritdoc />
        public TemplatesCollection Templates
        {
            get
            {
                if (_templates == null)
                    GetTemplates();

                return _templates;
            }
        }

        ///<inheritdoc />
        public IList<MediaInfo> Mediafiles
        {
            get
            {
                if (_mediaFiles == null)
                    GetMediafiles();

                return _mediaFiles;
            }
        }

        ///<inheritdoc />
        public IList<Thumbnail> Thumbnails
        {
            get
            {
                if (_thumbnails == null)
                    GetThumbnailList();
                return _thumbnails;
            }
        }

        ///<inheritdoc />
        public IList<string> Datafiles
        {
            get
            {
                if (_dataList == null)
                    GetDatalist();
                return _dataList;
            }
        }

        ///<inheritdoc />
        public string Version => GetVersion();

        ///<inheritdoc />
        public bool IsConnected => Connection != null && Connection.IsConnected;

        ///<inheritdoc />
        public IList<string> Fonts => GetFonts();

        /// <inheritdoc/>
        public CasparCGConnectionSettings ConnectionSettings { get; set; }

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
        public CasparDevice(IAMCPProtocolParser amcpProtocolParser)
        {
            AMCProtocolParser = amcpProtocolParser;
            Connection.ConnectionStateChanged += (s, e) => Server_ConnectionStateChanged(s, e);
        }

        /// <summary>
        /// Delegate for connection state changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        protected void Server_ConnectionStateChanged(object sender, ConnectionEventArgs e)
        {
            ConnectionStatusChanged?.Invoke(this, e);
        }

        /// <inheritdoc/>
        public virtual bool GLGc() => IsConnected && AMCProtocolParser.AmcpTcpParser.SendCommand(AMCPCommand.GLGC);

        /// <inheritdoc/>
        public virtual bool Restart() => IsConnected && AMCProtocolParser.AmcpTcpParser.SendCommand(AMCPCommand.RESTART);

        /// <inheritdoc/>
        public virtual bool ChannelGrid() => IsConnected && AMCProtocolParser.AmcpTcpParser.SendCommand(AMCPCommand.CHANNEL_GRID);

        #region Diagnostics

        /// <inheritdoc/>
        public virtual bool SetLogLevel(LogLevel logLevel) => AMCProtocolParser.AmcpTcpParser.SendCommand($"{AMCPCommand.LOG_LEVEL.ToAmcpValue()} {logLevel.ToAmcpValue()}");

        /// <inheritdoc/>
        public virtual bool SetLogCategory(LogCategory logCategory, bool enable) => AMCProtocolParser.AmcpTcpParser.SendCommand($"{AMCPCommand.LOG_CATEGORY.ToAmcpValue()} {logCategory.ToAmcpValue()} {(enable ? "1" : "0")}");

        #endregion

        #region Query

        /// <inheritdoc/>
        public virtual async Task<GLInfo> GetGLInfoAsync()
        {
            if (!IsConnected)
                return null;

            var eventWaiter = new EventAwaiter<GLInfoEventArgs>(
                h => AMCProtocolParser.GlInfoReceived += h,
                h => AMCProtocolParser.GlInfoReceived -= h);

            AMCProtocolParser.AmcpTcpParser.SendCommandAndCheckError(AMCPCommand.GLINFO);

            var e = await eventWaiter.WaitForEventRaised;
            return e.GLInfo;
        }

        /// <inheritdoc/>
        public virtual GLInfo GetGLInfo() => AsyncHelper.RunSync(GetGLInfoAsync);

        /// <inheritdoc/>
        public virtual IList<ChannelInfo> GetInfo() => AsyncHelper.RunSync(GetInfoAsync);

        /// <inheritdoc/>
        public virtual async Task<IList<ChannelInfo>> GetInfoAsync()
        {
            if (!IsConnected)
                return null;
            var eventWaiter = new EventAwaiter<InfoEventArgs>(
                h => AMCProtocolParser.InfoReceived += h,
                h => AMCProtocolParser.InfoReceived -= h);


            AMCProtocolParser.AmcpTcpParser.SendCommandAndCheckError(AMCPCommand.INFO);

            var e = await eventWaiter.WaitForEventRaised;
            if (e == null)
                return null;
            GenerateChannelManager(e.ChannelsInfo);
            return e.ChannelsInfo;
        }

        /// <inheritdoc/>
        public virtual async Task<IList<ThreadsInfo>> GetInfoThreadsAsync()
        {
            if (!IsConnected)
                return new List<ThreadsInfo>();

            var eventWaiter = new EventAwaiter<InfoThreadsEventArgs>(
                h => AMCProtocolParser.InfoThreadsReceive += h,
                h => AMCProtocolParser.InfoThreadsReceive -= h);

            AMCProtocolParser.AmcpTcpParser.SendCommandAndCheckError(AMCPCommand.INFO_THREADS);

            var e = await eventWaiter.WaitForEventRaised;
            return e.ThreadsInfo;
        }

        /// <inheritdoc/>
        public virtual IList<ThreadsInfo> GetInfoThreads() => AsyncHelper.RunSync(GetInfoThreadsAsync);

        /// <inheritdoc/>
        public virtual async Task<SystemInfo> GetInfoSystemAsync()
        {
            if (!IsConnected)
                return null;

            var eventWaiter = new EventAwaiter<InfoSystemEventArgs>(
                h => AMCProtocolParser.InfoSystemReceived += h,
                h => AMCProtocolParser.InfoSystemReceived -= h);

            AMCProtocolParser.AmcpTcpParser.SendCommandAndCheckError(AMCPCommand.INFO_SYSTEM);

            var e = await eventWaiter.WaitForEventRaised;
            return e?.SystemInfo;
        }

        /// <inheritdoc/>
        public virtual SystemInfo GetInfoSystem() => AsyncHelper.RunSync(GetInfoSystemAsync);

        /// <inheritdoc/>
        public virtual async Task<PathsInfo> GetInfoPathsAsync()
        {
            if (!IsConnected)
                return null;

            var eventWaiter = new EventAwaiter<InfoPathsEventArgs>(
                h => AMCProtocolParser.InfoPathsReceived += h,
                h => AMCProtocolParser.InfoPathsReceived -= h);

            AMCProtocolParser.AmcpTcpParser.SendCommandAndCheckError(AMCPCommand.INFO_PATHS);
            var e = await eventWaiter.WaitForEventRaised;

            return e.PathsInfo;
        }

        /// <inheritdoc/>
        public virtual PathsInfo GetInfoPaths() => AsyncHelper.RunSync(GetInfoPathsAsync);

        /// <inheritdoc/>
        public virtual TemplateInfo GetInfoTemplate(string templateFilePath) => AsyncHelper.RunSync(() => GetInfoTemplateAsync(new TemplateBaseInfo(templateFilePath)));

        /// <inheritdoc/>
        public virtual TemplateInfo GetInfoTemplate(TemplateBaseInfo template) => AsyncHelper.RunSync(() => GetInfoTemplateAsync(template));

        /// <inheritdoc/>
        public virtual Task<TemplateInfo> GetInfoTemplateAsync(string templateFilePath) => GetInfoTemplateAsync(new TemplateBaseInfo(templateFilePath));

        /// <inheritdoc/>
        public virtual async Task<TemplateInfo> GetInfoTemplateAsync(TemplateBaseInfo template)
        {
            if (!IsConnected)
                return null;

            var eventWaiter = new EventAwaiter<TemplateInfoEventArgs>(
                h => AMCProtocolParser.InfoTemplateReceived += h,
                h => AMCProtocolParser.InfoTemplateReceived -= h);

            AMCProtocolParser.AmcpTcpParser.SendCommandAndCheckError($"{AMCPCommand.INFO_TEMPLATE.ToAmcpValue()} \"{template.FullName}\"");
            var e = await eventWaiter.WaitForEventRaised;

            return e?.TemplateInfo;
        }

        /// <inheritdoc/>
        public virtual string GetVersion() => AsyncHelper.RunSync(() => GetVersionAsync());

        /// <inheritdoc/>
        public virtual async Task<string> GetVersionAsync()
        {
            if (!IsConnected)
                return null;

            var eventWaiter = new EventAwaiter<VersionEventArgs>(
                h => AMCProtocolParser.VersionRetrieved += h,
                h => AMCProtocolParser.VersionRetrieved -= h);

            AMCProtocolParser.AmcpTcpParser.SendCommandAndCheckError($"{AMCPCommand.VERSION.ToAmcpValue()}");
            var e = await eventWaiter.WaitForEventRaised;
            return e?.Version;
        }

        /// <inheritdoc/>
        public virtual async Task<IList<MediaInfo>> GetMediafilesAsync()
        {
            if (!IsConnected)
                return new List<MediaInfo>();

            var eventWaiter = new EventAwaiter<CLSEventArgs>(
                h => AMCProtocolParser.CLSReceived += h,
                h => AMCProtocolParser.CLSReceived -= h);
            AMCProtocolParser.AmcpTcpParser.SendCommandAndCheckError(AMCPCommand.CLS);

            var e = await eventWaiter.WaitForEventRaised;
            OnUpdatedMediafiles(e);
            return e?.Medias;
        }

        /// <inheritdoc/>
        public virtual IList<MediaInfo> GetMediafiles() => AsyncHelper.RunSync(() => GetMediafilesAsync());

        /// <inheritdoc/>
        public virtual async Task<TemplatesCollection> GetTemplatesAsync()
        {
            if (!IsConnected)
                return new TemplatesCollection();

            var eventWaiter = new EventAwaiter<TLSEventArgs>(
                h => AMCProtocolParser.TLSReceived += h,
                h => AMCProtocolParser.TLSReceived -= h);

            AMCProtocolParser.AmcpTcpParser.SendCommandAndCheckError(AMCPCommand.TLS);
            var e = await eventWaiter.WaitForEventRaised;
            OnUpdatedTemplatesList(e);
            return Templates;
        }

        /// <inheritdoc/>
        public virtual TemplatesCollection GetTemplates() => AsyncHelper.RunSync(() => GetTemplatesAsync());

        /// <inheritdoc/>
        public virtual async Task<IList<string>> GetFontsAsync()
        {

            if (!IsConnected)
                return Fonts;

            var eventWaiter = new EventAwaiter<AMCPEventArgs>(
                      h => AMCProtocolParser.FlsReceived += h,
                      h => AMCProtocolParser.FlsReceived -= h);

            AMCProtocolParser.AmcpTcpParser.SendCommandAndCheckError(AMCPCommand.TLS);
            await eventWaiter.WaitForEventRaised;

            return Fonts;
        }

        /// <inheritdoc/>
        public virtual IList<string> GetFonts() => AsyncHelper.RunSync(() => GetFontsAsync());

        #endregion

        #region Data

        ///<inheritdoc />
        public virtual async Task<IList<string>> GetDatalistAsync()
        {
            if (!IsConnected)
                return new List<string>();

            var eventWaiter = new EventAwaiter<DataListEventArgs>(
                h => AMCProtocolParser.DataListUpdated += h,
                h => AMCProtocolParser.DataListUpdated -= h);

            AMCProtocolParser.AmcpTcpParser.SendCommandAndCheckError(AMCPCommand.DATA_LIST);

            var e = await eventWaiter.WaitForEventRaised;
            OnUpdatedDataList(e);
            return Datafiles;
        }

        ///<inheritdoc />
        public virtual IList<string> GetDatalist() => AsyncHelper.RunSync(() => GetDatalistAsync());

        ///<inheritdoc />
        public virtual bool DeleteData(string name) => IsConnected && AMCProtocolParser.AmcpTcpParser.SendCommand($"{AMCPCommand.DATA_REMOVE.ToAmcpValue()} {name}");

        ///<inheritdoc />
        public virtual bool StoreData(string name, string data) => IsConnected && AMCProtocolParser.AmcpTcpParser.SendCommand($"{AMCPCommand.DATA_STORE.ToAmcpValue()} \"{name}\" \"{data}\"");

        ///<inheritdoc />
        public virtual async Task<string> GetDataAsync(string name)
        {

            if (!IsConnected)
                return null;

            var eventWaiter = new EventAwaiter<DataRetrieveEventArgs>(
                h => AMCProtocolParser.DataRetrieved += h,
                h => AMCProtocolParser.DataRetrieved -= h);

            AMCProtocolParser.AmcpTcpParser.SendCommandAndCheckError(
                $"{AMCPCommand.DATA_RETRIEVE.ToAmcpValue()} \"{name}\"");

            var e = await eventWaiter.WaitForEventRaised;
            return e.Data;
        }

        ///<inheritdoc />
        public virtual string GetData(string name) => AsyncHelper.RunSync(() => GetDataAsync(name));

        #endregion

        #region Thumbnail

        ///<inheritdoc />
        public async Task<IList<Thumbnail>> GetThumbnailListAsync()
        {
            if (!IsConnected)
                return new List<Thumbnail>();


            var eventWaiter = new EventAwaiter<ThumbnailsListEventArgs>(
                h => AMCProtocolParser.ThumbnailsListReceived += h,
                h => AMCProtocolParser.ThumbnailsListReceived -= h);


            AMCProtocolParser.AmcpTcpParser.SendCommandAndCheckError(AMCPCommand.THUMBNAIL_LIST);

            var e = await eventWaiter.WaitForEventRaised;
            OnUpdatedThumbnailList(e);
            return e?.Thumbnails;
        }

        ///<inheritdoc />
        public virtual IList<Thumbnail> GetThumbnailList() => AsyncHelper.RunSync(() => GetThumbnailListAsync());

        ///<inheritdoc />
        public virtual async Task<string> GetThumbnailAsync(string filename)
        {
            if (!IsConnected)
                return null;

            var eventWaiter = new EventAwaiter<ThumbnailsRetrieveEventArgs>(
                h => AMCProtocolParser.ThumbnailsRetrievedReceived += h,
                h => AMCProtocolParser.ThumbnailsRetrievedReceived -= h);


            AMCProtocolParser.AmcpTcpParser.SendCommandAndCheckError(
                $"{AMCPCommand.THUMBNAIL_RETRIEVE.ToAmcpValue()} {filename}");

            var e = await eventWaiter.WaitForEventRaised;
            return e.Base64Image;
        }

        ///<inheritdoc />
        public virtual string GetThumbnail(string filename) => AsyncHelper.RunSync(() => GetThumbnailAsync(filename));

        ///<inheritdoc />
        public virtual bool GenerateThumbnail(string filename) => IsConnected && AMCProtocolParser.AmcpTcpParser.SendCommand($"{AMCPCommand.THUMBNAIL_GENERATE.ToAmcpValue()} {filename}");

        ///<inheritdoc />
        public virtual bool GenerateAllThumbnail(string filename) => IsConnected && AMCProtocolParser.AmcpTcpParser.SendCommand(AMCPCommand.THUMBNAIL_GENERATEALL);

        #endregion

        ///<inheritdoc />
        public virtual bool Connect()
        {
            if (IsConnected)
                return false;
            if (ConnectionSettings == null)
                Connection.Connect();
            else
                Connection.Connect(ConnectionSettings);
            return true;
        }

        ///<inheritdoc />
        public bool Connect(string hostname)
        {
            ConnectionSettings = new CasparCGConnectionSettings(hostname);
            return Connect();
        }
        ///<inheritdoc />
        public bool Connect(string hostname, int acmpPort)
        {
            ConnectionSettings = new CasparCGConnectionSettings(hostname, acmpPort);
            return Connect();
        }

        ///<inheritdoc />
        public virtual void Disconnect() => Connection.Disconnect();


        /// <summary>
        /// Add or update channels info
        /// </summary>
        /// <param name="channelsInfos"></param>
        protected virtual void GenerateChannelManager(IEnumerable<ChannelInfo> channelsInfos)
        {
            _channels = _channels ?? new List<ChannelManager>();
            foreach (var channelInfo in channelsInfos)
            {
                var channel = _channels.FirstOrDefault(x => x.ID == channelInfo.ID);
                if (channel != null)
                    channel.VideoMode = channelInfo.VideoMode;
                else
                    _channels.Add(new ChannelManager(AMCProtocolParser, channelInfo.ID, channelInfo.VideoMode));
            }
            ChannelsUpdated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raise event <see cref="TemplatesUpdated"/>
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnUpdatedTemplatesList(TLSEventArgs e)
        {
            _templates = e == null ? null : new TemplatesCollection(e?.Templates);
            TemplatesUpdated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raise event <see cref="MediafilesUpdated"/>
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnUpdatedMediafiles(CLSEventArgs e)
        {
            _mediaFiles = e?.Medias;
            MediafilesUpdated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raise event <see cref="DatafilesUpdated"/>
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnUpdatedDataList(DataListEventArgs e)
        {
            _dataList = e?.Data;
            DatafilesUpdated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raise event <see cref="ThumbnailsUpdated"/>
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnUpdatedThumbnailList(ThumbnailsListEventArgs e)
        {
            _thumbnails = e?.Thumbnails;
            ThumbnailsUpdated?.Invoke(this, EventArgs.Empty);
        }



    }
}
