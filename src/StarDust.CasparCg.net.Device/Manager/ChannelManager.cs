using StarDust.CasparCG.net.AmcpProtocol;
using StarDust.CasparCG.net.Models;
using StarDust.CasparCG.net.Models.Info;
using StarDust.CasparCG.net.Models.Media;
using System.Linq;
using System.Text;
using System.Threading;

namespace StarDust.CasparCG.net.Device
{
    /// <summary>
    /// Manage the channel. Give you access to all the action that you can do for a channel.
    /// </summary>
    public class ChannelManager : ChannelInfo
    {
        const int MaxWaitTime = 5000 / 10; //(1000ms / 10ms d'attente)

        /// <summary>
        /// Character Generator Manager. Use this class to pilot graphics on Caspar CG
        /// </summary>
        public CGManager CG { get; protected set; }

        /// <summary>
        /// Mixer manager that pilot the mixer command on Caspar CG
        /// </summary>
        public MixerManager MixerManager { get; protected set; }


        /// <summary>
        /// Instance of class that send and receive message from/to CasparCG Server
        /// </summary>
        protected readonly IAMCPProtocolParser _amcpProtocolParser;

        /// <summary>
        /// Access to instance of tcp message parser
        /// </summary>
        protected IAMCPTcpParser _amcpTcpParser;

        /// <summary>
        /// object need to lock call on multithreading
        /// </summary>
        protected readonly object lockObject = new object();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="amcProtocolParser"></param>
        /// <param name="id">ID of the video channel</param>
        /// <param name="videoMode">mode of video. IE: 1080i50,720p50...</param>
        public ChannelManager(IAMCPProtocolParser amcProtocolParser, uint id, VideoMode videoMode)
        {
            _amcpProtocolParser = amcProtocolParser;
            _amcpTcpParser = amcProtocolParser.AmcpTcpParser;
            ID = id;
            VideoMode = videoMode;
            CG = new CGManager(this, _amcpTcpParser);
            MixerManager = new MixerManager(this, _amcpTcpParser);
        }




        /// <summary>
        /// Loads a clip to the foreground and plays the first frame before pausing. If any clip is playing on the target foreground then this clip will be replaced.
        /// </summary>
        /// <param name="clipname"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        public virtual bool Load(string clipname, bool loop)
        {
            return Load(new CasparPlayingInfoItem { Clipname = clipname, Loop = loop });
        }

        /// <summary>
        /// Loads a clip to the foreground and plays the first frame before pausing. If any clip is playing on the target foreground then this clip will be replaced.
        /// </summary>
        /// <param name="videoLayer"></param>
        /// <param name="clipname"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        public virtual bool Load(uint videoLayer, string clipname, bool loop)
        {
            return Load(new CasparPlayingInfoItem { VideoLayer = videoLayer, Clipname = clipname, Loop = loop });
        }

        /// <summary>
        /// Loads a clip to the foreground and plays the first frame before pausing. If any clip is playing on the target foreground then this clip will be replaced.
        /// </summary>
        /// <param name="item">parameter object to indicate transition,clip...</param>
        /// <returns></returns>
        public virtual bool Load(CasparPlayingInfoItem item)
        {
            var cmd = $"LOAD {ID}-{item.VideoLayer} {item.Clipname} {item.Transition?.ToString()}".Trim();
            return _amcpTcpParser.SendCommand(cmd);
        }


        /// <summary>
        /// Loads a producer in the background and prepares it for playout. If no layer is specified the default layer index will be used.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="auto">indicate if CasparCG need to play the clip automatically after the previous stopped</param>
        /// <returns></returns>
        public virtual bool LoadBG(CasparPlayingInfoItem item, bool auto = false)
        {
            string str = item.Clipname;
            var cmd = new StringBuilder();

            cmd.Append($"LOADBG {ID}-{item.VideoLayer} {item.Clipname}");
            cmd.Append(item.Loop ? " LOOP" : "");
            cmd.Append(item.Transition != null ? $" {item.Transition.ToString()}" : string.Empty);
            cmd.Append(item.Seek.HasValue ? $" SEEK {item.Seek}" : string.Empty);
            cmd.Append(item.Length.HasValue ? $" LENGTH {item.Length}" : string.Empty);
            cmd.Append(auto ? " AUTO" : string.Empty);

            return _amcpTcpParser.SendCommand(cmd.ToString());
        }



