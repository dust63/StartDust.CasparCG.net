using StarDust.CasparCG.net.AmcpProtocol;
using StarDust.CasparCG.net.Connection;
using StarDust.CasparCG.net.Device;
using StarDust.CasparCG.net.Models;
using StarDust.CasparCG.net.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using StarDust.CasparCG.net.Datas;
using Unity;
using Unity.Lifetime;

namespace StarDust.CasparCG.AMCP.net.ClientTestConsole
{
    class Program
    {

        static IUnityContainer _container;


        static void ConfigureIOC()
        {
            _container = new UnityContainer();
            _container.RegisterInstance<IServerConnection>(new ServerConnection(new CasparCGConnectionSettings("127.0.0.1")));
            _container.RegisterType(typeof(IAMCPTcpParser), typeof(AmcpTCPParser));
            _container.RegisterSingleton<IDataParser, CasparCGDataParser>();
            _container.RegisterType(typeof(IAMCPProtocolParser), typeof(AMCPProtocolParser));
            _container.RegisterType<ICasparDevice, CasparDevice>(new ContainerControlledLifetimeManager());
        }


        private static Dictionary<string, Action> commandList = new Dictionary<string, Action>(StringComparer.OrdinalIgnoreCase)
        {
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
            {"cg add", CgAdd }
        };



        static void Main(string[] args)
        {

            ConfigureIOC();

            var casparCGServer = _container.Resolve<ICasparDevice>();
            casparCGServer.ConnectionStatusChanged += CasparDevice_ConnectionStatusChanged;
            casparCGServer.Connect();

            while (true)
            {
                Console.WriteLine("List of command");
                Console.WriteLine(string.Join(Environment.NewLine, commandList.Select(x => $">>>>{x.Key}")));
                var input = Console.ReadLine();
                if (commandList.ContainsKey(input))
                    commandList[input].Invoke();
                else
                    Console.WriteLine("Invalid command");

                Console.WriteLine(string.Empty);
            }
        }

        private static void Clear()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            var channel = casparCGServer.Channels.FirstOrDefault();
            if (channel == null)
            {
                Console.WriteLine("No channel found");
                return;
            }

            channel.Clear();
        }

        private static void CgAdd()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            var channel = casparCGServer.Channels.FirstOrDefault();
            if (channel == null)
            {
                Console.WriteLine("No channel found");
                return;
            }
       
            channel.CG.Add(10,1, @"CASPARCG_FLASH_TEMPLATES_EXAMPLE_PACK_1/ADVANCEDTEMPLATE2");
            channel.CG.Play(10,1);
        }

        private static void CgUpdate()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
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
          
            channel.CG.Update(10,1, data);

        }

        private static void Cls()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            var clips = casparCGServer.GetMediafiles();
            Console.WriteLine(string.Join(Environment.NewLine, clips.Select(x => x.FullName)));
        }

        private static void Remove()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            casparCGServer.Channels.FirstOrDefault()?.Remove(700);
        }

        private static void Add()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            casparCGServer.Channels.FirstOrDefault()?.Add(ConsumerType.File, 700, "\"test.mp4\" -vcodec libx264 -acodec acc");
        }

        private static void Call()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            Play();
            casparCGServer.Channels.FirstOrDefault()?.Call(1, false, 50);
        }

        private static void GlInfo()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            var infos = casparCGServer.GetGLInfo();
            Console.WriteLine(string.Join("\r\n", infos));
        }

        private static void ThreadsInfo()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            var threads = casparCGServer.GetInfoThreads();
            Console.WriteLine(string.Join("\r\n", threads));
        }

        private static void PathsInfo()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            var info = casparCGServer.GetInfoPaths();
            Console.WriteLine($"Media Path: {info?.Mediapath}");
        }

        private static void SystemInfo()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            var systemInfo = casparCGServer.GetInfoSystem();
            Console.WriteLine($"System info: OS - {systemInfo?.Windows?.Name}");
        }

        private static void TemplateInfo()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            Console.WriteLine(casparCGServer.GetInfoTemplate(new TemplateBaseInfo { Folder = "CasparCG_Flash_Templates_Example_Pack_1", Name = "ADVANCEDTEMPLATE1" }).AuthorName);
        }

        private static void ChannelInfo()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            Console.WriteLine(casparCGServer.Channels.FirstOrDefault()?.GetInfo());
        }

        private static void CasparDevice_ConnectionStatusChanged(object sender, ConnectionEventArgs e)
        {
            Console.WriteLine("CasparCG Server connection is connected: " + e.Connected);
        }

        private static void ChannelGrid()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            casparCGServer.ChannelGrid();
        }

        private static void Thumb()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            var base64 = casparCGServer.GetThumbnail("AMB");
            Console.WriteLine(base64);
        }

        private static void CasparDevice_TemplatesUpdated(object sender, EventArgs e)
        {
            var casparCGServer = sender as ICasparDevice;
            foreach (var media in casparCGServer.Templates.All)
            {
                Console.WriteLine(media.Name);
            }
        }

        private static void Tls()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            var templates = casparCGServer.GetTemplates();
            foreach (var template in templates.All)
            {
                Console.WriteLine(template.FullName);
            }

        }

        private static void LoadBg()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            casparCGServer.Channels.First()?.LoadBG(new CasparPlayingInfoItem("AMB", new Transition(TransitionType.SLIDE, 5000)));
            casparCGServer.Channels.First()?.Play();
        }

        private static void Load()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            casparCGServer.Channels.First()?.LoadBG(new CasparPlayingInfoItem("AMB"), false);
            casparCGServer.Channels.First()?.Play();
        }

        private static void Info()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            var e = casparCGServer.GetInfo();
            foreach (var channelInfo in e)
            {
                Console.WriteLine($"Channel ID: {channelInfo.ID}, Status: {channelInfo.Status}, ActiveClip: {channelInfo.ActiveClip}");
            }
        
        }

        private static void CasparDevice_UpdatedMediafiles(object sender, EventArgs e)
        {
            var casparCGServer = sender as ICasparDevice;
            foreach (var media in casparCGServer.Mediafiles)
            {
                Console.WriteLine(media?.Name);
            }

        }




        private static void Version()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            Console.WriteLine(casparCGServer.GetVersion());
        }

        private static void Stop()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            var channel = casparCGServer.Channels.First(x => x.ID == 1);
            channel.Stop();
            channel.Clear();
        }

        private static void Play()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            var channel = casparCGServer.Channels.First(x => x.ID == 1);
            channel.LoadBG(new CasparPlayingInfoItem { VideoLayer = 1, Clipname = "AMB" });
            channel.Play(1);
        }

        public static void GetMedias()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            var clips = casparCGServer.GetMediafiles();
            Console.WriteLine(string.Join(",\r\n", clips.Select(x => x.Name)));
        }


        private static void PlayTemplate()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            var channel = casparCGServer.Channels.First(x => x.ID == 1);
            channel.CG.Add(10, 1, "caspar_text");
            channel.CG.Play(10, 1);
        }

        private static void PlayMixer()
        {
            var casparCGServer = _container.Resolve<ICasparDevice>();
            var channel = casparCGServer.Channels.First(x => x.ID == 1);
            channel.MixerManager.Brightness(1, 0.2F);
        }

    }
}
