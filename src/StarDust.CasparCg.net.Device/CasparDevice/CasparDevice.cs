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
               await Task.WhenAll(
               Task.Factory.StartNew(GetVersion),
                 GetInfoAsync(),
                 GetThumbnailListAsync(),
                 GetDatalistAsync(),
                 GetTemplatesAsync(),
                 GetMediafilesAsync(),
                 GetInfoPathsAsync(),
                 GetInfoSystemAsync());
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
        public GLInfo GetGLInfo()
        {
            return AsyncHelper.RunSync(GetGLInfoAsync);
        }

        /// <inheritdoc/>
        public IList<ChannelInfo> GetInfo()
        {
             return AsyncHelper.RunSync(GetInfoAsync);
        }

        /// <inheritdoc/>
        public async Task<IList<ChannelInfo>> GetInfoAsync()
        {
            if (!IsConnected)
                return null;
            var eventWaiter = new EventAwaiter<InfoEventArgs>(
                h => AMCProtocolParser.InfoReceived += h,
                h => AMCProtocolParser.InfoReceived -= h);


            AMCProtocolParser.AmcpTcpParser.SendCommandAndCheckError(AMCPCommand.INFO);

            var e = await eventWaiter.WaitForEventRaised;
            OnUpdatedChannelInfo(e);
            return e.ChannelsInfo;
        }



        /// <inheritdoc/>
        public async Task<IList<ThreadsInfo>> GetInfoThreadsAsync()
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
        public IList<ThreadsInfo> GetInfoThreads()
        {
            return AsyncHelper.RunSync(GetInfoThreadsAsync);
        }


        /// <inheritdoc/>
        public async Task<SystemInfo> GetInfoSystemAsync()
        {
            if (!IsConnected)
                return null;

            var eventWaiter = new EventAwaiter<InfoSystemEventArgs>(
                h => AMCProtocolParser.InfoSystemReceived += h,
                h => AMCProtocolParser.InfoSystemReceived -= h);

            AMCProtocolParser.AmcpTcpParser.SendCommandAndCheckError(AMCPCommand.INFO_SYSTEM);

            var e = await eventWaiter.WaitForEventRaised;
            return e.SystemInfo;
        }

        /// <inheritdoc/>
        public SystemInfo GetInfoSystem()
        {
            return AsyncHelper.RunSync(GetInfoSystemAsync);
        }

        /// <inheritdoc/>
        public async Task<PathsInfo> GetInfoPathsAsync()
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
        public PathsInfo GetInfoPaths()
        {
            return AsyncHelper.RunSync(GetInfoPathsAsync);
        }

        /// <inheritdoc/>
        public TemplateInfo GetInfoTemplate(string templateFilePath)
        {
            return AsyncHelper.RunSync(()=>GetInfoTemplateAsync(new TemplateBaseInfo(templateFilePath)));
        }

        /// <inheritdoc/>
        public TemplateInfo GetInfoTemplate(TemplateBaseInfo template)
        {
            return AsyncHelper.RunSync(() => GetInfoTemplateAsync(template));
        }

        /// <inheritdoc/>
        public Task<TemplateInfo> GetInfoTemplateAsync(string templateFilePath)
        {
            return GetInfoTemplateAsync(new TemplateBaseInfo(templateFilePath));
        }

        /// <inheritdoc/>
        public async Task<TemplateInfo> GetInfoTemplateAsync(TemplateBaseInfo template)
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

            var eventWaiter = new EventAwaiter<CLSEventArgs>(
                h => AMCProtocolParser.CLSReceived += h,
                h => AMCProtocolParser.CLSReceived -= h);
            AMCProtocolParser.AmcpTcpParser.SendCommandAndCheckError(AMCPCommand.CLS);

            var e = await eventWaiter.WaitForEventRaised;
            OnUpdatedMediafiles(e);
            return Mediafiles;
        }

        /// <inheritdoc/>
        public IList<MediaInfo> GetMediafiles()
        {
            return AsyncHelper.RunSync(() => GetMediafilesAsync());

        }

        /// <inheritdoc/>
        public async Task<TemplatesCollection> GetTemplatesAsync()
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
        public TemplatesCollection GetTemplates()
        {
            return AsyncHelper.RunSync(() => GetTemplatesAsync());
        }

        /// <inheritdoc/>
        public async Task<IList<string>> GetFontsAsync()
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
        public IList<string> GetFonts()
        {
            return AsyncHelper.RunSync(() => GetFontsAsync());
        }




        #endregion


        #region Data

        ///<inheritdoc />
        public async Task<IList<string>> GetDatalistAsync()
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
        public IList<string> GetDatalist()
        {
            return AsyncHelper.RunSync(() => GetDatalistAsync());
        }

        ///<inheritdoc />
        public bool DeleteData(string name)
        {
            return IsConnected && AMCProtocolParser.AmcpTcpParser.SendCommand($"{AMCPCommand.DATA_REMOVE.ToAmcpValue()} {name}");
        }

        ///<inheritdoc />
        public bool StoreData(string name, string data)
        {
            return IsConnected && AMCProtocolParser.AmcpTcpParser.SendCommand($"{AMCPCommand.DATA_STORE.ToAmcpValue()} \"{name}\" \"{data}\"");
        }

        ///<inheritdoc />
        public async Task<string> GetDataAsync(string name)
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
        public string GetData(string name)
        {
            return AsyncHelper.RunSync(() => GetDataAsync(name));
        }
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
            return Thumbnails;
        }

        ///<inheritdoc />
        public IList<Thumbnail> GetThumbnailList()
        {
            return AsyncHelper.RunSync(() => GetThumbnailListAsync());
        }

        ///<inheritdoc />
        public async Task<string> GetThumbnailAsync(string filename)
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
        public string GetThumbnail(string filename)
        {
            return AsyncHelper.RunSync(() => GetThumbnailAsync(filename));
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
            Datafiles = e.Data;
            DatafilesUpdated?.Invoke(this, EventArgs.Empty);
        }


        private void OnUpdatedThumbnailList(ThumbnailsListEventArgs e)
        {
            Thumbnails = e.Thumbnails;
            ThumbnailsUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
