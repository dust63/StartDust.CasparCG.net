using StarDust.CasparCG.net.Models.Info;
using StarDust.CasparCG.net.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StarDust.CasparCG.net.AmcpProtocol
{
    ///<inheritdoc/>
    /// <summary>
    /// In charge to get Tcp parsed response and trigger event according to the command received. Parse the string datas to object
    /// </summary>
    public class AMCPProtocolParser : IAMCPProtocolParser

    {

        #region Properties

        /// <summary>
        /// Tcp Parser for the CasparCg Server tcp packet
        /// </summary>
        public IAMCPTcpParser AmcpTcpParser { get; private set; }

        /// <summary>
        /// Parser for media like tls, cls, fls
        /// </summary>
        public IDataParser DataParser { get; set; }

        #endregion


        #region Events
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> CGRemoveReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> MixerClearReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> DataStoreReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> LogCategoryReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> LockReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> DataRemoveReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> CGPlayReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> CGAddReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> CGStopReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> CGNextReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> CGClearReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> CGUpdateReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> LogLevelReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> PrintReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> PauseReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> ResumeReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> CGInvokeReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> CGInfoReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> MixerKeyerReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> MixerChromaReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> MixerBlendReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> MixerOpacityReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> MixerBrightnessReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> MixerSaturationReceive;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> MixerContrastReceive;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> MixerFillReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> MixerClipReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> MixerAnchorReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> MixerPerspectiveReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> MixerCropReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> MixerRotationReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> MixerMipMapReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> MixerVolumeReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> MixerMasterVolumeReceived;
        ///<inheritdoc />
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> MixerStraightReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> MixerGridReceive;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> MixerCommitReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> ChannelGridReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> HelpConsumerReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> HelpProducerReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> HelpReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> RestartReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> KillReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> ByeReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> GlgcReceived;
        ///<inheritdoc />
        public event EventHandler<GLInfoEventArgs> GlInfoReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> DiagReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> InfoDelayReceived;
        ///<inheritdoc />
        public event EventHandler<InfoThreadsEventArgs> InfoThreadsReceive;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> InfoQueuesReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> InfoServerReceived;
        ///<inheritdoc />
        public event EventHandler<InfoSystemEventArgs> InfoSystemReceived;
        ///<inheritdoc />
        public event EventHandler<InfoPathsEventArgs> InfoPathsReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> InfoConfigReceived;
        ///<inheritdoc />
        public event EventHandler<TemplateInfoEventArgs> InfoTemplateReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> StatusReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> FlsReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> CinfReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> ThumbnailGenerateAllReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> ThumbnailGenerateReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> SwapReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> AddReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> RemoveReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> CallReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> MixerReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> SetReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> ClearReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> StopReceived;
        ///<inheritdoc />
        public event EventHandler<AMCPEventArgs> PlayReceived;

        public event EventHandler<DataRetrieveEventArgs> DataRetrieved;
        ///<inheritdoc />
        public event EventHandler<DataListEventArgs> DataListUpdated;
        ///<inheritdoc />
        public event EventHandler<LoadEventArgs> LoadedBg;
        ///<inheritdoc />
        public event EventHandler<VersionEventArgs> VersionRetrieved;
        ///<inheritdoc />
        public event EventHandler<LoadEventArgs> Loaded;
        ///<inheritdoc />
        public event EventHandler<TLSEventArgs> TLSReceived;
        ///<inheritdoc />
        public event EventHandler<CLSEventArgs> CLSReceived;
        ///<inheritdoc />
        public event EventHandler<InfoEventArgs> InfoReceived;
        ///<inheritdoc />
        public event EventHandler<ThumbnailsRetreiveEventArgs> ThumbnailsRetrievedReceived;
        ///<inheritdoc />
        public event EventHandler<ThumbnailsListEventArgs> ThumbnailsListReceived;

        #endregion


        #region Ctor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="amcpTcpParser"></param>
        /// <param name="mediaParser"></param>
        public AMCPProtocolParser(IAMCPTcpParser amcpTcpParser, IDataParser mediaParser)
        {
            AmcpTcpParser = amcpTcpParser;
            AmcpTcpParser.ResponseParsed += TcpResponseParsed;

            DataParser = mediaParser;
        }

        #endregion

        /// <summary>
        /// Call when Amcp TCP parse end to parse a TCP message
        /// </summary>
        /// <param name="sender">IAmcpTcpParser</param>
        /// <param name="e"></param>
        protected virtual void TcpResponseParsed(object sender, AMCPEventArgs e)
        {
            if (e.Error != AMCPError.None)
                return;


            switch (e.Command)
            {
                case AMCPCommand.LOAD:
                    this.OnLoad(e);
                    break;
                case AMCPCommand.LOADBG:
                    this.OnLoadBg(e);
                    break;
                case AMCPCommand.CLS:
                    this.OnCLS(e);
                    break;
                case AMCPCommand.VERSION:
                    this.OnVersion(e.Data[0]);
                    break;
                case AMCPCommand.TLS:
                    this.OnTLS(e);
                    break;
                case AMCPCommand.INFO:
                    this.OnInfo(e);
                    break;
                case AMCPCommand.DATA_RETRIEVE:
                    this.OnDataRetrieve(e);
                    break;
                case AMCPCommand.DATA_LIST:
                    OnDataList(e);
                    break;
                case AMCPCommand.THUMBNAIL_LIST:
                    OnThumbnalList(e);
                    break;
                case AMCPCommand.THUMBNAIL_RETRIEVE:
                    OnThumbnailRetrieve(e);
                    break;
                case AMCPCommand.PLAY:
                    OnPlayReceived(e);
                    break;
                case AMCPCommand.STOP:
                    OnStopReceived(e);
                    break;
                case AMCPCommand.DATA:
                    OnDataRetrieve(e);
                    break;
                case AMCPCommand.CLEAR:
                    OnClearReceived(e);
                    break;
                case AMCPCommand.SET:
                    OnSetReceived(e);
                    break;
                case AMCPCommand.MIXER:
                    OnMixerReceived(e);
                    break;
                case AMCPCommand.CALL:
                    OnCallReceived(e);
                    break;
                case AMCPCommand.REMOVE:
                    OnRemoveReceived(e);
                    break;
                case AMCPCommand.ADD:
                    OnAddReceived(e);
                    break;
                case AMCPCommand.SWAP:
                    OnSwapReceived(e);
                    break;
                case AMCPCommand.THUMBNAIL_GENERATE:
                    OnThumbnailGenerateReceived(e);
                    break;
                case AMCPCommand.THUMBNAIL_GENERATEALL:
                    OnThumbnailGenerateAllReceived(e);
                    break;
                case AMCPCommand.CINF:
                    OnCinfReceived(e);
                    break;
                case AMCPCommand.FLS:
                    OnFlsReceived(e);
                    break;
                case AMCPCommand.STATUS:
                    OnStatusReceived(e);
                    break;
                case AMCPCommand.INFO_TEMPLATE:
                    OnInfoTemplateReceived(e);
                    break;
                case AMCPCommand.INFO_CONFIG:
                    OnInfoConfigReceived(e);
                    break;
                case AMCPCommand.INFO_PATHS:
                    OnInfoPathsReceived(e);
                    break;
                case AMCPCommand.INFO_SYSTEM:
                    OnInfoSystemReceived(e);
                    break;
                case AMCPCommand.INFO_SERVER:
                    OnInfoServerReceived(e);
                    break;
                case AMCPCommand.INFO_QUEUES:
                    OnInfoQueuesReceived(e);
                    break;
                case AMCPCommand.INFO_THREADS:
                    OnInfoThreadsReceived(e);
                    break;
                case AMCPCommand.INFO_DELAY:
                    OnInfoDelayReceived(e);
                    break;
                case AMCPCommand.DIAG:
                    OnDiagReceived(e);
                    break;
                case AMCPCommand.GLINFO:
                    OnGlInfoReceived(e);
                    break;
                case AMCPCommand.GLGC:
                    OnGlgcReceived(e);
                    break;
                case AMCPCommand.BYE:
                    OnByeReceived(e);
                    break;
                case AMCPCommand.KILL:
                    OnKillReceived(e);
                    break;
                case AMCPCommand.RESTART:
                    OnRestartReceived(e);
                    break;
                case AMCPCommand.HELP:
                    OnHelpReceived(e);
                    break;
                case AMCPCommand.HELP_PRODUCER:
                    OnHelpProducerReceived(e);
                    break;
                case AMCPCommand.HELP_CONSUMER:
                    OnHelpConsumerReceived(e);
                    break;
                case AMCPCommand.CHANNEL_GRID:
                    OnChannelGridReceived(e);
                    break;
                case AMCPCommand.MIXER_COMMIT:
                    OnMixerCommitReceived(e);
                    break;
                case AMCPCommand.MIXER_GRID:
                    OnMixerGridReceived(e);
                    break;
                case AMCPCommand.MIXER_STRAIGHT_ALPHA_OUTPUT:
                    OnMixerStraightReceived(e);
                    break;
                case AMCPCommand.MIXER_MASTERVOLUME:
                    OnMixerMasterVolumeReceived(e);
                    break;
                case AMCPCommand.MIXER_VOLUME:
                    OnMixerVolumeReceived(e);
                    break;
                case AMCPCommand.MIXER_MIPMAP:
                    OnMixerMipMapReceived(e);
                    break;
                case AMCPCommand.MIXER_ROTATION:
                    OnMixerRotationReceived(e);
                    break;
                case AMCPCommand.MIXER_CROP:
                    OnMixerCropReceived(e);
                    break;
                case AMCPCommand.MIXER_PERSPECTIVE:
                    OnMixerPerspectiveReceived(e);
                    break;
                case AMCPCommand.MIXER_ANCHOR:
                    OnMixerAnchorReceived(e);
                    break;
                case AMCPCommand.MIXER_CLIP:
                    OnMixerClipReceived(e);
                    break;
                case AMCPCommand.MIXER_FILL:
                    OnMixerFillReceived(e);
                    break;
                case AMCPCommand.MIXER_CONTRAST:
                    OnMixerContrastReceived(e);
                    break;
                case AMCPCommand.MIXER_SATURATION:
                    OnMixerSaturationReceived(e);
                    break;
                case AMCPCommand.MIXER_BRIGHTNESS:
                    OnMixerBrightnessReceived(e);
                    break;
                case AMCPCommand.MIXER_OPACITY:
                    OnMixerOpacityReceived(e);
                    break;
                case AMCPCommand.MIXER_BLEND:
                    OnMixerBlendReceived(e);
                    break;
                case AMCPCommand.MIXER_CHROMA:
                    OnMixerChromaReceived(e);
                    break;
                case AMCPCommand.MIXER_KEYER:
                    OnMixerKeyerReceived(e);
                    break;
                case AMCPCommand.CG_INFO:
                    OnCGInfoReceived(e);
                    break;
                case AMCPCommand.CG_INVOKE:
                    OnCGInvokeReceived(e);
                    break;
                case AMCPCommand.RESUME:
                    OnResumeReceived(e);
                    break;
                case AMCPCommand.PAUSE:
                    OnPauseReceived(e);
                    break;
                case AMCPCommand.PRINT:
                    OnPrintReceived(e);
                    break;
                case AMCPCommand.LOG_LEVEL:
                    OnLogLevelReceived(e);
                    break;
                case AMCPCommand.CG_UPDATE:
                    OnCGUpadteReceived(e);
                    break;
                case AMCPCommand.CG_CLEAR:
                    OnCGClearReceived(e);
                    break;
                case AMCPCommand.CG_REMOVE:
                    OnCGRemoveReceived(e);
                    break;
                case AMCPCommand.CG_NEXT:
                    OnCGNextReceived(e);
                    break;
                case AMCPCommand.CG_STOP:
                    OnCGStopReceived(e);
                    break;
                case AMCPCommand.CG_PLAY:
                    OnCGPlayReceived(e);
                    break;
                case AMCPCommand.CG_ADD:
                    OnCGAddReceived(e);
                    break;
                case AMCPCommand.DATA_REMOVE:
                    OnDataRemoveReceived(e);
                    break;
                case AMCPCommand.LOCK:
                    OnLockReceived(e);
                    break;
                case AMCPCommand.LOG_CATEGORY:
                    OnLogCategoryReceived(e);
                    break;
                case AMCPCommand.DATA_STORE:
                    OnDataStoreReceived(e);
                    break;
                case AMCPCommand.MIXER_CLEAR:
                    OnMixerClearReceived(e);
                    break;
            }
        }



        #region Methoods for notifications Event



        protected virtual void OnGlInfoReceived(AMCPEventArgs e)
        {
            var glInfo = DataParser.ParseGLInfo(e.Data.FirstOrDefault());
            GlInfoReceived?.Invoke(this, new GLInfoEventArgs(glInfo));
        }

        protected virtual void OnMixerClearReceived(AMCPEventArgs e)
        {
            MixerClearReceived?.Invoke(this, e);
        }

        protected virtual void OnDataStoreReceived(AMCPEventArgs e)
        {
            DataStoreReceived?.Invoke(this, e);
        }

        protected virtual void OnLogCategoryReceived(AMCPEventArgs e)
        {
            LogCategoryReceived?.Invoke(this, e);
        }

        protected virtual void OnLockReceived(AMCPEventArgs e)
        {
            LockReceived?.Invoke(this, e);
        }

        protected virtual void OnDataRemoveReceived(AMCPEventArgs e)
        {
            DataRemoveReceived?.Invoke(this, e);
        }

        protected virtual void OnCGAddReceived(AMCPEventArgs e)
        {
            CGAddReceived?.Invoke(this, e);
        }

        protected virtual void OnCGPlayReceived(AMCPEventArgs e)
        {
            CGPlayReceived?.Invoke(this, e);
        }

        protected virtual void OnCGStopReceived(AMCPEventArgs e)
        {
            CGStopReceived?.Invoke(this, e);
        }

        protected virtual void OnCGNextReceived(AMCPEventArgs e)
        {
            CGNextReceived?.Invoke(this, e);
        }

        protected virtual void OnCGRemoveReceived(AMCPEventArgs e)
        {
            CGRemoveReceived?.Invoke(this, e);
        }

        protected virtual void OnCGClearReceived(AMCPEventArgs e)
        {
            CGClearReceived?.Invoke(this, e);
        }

        protected virtual void OnCGUpadteReceived(AMCPEventArgs e)
        {
            CGUpdateReceived?.Invoke(this, e);
        }

        protected virtual void OnLogLevelReceived(AMCPEventArgs e)
        {
            LogLevelReceived?.Invoke(this, e);
        }

        protected virtual void OnPrintReceived(AMCPEventArgs e)
        {
            PrintReceived?.Invoke(this, e);
        }

        protected virtual void OnPauseReceived(AMCPEventArgs e)
        {
            PauseReceived?.Invoke(this, e);
        }

        protected virtual void OnResumeReceived(AMCPEventArgs e)
        {
            ResumeReceived?.Invoke(this, e);
        }

        protected virtual void OnCGInvokeReceived(AMCPEventArgs e)
        {
            CGInvokeReceived?.Invoke(this, e);
        }

        protected virtual void OnCGInfoReceived(AMCPEventArgs e)
        {
            CGInfoReceived?.Invoke(this, e);
        }

        protected virtual void OnMixerKeyerReceived(AMCPEventArgs e)
        {
            MixerKeyerReceived?.Invoke(this, e);
        }

        protected virtual void OnMixerChromaReceived(AMCPEventArgs e)
        {
            MixerChromaReceived?.Invoke(this, e);
        }

        protected virtual void OnMixerBlendReceived(AMCPEventArgs e)
        {
            MixerBlendReceived?.Invoke(this, e);
        }

        protected virtual void OnMixerOpacityReceived(AMCPEventArgs e)
        {
            MixerOpacityReceived?.Invoke(this, e);
        }

        protected virtual void OnMixerBrightnessReceived(AMCPEventArgs e)
        {
            MixerBrightnessReceived?.Invoke(this, e);
        }

        protected virtual void OnMixerSaturationReceived(AMCPEventArgs e)
        {
            MixerSaturationReceive?.Invoke(this, e);
        }

        protected virtual void OnMixerContrastReceived(AMCPEventArgs e)
        {
            MixerContrastReceive?.Invoke(this, e);
        }

        protected virtual void OnMixerFillReceived(AMCPEventArgs e)
        {
            MixerFillReceived?.Invoke(this, e);
        }

        protected virtual void OnMixerClipReceived(AMCPEventArgs e)
        {
            MixerClipReceived?.Invoke(this, e);
        }

        protected virtual void OnMixerAnchorReceived(AMCPEventArgs e)
        {
            MixerAnchorReceived?.Invoke(this, e);
        }

        protected virtual void OnMixerPerspectiveReceived(AMCPEventArgs e)
        {
            MixerPerspectiveReceived?.Invoke(this, e);
        }

        protected virtual void OnMixerCropReceived(AMCPEventArgs e)
        {
            MixerCropReceived?.Invoke(this, e);
        }

        protected virtual void OnMixerRotationReceived(AMCPEventArgs e)
        {
            MixerRotationReceived?.Invoke(this, e);
        }

        protected virtual void OnMixerMipMapReceived(AMCPEventArgs e)
        {
            MixerMipMapReceived?.Invoke(this, e);
        }

        protected virtual void OnMixerVolumeReceived(AMCPEventArgs e)
        {
            MixerVolumeReceived?.Invoke(this, e);
        }

        protected virtual void OnMixerMasterVolumeReceived(AMCPEventArgs e)
        {
            MixerMasterVolumeReceived?.Invoke(this, e);
        }

        protected virtual void OnMixerStraightReceived(AMCPEventArgs e)
        {
            MixerStraightReceived?.Invoke(this, e);
        }

        protected virtual void OnMixerGridReceived(AMCPEventArgs e)
        {
            MixerGridReceive?.Invoke(this, e);
        }

        protected virtual void OnMixerCommitReceived(AMCPEventArgs e)
        {
            MixerCommitReceived?.Invoke(this, e);
        }

        protected virtual void OnChannelGridReceived(AMCPEventArgs e)
        {
            ChannelGridReceived?.Invoke(this, e);
        }

        protected virtual void OnHelpConsumerReceived(AMCPEventArgs e)
        {
            HelpConsumerReceived?.Invoke(this, e);
        }

        protected virtual void OnHelpProducerReceived(AMCPEventArgs e)
        {
            HelpProducerReceived?.Invoke(this, e);
        }

        protected virtual void OnHelpReceived(AMCPEventArgs e)
        {
            HelpReceived?.Invoke(this, e);
        }

        protected virtual void OnRestartReceived(AMCPEventArgs e)
        {
            RestartReceived?.Invoke(this, e);
        }

        protected virtual void OnKillReceived(AMCPEventArgs e)
        {
            KillReceived?.Invoke(this, e);
        }

        protected virtual void OnByeReceived(AMCPEventArgs e)
        {
            ByeReceived?.Invoke(this, e);
        }

        protected virtual void OnGlgcReceived(AMCPEventArgs e)
        {
            GlgcReceived?.Invoke(this, e);
        }

        protected virtual void OnDiagReceived(AMCPEventArgs e)
        {
            DiagReceived?.Invoke(this, e);
        }

        protected virtual void OnInfoDelayReceived(AMCPEventArgs e)
        {
            InfoDelayReceived?.Invoke(this, e);
        }

        protected virtual void OnInfoThreadsReceived(AMCPEventArgs e)
        {
            InfoThreadsReceive?.Invoke(this, new InfoThreadsEventArgs(
                e.Data.Select(DataParser.ParseInfoThreads)
                    .ToList()
                ));
        }

        protected virtual void OnInfoQueuesReceived(AMCPEventArgs e)
        {
            InfoQueuesReceived?.Invoke(this, e);
        }

        protected virtual void OnInfoServerReceived(AMCPEventArgs e)
        {
            InfoServerReceived?.Invoke(this, e);
        }

        private void OnInfoSystemReceived(AMCPEventArgs e)
        {
            var systemInfo = DataParser.ParseInfoSystem(e.Data.FirstOrDefault());
            InfoSystemReceived?.Invoke(this, new InfoSystemEventArgs(systemInfo));
        }

        protected virtual void OnInfoPathsReceived(AMCPEventArgs e)
        {
            var infoPaths = DataParser.ParseInfoPaths(e.Data.FirstOrDefault());
            InfoPathsReceived?.Invoke(this, new InfoPathsEventArgs(infoPaths));
        }

        protected virtual void OnInfoConfigReceived(AMCPEventArgs e)
        {
            InfoConfigReceived?.Invoke(this, e);
        }

        protected virtual void OnInfoTemplateReceived(AMCPEventArgs e)
        {

            var templateInfo = DataParser.ParseTemplateInfo(e.Data.FirstOrDefault());
            InfoTemplateReceived?.Invoke(this, new TemplateInfoEventArgs(templateInfo));
        }

        private void OnStatusReceived(AMCPEventArgs e)
        {
            StatusReceived?.Invoke(this, e);
        }

        protected virtual void OnFlsReceived(AMCPEventArgs e)
        {
            FlsReceived?.Invoke(this, e);
        }

        protected virtual void OnCinfReceived(AMCPEventArgs e)
        {
            CinfReceived?.Invoke(this, e);
        }

        protected virtual void OnThumbnailGenerateAllReceived(AMCPEventArgs e)
        {
            ThumbnailGenerateAllReceived?.Invoke(this, e);
        }

        protected virtual void OnThumbnailGenerateReceived(AMCPEventArgs e)
        {
            ThumbnailGenerateReceived?.Invoke(this, e);
        }

        private void OnSwapReceived(AMCPEventArgs e)
        {
            SwapReceived?.Invoke(this, e);
        }

        protected virtual void OnAddReceived(AMCPEventArgs e)
        {
            AddReceived?.Invoke(this, e);
        }

        protected virtual void OnRemoveReceived(AMCPEventArgs e)
        {
            RemoveReceived?.Invoke(this, e);
        }

        protected virtual void OnCallReceived(AMCPEventArgs e)
        {
            CallReceived?.Invoke(this, e);
        }

        protected virtual void OnMixerReceived(AMCPEventArgs e)
        {
            MixerReceived?.Invoke(this, e);
        }

        protected virtual void OnSetReceived(AMCPEventArgs e)
        {
            SetReceived?.Invoke(this, e);
        }

        protected virtual void OnClearReceived(AMCPEventArgs e)
        {
            ClearReceived?.Invoke(this, e);
        }

        protected virtual void OnStopReceived(AMCPEventArgs e)
        {
            StopReceived?.Invoke(this, e);
        }

        protected virtual void OnPlayReceived(AMCPEventArgs e)
        {
            PlayReceived?.Invoke(this, e);
        }

        protected virtual void OnDataList(AMCPEventArgs e)
        {
            DataListUpdated?.Invoke(this, new DataListEventArgs(e.Data));
        }

        protected virtual void OnThumbnailRetrieve(AMCPEventArgs e)
        {
            ThumbnailsRetrievedReceived?.Invoke(this, new ThumbnailsRetreiveEventArgs(e.Data.FirstOrDefault()));
        }

        protected virtual void OnThumbnalList(AMCPEventArgs e)
        {

            var thumbnails = new List<Thumbnail>();
            foreach (string data in e.Data)
            {
                thumbnails.Add(DataParser.ParseThumbnailData(data));
            }
            ThumbnailsListReceived?.Invoke(this, new ThumbnailsListEventArgs(thumbnails));

        }

        protected virtual void OnLoadBg(AMCPEventArgs e)
        {
            LoadedBg?.Invoke(this, new LoadEventArgs(e.Data.FirstOrDefault() ?? string.Empty));
        }

        protected virtual void OnVersion(string v)
        {
            VersionRetrieved?.Invoke(this, new VersionEventArgs(v));
        }

        protected virtual void OnLoad(AMCPEventArgs e)
        {
            Loaded?.Invoke(this, new LoadEventArgs(e.Data.FirstOrDefault() ?? string.Empty));
        }

        protected virtual void OnDataRetrieve(AMCPEventArgs e)
        {
            if (e.Error == AMCPError.FileNotFound)
            {
                DataRetrieved?.Invoke(this, new DataRetrieveEventArgs(string.Empty));
            }


            if (e.Error == AMCPError.None && e.Data.Any())
            {

                DataRetrieved?.Invoke(this, new DataRetrieveEventArgs(e.Data.FirstOrDefault()));
            }
            else
            {
                DataRetrieved?.Invoke(this, new DataRetrieveEventArgs(string.Empty));
            }


        }

        private void OnTLS(AMCPEventArgs e)
        {
            var templates = e.Data.Select(DataParser.ParseTemplate).Where(x => x != null).ToList();
            TLSReceived?.Invoke(this, new TLSEventArgs(templates));
        }

        private void OnCLS(AMCPEventArgs e)
        {
            List<MediaInfo> medias = e.Data.Select(DataParser.ParseClipData).Where(x => x != null).ToList();
            CLSReceived?.Invoke(this, new CLSEventArgs(medias));
        }

        private void OnInfo(AMCPEventArgs e)
        {
            var infos = e.Data
                 .Select(DataParser.ParseChannelInfo)
                 .Where(x => x != null).ToList();
            InfoReceived?.Invoke(this, new InfoEventArgs(infos));
        }



        #endregion



        #region Public Methods


        #endregion

    }
}
