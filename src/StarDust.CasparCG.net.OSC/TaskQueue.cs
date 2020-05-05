using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarDust.CasparCG.net.OSC
{
    internal class TaskQueue
    {
        private Task previous = Task.FromResult(false);
        private object key = new object();

        public Task<T> Enqueue<T>(Func<Task<T>> taskGenerator)
        {
            lock (key)
            {
                var next = previous.ContinueWith(t => taskGenerator()).Unwrap();
                previous = next;
                return next;
            }
        }
        public Task Enqueue(Func<Task> taskGenerator)
        {
            lock (key)
            {
                var next = previous.ContinueWith(t => taskGenerator()).Unwrap();
                previous = next;
                return next;
            }
        }
    }
}
