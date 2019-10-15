using System.Net.Sockets;
using System.Text;

namespace SimpleTCP
{

    /// <summary>
    /// Tcp message
    /// </summary>
    public class Message
    {
        private readonly Encoding _encoder;
        private readonly bool _autoTrim;
        private readonly string _writeLineDelimiter;

        internal Message(byte[] data, TcpClient tcpClient, Encoding stringEncoder, string lineDelimiter)
        {
            Data = data;
            TcpClient = tcpClient;
            _encoder = stringEncoder;
            _writeLineDelimiter = lineDelimiter;
        }

        internal Message(byte[] data, TcpClient tcpClient, Encoding stringEncoder, string lineDelimiter, bool autoTrim)
        {
            Data = data;
            TcpClient = tcpClient;
            _encoder = stringEncoder;
            _writeLineDelimiter = lineDelimiter;
            _autoTrim = autoTrim;
        }


        /// <summary>
        /// Data received
        /// </summary>
        public byte[] Data { get;  }


        /// <summary>
        /// String message
        /// </summary>
        public string MessageString => _autoTrim ? _encoder.GetString(Data).Trim() : _encoder.GetString(Data);


        /// <summary>
        /// Reply to the response
        /// </summary>
        /// <param name="data"></param>
        public void Reply(byte[] data)
        {
            TcpClient.GetStream().Write(data, 0, data.Length);
        }

        /// <summary>
        /// Reply to the response
        /// </summary>
        /// <param name="data"></param>
        public void Reply(string data)
        {
            if (string.IsNullOrEmpty(data))
                return;
            Reply(_encoder.GetBytes(data));
        }

        /// <summary>
        /// Reply to the response with a "\r\n
        /// </summary>
        /// <param name="data"></param>
        public void ReplyLine(string data)
        {
            if (string.IsNullOrEmpty(data))
                return;
            if (data.EndsWith(_writeLineDelimiter))
                Reply(data + _writeLineDelimiter);
            else
                Reply(data);
        }


        /// <summary>
        /// Tcp client instance
        /// </summary>
        public TcpClient TcpClient { get; }
    }
}
