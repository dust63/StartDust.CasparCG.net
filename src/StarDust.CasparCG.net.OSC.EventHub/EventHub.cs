using Rug.Osc;
using StarDust.CasparCG.net.OSC.EventHub.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StarDust.CasparCG.net.OSC.EventHub
{
    public interface ICasparCGOscEventsHub
    {
        IOscListener CasparCgOscListener { get; }
       

        event EventHandler<PlaybackClipClipChangedEventArgs> PlaybackClipChanged;
        event EventHandler<PlaybackClipPathEventArgs> PlaybackClipPathChanged;
        event EventHandler<PlaybackClipTimeEventArgs> PlaybackClipTimeChanged;
        event EventHandler<PlaybackClipFrameEventArgs> PlaybackClipFrameChanged;
        event EventHandler<PlaybackClipFrameRateEventArgs> PlaybackClipFrameRateChanged;
        event EventHandler<StreamFramerateEventArgs> StreamFrameRateChanged;
        event EventHandler<PlaybackClipWidthEventArgs> PlaybackClipWidthChanged;
        event EventHandler<PlaybackClipHeightEventArgs> PlaybackClipHeightChanged;
        event EventHandler<PlaybackClipFieldEventArg> PlaybackClipFieldChanged;
        event EventHandler<PlaybackClipCodecEventArg> PlaybackClipVideoCodecChanged;
        event EventHandler<PlaybackClipCodecEventArg> PlaybackClipAudioCodecChanged;
        event EventHandler<PlaybackClipAudioSampleRateEventArg> PlaybackClipAudioSampleRateChanged;
        event EventHandler<PlaybackClipAudioChannelsEventArg> PlaybackClipAudioChannelsChanged;
        event EventHandler<PlaybackLoopEventArgs> PlaybackLoopChanged;
        event EventHandler<ProfilerTimeEventArgs> ProfilerTimeChanged;
        event EventHandler<OutputPortTypeEventArgs> OutputPortChanged;
        event EventHandler<LayerActiveTimeEventArgs> LayerActiveTimeChanged;
        event EventHandler<LayerActiveFrameEventArgs> LayerActiveFrameChanged;
        event EventHandler<LayerTypeEventArgs> LayerTypeChanged;
        event EventHandler<LayerTypeEventArgs> BackgroundLayerTypeChanged;
        event EventHandler<LayerProfilerEventArgs> LayerProfilerChanged;
        event EventHandler<LayerPausedEventArgs> LayerPausedChanged;
        event EventHandler<TemplatePathEventArgs> TemplatePathChanged;
        event EventHandler<TemplateWidthEventArgs> TemplateWidthChanged;
        event EventHandler<TemplateHeightEventArgs> TemplateHeightChanged;
        event EventHandler<TemplateFpsEventArgs> TemplateFpsChanged;
        event EventHandler<BufferEventArgs> FlashProducerBufferChanged;
        event EventHandler<MixerAudioChannelsCountEventArgs> MixerAudioChannelsCountChanged;
        event EventHandler<MixerAudioDbfsEventArgs> MixerAudioDbfsChanged;
    }

    public class CasparCGOscEventsHub : ICasparCGOscEventsHub
    {
        private const string ProgressiveValue = "progressive";
        private const string Port = "port";

        public event EventHandler<PlaybackClipClipChangedEventArgs> PlaybackClipChanged;
        public event EventHandler<PlaybackClipPathEventArgs> PlaybackClipPathChanged;
        public event EventHandler<PlaybackClipTimeEventArgs> PlaybackClipTimeChanged;
        public event EventHandler<StreamFramerateEventArgs> StreamFrameRateChanged;
        public event EventHandler<PlaybackClipFrameEventArgs> PlaybackClipFrameChanged;
        public event EventHandler<PlaybackClipFrameRateEventArgs> PlaybackClipFrameRateChanged;
        public event EventHandler<PlaybackClipWidthEventArgs> PlaybackClipWidthChanged;
        public event EventHandler<PlaybackClipHeightEventArgs> PlaybackClipHeightChanged;
        public event EventHandler<PlaybackClipFieldEventArg> PlaybackClipFieldChanged;
        public event EventHandler<PlaybackClipCodecEventArg> PlaybackClipVideoCodecChanged;
        public event EventHandler<PlaybackClipCodecEventArg> PlaybackClipAudioCodecChanged;
        public event EventHandler<PlaybackClipAudioSampleRateEventArg> PlaybackClipAudioSampleRateChanged;
        public event EventHandler<PlaybackClipAudioChannelsEventArg> PlaybackClipAudioChannelsChanged;
        public event EventHandler<PlaybackClipAudioFormatEventArg> PlaybackClipAudioFormatChanged;
        public event EventHandler<PlaybackLoopEventArgs> PlaybackLoopChanged;
        public event EventHandler<ProfilerTimeEventArgs> ProfilerTimeChanged;
        public event EventHandler<OutputPortTypeEventArgs> OutputPortChanged;
        public event EventHandler<ConsumerFrameCreatedEventArgs> ConsumerFrameCreatedChanged;
        public event EventHandler<LayerActiveTimeEventArgs> LayerActiveTimeChanged;
        public event EventHandler<LayerActiveFrameEventArgs> LayerActiveFrameChanged;
        public event EventHandler<LayerTypeEventArgs> LayerTypeChanged;
        public event EventHandler<LayerTypeEventArgs> BackgroundLayerTypeChanged;
        public event EventHandler<LayerProfilerEventArgs> LayerProfilerChanged;
        public event EventHandler<LayerPausedEventArgs> LayerPausedChanged;
        public event EventHandler<TemplatePathEventArgs> TemplatePathChanged;
        public event EventHandler<TemplateWidthEventArgs> TemplateWidthChanged;
        public event EventHandler<TemplateHeightEventArgs> TemplateHeightChanged;
        public event EventHandler<TemplateFpsEventArgs> TemplateFpsChanged;
        public event EventHandler<BufferEventArgs> FlashProducerBufferChanged;
        public event EventHandler<MixerAudioChannelsCountEventArgs> MixerAudioChannelsCountChanged;
        public event EventHandler<MixerAudioDbfsEventArgs> MixerAudioDbfsChanged;

        public IOscListener CasparCgOscListener { get; protected set; }


        /// <summary>
        /// Provide regex pattern and action to do for for the pattern. Params of the action should be regex pattern and osc message
        /// </summary>
        protected Dictionary<string, Action<OscMessage>> RegexEventParser = new Dictionary<string, Action<OscMessage>>();

        public CasparCGOscEventsHub(IOscListener casparCgOscListener)
        {
            if (casparCgOscListener == null) throw new ArgumentException(nameof(casparCgOscListener));
            CasparCgOscListener = casparCgOscListener;
            CasparCgOscListener.ListenerStarted += OnCasparCgOscListenerStarted;
            CasparCgOscListener.ListenerStarted += OnCasparCgOscListenerStopped;
            CasparCgOscListener.OscMessageReceived += OnCasparCgOscMessageReceived;
            InitializeParsers();
        }



        protected virtual void InitializeParsers()
        {
            //   AddOrUpdateParser("/channel/[0-9]/stage/layer/[0-9].*?/file/name", OnClipNameChanged);

            //Output Channel messages
            AddOrUpdateParser("/channel/[0-9]*[0-9].*?/profiler/time", OnProfilerTimeChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9].*?/output/port/[0-9]*[0-9]/type", OnOutputTypePortChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9].*?/output/port/[0-9]*[0-9]/frame", OnConsumerFrameCreatedChanged);

            //Stage Message
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9]/time", OnLayerActiveTimeChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9]/frame", OnLayerActiveFrameChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9]/type", OnLayerTypeChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9]/background/type", OnBackgroundLayerTypeChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9]/profiler/time", OnLayerProfilerChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9]/paused", OnLayerPaused);


            //Flash producer message
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9]/host/path", OnTemplatePathChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9]/host/width", OnTemplateWidthChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9]/host/height", OnTemplateHeightChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9]/host/fps", OnTemplateFpsChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9]/buffer", OnFlashBufferChanged);

            //Mixer message
            AddOrUpdateParser("/channel/[0-9]*[0-9]/mixer/audio/nb_channels", OnMixerAudioChannelsCountChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/mixer/audio/dBFS", OnMixerAudioDbfsChanged);

            //FFMPEG PRODUCER https://github.com/CasparCG/help/wiki/FFmpeg-Producer#osc-data
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9].*?/file/name", OnPlaybackClipNameChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9].*?/file/path", OnPlaybackClipPathChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9].*?/file/time", OnPlaybackClipTimeChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9].*?/file/frame", OnPlaybackClipFrameChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9].*?/file/[0-9]*[0-9]/fps", OnStreamFramerateChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9].*?/file/fps", OnPlaybackClipFrameRateChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9].*?/file/video/width", OnPlaybackClipWidthChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9].*?/file/video/height", OnPlaybackClipHeightChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9].*?/file/video/field", OnPlaybackClipFieldChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9].*?/file/video/codec", OnPlaybackClipVideoCodecChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9].*?/file/audio/codec", OnPlaybackClipAudioCodecChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9].*?/file/audio/sample-rate", OnPlaybackClipAudioFrameRateChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9].*?/file/audio/channels", OnPlaybackClipAudioChannelsChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9].*?/file/audio/format", OnPlaybackClipAudioFormatChanged);
            AddOrUpdateParser("/channel/[0-9]*[0-9]/stage/layer/[0-9]*[0-9].*?/loop", OnPlaybackLoopChanged);
        }

        protected virtual void OnMixerAudioDbfsChanged(OscMessage message)
        {
            var channelId = message.GetChannel();
            if (channelId == null)
                return;
            var dbfs = (float)(message.ElementAtOrDefault(0));
            MixerAudioDbfsChanged?.Invoke(this, new MixerAudioDbfsEventArgs(dbfs, channelId.Value));
        }

        protected virtual void OnMixerAudioChannelsCountChanged(OscMessage message)
        {

            var channelId = message.GetChannel();
            if (channelId == null)
                return;
            var channelsCount = Convert.ToInt32(message.ElementAtOrDefault(0));
            MixerAudioChannelsCountChanged?.Invoke(this, new MixerAudioChannelsCountEventArgs(channelsCount, channelId.Value));
        }

        protected virtual void OnFlashBufferChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;
            var buffer = Convert.ToInt32(message.ElementAtOrDefault(0));
            FlashProducerBufferChanged?.Invoke(this, new BufferEventArgs(buffer, channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        protected virtual void OnTemplateFpsChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;
            var fps = (float)message.ElementAtOrDefault(0);
            TemplateFpsChanged?.Invoke(this, new TemplateFpsEventArgs(fps, channelLayer.Item1.Value, channelLayer.Item2.Value));
        }    

        protected virtual void OnTemplateHeightChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;
            var height = Convert.ToUInt32(message.ElementAtOrDefault(0));
            TemplateHeightChanged?.Invoke(this, new TemplateHeightEventArgs(height, channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        protected virtual void OnTemplateWidthChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;
            var width = Convert.ToUInt32(message.ElementAtOrDefault(0));
            TemplateWidthChanged?.Invoke(this, new TemplateWidthEventArgs(width, channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        protected virtual void OnTemplatePathChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;
            var path = message.ElementAtOrDefault(0)?.ToString();
            TemplatePathChanged?.Invoke(this, new TemplatePathEventArgs(path, channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        protected virtual void OnLayerPaused(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;

            var isPause = (bool?)message.ElementAtOrDefault(0);
            if (isPause == null)
                return;
            LayerPausedChanged?.Invoke(this, new LayerPausedEventArgs(isPause.Value, channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        protected virtual void OnLayerProfilerChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;

            var actual = (float?)message.ElementAtOrDefault(0);
            var expected = (float?)message.ElementAtOrDefault(1);
            if (actual == null || expected == null)
                return;

            LayerProfilerChanged?.Invoke(this, new LayerProfilerEventArgs(actual.Value, expected.Value, channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        protected virtual void OnBackgroundLayerTypeChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;

            var type = message.ElementAtOrDefault(0)?.ToString();

            BackgroundLayerTypeChanged?.Invoke(this, new LayerTypeEventArgs(type, channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        protected virtual void OnLayerTypeChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;

            var type = message.ElementAtOrDefault(0)?.ToString();

            LayerTypeChanged?.Invoke(this, new LayerTypeEventArgs(type, channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        protected virtual void OnLayerActiveFrameChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;

            var frame = Convert.ToUInt32(message.ElementAtOrDefault(0));

            LayerActiveFrameChanged?.Invoke(this, new LayerActiveFrameEventArgs(frame, channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        protected virtual void OnLayerActiveTimeChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;

            var time = (float?)message.ElementAtOrDefault(0);
            if (time == null)
                return;

            LayerActiveTimeChanged?.Invoke(this, new LayerActiveTimeEventArgs(time.Value, channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        private void OnConsumerFrameCreatedChanged(OscMessage message)
        {
            var channel = message.GetChannel();
            if (channel == null)
                return;

            var usedFrames = Convert.ToInt32(message.ElementAtOrDefault(0));
            var availableFrames = Convert.ToInt32(message.ElementAtOrDefault(1));
            var port = message.GetNextAddressPartOf(Port).TryParseNullable();

            ConsumerFrameCreatedChanged?.Invoke(this, new ConsumerFrameCreatedEventArgs(usedFrames, availableFrames, port.GetValueOrDefault(0), channel.Value));
        }

        private void OnOutputTypePortChanged(OscMessage message)
        {
            //TODO ADD TEST
            var channel = message.GetChannel();
            if (channel == null)
                return;

            var type = message.ElementAtOrDefault(0)?.ToString();
            var port = message.GetNextAddressPartOf(Port).TryParseNullable();

            OutputPortChanged?.Invoke(this, new OutputPortTypeEventArgs(type, port.GetValueOrDefault(0), channel.Value));
        }

        protected virtual void OnProfilerTimeChanged(OscMessage message)
        {
            var channel = message.GetChannel();
            if (channel == null)
                return;

            var currentValue = (float)(message.ElementAtOrDefault(0));
            var expectedValue = (float)(message.ElementAtOrDefault(1));

            ProfilerTimeChanged?.Invoke(this, new ProfilerTimeEventArgs(currentValue, expectedValue, channel.Value));

        }

        protected virtual void OnPlaybackLoopChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;

            var isLoop = (bool)message.FirstOrDefault();

            PlaybackLoopChanged?.Invoke(this, new PlaybackLoopEventArgs(isLoop, message.IsBackground(), channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        protected virtual void OnPlaybackClipAudioFormatChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;

            var format = message.FirstOrDefault()?.ToString();

            PlaybackClipAudioFormatChanged?.Invoke(this, new PlaybackClipAudioFormatEventArg(format, message.IsBackground(), channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        protected virtual void OnPlaybackClipAudioChannelsChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;

            var channels = Convert.ToInt32(message.ElementAtOrDefault(0));

            PlaybackClipAudioChannelsChanged?.Invoke(this, new PlaybackClipAudioChannelsEventArg((uint)channels, message.IsBackground(), channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        protected virtual void OnPlaybackClipAudioFrameRateChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;

            var sampleRate = Convert.ToInt32(message.ElementAtOrDefault(0));

            PlaybackClipAudioSampleRateChanged?.Invoke(this, new PlaybackClipAudioSampleRateEventArg((uint)sampleRate, message.IsBackground(), channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        protected virtual void OnPlaybackClipAudioCodecChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;

            var codec = message.FirstOrDefault()?.ToString();

            PlaybackClipAudioCodecChanged?.Invoke(this, new PlaybackClipCodecEventArg(codec, message.IsBackground(), channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        protected virtual void OnPlaybackClipVideoCodecChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;

            var codec = message.FirstOrDefault()?.ToString();

            PlaybackClipVideoCodecChanged?.Invoke(this, new PlaybackClipCodecEventArg(codec, message.IsBackground(), channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        protected virtual void OnPlaybackClipFieldChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;

            var isProgresive = ProgressiveValue.Equals(message.FirstOrDefault()?.ToString(), StringComparison.OrdinalIgnoreCase);

            PlaybackClipFieldChanged?.Invoke(this, new PlaybackClipFieldEventArg(isProgresive, message.IsBackground(), channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        protected virtual void OnPlaybackClipHeightChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;

            var height = Convert.ToInt32(message.ElementAtOrDefault(0));

            PlaybackClipHeightChanged?.Invoke(this, new PlaybackClipHeightEventArgs((uint)height, message.IsBackground(), channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        protected virtual void OnPlaybackClipWidthChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;

            var width = Convert.ToInt32(message.ElementAtOrDefault(0));

            PlaybackClipWidthChanged?.Invoke(this, new PlaybackClipWidthEventArgs((uint)width, message.IsBackground(), channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        protected virtual void OnPlaybackClipFrameRateChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;

            var fps = (float)(message.ElementAtOrDefault(0));

            PlaybackClipFrameRateChanged?.Invoke(this, new PlaybackClipFrameRateEventArgs(fps, message.IsBackground(), channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        protected virtual void OnPlaybackClipFrameChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;

            var elapsed = Convert.ToInt32(message.ElementAtOrDefault(0));
            var total = Convert.ToInt32(message.ElementAtOrDefault(1));

            PlaybackClipFrameChanged?.Invoke(this, new PlaybackClipFrameEventArgs((uint)elapsed, (uint)total, message.IsBackground(), channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        protected virtual void OnStreamFramerateChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;

            var fps = (float)(message.ElementAtOrDefault(0));
            var streamId = uint.TryParse(message.GetNextAddressPartOf("file"), out uint outValue) ? (uint?)outValue : null;
            StreamFrameRateChanged?.Invoke(this, new StreamFramerateEventArgs(fps, streamId.GetValueOrDefault(0), message.IsBackground(), channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        protected virtual void OnPlaybackClipTimeChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;

            var elapsed = (float)(message.ElementAtOrDefault(0));
            var total = (float)(message.ElementAtOrDefault(1));

            PlaybackClipTimeChanged?.Invoke(this, new PlaybackClipTimeEventArgs(elapsed, total, message.IsBackground(), channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        protected virtual void OnPlaybackClipPathChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;

            PlaybackClipPathChanged?.Invoke(this, new PlaybackClipPathEventArgs(message.Single().ToString(), message.IsBackground(), channelLayer.Item1.Value, channelLayer.Item2.Value));
        }

        protected virtual void OnPlaybackClipNameChanged(OscMessage message)
        {
            var channelLayer = message.GetChannelAndLayer();
            if (channelLayer.Item1 == null || channelLayer.Item2 == null)
                return;

            PlaybackClipChanged?.Invoke(this, new PlaybackClipClipChangedEventArgs(message.Single().ToString(), message.IsBackground(), channelLayer.Item1.Value, channelLayer.Item2.Value));
        }



        protected CasparCGOscEventsHub AddOrUpdateParser(string regex, Action<OscMessage> parser)
        {
            if (RegexEventParser.ContainsKey(regex))
                RegexEventParser[regex] = parser;
            else
                RegexEventParser.Add(regex, parser);

            return this;
        }




        protected virtual void OnCasparCgOscMessageReceived(object sender, OscMessageEventArgs e)
        {
            var message = e.OscPacket as OscMessage;
            if (message == null)
                return;

            var parserActions = RegexEventParser.Where(x => Regex.Match(message.Address, x.Key).Success);
            foreach (var parser in parserActions)
            {
                parser.Value(e.OscPacket as OscMessage);
            }
        }

        protected virtual void OnCasparCgOscListenerStopped(object sender, EventArgs e)
        {

        }

        protected virtual void OnCasparCgOscListenerStarted(object sender, EventArgs e)
        {

        }
    }


    public static class CasparCGOscMessageExtensions
    {
        public static readonly string ChannelTag = "channel";
        public static readonly string LayerTag = "layer";


        public static bool IsBackground(this OscMessage message)
        {
            return message.Address.ToLowerInvariant().Contains("background");
        }

        public static string[] SplitAddress(this OscMessage message)
        {
            return message.Address.ToLowerInvariant().Split('/');
        }
        public static ushort? GetChannel(this OscMessage message)
        {
            var splittedAddress = message.SplitAddress();
            var channel = splittedAddress.GetChannel();
            return channel;
        }

        public static (ushort?, ushort?) GetChannelAndLayer(this OscMessage message)
        {
            var splittedAddress = message.SplitAddress();
            var channel = splittedAddress.GetChannel();
            var layer = splittedAddress.GetLayer();
            return (channel, layer);
        }

        public static string GetPreviousAddressPartOf(this OscMessage message, string addressPart)
        {
            var splittedAddress = message.SplitAddress();
            return GetPreviousAddressPartOf(splittedAddress, addressPart);
        }


        /// <summary>
        /// Return the previous value before an address part
        /// </summary>
        /// <param name="splitAddress"></param>
        /// <param name="addressPart"></param>
        /// <returns></returns>
        public static string GetPreviousAddressPartOf(this string[] splitAddress, string addressPart)
        {
            var partIndex = Array.IndexOf(splitAddress, addressPart);
            if (partIndex <= 0)
                return null;
            return splitAddress.ElementAtOrDefault(partIndex - 1);
        }

        /// <summary>
        /// Return the next value after an address part
        /// </summary>
        /// <param name="splitAddress"></param>
        /// <param name="addressPart"></param>
        /// <returns></returns>
        public static string GetNextAddressPartOf(this string[] splitAddress, string addressPart)
        {
            var partIndex = Array.IndexOf(splitAddress, addressPart);
            if (partIndex >= splitAddress.Length)
                return null;
            return splitAddress.ElementAtOrDefault(partIndex + 1);
        }

        public static string GetNextAddressPartOf(this OscMessage message, string addressPart)
        {
            var splittedAddress = message.SplitAddress();
            return GetNextAddressPartOf(splittedAddress, addressPart);
        }

        public static ushort? GetChannel(this string[] splitAddress)
        {
            return ParsingHelper.TryParseNullable(splitAddress.GetNextAddressPartOf(ChannelTag));
        }

        public static ushort? GetLayer(this string[] splitAddress)
        {
            return ParsingHelper.TryParseNullable(splitAddress.GetNextAddressPartOf(LayerTag));
        }


    }

    public static class ParsingHelper
    {
        public static ushort? TryParseNullable(this string val)
        {
            return ushort.TryParse(val, out ushort outValue) ? (ushort?)outValue : null;
        }

    }

}
