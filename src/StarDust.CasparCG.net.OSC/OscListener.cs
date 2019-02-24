using Rug.Osc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StarDust.CasparCG.net.OSC
{
    public class OscListener : IOscListener
    {
        private bool _isListening;
        private OscReceiver _oscReveiver;
        private readonly Dictionary<string, Regex> _registeredMethod = new Dictionary<string, Regex>();
        private bool _isSomeMethodRegistered;
        private readonly object _lockObject = new object();


        private string _oscServerIp;

        public string OscServerIp
        {
            get { return _oscServerIp; }
            private set
            {
                IPAddress.Parse(value);
                _oscServerIp = value;
            }
        }



        public void RegisterMethod(string method)
        {
            lock (_lockObject)
            {
                if (_registeredMethod.ContainsKey(method))
                    return;
                //We replace [0-9] to [0-9]\d* to allow all digit in regex
                _registeredMethod.Add(method, new Regex(method.Replace("[0-9]", "[0-9]\\d*"), RegexOptions.Compiled));
                _isSomeMethodRegistered = true;
            }


        }

        public void UnregisterMethod(string method)
        {
            lock (_lockObject)
            {
                _registeredMethod.Remove(method);
                _isSomeMethodRegistered = _registeredMethod.Any();
            }
        }




        public int OscServerPort { get; private set; }

        public event EventHandler<OscMessageEventArgs> OscMessageReceived;


        public async Task StartListening(string ipAddress, int port)
        {

            if (port <= 0)
                throw new ArgumentException("Please provide a valid port value");

            if (string.IsNullOrEmpty(ipAddress))
                throw new ArgumentNullException(nameof(ipAddress));

            OscServerPort = port;

            //Try to parse the ip
            var ipToListen = IPAddress.Parse(ipAddress);
            OscServerIp = ipAddress;


            if (_isListening)
            {
                StopListening();
                await StartListening(ipAddress, port);
                return;
            }

            _oscReveiver = new OscReceiver(ipToListen, OscServerPort);
            _oscReveiver.Connect();

            //Starting a thread  to listen message
            await Task.Factory.StartNew(async () => { await WaitForMessage(); });
        }

        private async Task WaitForMessage()
        {


            try
            {
                while (_oscReveiver.State != OscSocketState.Closed)
                {
                    // if we are in a state to recieve
                    if (_oscReveiver.State != OscSocketState.Connected)
                    {

                        continue;
                    }
                    // get the next message 
                    // this will block until one arrives or the socket is closed
                    var packet = _oscReveiver.Receive();

                    //Treat the message
                    await Task.Factory.StartNew(() => OnMessageReceive(packet));

                }
            }
            catch (Exception)
            {
                //Exception cause here do nothing for the moment
            }
            _isListening = false;
        }


        protected void OnMessageReceive(OscPacket oscPacketReceived)
        {
            if (!_isSomeMethodRegistered)
            {
                OscMessageReceived?.Invoke(this, new OscMessageEventArgs(oscPacketReceived));
                return;
            }

            var bundle = oscPacketReceived as OscBundle;
            if (bundle == null)
            {
                return;
            }
            //example regex taht work for all layer and channel -> /channel/[0-9]\d*/stage/layer/[0-9]\d*/background/producer
            var bundleToNotify = bundle
                    .OfType<OscMessage>()
                    .Where(x => _registeredMethod.Any(m => m.Value.IsMatch(x.Address)));
            foreach (var oscMessage in bundleToNotify)
            {
                OscMessageReceived?.Invoke(this, new OscMessageEventArgs(oscMessage));
            }


        }

        public void StopListening()
        {
            if (_isListening)
                _oscReveiver?.Close();
        }
    }
}
