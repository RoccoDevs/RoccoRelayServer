﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace RoccoServe.Framework.Server.Infrastructure
{
    internal static class TaskExtensions
    {
        public static async Task<bool> WithCancellation(this Task task, CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);

            // This disposes the registration as soon as one of the tasks trigger
            using (cancellationToken.Register(state =>
            {
                ((TaskCompletionSource<object>)state).TrySetResult(null);
            },
            tcs))
            {
                var resultTask = await Task.WhenAny(task, tcs.Task).ConfigureAwait(false);
                if (resultTask == tcs.Task)
                {
                    // Operation cancelled
                    return false;
                }

                await task.ConfigureAwait(false);
                return true;
            }
        }

        public static async Task<bool> TimeoutAfter(this Task task, TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource();
            var delayTask = Task.Delay(timeout, cts.Token);

            var resultTask = await Task.WhenAny(task, delayTask).ConfigureAwait(false);
            if (resultTask == delayTask)
            {
                // Operation cancelled
                return false;
            }
            else
            {
                // Cancel the timer task so that it does not fire
                cts.Cancel();
            }

            await task.ConfigureAwait(false);
            return true;
        }
    }
}
