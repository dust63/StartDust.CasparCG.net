using System;
using System.Threading;
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

        private CancellationTokenSource _cancellationTokenSource;

        private readonly Action<EventHandler<TEventArgs>> _unsubscribe;


        /// <summary>
        /// Timeout to cancel the event awaiting
        /// </summary>
#if DEBUG
        private TimeSpan _timeout = TimeSpan.FromSeconds(15);

#else
        /// <summary>
        /// Timeout to cancel the event awaiting
        /// </summary>
        private TimeSpan _timeout = TimeSpan.FromSeconds(5);
#endif

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="subscribe"></param>
        /// <param name="unsubscribe"></param>
        /// <param name="timeout"> default value is set to 5sec</param>
        public EventAwaiter(Action<EventHandler<TEventArgs>> subscribe, Action<EventHandler<TEventArgs>> unsubscribe, TimeSpan? timeout = null)
        {
            subscribe(Subscription);
            _unsubscribe = unsubscribe;
            _timeout = timeout ?? _timeout;
        }

        /// <summary>
        /// Cancel the awaiter of the event
        /// </summary>
        public void Cancel()
        {
            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Task to wait for the event raised
        /// </summary>
        public Task<TEventArgs> WaitForEventRaised => GetEventAwaiter();


        private Task<TEventArgs> GetEventAwaiter()
        {
            _cancellationTokenSource = new CancellationTokenSource(_timeout);
            _cancellationTokenSource.Token.Register(() => _eventArrived.SetCanceled(), useSynchronizationContext: false);
            return _eventArrived.Task.ContinueWith(t =>
            {
                if (t.IsCanceled)
                    return default(TEventArgs);

                _cancellationTokenSource?.Dispose();
                return t.Result;
            });

        }

        /// <summary>
        /// Event listener method
        /// </summary>
        private EventHandler<TEventArgs> Subscription => (s, e) =>
        {
            _unsubscribe(Subscription);
            try
            {
                _eventArrived.TrySetResult(e);
            }
            catch (Exception ex)
            {
                _eventArrived.TrySetException(ex);
            }
        };
    }

}
