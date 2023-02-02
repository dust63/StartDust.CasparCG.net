using System;
using System.Collections.Generic;
using System.Linq;
using StarDust.CasparCG.net.Connection;
using StarDust.CasparCG.net.Datas;
using StarDust.CasparCG.net.Device;
using StarDust.CasparCG.net.Models;
using StarDust.CasparCG.net.Models.Media;

namespace StarDust.Demo.AMCP.netcore
{
    public class Executor
    {
        private readonly ICasparDevice _casparCGServer;
        private Dictionary<string, Action> _commandList;

        public Executor(ICasparDevice casparCGServer)
        {
            _casparCGServer = casparCGServer;
            InitializeCommands();
        }

        internal void Execute()
        {
            DisplayCommand();
            while (true)
            {
                var input = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Yellow;

                if (_commandList.TryGetValue(input, out Action value))
                {
                    try
                    {
                        if (input != "connect" && !CheckConnection())
                            continue;
                        value.Invoke();
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine($"Error on {input} block.", e.ToString());
                        Console.WriteLine("Tap any key to continue...");
                        Console.Read();
                    }

                    DisplayCommand();
                }
                else
                {
                    InvalidCommand();
                }

                Console.WriteLine(string.Empty);
            }
        }

        private void InitializeCommands()
        {
            _commandList = new(StringComparer.OrdinalIgnoreCase)
            {
                {"connect", Connect },
                {"disconnect", Disconnect },
                {"play",Play },
                {"stop",Stop },
                {"version",Version },
                {"info",Info },
                {"load",Load },
                {"loadbg",LoadBg },
                {"cls",Cls },
                {"tls",Tls },
                {"thumb",Thumb },
                { "template", PlayTemplate},
                { "channel info", ChannelInfo},
                { "threads info", ThreadsInfo},
                { "channel grid", ChannelGrid},
                { "mixer", PlayMixer},
                { "templateinfo", TemplateInfo},
                { "systeminfo", SystemInfo},
                { "pathsinfo", PathsInfo},
                { "glinfo", GlInfo},
                { "call", Call},
                { "add", Add},
                { "remove", Remove},
                { "cg update", CgUpdate},
                { "clear", Clear},
                { "cg add", CgAdd },
                { "play empty", PlayEmpty },
                { "play transition", PlayTransition },
                { "get info", GetInfo }
            };
        }

        private void DisplayCommand()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("__________________________________");
            Console.WriteLine("List of command");
            Console.WriteLine("__________________________________");
            Console.WriteLine(string.Join(Environment.NewLine, _commandList.Select(x => $">>>>{x.Key}")));
            Console.WriteLine("__________________________________");
            Console.WriteLine();
            Console.WriteLine("Type your command:");
        }

        private static void InvalidCommand()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Invalid command");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private bool CheckConnection()
        {
            if (_casparCGServer.IsConnected)
                return true;

            Console.WriteLine("Plase launch the connect command before");
            return false;
        }

        private void Clear()
        {
            var channel = _casparCGServer.Channels.FirstOrDefault();
            if (channel == null)
            {
                Console.WriteLine("No channel found");
                return;
            }

            channel.Clear();
            EnterToContinue();
        }

        private void CgAdd()
        {
            var channel = _casparCGServer.Channels.FirstOrDefault();
            if (channel == null)
            {
                Console.WriteLine("No channel found");
                return;
            }

            channel.CG.Add(10, 1, @"CASPARCG_FLASH_TEMPLATES_EXAMPLE_PACK_1/ADVANCEDTEMPLATE2");
            channel.CG.Play(10, 1);
            EnterToContinue();
        }

        private void CgUpdate()
        {
            var channel = _casparCGServer.Channels.FirstOrDefault();
            if (channel == null)
            {
                Console.WriteLine("No channel found");
                return;
            }

            Console.WriteLine("Before update do a cg add");
            Console.WriteLine("Please provide text to update:");
            var data = new CasparCGDataCollection();
            data.Add("f0", Console.ReadLine());

            channel.CG.Update(10, 1, data);
            EnterToContinue();
        }

        private void GetInfo()
        {
            var channel = _casparCGServer.Channels.FirstOrDefault();
            if (channel == null)
            {
                Console.WriteLine("No channel found");
                return;
            }

            var data = channel.GetInfo();
            Console.WriteLine($"Channel 1. Status: {data.Status}, Mode: {data.VideoMode}");

            EnterToContinue();
        }

        private void Cls()
        {
            var clips = _casparCGServer.GetMediafiles();
            Console.WriteLine(string.Join(Environment.NewLine, clips.Select(x => x.FullName)));
            EnterToContinue();
        }

        private void Remove()
        {
            _casparCGServer.Channels.FirstOrDefault()?.Remove(700);
            EnterToContinue();
        }

        private void Add()
        {
            _casparCGServer.Channels.FirstOrDefault()?.Add(ConsumerType.File, 700, "\"test.mp4\" -vcodec libx264 -acodec acc");
            EnterToContinue();
        }

        private void Call()
        {
            Play();
            _casparCGServer.Channels.FirstOrDefault()?.Call(1, false, 50);
            EnterToContinue();
        }

