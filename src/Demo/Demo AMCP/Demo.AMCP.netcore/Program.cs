using StarDust.CasparCG.net.AmcpProtocol;
using StarDust.CasparCG.net.Connection;
using StarDust.CasparCG.net.Datas;
using StarDust.CasparCG.net.Device;
using StarDust.CasparCG.net.Models;
using StarDust.CasparCG.net.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;
using Unity.Lifetime;

namespace StarDust.CasparCG.AMCP.net.ClientTestConsole
{
    class Program
    {

        static IUnityContainer _container;
        static ICasparDevice casparCGServer;

        static void ConfigureIOC()
        {
            _container = new UnityContainer();
            _container.RegisterType<IServerConnection, ServerConnection>();
            _container.RegisterType(typeof(IAMCPTcpParser), typeof(AmcpTCPParser));
            _container.RegisterSingleton<IDataParser, CasparCGDataParser>();
            _container.RegisterType(typeof(IAMCPProtocolParser), typeof(AMCPProtocolParser));
            _container.RegisterType<ICasparDevice, CasparDevice>(new ContainerControlledLifetimeManager());
        }


        private static Dictionary<string, Action> commandList = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase)
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
            {"cg add", CgAdd },
            {"play empty", PlayEmpty },
            {"play transition", PlayTransition },
            {"get info", GetInfo }
        };


        private static void Disconnect()
        {
            if (!casparCGServer?.IsConnected ?? false)
                return;
            casparCGServer.ConnectionStatusChanged -= CasparDevice_ConnectionStatusChanged;
            casparCGServer.Disconnect();
        }

        private static void Connect()
        {
            if (casparCGServer?.IsConnected ?? false)
                return;
            casparCGServer.ConnectionStatusChanged += CasparDevice_ConnectionStatusChanged;
            casparCGServer.Connect("127.0.0.1");
        }

        static void Main(string[] args)
        {

            ConfigureIOC();
            casparCGServer = _container.Resolve<ICasparDevice>();


            DisplayCommand();
            while (true)
            {
                var input = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Yellow;

                if (commandList.ContainsKey(input))
                {
                    try
                    {
                        if (input != "connect" && !CheckConnection())
                            continue;
                        commandList[input].Invoke();
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

        private static void DisplayCommand()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("__________________________________");
            Console.WriteLine("List of command");
            Console.WriteLine("__________________________________");
            Console.WriteLine(string.Join(Environment.NewLine, commandList.Select(x => $">>>>{x.Key}")));
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

        private static bool CheckConnection()
        {

            if (casparCGServer.IsConnected)
                return true;

            Console.WriteLine("Plase launch the connect command before");
            return false;
        }

        private static void Clear()
        {

            var channel = casparCGServer.Channels.FirstOrDefault();
            if (channel == null)
            {
                Console.WriteLine("No channel found");
                return;
            }

            channel.Clear();
            EnterToContinue();
        }

        private static void CgAdd()
        {

            var channel = casparCGServer.Channels.FirstOrDefault();
            if (channel == null)
            {
                Console.WriteLine("No channel found");
                return;
            }

            channel.CG.Add(10, 1, @"CASPARCG_FLASH_TEMPLATES_EXAMPLE_PACK_1/ADVANCEDTEMPLATE2");
            channel.CG.Play(10, 1);
            EnterToContinue();
        }

        private static void CgUpdate()
        {

            var channel = casparCGServer.Channels.FirstOrDefault();
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

        private static void GetInfo()
        {

            var channel = casparCGServer.Channels.FirstOrDefault();
            if (channel == null)
            {
                Console.WriteLine("No channel found");
                return;
            }


            var data = channel.GetInfo();
            Console.WriteLine($"Channel 1. Status: {data.Status}, Mode: {data.VideoMode}");


            EnterToContinue();

        }

        private static void Cls()
        {

            var clips = casparCGServer.GetMediafiles();
            Console.WriteLine(string.Join(Environment.NewLine, clips.Select(x => x.FullName)));
            EnterToContinue();
        }

        private static void Remove()
        {

            casparCGServer.Channels.FirstOrDefault()?.Remove(700);
            EnterToContinue();
        }

        private static void Add()
        {

            casparCGServer.Channels.FirstOrDefault()?.Add(ConsumerType.File, 700, "\"test.mp4\" -vcodec libx264 -acodec acc");
            EnterToContinue();
        }

        private static void Call()
        {

            Play();
            casparCGServer.Channels.FirstOrDefault()?.Call(1, false, 50);
            EnterToContinue();
        }

        private static void GlInfo()
        {

            var infos = casparCGServer.GetGLInfo();
            Console.WriteLine(string.Join("\r\n", infos));
            EnterToContinue();
        }

        private static void ThreadsInfo()
        {

            var threads = casparCGServer.GetInfoThreads();
            Console.WriteLine(string.Join("\r\n", threads));
            EnterToContinue();
        }

        private static void PathsInfo()
        {

            var info = casparCGServer.GetInfoPaths();
            Console.WriteLine($"Media Path: {info?.Mediapath}");
            EnterToContinue();
        }

        private static void SystemInfo()
        {

            var systemInfo = casparCGServer.GetInfoSystem();
            Console.WriteLine($"System info: OS - {systemInfo?.Windows?.Name}");
            EnterToContinue();
        }

        private static void TemplateInfo()
        {


            Console.WriteLine(casparCGServer.GetInfoTemplate(casparCGServer.Templates.All.First()).AuthorName);
            EnterToContinue();
        }

        private static void ChannelInfo()
        {

            Console.WriteLine(casparCGServer.Channels.FirstOrDefault()?.GetInfo());
            EnterToContinue();
        }

        private static void CasparDevice_ConnectionStatusChanged(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine("CasparCG Server connection is connected: " + e.Connected);
        }

        private static void ChannelGrid()
        {

            casparCGServer.ChannelGrid();
            EnterToContinue();
        }

        private static void Thumb()
        {

            var base64 = casparCGServer.GetThumbnail("AMB");
            Console.WriteLine(base64);
            EnterToContinue();
        }

        private static void CasparDevice_TemplatesUpdated(object sender, EventArgs e)
        {
            var casparCGServer = sender as ICasparDevice;
            foreach (var media in casparCGServer.Templates.All)
            {
                Console.WriteLine(media.Name);
            }
            EnterToContinue();
        }

        private static void Tls()
        {

            var templates = casparCGServer.GetTemplates();
            foreach (var template in templates.All)
            {
                Console.WriteLine(template.FullName);
            }
            EnterToContinue();
        }

        private static void LoadBg()
        {

            casparCGServer.Channels.First()?.LoadBG(new CasparPlayingInfoItem("AMB", new Transition(TransitionType.SLIDE, 5000)));
            casparCGServer.Channels.First()?.Play();
            EnterToContinue();
        }

        private static void Load()
        {

            casparCGServer.Channels.First()?.LoadBG(new CasparPlayingInfoItem("AMB"), false);
            casparCGServer.Channels.First()?.Play();
            EnterToContinue();
        }

        private static void Info()
        {

            var e = casparCGServer.GetInfo();
            foreach (var channelInfo in e)
            {
                Console.WriteLine($"Channel ID: {channelInfo.ID}, Status: {channelInfo.Status}, ActiveClip: {channelInfo.ActiveClip}");
            }
            EnterToContinue();
        }

        private static void CasparDevice_UpdatedMediafiles(object sender, EventArgs e)
        {
            var casparCGServer = sender as ICasparDevice;
            foreach (var media in casparCGServer.Mediafiles)
            {
                Console.WriteLine(media?.Name);
            }
            EnterToContinue();
        }




        private static void Version()
        {

            Console.WriteLine(casparCGServer.GetVersion());
            EnterToContinue();
        }

        private static void Stop()
        {

            var channel = casparCGServer.Channels.First(x => x.ID == 1);
            channel.Stop();
            channel.Clear();
            EnterToContinue();
        }

        private static void Play()
        {
            var channel = casparCGServer.Channels.First(x => x.ID == 1);
            channel.LoadBG(new CasparPlayingInfoItem { VideoLayer = 1, Clipname = "AMB" });
            channel.Play(1);
            EnterToContinue();
        }

        private static void PlayTransition()
        {

            var channel = casparCGServer.Channels.First(x => x.ID == 1);
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


        private static void PlayEmpty()
        {

            var channel = casparCGServer.Channels.First(x => x.ID == 1);
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


        public static void GetMedias()
        {

            var clips = casparCGServer.GetMediafiles();
            Console.WriteLine(string.Join(",\r\n", clips.Select(x => x.Name)));
            EnterToContinue();
        }


        private static void PlayTemplate()
        {

            var channel = casparCGServer.Channels.First(x => x.ID == 1);
            channel.CG.Add(10, 1, "caspar_text");
            channel.CG.Play(10, 1);
            EnterToContinue();
        }

        private static void PlayMixer()
        {

            var channel = casparCGServer.Channels.First(x => x.ID == 1);
            channel.MixerManager.Brightness(1, 0.2F);
            EnterToContinue();
        }

        private static void EnterToContinue()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Tap enter to continue...");
            Console.ReadLine();

        }

    }
}