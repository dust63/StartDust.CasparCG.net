using Rug.Osc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StarDust.CasparCG.net.OSC
{
    public class OscListener : IOscListener
    {

        #region Fields

        private bool _isListening;
        private OscReceiver _oscReveiver;
        private readonly Dictionary<string, Regex> _whiteListedAddresses = new Dictionary<string, Regex>();
        private readonly Dictionary<string, Regex> _blackListedAddresses = new Dictionary<string, Regex>();
        private readonly object _lockObject = new object();
        private readonly ConcurrentDictionary<string, OscMessage> _packetAlreadyNotified = new ConcurrentDictionary<string, OscMessage>();

        #endregion

        #region Properties

        /// <inheritdoc cref=""/>
        public bool IsFilteringAddress { get; private set; }

        /// <inheritdoc cref=""/>
        public IList<string> AddressWhiteList
        {
            get
            {
                return _whiteListedAddresses.Select(x => x.Key).ToList();
            }
        }

        /// <inheritdoc cref=""/>
        public IList<string> AddressBlackList
        {
            get
            {
                return _blackListedAddresses.Select(x => x.Key).ToList();
            }
        }

        /// <inheritdoc cref=""/>
        public string OscServerIp { get; set; }

        /// <inheritdoc cref=""/>
        public int OscServerPort { get; private set; }

        /// <inheritdoc cref=""/>
        public bool NotifyOnce { get; set; } = true;

        #endregion

        #region Events

        public event EventHandler<OscMessageEventArgs> OscMessageReceived;

        #endregion

        #region Public Methods

        public void StartListening(string ipAddress, int port)
        {

            if (port <= 0)
                throw new ArgumentException("Please provide a valid port value");

            OscServerPort = port;

            if (string.IsNullOrEmpty(ipAddress))
                throw new ArgumentNullException(nameof(ipAddress));
            //Try to parse the ip
            var ipToListen = IPAddress.Parse(ipAddress);
            OscServerIp = ipAddress;

            //If we are already listening we restart the listener
            if (_isListening)
            {
                StopListening();
                StartListening(ipAddress, port);
                return;
            }

            //Starting a thread  to listen message
            Task.Run(() => { ListenToMessage(ipToListen, port); });
        }




        public void StopListening()
        {
            if (_isListening)
                _oscReveiver?.Close();
        }





        public void AddToAddressWhiteList(string address)
        {

            if (_whiteListedAddresses.ContainsKey(address))
                return;
            lock (_lockObject)
            {
                //We replace [0-9] to [0-9]\d* to allow all digit in regex
                _whiteListedAddresses.Add(address, new Regex(address.Replace("[0-9]", "[0-9]\\d*"), RegexOptions.Compiled));
            }
            IsFilteringAddress = true;

        }

        public void RemoveFromAddressWhiteList(string address)
        {
            lock (_lockObject)
            {
                _whiteListedAddresses.Remove(address);
                IsFilteringAddress = _whiteListedAddresses.Any();
            }
        }

        public void ClearAddressWhiteList()
        {
            lock (_lockObject)
            {
                _whiteListedAddresses.Clear();
                IsFilteringAddress = false;
            }
        }


        public void AddToAddressBlackList(string address)
        {

            if (_blackListedAddresses.ContainsKey(address))
                return;
            lock (_lockObject)
            {
                _blackListedAddresses.Add(address, new Regex(address.Replace("[0-9]", "[0-9]\\d*"), RegexOptions.Compiled));
            }

        }

        public void RemoveFromAddressBlackListed(string address)
        {
            if (!_blackListedAddresses.ContainsKey(address))
                return;
            lock (_lockObject)
            {
                _blackListedAddresses.Remove(address);
            }
        }

        public void ClearAddressBlackList()
        {
            lock (_lockObject)
            {
                _blackListedAddresses.Clear();
            }
        }

        #endregion

        #region Private Methods

        protected void OnMessageReceive(OscPacket oscPacketReceived)
        {
            var bundle = oscPacketReceived as OscBundle;
            if (bundle == null)
            {
                return;
            }

            //example regex taht work for all layer and channel -> /channel/[0-9]\d*/stage/layer/[0-9]\d*/background/producer
            var bundleToNotify = bundle
                .OfType<OscMessage>();

            if (IsFilteringAddress)
                bundleToNotify = bundleToNotify
                .Where(x => _whiteListedAddresses.Any(m => m.Value.IsMatch(x.Address)));

            if (NotifyOnce)
                bundleToNotify = bundleToNotify.Where(oscMessage =>
                    !_packetAlreadyNotified.Any(x => x.Key == oscMessage.Address && x.Value.Equals(oscMessage)));

            bundleToNotify = bundleToNotify.Where(x => !_blackListedAddresses.Any(m => m.Value.IsMatch(x.Address)));

            foreach (var oscMessage in bundleToNotify)
            {
                OscMessageReceived?.Invoke(this, new OscMessageEventArgs(oscMessage));
                if (NotifyOnce)
                    _packetAlreadyNotified.AddOrUpdate(oscMessage.Address, oscMessage, (key, oldvalue) => oscMessage);

            }
        }


        private void ListenToMessage(IPAddress ipAddress, int port)
        {

            try
            {
                using (_oscReveiver = new OscReceiver(ipAddress, OscServerPort))
                {
                    _oscReveiver.Connect();
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
                        Task.Run(() => OnMessageReceive(packet));

                    }
                }
            }
            catch (Exception)
            {
                //Exception cause here do nothing for the moment
            }
            _isListening = false;

        }




        #endregion

    }
}
