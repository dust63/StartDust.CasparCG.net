using StarDust.CasparCG.net.Models;
using System;
using System.Collections.Generic;

namespace StarDust.CasparCG.net.AmcpProtocol
{

    /// <summary>
    /// Use when we receive the templates information
    /// </summary>
    public class TLSEventArgs : EventArgs
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="templates"></param>
        public TLSEventArgs(List<TemplateBaseInfo> templates)
        {
            this.Templates = templates;
        }


        /// <summary>
        /// List of the template present on the server
        /// </summary>
        public List<TemplateBaseInfo> Templates { get; }
    }
}
