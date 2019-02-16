using System;
using System.Collections.Generic;

namespace StarDust.CasparCG.AmcpProtocol
{
  public class AMCPEventArgs : EventArgs
  {


    public AMCPCommand Command { get; set; } = AMCPCommand.None;
    
    public AMCPError Error { get; set; } = AMCPError.None;

    public List<string> Data { get; } = new List<string>();
  }
}