        /// <summary>
        /// Moves clip from background to foreground and starts playing it. If a transition (see LOADBG) is prepared, it will be executed.
        /// </summary>
        /// <returns></returns>
        public virtual bool Play()
        {
            return _amcpTcpParser.SendCommand($"{AMCPCommand.PLAY.ToAmcpValue()} {ID}");
        }

        /// <summary>
        /// Moves clip from background to foreground and starts playing it. If a transition (see LOADBG) is prepared, it will be executed.
        /// </summary>
        /// <param name="videoLayer"></param>
        /// <returns></returns>
        public virtual bool Play(uint videoLayer)
        {
            return _amcpTcpParser.SendCommand($"{AMCPCommand.PLAY.ToAmcpValue()} {ID}-{videoLayer}");
        }

        /// <summary>
        /// Play item with info for transition clip
        /// </summary>     
        /// <param name="playingInfoItem">parameters to play item</param>
        /// <returns></returns>
        public virtual bool Play(CasparPlayingInfoItem playingInfoItem)
        {
            return _amcpTcpParser.SendCommand($"{AMCPCommand.PLAY.ToAmcpValue()} {ID}-{playingInfoItem.VideoLayer} {playingInfoItem.Clipname} {playingInfoItem.Transition.ToString()}");
        }

        /// <summary>
        /// Calls method on the specified producer with the provided param string.
        /// </summary>
        /// <param name="videoLayer"></param>
        /// <param name="loop"></param>
        /// <param name="seek"></param>
        /// <returns></returns>
        public virtual bool Call(uint videoLayer, bool? loop, int? seek)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"{AMCPCommand.CALL.ToAmcpValue()} {ID}-{videoLayer}");

            if (loop == null && seek == null)
                return false;

            if (loop ?? false)
                stringBuilder.Append(" LOOP");

            if (seek.HasValue)
                stringBuilder.Append($" SEEK {seek.Value}");


