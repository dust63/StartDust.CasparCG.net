using System;
using System.Collections.Generic;

namespace StarDust.CasparCG.net.OSC
{

    /// <summary>
    /// An OSC message listener
    /// </summary>
    public interface IOscListener
    {
        /// <summary>
        /// Raised when OSC Listener is started
        /// </summary>
        event EventHandler ListenerStarted;

        /// <summary>
        /// Raised when OSC Listener is stopped
        /// </summary>
        event EventHandler ListenerStopped;

        /// <summary>
        /// Osc Server ip that we are listening to
        /// </summary>
        string OscServerIp { get; }

        /// <summary>
        /// Osc Server port that we are listenning to.
        /// </summary>
        int OscServerPort { get; }


        /// <summary>
        /// Indicate if we filtered for some addresses
        /// </summary>
        bool IsFilteringAddress { get; }


        /// <summary>
        /// If we want to notify only once time an Osc message with same value
        /// </summary>
        bool NotifyOnce { get; set; }


        /// <summary>
        /// List of address that are filtered
        /// </summary>
        IList<string> AddressesFiltered { get; }


        /// <summary>
        /// Addresses that are blacklisted
        /// </summary>
        IList<string> AddressBlackList { get; }

        /// <summary>
        /// Fired when an OSC Message or bundle was received from CasparCG Server
        /// </summary>
        event EventHandler<OscMessageEventArgs> OscMessageReceived;


        /// <summary>
        /// Start to listen osc message on local host
        /// </summary>        
        /// <param name="port"></param>
        /// <returns></returns>
        void StartListening(int port);

        /// <summary>
        /// Start to listen osc message
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        void StartListening(string ipAddress, int port);


        /// <summary>
        /// Stop to listen OSC message
        /// </summary>
        void StopListening();


        /// <summary>
        /// Register an address. When you add an address, you will notify only for addresses that you stored.
        /// If no filter was added you will notify for all addresses
        /// </summary>
        /// <param name="address"></param>
        void AddToAddressFiltered(string address);

        /// <summary>
        /// Register an address to filter by a regex pattern. When you add an address, you will notify only for addresses that you stored.
        /// If no filter was added you will notify for all addresses
        /// </summary>
        /// <param name="address"></param>
        void AddToAddressFilteredWithRegex(string regexPattern);


        /// <summary>
        /// Remove an address from filtering. So for this address you will not be notify if some filtered are setted
        /// </summary>
        /// <param name="address"></param>
        void RemoveFromAddressFiltered(string address);


        /// <summary>
        /// Remove all addresses that are stored
        /// </summary>
        void ClearAddressWhiteList();

        /// <summary>
        /// Forbiden a method to be notified
        /// </summary>
        /// <param name="address"></param>
        void AddToAddressBlackList(string address);


        /// <summary>
        /// Forbiden a method to be notified by regex pattern
        /// </summary>
        /// <param name="address"></param>
        void AddToAddressBlackListWithRegex(string regexPattern);


        /// <summary>
        /// Allow a method to be notified
        /// </summary>
        /// <param name="address"></param>
        void RemoveFromAddressBlackListed(string address);

        /// <summary>
        /// Remove all black listed addresses
        /// </summary>
        void ClearAddressBlackList();
    }
}