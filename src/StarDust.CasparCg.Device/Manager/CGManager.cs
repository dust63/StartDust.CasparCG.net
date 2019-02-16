using  StarDust.CasparCG.AmcpProtocol;
using  StarDust.CasparCG.Models.Info;

namespace  StarDust.CasparCG.Device
{
    public class CGManager
    {
        public IAMCPTcpParser AmcpTcpParser { get; }
        protected ChannelInfo Channel { get; private set; }

        public CGManager(ChannelInfo channel, IAMCPTcpParser amcpTcpParser)
        {
            AmcpTcpParser = amcpTcpParser;
            Channel = channel;
        }

        public void Add(uint layer, string template)
        {
            Add(layer, template, false, string.Empty);
        }

        public void Add(int videoLayer, uint layer, string template)
        {
            Add(videoLayer, layer, template, false, string.Empty);
        }

        public void Add(uint layer, string template, bool bPlayOnLoad)
        {
            Add(layer, template, bPlayOnLoad, string.Empty);
        }

        public void Add(int videoLayer, uint layer, string template, bool bPlayOnLoad)
        {
            Add(videoLayer, layer, template, bPlayOnLoad, string.Empty);
        }

        public void Add(uint layer, string template, string data)
        {
            Add(layer, template, false, data);
        }

        public void Add(int videoLayer, uint layer, string template, string data)
        {
            Add(videoLayer, layer, template, false, data);
        }

        public void Add(uint layer, string template, bool bPlayOnLoad, string data)
        {
            AmcpTcpParser.SendCommandAndGetStatus("CG " + Channel.ID + " ADD " + layer + " \"" + template + "\" " + (bPlayOnLoad ? "1" : "0") + " \"" + (!string.IsNullOrEmpty(data) ? data : string.Empty) + "\"");
        }

        public void Add(int videoLayer, uint layer, string template, bool bPlayOnLoad, string data)
        {
            if (videoLayer == -1)
                Add(layer, template, bPlayOnLoad, data);
            else
                AmcpTcpParser.SendCommandAndGetStatus("CG " + Channel.ID + "-" + videoLayer + " ADD " + layer + " \"" + template + "\" " + (bPlayOnLoad ? "1" : "0") + " \"" + (!string.IsNullOrEmpty(data) ? data : string.Empty) + "\"");
        }

        public void Add(uint layer, string template, ICGDataContainer data)
        {
            Add(layer, template, false, data);
        }

        public void Add(int videoLayer, uint layer, string template, ICGDataContainer data)
        {
            Add(videoLayer, layer, template, false, data);
        }

        public void Add(uint layer, string template, bool bPlayOnLoad, ICGDataContainer data)
        {
            AmcpTcpParser.SendCommandAndGetStatus("CG " + Channel.ID + " ADD " + layer + " \"" + template + "\" " + (bPlayOnLoad ? "1" : "0") + " \"" + (data != null ? data.ToAMCPEscapedXml() : string.Empty) + "\"");
        }

        public void Add(int videoLayer, uint layer, string template, bool bPlayOnLoad, ICGDataContainer data)
        {
            if (videoLayer == -1)
                Add(layer, template, bPlayOnLoad, data);
            else
                AmcpTcpParser.SendCommandAndGetStatus("CG " + Channel.ID + "-" + videoLayer + " ADD " + layer + " \"" + template + "\" " + (bPlayOnLoad ? "1" : "0") + " \"" + (data != null ? data.ToAMCPEscapedXml() : string.Empty) + "\"");
        }

        public void Remove(uint layer)
        {
            AmcpTcpParser.SendCommandAndGetStatus("CG " + Channel.ID + " REMOVE " + layer);
        }

        public void Remove(int videoLayer, uint layer)
        {
            if (videoLayer == -1)
                Remove(layer);
            else
                AmcpTcpParser.SendCommandAndGetStatus("CG " + Channel.ID + "-" + videoLayer + " REMOVE " + layer);
        }

        public void Clear()
        {
            AmcpTcpParser.SendCommandAndGetStatus("CG " + Channel.ID + " CLEAR");
        }

        public void Clear(int videoLayer)
        {
            if (videoLayer == -1)
                Clear();
            else
                AmcpTcpParser.SendCommandAndGetStatus("CG " + Channel.ID + "-" + videoLayer + " CLEAR");
        }

        public void Play(uint layer)
        {
            AmcpTcpParser.SendCommandAndGetStatus("CG " + Channel.ID + " PLAY " + layer);
        }

        public void Play(int videoLayer, uint layer)
        {
            if (videoLayer == -1)
                Play(layer);
            else
                AmcpTcpParser.SendCommandAndGetStatus("CG " + Channel.ID + "-" + videoLayer + " PLAY " + layer);
        }

        public void Stop(uint layer)
        {
            AmcpTcpParser.SendCommandAndGetStatus("CG " + Channel.ID + " STOP " + layer);
        }

        public void Stop(int videoLayer, uint layer)
        {
            if (videoLayer == -1)
                Stop(layer);
            else
                AmcpTcpParser.SendCommandAndGetStatus("CG " + Channel.ID + "-" + videoLayer + " STOP " + layer);
        }

        public void Next(uint layer)
        {
            AmcpTcpParser.SendCommandAndGetStatus("CG " + Channel.ID + " NEXT " + layer);
        }

        public void Next(int videoLayer, uint layer)
        {
            if (videoLayer == -1)
                Next(layer);
            else
                AmcpTcpParser.SendCommandAndGetStatus("CG " + Channel.ID + "-" + videoLayer + " NEXT " + layer);
        }

        public void Update(uint layer, ICGDataContainer data)
        {
            AmcpTcpParser.SendCommandAndGetStatus("CG " + Channel.ID + " UPDATE " + layer + "  \"" + data.ToAMCPEscapedXml() + "\"");
        }

        public void Update(int videoLayer, uint layer, ICGDataContainer data)
        {
            if (videoLayer == -1)
                Update(layer, data);
            else
                AmcpTcpParser.SendCommandAndGetStatus("CG " + Channel.ID + "-" + videoLayer + " UPDATE " + layer + "  \"" + data.ToAMCPEscapedXml() + "\"");
        }

        public void Invoke(uint layer, string method)
        {
            AmcpTcpParser.SendCommandAndGetStatus("CG " + Channel.ID + " INVOKE " + layer + " " + method);
        }

        public void Invoke(int videoLayer, uint layer, string method)
        {
            if (videoLayer == -1)
                Invoke(layer, method);
            else
                AmcpTcpParser.SendCommandAndGetStatus("CG " + Channel.ID + "-" + videoLayer + " INVOKE " + layer + " " + method);
        }

        public void Info()
        {
            AmcpTcpParser.SendCommandAndGetStatus("CG " + Channel.ID + " INFO");
        }

        public void Info(int videoLayer)
        {
            if (videoLayer == -1)
                Info();
            else
                AmcpTcpParser.SendCommandAndGetStatus("CG " + Channel.ID + "-" + videoLayer + " INFO");
        }
    }
}
