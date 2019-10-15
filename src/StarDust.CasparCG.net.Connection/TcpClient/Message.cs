using System.Net.Sockets;
using System.Text;

namespace SimpleTCP
{
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

        public byte[] Data { get;  }

        public string MessageString => _autoTrim ? _encoder.GetString(Data).Trim() : _encoder.GetString(Data);

        public void Reply(byte[] data)
        {
            TcpClient.GetStream().Write(data, 0, data.Length);
        }

        public void Reply(string data)
        {
            if (string.IsNullOrEmpty(data))
                return;
            Reply(_encoder.GetBytes(data));
        }

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
