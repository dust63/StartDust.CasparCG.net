using  StarDust.CasparCG.AmcpProtocol;
using  StarDust.CasparCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace  StarDust.CasparCG.Device
{
  public class CasparCGDataCollection : ICGDataContainer
  {
    private Dictionary<string, ICGComponentData> data_ = new Dictionary<string, ICGComponentData>();

    public void SetData(string name, string value)
    {
      data_[name] = new CGTextFieldData(value);
    }

    public void SetData(string name, ICGComponentData data)
    {
      data_[name] = data;
    }

    public ICGComponentData GetData(string name)
    {
      if (!string.IsNullOrEmpty(name) && data_.ContainsKey(name))
        return data_[name];
      return  null;
    }

    public void Clear()
    {
      data_.Clear();
    }

    public void RemoveData(string name)
    {
      if (string.IsNullOrEmpty(name) || !data_.ContainsKey(name))
        return;
      data_.Remove(name);
    }

    public List<CGDataPair> DataPairs
    {
      get
      {
        List<CGDataPair> dataPairs = new List<CGDataPair>();
        data_.ToList().ForEach(d => dataPairs.Add(new CGDataPair(d.Key, d.Value)));
        return dataPairs;
      }
    }

    public string ToXml()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("<templateData>");
      foreach (string key in data_.Keys)
      {
        sb.Append("<componentData id=\"" + key + "\">");
        data_[key].ToXml(sb);
        sb.Append("</componentData>");
      }
      sb.Append("</templateData>");
      return sb.ToString();
    }

    public string ToAMCPEscapedXml()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("<templateData>");
      foreach (string key in data_.Keys)
      {
        sb.Append("<componentData id=\\\"" + key + "\\\">");
        data_[key].ToAMCPEscapedXml(sb);
        sb.Append("</componentData>");
      }
      sb.Append("</templateData>");
      sb.Replace(Environment.NewLine, "\\n");
      return sb.ToString();
    }
  }
}
