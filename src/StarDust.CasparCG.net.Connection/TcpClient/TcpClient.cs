using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleTCP
{

    /// <summary>
    /// Object to instantiated connection to tcp server
    /// </summary>
    public class SimpleTcpClient : IDisposable
    {
        private TcpClient _tcpClient;
        private readonly List<byte> _queuedMsg = new List<byte>();
        private bool _disposedValue;

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
        /// Raised when tcp client is disconnected
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
        public bool IsConnected => _tcpClient?.Client != null && IsSocketConnected(_tcpClient.Client);

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
        /// Interval to read data from the socket
        /// </summary>
        public int ReadLoopIntervalMs { get; set; } = 10;

        /// <summary>
        /// Trim white space
        /// </summary>
        public bool AutoTrimStrings { get; set; }


        /// <summary>
        /// Interval to check if the socket is already up
        /// </summary>
        public int CheckConnectivityInterval { get; set; } = 500;

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
                throw new ArgumentNullException(nameof(hostNameOrIpAddress), "Please provide an ip or an hostname");

            if (_tcpClient != null)
                Disconnect();

            _tcpClient = new TcpClient();
            _tcpClient.Connect(hostNameOrIpAddress, port);

            _readTimer = new Timer((o) => GetTcpDataReceived(), null, 0, ReadLoopIntervalMs);
            _checkTimer = new Timer((o) => CheckConnectivity(), null, 0, CheckConnectivityInterval);
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
                throw new InvalidOperationException($"Cannot sent data, the tcp connection is closed on {Hostname}:{Port}.");

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
        public Task SendAsync(byte[] data)
        {
            if (_tcpClient == null)
                throw new InvalidOperationException("Cannot send data to a null TcpClient (check to see if Connect was called)");
            return _tcpClient.GetStream().WriteAsync(data, 0, data.Length);
        }

        /// <summary>
        /// Send string to the server asynchronously
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Task SendAsync(string data)
        {
            return data == null ? Task.Delay(0) : SendAsync(StringEncoder.GetBytes(data));
        }

        /// <summary>
        /// Send data to the server and end with new line
        /// </summary>
        /// <param name="data"></param>
        public Task SendLineAsync(string data)
        {
            if (string.IsNullOrEmpty(data))
                return Task.Delay(0);

            return !data.EndsWith(SendDelimiter) ? SendAsync(data + SendDelimiter) : SendAsync(data);
        }

        /// <summary>
        /// Send data to the server and wait for a reply. If the server doesn't reply in given time, return null.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="timeout">how maximum time we need to wait for the reply</param>
        public Message SendAndGetReply(string data, TimeSpan timeout)
        {
            return SendAndGetReplyAsync(data, timeout).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Send data to the server and wait for a reply. if the server doesn't reply in given time, return null.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="timeout">how maximum time we need to wait for the reply</param>
        public async Task<Message> SendAndGetReplyAsync(string data, TimeSpan timeout)
        {

            using (var signal = new SemaphoreSlim(0, 1))
            {
                Message mReply = null;
                var handler = GetTempHandler<Message>(m =>
                {

                    mReply = m;
                    signal.Release();
                });
                DataReceived += handler;

                await SendAsync(data);
                await signal.WaitAsync(timeout);
                DataReceived -= handler;
                return mReply;
            }
        }



        /// <summary>
        /// Send string to the server, end with new line and wait for a reply. if the server doesn't reply in given time, return null.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="timeout">how maximum time we need to wait for the reply</param>
        public Message SendLineAndGetReply(string data, TimeSpan timeout)
        {
            return SendLineAndGetReplyAsync(data, timeout).GetAwaiter().GetResult();
        }



        /// <summary>
        /// Send string to the server, end with new line and wait for a reply. if the server doesn't reply in given time, return null.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="timeout">how maximum time we need to wait for the reply</param>
        public async Task<Message> SendLineAndGetReplyAsync(string data, TimeSpan timeout)
        {
            using (var signal = new SemaphoreSlim(0, 1))
            {
                Message mReply = null;
                var handler = GetTempHandler<Message>(m =>
                {

                    mReply = m;
                    signal.Release();
                });
                DataReceived += handler;
                await SendLineAsync(data);
                await signal.WaitAsync(timeout);
                DataReceived -= handler;
                return mReply;
            }
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
            if (_disposedValue)
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
            finally
            {
                _tcpClient = null;
                _disposedValue = true;
            }
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


        private void GetTcpDataReceived()
        {
            lock (_readLocker)
            {
                if (_tcpClient == null)
                    throw new NullReferenceException(nameof(_tcpClient));

                if (!_tcpClient?.Connected ?? false)
                    return;



                if (_tcpClient.Available == 0)
                {
                    Thread.Sleep(10);
                }
                else
                {
                    var byteList = new List<byte>();
                    while (_tcpClient.Available > 0 && _tcpClient.Connected)
                    {
                        var buffer = new byte[1];
                        _tcpClient.Client.Receive(buffer, 0, 1, SocketFlags.None);
                        byteList.AddRange(buffer);
                        if (buffer[0] == StringEncoder.GetBytes(ReceiveDelimiter).First())
                        {
                            var array = _queuedMsg.ToArray();
                            _queuedMsg.Clear();
                            OnDelimiterMessageReceived(_tcpClient, array);
                        }
                        else
                            _queuedMsg.AddRange(buffer);
                    }

                    if (byteList.Count <= 0)
                        return;

                    OnEndTransmissionReceived(_tcpClient, byteList.ToArray());
                }
            }
        }

        private void OnDelimiterMessageReceived(TcpClient client, byte[] msg)
        {
            var e = new Message(msg, client, StringEncoder, ReceiveDelimiter, AutoTrimStrings);
            DelimiterDataReceived?.Invoke(this, e);
        }

        private void OnEndTransmissionReceived(TcpClient client, byte[] msg)
        {
            var e = new Message(msg, client, StringEncoder, ReceiveDelimiter, AutoTrimStrings);
            DataReceived?.Invoke(this, e);
        }

        private static EventHandler<T> GetTempHandler<T>(Action<T> toDo)
        {
            void Handler(object s, T e) => toDo(e);
            return Handler;
        }
    }
}
