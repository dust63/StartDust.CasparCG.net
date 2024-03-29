﻿using System;
using System.Runtime.Serialization;

namespace StarDust.CasparCG.net.Connection
{
    /// <summary>
    /// Settings to connect to CasparCG Server
    /// </summary>
    [Serializable]
    public class CasparCGConnectionSettings : IEquatable<CasparCGConnectionSettings>
    {
        /// <summary>
        /// Value to indicate how many time we wait to check connectivity when server is disconnect not normaly
        /// </summary>
        public const int DefaultReconnectInterval = 5000;

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostname">Hostame of the CasparCG Server</param>
        /// <param name="port">Amcp tcp port protocol</param>
        public CasparCGConnectionSettings(string hostname, int port)
        {
            this.Hostname = hostname;
            this.Port = port;
            this.ReconnectInterval = DefaultReconnectInterval;
        }

        /// <summary>
        /// Default amcp port used -> 5250
        /// </summary>
        /// <param name="hostname"></param>
        public CasparCGConnectionSettings(string hostname) : this(hostname, 5250)
        {
        }

        #endregion

        /// <summary>
        /// Hostname of CasparCG server
        /// </summary>
        [DataMember]
        public string Hostname { get; set; }

        /// <summary>
        /// AMCP port protocol to use
        /// </summary>
        [DataMember]
        public int Port { get; set; }

        /// <summary>
        /// Start connection when the connection is initialize
        /// </summary>
        [DataMember]
        public bool AutoConnect { get; set; }

        /// <summary>
        /// How many to wait when CasparCG server is disconnected not normally to try to reconnect on it
        /// </summary>
        [DataMember]
        public int ReconnectInterval { get; set; }

        /// <summary>
        /// Check equality between CasparCGConnectionSettings
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(CasparCGConnectionSettings other)
        {
            if (other == null)
                return false;
            return string.Equals(other.Hostname, Hostname, StringComparison.OrdinalIgnoreCase) && other.Port == Port;
        }
    }
}
