using System;
using System.Threading.Tasks;

namespace StartDust.CasparCG.net.Crosscutting
{
    /// <summary>
    /// Class to manage async event listening
    /// </summary>
    /// <typeparam name="TEventArgs"></typeparam>
    public class EventAwaiter<TEventArgs>
    {
        private readonly TaskCompletionSource<TEventArgs> _eventArrived = new TaskCompletionSource<TEventArgs>();

        private readonly Action<EventHandler<TEventArgs>> _unsubscribe;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="subscribe"></param>
        /// <param name="unsubscribe"></param>
        public EventAwaiter(Action<EventHandler<TEventArgs>> subscribe, Action<EventHandler<TEventArgs>> unsubscribe)
        {
            subscribe(Subscription);
            _unsubscribe = unsubscribe;
        }

        /// <summary>
        /// Task to wait for the event raised
        /// </summary>
        public Task<TEventArgs> WaitForEventRaised => _eventArrived.Task;

        /// <summary>
        /// Event listener method
        /// </summary>
        private EventHandler<TEventArgs> Subscription => (s, e) =>
        {
            _eventArrived.TrySetResult(e);
            _unsubscribe(Subscription);
        };
    }
    
}
