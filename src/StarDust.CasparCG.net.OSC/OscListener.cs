using Rug.Osc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace StarDust.CasparCG.net.OSC
{
    public class OscListener : IOscListener
    {

        #region Fields

 
        private OscReceiver _oscReveiver;
        private readonly Dictionary<string, Regex> _whiteListedAddresses = new Dictionary<string, Regex>();
        private readonly Dictionary<string, Regex> _blackListedAddresses = new Dictionary<string, Regex>();
        private readonly object _lockObject = new object();
        private readonly ConcurrentDictionary<string, OscMessage> _packetAlreadyNotified = new ConcurrentDictionary<string, OscMessage>();
        private CancellationTokenSource _cancelationTokenSource;
        private TaskQueue _taskQueue = new TaskQueue();

        #endregion

        #region Properties

        /// <inheritdoc cref=""/>
        public bool IsFilteringAddress { get; private set; }

        /// <inheritdoc cref=""/>
        public IList<string> AddressesFiltered
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
        public event EventHandler ListenerStarted;
        public event EventHandler ListenerStopped;

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
            if (_oscReveiver?.State == OscSocketState.Connected)
            {
                StopListening();
                StartListening(ipAddress, port);
                return;
            }
            _cancelationTokenSource = new CancellationTokenSource();
            _packetAlreadyNotified.Clear();
            //Starting a thread  to listen message
            Task.Factory.StartNew(() => { 
                ListenToMessage(ipToListen, port,_cancelationTokenSource.Token); },
                _cancelationTokenSource.Token, 
                TaskCreationOptions.LongRunning, TaskScheduler.Default);

            ListenerStarted?.Invoke(this, EventArgs.Empty);
        }


        public void StartListening(int port)
        {
            StartListening("127.0.0.1", port);
        }


        public void StopListening()
        {
            if (_oscReveiver?.State == OscSocketState.Connected)
            {
                _cancelationTokenSource.Cancel();
                _oscReveiver?.Close();
                ListenerStopped?.Invoke(this, EventArgs.Empty);                
            }              
        }





        public void AddToAddressFiltered(string address)
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

        public void RemoveFromAddressFiltered(string address)
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

        public void AddToAddressFilteredWithRegex(string regexPattern)
        {
            if (_whiteListedAddresses.ContainsKey(regexPattern))
                return;
            lock (_lockObject)
            {
                _whiteListedAddresses.Add(regexPattern, new Regex(regexPattern, RegexOptions.Compiled));
            }
            IsFilteringAddress = true;
        }

        public void AddToAddressBlackListWithRegex(string regexPattern)
        {
            if (_blackListedAddresses.ContainsKey(regexPattern))
                return;
            lock (_lockObject)
            {
                _blackListedAddresses.Add(regexPattern, new Regex(regexPattern, RegexOptions.Compiled));
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
            else
                bundleToNotify = bundleToNotify.Where(x => !_blackListedAddresses.Any(m => m.Value.IsMatch(x.Address)));

            if (NotifyOnce)
                bundleToNotify = bundleToNotify.Where(oscMessage =>
                    !_packetAlreadyNotified.Any(x => x.Key == oscMessage.Address && x.Value.Equals(oscMessage)));



            foreach (var oscMessage in bundleToNotify)
            {
                OscMessageReceived?.Invoke(this, new OscMessageEventArgs(oscMessage));
                if (NotifyOnce)
                    _packetAlreadyNotified.AddOrUpdate(oscMessage.Address, oscMessage, (key, oldvalue) => oscMessage);

            }
        }


        private void ListenToMessage(IPAddress ipAddress, int port, CancellationToken token)
        {

            try
            {
                using (_oscReveiver = new OscReceiver(ipAddress, OscServerPort))
                {
                    _oscReveiver.Connect();
                    while (_oscReveiver.State != OscSocketState.Closed && !token.IsCancellationRequested)
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
                        _taskQueue.Enqueue(() => Task.Run(() => OnMessageReceive(packet)));                    

                    }
                }
            }
            catch (Exception)
            {
                //Exception cause here do nothing for the moment
            }        

        }







        #endregion

    }
}
