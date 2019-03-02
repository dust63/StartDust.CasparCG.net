using StarDust.CasparCG.net.Connection;
using StarDust.CasparCG.net.OSC;
using System;
using Unity;
using Unity.Lifetime;

namespace Demo.OSC.netcore
{
    class Program
    {

        static IUnityContainer _container;

        static void ConfigureIOC()
        {
            _container = new UnityContainer();
            _container.RegisterType<IOscListener, OscListener>(new ContainerControlledLifetimeManager());
            _container.RegisterInstance<IServerConnection>(new ServerConnection(new CasparCGConnectionSettings("127.0.0.1")));
        }


        static void Main(string[] args)
        {
            ConfigureIOC();


            OscStart();

            while (true)
            {
                var input = Console.ReadLine();
                if (input.Equals("osc start", StringComparison.InvariantCultureIgnoreCase))
                    OscStart();
                if (input.Equals("blacklist", StringComparison.InvariantCultureIgnoreCase))
                    Blacklist();
                if (input.Equals("remove blacklist", StringComparison.InvariantCultureIgnoreCase))
                    RemoveBlacklist();
                if (input.Equals("filter", StringComparison.InvariantCultureIgnoreCase))
                    Filter();
                if (input.Equals("remove filter", StringComparison.InvariantCultureIgnoreCase))
                    RemoveFilter();
                if (input.Equals("osc stop", StringComparison.InvariantCultureIgnoreCase))
                    OscStop();
            }
        }

        private static void RemoveBlacklist()
        {
            var oscListener = _container.Resolve<IOscListener>();
            //oscListener.RemoveFromAddressBlackListed("/channel/[0-9]/output/consume_time");
            oscListener.RemoveFromAddressBlackListed("/channel/1/stage/layer/1/profiler/time");
        }

        private static void Blacklist()
        {
            var oscListener = _container.Resolve<IOscListener>();
            //oscListener.AddToAddressBlackList("/channel/[0-9]/output/consume_time");
            oscListener.AddToAddressBlackListWithRegex("/channel/1/stage/layer/1/profiler/time");
        }

        private static void RemoveFilter()
        {
            var oscListener = _container.Resolve<IOscListener>();
            oscListener.RemoveFromAddressFiltered("^/channel/[0-9]/stage/layer/1(?!.*?time)");
        }

        private static void Filter()
        {
            var oscListener = _container.Resolve<IOscListener>();
            oscListener.AddToAddressFilteredWithRegex("^/channel/[0-9]/stage/layer/1(?!.*?time)");
        }

        private static void OscStop()
        {
            var oscListener = _container.Resolve<OscListener>();
            oscListener.StopListening();
            _container.Resolve<IServerConnection>().Disconnect();
            Console.WriteLine("Osc listener stopped");

        }

        private static async void OscStart()
        {
            _container.Resolve<IServerConnection>().Connect();
            var oscListener = _container.Resolve<IOscListener>();

            oscListener.OscMessageReceived += OscListener_OscMessageReceived;
            oscListener.StartListening("127.0.0.1", 6250);
            Console.WriteLine("Osc listener strarted");
        }






        private static void OscListener_OscMessageReceived(object sender, OscMessageEventArgs e)
        {
            Console.WriteLine(e.OscPacket);
        }

    }
}
