using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StarDust.CasparCG.net
{
    /// <summary>
    /// Provode method to enhance async programation
    /// </summary>
    public static class AsyncHelper
    {
        private static readonly TaskFactory _myTaskFactory = new
            TaskFactory(CancellationToken.None,
                TaskCreationOptions.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);

        /// <summary>
        /// Run synchronously a async method cleanly.
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return AsyncHelper._myTaskFactory
                .StartNew<Task<TResult>>(func)
                .Unwrap<TResult>()
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Run synchronously a async method cleanly.
        /// </summary>
        /// <param name="func"></param>
        public static void RunSync(Func<Task> func)
        {
            AsyncHelper._myTaskFactory
                .StartNew<Task>(func)
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }


        /// <summary>
        /// Run async method with a timeout management. Throw exception if timeout exceed
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="timeout"></param>
        ///  <param name="throwTimeoutException"></param>
        /// <returns></returns>
        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout, bool throwTimeoutException = true)
        {

            using (var timeoutCancellationTokenSource = new CancellationTokenSource())
            {

                var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token));
                if (completedTask == task)
                {
                    timeoutCancellationTokenSource.Cancel();
                    return await task;  // Very important in order to propagate exceptions
                }

                if (throwTimeoutException)
                {
                    throw new TimeoutException("The operation has timed out.");
                }

                return default(TResult);
            }
        }
    }
}
