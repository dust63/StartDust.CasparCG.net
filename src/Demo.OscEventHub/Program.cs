using StarDust.CasparCG.net.OSC;
using StarDust.CasparCG.net.OSC.EventHub;
using StarDust.CasparCG.net.OSC.EventHub.Events;
using System;
using Unity;
using Unity.Lifetime;

namespace Demo.OscEventHub
{
    class Program
    {
        static UnityContainer _container;

        static void Main(string[] args)
        {

            _container = new UnityContainer();
            _container.RegisterType<IOscListener, OscListener>(new ContainerControlledLifetimeManager());
            _container.RegisterType<ICasparCGOscEventsHub, CasparCGOscEventsHub>();

            var eventHub = _container.Resolve<ICasparCGOscEventsHub>();
            
            //Attach the desire event
            eventHub.PlaybackClipChanged += OnPlaybackClipChanged;
            
            //Start to listen Osc message on 6250 port. Check your Osc port on the config of Caspar CG
            eventHub.CasparCgOscListener.StartListening(6250);
                       
            
            Console.WriteLine("Listening for OSC Message");
            Console.WriteLine("Play a clip, then play another clip and you should see message appear on the console");
            Console.WriteLine("-------------------------------------\r\n");
            Console.WriteLine("Tap any key to exit...");
            Console.Read();
        }

        private static void OnPlaybackClipChanged(object sender, PlaybackClipClipChangedEventArgs e)
        {
            Console.WriteLine($"Playback clip changed {e.ActiveClip}. On channel {e.ChannelId} and layer {e.LayerId}");
        }


    }
}
