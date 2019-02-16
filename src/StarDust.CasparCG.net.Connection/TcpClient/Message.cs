using System.Net.Sockets;
using System.Text;

namespace SimpleTCP
{
    public class Message
    {
        private readonly Encoding _encoder = (Encoding)null;
        private readonly bool _autoTrim = false;
        private readonly TcpClient _tcpClient;
        private readonly string _writeLineDelimiter;

        internal Message(byte[] data, TcpClient tcpClient, Encoding stringEncoder, string lineDelimiter)
        {
            Data = data;
            _tcpClient = tcpClient;
            _encoder = stringEncoder;
            _writeLineDelimiter = lineDelimiter;
        }

        internal Message(byte[] data, TcpClient tcpClient, Encoding stringEncoder, string lineDelimiter, bool autoTrim)
        {
            Data = data;
            _tcpClient = tcpClient;
            _encoder = stringEncoder;
            _writeLineDelimiter = lineDelimiter;
            _autoTrim = autoTrim;
        }

        public byte[] Data { get; private set; }

        public string MessageString
        {
            get
            {
                if (_autoTrim)
                    return _encoder.GetString(Data).Trim();
                return _encoder.GetString(Data);
            }
        }

        public void Reply(byte[] data)
        {
            _tcpClient.GetStream().Write(data, 0, data.Length);
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

        public TcpClient TcpClient
        {
            get
            {
                return _tcpClient;
            }
        }
    }
}