        private void GlInfo()
        {
            var infos = _casparCGServer.GetGLInfo();
            Console.WriteLine(string.Join("\r\n", infos));
            EnterToContinue();
        }

        private void ThreadsInfo()
        {
            var threads = _casparCGServer.GetInfoThreads();
            Console.WriteLine(string.Join("\r\n", threads));
            EnterToContinue();
        }

        private void PathsInfo()
        {
            var info = _casparCGServer.GetInfoPaths();
            Console.WriteLine($"Media Path: {info?.Mediapath}");
            EnterToContinue();
        }

        private void SystemInfo()
        {
            var systemInfo = _casparCGServer.GetInfoSystem();
            Console.WriteLine($"System info: OS - {systemInfo?.Windows?.Name}");
            EnterToContinue();
        }

        private void TemplateInfo()
        {
            Console.WriteLine(_casparCGServer.GetInfoTemplate(_casparCGServer.Templates.All.First()).AuthorName);
            EnterToContinue();
        }

        private void ChannelInfo()
        {
            Console.WriteLine(_casparCGServer.Channels.FirstOrDefault()?.GetInfo());
            EnterToContinue();
        }

        private void CasparDevice_ConnectionStatusChanged(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine("CasparCG Server connection is connected: " + e.Connected);
        }

        private void ChannelGrid()
        {
            _casparCGServer.ChannelGrid();
            EnterToContinue();
        }

        private void Thumb()
        {
            var base64 = _casparCGServer.GetThumbnail("AMB");
            Console.WriteLine(base64);
            EnterToContinue();
        }

        private void Tls()
        {
            var templates = _casparCGServer.GetTemplates();
            foreach (var template in templates.All)
            {
                Console.WriteLine(template.FullName);
            }
            EnterToContinue();
        }

        private void LoadBg()
        {
            _casparCGServer.Channels.First()?.LoadBG(new CasparPlayingInfoItem("AMB", new Transition(TransitionType.SLIDE, 5000)));
            _casparCGServer.Channels.First()?.Play();
            EnterToContinue();
        }

        private void Load()
        {
            _casparCGServer.Channels.First()?.LoadBG(new CasparPlayingInfoItem("AMB"), false);
            _casparCGServer.Channels.First()?.Play();
            EnterToContinue();
        }

        private void Info()
        {
            var e = _casparCGServer.GetInfo();
            foreach (var channelInfo in e)
            {
                Console.WriteLine($"Channel ID: {channelInfo.ID}, Status: {channelInfo.Status}, ActiveClip: {channelInfo.ActiveClip}");
            }
            EnterToContinue();
        }

        private void Version()
        {
            Console.WriteLine(_casparCGServer.GetVersion());
            EnterToContinue();
        }

        private void Stop()
        {
            var channel = _casparCGServer.Channels.First(x => x.ID == 1);
            channel.Stop();
            channel.Clear();
            EnterToContinue();
        }

        private void Play()
        {
            var channel = _casparCGServer.Channels.First(x => x.ID == 1);
            channel.LoadBG(new CasparPlayingInfoItem { VideoLayer = 1, Clipname = "AMB" });
            channel.Play(1);
            EnterToContinue();
        }

        private void PlayTransition()
        {
            var channel = _casparCGServer.Channels.First(x => x.ID == 1);
            channel.Play(new CasparPlayingInfoItem
            {
                VideoLayer = 1,
                Clipname = "AMB",
                Transition = new Transition
                {
                    Direction = TransitionDirection.LEFT,
                    Type = TransitionType.PUSH,
                    Duration = 20
                }
            });
            EnterToContinue();
        }


        private void PlayEmpty()
        {
            var channel = _casparCGServer.Channels.First(x => x.ID == 1);
            channel.Play(new CasparPlayingInfoItem
            {
                VideoLayer = 1,
                Clipname = "EMPTY",
                Transition = new Transition
                {
                    Direction = TransitionDirection.LEFT,
                    Type = TransitionType.PUSH,
                    Duration = 100
                }
            });
            EnterToContinue();
        }

        public void GetMedias()
        {
            var clips = _casparCGServer.GetMediafiles();
            Console.WriteLine(string.Join(",\r\n", clips.Select(x => x.Name)));
            EnterToContinue();
        }

        private void PlayTemplate()
        {
            var channel = _casparCGServer.Channels.First(x => x.ID == 1);
            channel.CG.Add(10, 1, "caspar_text");
            channel.CG.Play(10, 1);
            EnterToContinue();
        }

        private void PlayMixer()
        {
            var channel = _casparCGServer.Channels.First(x => x.ID == 1);
            channel.MixerManager.Brightness(1, 0.2F);
            EnterToContinue();
        }

        private void EnterToContinue()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Tap enter to continue...");
            Console.ReadLine();
        }

        private void Disconnect()
        {
            if (!_casparCGServer?.IsConnected ?? false)
                return;
            _casparCGServer.ConnectionStatusChanged -= CasparDevice_ConnectionStatusChanged;
            _casparCGServer.Disconnect();
        }

        private void Connect()
        {
            if (_casparCGServer?.IsConnected ?? false)
                return;
            _casparCGServer.ConnectionStatusChanged += CasparDevice_ConnectionStatusChanged;
            _casparCGServer.Connect("127.0.0.1");
        }
    }
}