            return _amcpTcpParser.SendCommand(stringBuilder.ToString());
        }

        /// <summary>
        /// Swaps layers between channels (both foreground and background will be swapped).By setting transforms to true, the transformations of the layers are swapped as well.
        /// </summary>
        /// <param name="videoLayer"></param>
        /// <param name="channelIDToSwap"></param>
        /// <param name="videoLayerToSwap"></param>
        /// <param name="transforms"></param>
        /// <returns></returns>
        public virtual bool Swap(uint videoLayer, int channelIDToSwap, int videoLayerToSwap, bool transforms)
        {
            return _amcpTcpParser.SendCommand($"{AMCPCommand.SWAP.ToAmcpValue()} {ID}-{videoLayer} {channelIDToSwap}-{videoLayerToSwap}{(transforms ? " TRANSFORMS" : "")}");
        }



        /// <summary>
        /// Adds a consumer to the specified video channel.
        /// </summary>
        /// <param name="consumerIndex"></param>
        /// <param name="consumer">overrides the index that the consumer itself decides and can later be </param>

        /// <returns></returns>
        public virtual bool Add(ConsumerType consumer, uint consumerIndex)
        {
            return _amcpTcpParser.SendCommand($"{AMCPCommand.ADD.ToAmcpValue()} {ID} {consumer.ToAmcpValue()} {consumerIndex}");
        }


        /// <summary>
        /// Adds a consumer to the specified video channel with parameters
        /// </summary>
        /// <param name="consumer"></param>
        /// <param name="consumerIndex"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual bool Add(ConsumerType consumer, uint? consumerIndex = null, string parameters = null)
        {

            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"{AMCPCommand.ADD.ToAmcpValue()} {ID}");

            if (consumerIndex != null)
                stringBuilder.Append($"-{consumerIndex}");

            stringBuilder.Append($" {consumer.ToAmcpValue()}");

            if (!string.IsNullOrEmpty(parameters))
                stringBuilder.Append($" {parameters}");

            return _amcpTcpParser.SendCommand(stringBuilder.ToString());
        }

        /// <summary>
        /// Remove a consumer by his index
        /// </summary>
        /// <param name="consumerIndex">If consumerIndex is given, the consumer will be removed via its id</param>
        /// <returns></returns>
        public virtual bool Remove(uint consumerIndex)
        {

            return _amcpTcpParser.SendCommand($"{AMCPCommand.REMOVE.ToAmcpValue()} {ID}-{consumerIndex}");
        }


        /// <summary>
        /// Remove a consumer by his index
        /// </summary>
        /// <param name="parameters">If parameters are given instead, the consumer matching those parameters </param>
        /// <returns></returns>
        public virtual bool Remove(string parameters)
        {
            return _amcpTcpParser.SendCommand($"{AMCPCommand.REMOVE.ToAmcpValue()} {ID} {parameters}");
        }

        /// <summary>
        /// Pause playout for on the channel layer 0
        /// </summary>
        /// <returns></returns>
        public virtual bool Pause()
        {
            return _amcpTcpParser.SendCommand($"{AMCPCommand.PAUSE.ToAmcpValue()} {ID}");
        }

        /// <summary>
        /// Pause playout for on the channel on the given layer
        /// </summary>
        /// <param name="videoLayer"></param>
        /// <returns></returns>
        public virtual bool Pause(uint videoLayer)
        {
            return _amcpTcpParser.SendCommand($"{AMCPCommand.PAUSE.ToAmcpValue()} {ID}-{videoLayer}");
        }

        /// <summary>
        /// Resume playout for channel layer 0
        /// </summary>
        /// <returns></returns>
        public virtual bool Resume()
        {
            return _amcpTcpParser.SendCommand($"{AMCPCommand.RESUME.ToAmcpValue()} {ID}");
        }

        /// <summary>
        /// Resume playout for the given layer
        /// </summary>
        /// <param name="videoLayer"></param>
        /// <returns></returns>
        public virtual bool Resume(uint videoLayer)
        {
            return _amcpTcpParser.SendCommand($"{AMCPCommand.RESUME.ToAmcpValue()} {ID}-{videoLayer}");
        }

        /// <summary>
        /// Stop playout on the layer 0.
        /// </summary>
        /// <returns></returns>
        public virtual bool Stop()
        {
            return _amcpTcpParser.SendCommand($"{AMCPCommand.STOP.ToAmcpValue()} {ID}");
        }

        /// <summary>
        /// Stop playout on the specified layer.
        /// </summary>
        /// <param name="videoLayer"></param>
        /// <returns></returns>
        public virtual bool Stop(int videoLayer)
        {
            if (videoLayer == -1)
                return Stop();
            return _amcpTcpParser.SendCommand($"{AMCPCommand.STOP.ToAmcpValue()} {ID}-{videoLayer}");
        }

        /// <summary>
        /// Removes all clips (both foreground and background) for all layers.
        /// </summary>
        /// <returns></returns>
        public virtual bool Clear()
        {
            return _amcpTcpParser.SendCommand($"{AMCPCommand.CLEAR.ToAmcpValue()} {ID}");
        }

        /// <summary>
        /// Removes all clips (both foreground and background) of the specified layer.
        /// </summary>
        /// <param name="videoLayer"></param>
        /// <returns></returns>
        public virtual bool Clear(int videoLayer)
        {
            if (videoLayer == -1)
                return Clear();
            return _amcpTcpParser.SendCommand($"{AMCPCommand.CLEAR.ToAmcpValue()} {ID}-{videoLayer}");
        }

        /// <summary>
        /// Set the video mode for this channel
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public virtual bool SetMode(VideoMode mode)
        {
            return _amcpTcpParser.SendCommand($"SET {ID} MODE {mode.ToAmcpValue()}");
        }

        /// <summary>
        /// Set audio layout for the channel
        /// </summary>
        /// <param name="layout"></param>
        /// <returns></returns>
        public virtual bool SetChannelLayout(ChannelLayout layout)
        {
            return _amcpTcpParser.SendCommand($"SET {ID} CHANNEL_LAYOUT {layout.ToAmcpValue()}");
        }

        /// <summary>
        /// Allows for exclusive access to a channel.
        /// </summary>
        /// <param name="lockAction"></param>
        /// <returns></returns>
        public virtual bool Lock(LockAction lockAction)
        {
            return _amcpTcpParser.SendCommand($"{AMCPCommand.LOCK.ToAmcpValue()} {ID} {lockAction}");
        }

        /// <summary>
        /// Allows for exclusive access to a channel.
        /// </summary>
        /// <param name="lockAction"></param>
        /// <param name="lockPhrase"></param>
        /// <returns></returns>
        public virtual bool Lock(LockAction lockAction, string lockPhrase)
        {
            return _amcpTcpParser.SendCommand($"{AMCPCommand.LOCK.ToAmcpValue()} {ID} {lockAction} {lockPhrase}");
        }

        /// <summary>
        /// Saves an RGBA PNG bitmap still image of the contents of the specified channel in the media folder.
        /// </summary>
        /// <returns></returns>
        public virtual bool Print()
        {
            return _amcpTcpParser.SendCommand($"{AMCPCommand.PRINT.ToAmcpValue()} {ID}");
        }




        /// <summary>
        /// Changes the video format of the channel.
        /// </summary>
        /// <param name="videoMode"></param>
        /// <returns></returns>
        public virtual bool SetVideoMode(VideoMode videoMode)
        {
            return _amcpTcpParser.SendCommand($"{AMCPCommand.SET.ToAmcpValue()} {ID} MODE {videoMode.ToAmcpValue()}");
        }



        /// <summary>
        /// Get info about this channel
        /// </summary> 
        /// <returns></returns>
        public virtual ChannelInfo GetInfo()
        {
            lock (lockObject)
            {
                var raised = false;
                var counter = 0;
                void handler(object o, InfoEventArgs e)
                {
                    var info = e.ChannelsInfo.FirstOrDefault(x => x.ID == ID);
                    VideoMode = info.VideoMode;
                    Status = info.Status;
                    Output = info.Output;
                    Mixer = info.Mixer;
                    Index = info.Index;
                    Stage = info.Stage;
                    raised = true;
                }

                _amcpProtocolParser.InfoReceived += handler;
                _amcpProtocolParser.AmcpTcpParser.SendCommand($"{AMCPCommand.INFO.ToAmcpValue()} {ID}");

                while (!raised && counter < MaxWaitTime)
                {
                    Thread.Sleep(10);
                    counter++;
                }

                _amcpProtocolParser.InfoReceived -= handler;
            }

            return this;
        }

        /// <summary>
        /// Get info about a layer
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public virtual ChannelInfo GetInfo(uint layer)
        {
            lock (lockObject)
            {
                var raised = false;
                var counter = 0;
                void handler(object o, InfoEventArgs e)
                {
                    var info = e.ChannelsInfo.FirstOrDefault(x => x.ID == ID);
                    VideoMode = info.VideoMode;
                    Status = info.Status;
                    Output = info.Output;
                    Mixer = info.Mixer;
                    Index = info.Index;
                    Stage = info.Stage;
                    raised = true;
                }

                _amcpProtocolParser.InfoReceived += handler;
                _amcpProtocolParser.AmcpTcpParser.SendCommand($"{AMCPCommand.INFO.ToAmcpValue()} {ID}-{layer}");

                while (!raised && counter < MaxWaitTime)
                {
                    Thread.Sleep(10);
                    counter++;
                }

                _amcpProtocolParser.InfoReceived -= handler;
            }

            return this;
        }

    }
}
