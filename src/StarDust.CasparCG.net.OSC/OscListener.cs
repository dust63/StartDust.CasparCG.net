using Rug.Osc;
using System;
using System.Net;
using System.Threading.Tasks;

namespace StarDust.CasparCG.net.OSC
{
    public class OscListener : IOscListener
    {
        private bool _isListening;
        private OscReceiver _oscReveiver;

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
            OscMessageReceived?.Invoke(this, new OscMessageEventArgs(oscPacketReceived));
        }

        public void StopListening()
        {
            if (_isListening)
                _oscReveiver?.Close();
        }
    }
}
