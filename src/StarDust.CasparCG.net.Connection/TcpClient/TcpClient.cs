using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleTCP
{

    /// <summary>
    /// Object to instanciated connection to tcp server
    /// </summary>
    public class SimpleTcpClient : IDisposable
    {
        private TcpClient _tcpClient;
        private readonly List<byte> _queuedMsg = new List<byte>();
        private bool disposedValue = false;

        private Timer _readTimer;
        private Timer _checkTimer;
        private bool _connectionSuccess;
        private bool _alreadySendDisconnectionEvent;
        private readonly object _readLocker = new object();
        private readonly object _checkLocker = new object();


        /// <summary>
        /// Raised when tcp client is connected
        /// </summary>
        public event EventHandler ConnectedEvent;

        /// <summary>
        /// Raised when tcp client is disconnecte
        /// </summary>
        public event EventHandler DisconnectedEvent;

        /// <summary>
        /// Raise when the tcp connection was broken
        /// </summary>
        public event EventHandler ConnectionLost;

        /// <summary>
        /// Raise when data received for the given delimiter
        /// </summary>
        public event EventHandler<Message> DelimiterDataReceived;

        /// <summary>
        /// Raise when data is received
        /// </summary>
        public event EventHandler<Message> DataReceived;


        /// <summary>
        /// Check is tcp client is connected
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return _tcpClient?.Client != null && IsSocketConnected(_tcpClient.Client);
            }
        }

        /// <summary>
        /// Delimiter that are send of the end of string
        /// </summary>
        public string SendDelimiter { get; set; }

        /// <summary>
        /// Delimiter that are send of the end of string
        /// </summary>
        public string ReceiveDelimiter { get; set; } = "\r\n";

        /// <summary>
        /// Encoder to send string datas
        /// </summary>
        public Encoding StringEncoder { get; set; } = Encoding.UTF8;

        /// <summary>
        /// Intertvall to read datas from the socket
        /// </summary>
        public int ReadLoopIntervalMs { get; set; } = 10;

        /// <summary>
        /// Trim white space
        /// </summary>
        public bool AutoTrimStrings { get; set; }


        /// <summary>
        /// Intervall to check if the socket is already up
        /// </summary>
        public int CheckConnectivityIntervall { get; set; } = 500;

        /// <summary>
        /// Current hostname connected
        /// </summary>
        public string Hostname { get; private set; }

        /// <summary>
        /// Current port connected
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// If we want to reconnect when the socket was aborted. By default true.
        /// </summary>
        public bool AutoReconnect { get; set; } = true;

        /// <summary>
        /// Ctor
        /// </summary>
        public SimpleTcpClient()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="delimiter">delimiter to split data block received</param>
        /// <param name="encoderStyle">type of encoding</param>
        public SimpleTcpClient(string delimiter, Encoding encoderStyle)
        {
            StringEncoder = encoderStyle;
            SendDelimiter = delimiter;
        }

        /// <summary>
        /// Connect on tcp server
        /// </summary>
        /// <param name="hostNameOrIpAddress">Hostname or ip of the tcp server</param>
        /// <param name="port"></param>
        public void Connect(string hostNameOrIpAddress, int port)
        {
            Hostname = hostNameOrIpAddress;
            Port = port;

            if (string.IsNullOrEmpty(hostNameOrIpAddress))
                throw new ArgumentNullException(nameof(hostNameOrIpAddress), "Please provide an ip or an hotname");

            if (_tcpClient != null)
                Disconnect();

            _tcpClient = new TcpClient();
            _tcpClient.Connect(hostNameOrIpAddress, port);

            _readTimer = new Timer((o) => GetTcpDatasReceived(), null, 0, ReadLoopIntervalMs);
            _checkTimer = new Timer((o) => CheckConnectivity(), null, 0, CheckConnectivityIntervall);
            _connectionSuccess = true;
            OnConnectedEvent();
        }


        /// <summary>
        /// Disconnect tcp connection
        /// </summary>
        public void Disconnect()
        {
            if (_tcpClient == null)
                return;

            _tcpClient.Close();
            _tcpClient = null;
            _readTimer.Dispose();
            _checkTimer.Dispose();
            OnDisconnectedEvent();
        }



        private static bool IsSocketConnected(Socket socket)
        {
            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch
            {
                return false;
            }
        }


        #region Send
        /// <summary>
        /// Send byte to the server
        /// </summary>
        /// <param name="data"></param>
        public void Send(byte[] data)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Cannot send data to a null TcpClient (check to see if Connect was called)");

            _tcpClient.GetStream().Write(data, 0, data.Length);
        }
        /// <summary>
        /// Send string data to the server
        /// </summary>
        /// <param name="data"></param>
        public void Send(string data)
        {
            if (string.IsNullOrEmpty(data))
                return;
            Send(StringEncoder.GetBytes(data));
        }
        /// <summary>
        /// Send data to the server and end with new line
        /// </summary>
        /// <param name="data"></param>
        public void SendLine(string data)
        {
            if (string.IsNullOrEmpty(data))
                return;

            if (!data.EndsWith(SendDelimiter))
                Send(data + SendDelimiter);
            else
                Send(data);
        }
        /// <summary>
        /// Send byte to the server asynchronously
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SendAsync(byte[] data)
        {
            if (_tcpClient == null)
                throw new InvalidOperationException("Cannot send data to a null TcpClient (check to see if Connect was called)");
            await _tcpClient.GetStream().WriteAsync(data, 0, data.Length);
        }

        /// <summary>
        /// Send string to the server asynchronously
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SendAsync(string data)
        {
            if (data == null)
                return;
            await SendAsync(StringEncoder.GetBytes(data));
        }

        /// <summary>
        /// Send data to the server and end with new line
        /// </summary>
        /// <param name="data"></param>
        public async Task SendLineAsync(string data)
        {
            if (string.IsNullOrEmpty(data))
                return;
            if (!data.EndsWith(SendDelimiter))
                await SendAsync(data + SendDelimiter);
            else
                await SendAsync(data);
        }

        /// <summary>
        /// Send data to the server and wait for a reply. if the server doen't reply in given time, return null.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="timeout">how maximum time we need to wait for the reply</param>
        public Message SendAndGetReply(string data, TimeSpan timeout)
        {
            Message mReply = null;
            DataReceived += (s, e) => mReply = e;
            Send(data);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (mReply == null && stopwatch.Elapsed < timeout)
                Thread.Sleep(10);
            return mReply;
        }

        /// <summary>
        /// Send data to the server and wait for a reply. if the server doen't reply in given time, return null.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="timeout">how maximum time we need to wait for the reply</param>
        public async Task<Message> SendAndGetReplyAsync(string data, TimeSpan timeout)
        {
            Message mReply = null;
            DataReceived += (s, e) => mReply = e;
            await SendAsync(data);
            await Task.Run(() =>
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                while (mReply == null && stopwatch.Elapsed < timeout)
                    Thread.Sleep(10);
            });
            return mReply;
        }

        /// <summary>
        /// Send string to the server, end with new line and wait for a reply. if the server doen't reply in given time, return null.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="timeout">how maximum time we need to wait for the reply</param>
        public Message SendLineAndGetReply(string data, TimeSpan timeout)
        {
            Message mReply = null;
            DataReceived += (s, e) => mReply = e;
            SendLine(data);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (mReply == null && stopwatch.Elapsed < timeout)
                Thread.Sleep(10);
            return mReply;
        }

        /// <summary>
        /// Send string to the server, end with new line and wait for a reply. if the server doen't reply in given time, return null.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="timeout">how maximum time we need to wait for the reply</param>
        public async Task<Message> SendLineAndGetReplyAsync(string data, TimeSpan timeout)
        {
            Message mReply = null;
            DataReceived += (s, e) => mReply = e;
            await SendLineAsync(data);
            await Task.Run(() =>
           {
               Stopwatch stopwatch = new Stopwatch();
               stopwatch.Start();
               while (mReply == null && stopwatch.Elapsed < timeout)
                   Thread.Sleep(10);
           });
            return mReply;
        }
        #endregion

        /// <summary>
        /// Disposing connection
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// When tcp client is connected to the server
        /// </summary>
        protected virtual void OnConnectedEvent()
        {
            ConnectedEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// When tcp client is disconnected from the server
        /// </summary>
        protected virtual void OnDisconnectedEvent()
        {
            DisconnectedEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Dispose connection
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;

            if (!disposing)
            {
                _readTimer.Dispose();
                _checkTimer.Dispose();
            }
            try
            {
                _tcpClient?.Close();
            }
            catch
            {
            }
            _tcpClient = null;
            disposedValue = true;
        }

        /// <summary>
        /// Check is the server is already reachable
        /// </summary>
        protected virtual void CheckConnectivity()
        {
            lock (_checkLocker)
            {
                if (_connectionSuccess && IsSocketConnected(_tcpClient.Client))
                {                  
                    return;
                }                  

                //If we are not connected again
                if (!_alreadySendDisconnectionEvent)
                {
                    ConnectionLost?.Invoke(this, EventArgs.Empty);
                    _alreadySendDisconnectionEvent = true;
                }

                if (AutoReconnect)
                {
                    try
                    {
                        Connect(Hostname, Port);                      
                        _alreadySendDisconnectionEvent = false;
                    }
                    catch (SocketException)
                    {
                        //Can't reconnect
                        _connectionSuccess = false;
                    }
                }
                else
                {
                    Disconnect();
                    _alreadySendDisconnectionEvent = false;
                }    
            }
        }


        private void GetTcpDatasReceived()
        {
            lock (_readLocker)
            {

                if (!_tcpClient?.Connected ?? false)
                    return;

                TcpClient tcpClient = _tcpClient;
                if (tcpClient.Available == 0)
                {
                    Thread.Sleep(10);
                }
                else
                {
                    var byteList = new List<byte>();

                    while (tcpClient.Available > 0 && tcpClient.Connected)
                    {
                        var buffer = new byte[1];
                        tcpClient.Client.Receive(buffer, 0, 1, SocketFlags.None);
                        byteList.AddRange(buffer);
                        if (buffer[0] == StringEncoder.GetBytes(ReceiveDelimiter).First())
                        {
                            byte[] array = _queuedMsg.ToArray();
                            _queuedMsg.Clear();
                            OnDelimiterMessageReceived(tcpClient, array);
                        }
                        else
                            _queuedMsg.AddRange(buffer);
                    }

                    if (byteList.Count <= 0)
                        return;

                    OnEndTransmissionReceived(tcpClient, byteList.ToArray());
                }
            }
        }

        private void OnDelimiterMessageReceived(TcpClient client, byte[] msg)
        {
            Message e = new Message(msg, client, StringEncoder, ReceiveDelimiter, AutoTrimStrings);
            DelimiterDataReceived?.Invoke(this, e);
        }

        private void OnEndTransmissionReceived(TcpClient client, byte[] msg)
        {
            Message e = new Message(msg, client, StringEncoder, ReceiveDelimiter, AutoTrimStrings);
            DataReceived?.Invoke(this, e);
        }
    }
}
