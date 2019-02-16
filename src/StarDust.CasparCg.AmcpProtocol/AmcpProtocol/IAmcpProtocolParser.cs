using System;

namespace StarDust.CasparCG.AmcpProtocol
{
    public interface IAMCPProtocolParser
    {
        IAMCPTcpParser AmcpTcpParser { get; }
        IDataParser DataParser { get; set; }

        event EventHandler<DataRetrieveEventArgs> DataRetrieved;
        event EventHandler<DataListEventArgs> DataListUpdated;
        event EventHandler<LoadEventArgs> LoadedBg;
        event EventHandler<VersionEventArgs> VersionRetrieved;
        event EventHandler<LoadEventArgs> Loaded;
        event EventHandler<TLSEventArgs> TLSReceived;
        event EventHandler<CLSEventArgs> CLSReceived;
        event EventHandler<InfoEventArgs> InfoReceived;
        event EventHandler<ThumbnailsRetreiveEventArgs> ThumbnailsRetrievedReceived;
        event EventHandler<ThumbnailsListEventArgs> ThumbnailsListReceived;

        event EventHandler<AMCPEventArgs> CGRemoveReceived;
        event EventHandler<AMCPEventArgs> MixerClearReceived;
        event EventHandler<AMCPEventArgs> DataStoreReceived;
        event EventHandler<AMCPEventArgs> LogCategoryReceived;
        event EventHandler<AMCPEventArgs> LockReceived;
        event EventHandler<AMCPEventArgs> DataRemoveReceived;
        event EventHandler<AMCPEventArgs> CGPlayReceived;
        event EventHandler<AMCPEventArgs> CGAddReceived;
        event EventHandler<AMCPEventArgs> CGStopReceived;
        event EventHandler<AMCPEventArgs> CGNextReceived;
        event EventHandler<AMCPEventArgs> CGClearReceived;
        event EventHandler<AMCPEventArgs> CGUpadteReceived;
        event EventHandler<AMCPEventArgs> LogLevelReceived;
        event EventHandler<AMCPEventArgs> PrintReceived;
        event EventHandler<AMCPEventArgs> PauseReceived;
        event EventHandler<AMCPEventArgs> ResumeReceiv;
        event EventHandler<AMCPEventArgs> CGInvokeReceived;
        event EventHandler<AMCPEventArgs> CGInfoReceived;
        event EventHandler<AMCPEventArgs> MixerKeyerReceived;
        event EventHandler<AMCPEventArgs> MixerChromaReceived;
        event EventHandler<AMCPEventArgs> MixerBlendReceived;
        event EventHandler<AMCPEventArgs> MixerOpacityReceived;
        event EventHandler<AMCPEventArgs> MixerBrightnessReceived;
        event EventHandler<AMCPEventArgs> MixerSaturationReceive;
        event EventHandler<AMCPEventArgs> MixerContrastReceive;
        event EventHandler<AMCPEventArgs> MixerFillReceived;
        event EventHandler<AMCPEventArgs> MixerClipReceived;
        event EventHandler<AMCPEventArgs> MixerAnchorReceived;
        event EventHandler<AMCPEventArgs> MixerPerspectiveReceived;
        event EventHandler<AMCPEventArgs> MixerCropReceived;
        event EventHandler<AMCPEventArgs> MixerRotationReceived;
        event EventHandler<AMCPEventArgs> MixerMipMapReceived;
        event EventHandler<AMCPEventArgs> MixerVolumeReceived;
        event EventHandler<AMCPEventArgs> MixerMasterVolumeReceived;
        event EventHandler<AMCPEventArgs> MixerStraightReceived;
        event EventHandler<AMCPEventArgs> MixerGridReceive;
        event EventHandler<AMCPEventArgs> MixerCommitReceived;
        event EventHandler<AMCPEventArgs> ChannelGridReceived;
        event EventHandler<AMCPEventArgs> HelpConsumerReceived;
        event EventHandler<AMCPEventArgs> HelpProducerReceived;
        event EventHandler<AMCPEventArgs> HelpReceived;
        event EventHandler<AMCPEventArgs> RestartReceived;
        event EventHandler<AMCPEventArgs> KillReceived;
        event EventHandler<AMCPEventArgs> ByeReceived;
        event EventHandler<GLInfoEventArgs> GlInfoReceived;
        event EventHandler<AMCPEventArgs> GlgcReceived;
        event EventHandler<AMCPEventArgs> DiagReceived;
        event EventHandler<AMCPEventArgs> InfoDelayReceived;
        event EventHandler<InfoThreadsEventArgs> InfoThreadsReceive;
        event EventHandler<AMCPEventArgs> InfoQueuesReceived;
        event EventHandler<AMCPEventArgs> InfoServerReceived;
        event EventHandler<InfoSystemEventArgs> InfoSystemReceived;
        event EventHandler<InfoPathsEventArgs> InfoPathsReceived;
        event EventHandler<AMCPEventArgs> InfoConfigReceived;
        event EventHandler<TemplateInfoEventArgs> InfoTemplateReceived;
        event EventHandler<AMCPEventArgs> StatusReceived;
        event EventHandler<AMCPEventArgs> FlsReceived;
        event EventHandler<AMCPEventArgs> CinfReceived;
        event EventHandler<AMCPEventArgs> ThumbnailGenerateAllReceived;
        event EventHandler<AMCPEventArgs> ThumbnailGenerateReceived;
        event EventHandler<AMCPEventArgs> SwapReceived;
        event EventHandler<AMCPEventArgs> AddReceived;
        event EventHandler<AMCPEventArgs> RemoveReceived;
        event EventHandler<AMCPEventArgs> CallReceived;
        event EventHandler<AMCPEventArgs> MixerReceived;
        event EventHandler<AMCPEventArgs> SetReceived;
        event EventHandler<AMCPEventArgs> ClearReceived;
        event EventHandler<AMCPEventArgs> StopReceived;
        event EventHandler<AMCPEventArgs> PlayReceived;




    }
}
