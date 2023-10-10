using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace tasks
{
    /// <summary>
    /// Just a example class of how to execute callbacks when a Task is canceled
    /// </summary>
    class WebClientWrapper
    {
        private WebClient wc = new WebClient();

        private async Task LongRunningOperation(CancellationToken token)
        {
            if (token.IsCancellationRequested) return;

            // ctr is create to be disposed after the function Download is complete
            // if the variable doesn't exist, the delegate registered won't be disposed.
            using (CancellationTokenRegistration ctr = token.Register(() => wc.CancelAsync()))
            {
                wc.DownloadStringAsync(new Uri("http://www.engineerspock.com"));
            }
        }
    }

    public class MethodExamples
    {
        /// <summary>
        /// It a blocking call for a period of time that returns a cancelable task.
        /// </summary>
        private static void DelayExample()
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            Task<int> t1 = Task.Run(() => Print(cts.Token), cts.Token);
            Task<int> t2 = null;

            Console.WriteLine("Started t1");

            Task delayTask = Task.Delay(5000, cts.Token);
            delayTask.ContinueWith(t =>
            {
                t2 = Task.Run(() => Print(cts.Token), cts.Token);
                Console.WriteLine("Started t2");
            }, cts.Token);
        }

        /// <summary>
        /// It's a non-blocking call. It returns a Task that will complete when any of the
        /// supplied tasks has completed. It's used in async programming when you want to
        /// perform other operations when any of the tasks completes, without blocking the
        /// current thread.
        /// </summary>
        private static void WhenAnyExample()
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            Task<int> t1 = Task.Run(() => Print(cts.Token), cts.Token);
            Task<int> t2 = Task.Run(() => Print(cts.Token), cts.Token);

            Task<Task<int>> completedTask = Task.WhenAny(t1, t2);
            completedTask.Wait(cts.Token);

            Console.WriteLine($"Task id {completedTask.Result.Id} completed.");
        }

        /// <summary>
        /// It will block the calling thread until any of the provided tasks completes,
        /// faults, or is cancelled. It returns the integer index of the completed task
        /// in the tasks array.
        /// </summary>
        private static void WaitAnyExample()
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            Task<int> t1 = Task.Run(() => Print(cts.Token), cts.Token);
            Task<int> t2 = Task.Run(() => Print(cts.Token), cts.Token);

            int result = Task.WaitAny(t1, t2);

            Console.WriteLine($"Thread idx [{result}] completed.");
        }

        /// <summary>
        /// It blocks the caller thread to wait a task.
        /// </summary>
        private static void WaitExample()
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            Task<int> t1 = Task.Run(() => Print(cts.Token), cts.Token);
            Task<int> t2 = Task.Run(() => Print(cts.Token), cts.Token);

            t1.Wait(cts.Token);
            t2.Wait(cts.Token);

            Console.WriteLine($"t1: {t1.Result}, t2: {t2.Result}");
        }

        /// <summary>
        /// It doesn't block the caller thread, it returns a task that will be completed when all the array
        /// of tasks is completed and the delegated that is used will return all the tasks that continue tasks
        /// was waiting.
        /// </summary>
        private static void ContinueWhenAllExample()
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            Task<int> t1 = Task.Run(() => Print(cts.Token), cts.Token);
            Task<int> t2 = Task.Run(() => Print(cts.Token), cts.Token);

            Task.Factory.ContinueWhenAll(new[] { t1, t2 }, tasks =>
            {
                Task<int> t1Task = tasks[0];
                Task<int> t2Task = tasks[1];

                Console.WriteLine($"t1Task: {t1Task.Result}, t2Task: {t2Task.Result}");
            });

            Console.WriteLine("Thread doesn't blocked");
        }

        /// <summary>
        /// doesn't block the calling thread as well and return a task when the
        /// task that is awaited finishes.
        /// </summary>
        private static void ContinueWith()
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            Task<int> t1 = Task.Run(() => Print(cts.Token), cts.Token);
            Task continuationTask = t1.ContinueWith(prevTask =>
            {
                Console.WriteLine($"How many numbers were processed by prev. task: {prevTask.Result}");
                Task<int> t2 = Task.Run(() => Print(cts.Token), cts.Token);
            }, TaskContinuationOptions.OnlyOnRanToCompletion); // Just execute the delegate when the status is RanRoCompletion
            continuationTask.ContinueWith(t => Console.WriteLine("Ok,this failed!"),
                TaskContinuationOptions.OnlyOnFaulted); // Just execute the delegate when the status is Faulted

            Console.WriteLine("Main thread is not blocked");
        }

        private static void ExecuteTasksInDifferentThreadExample()
        {
            CancellationTokenSource parentCts = new CancellationTokenSource();
            CancellationTokenSource childCts = CancellationTokenSource.CreateLinkedTokenSource(parentCts.Token);

            /*
             * When you pass the cancellation token to StartNew(), it associates the token with the task,
             * allowing the task to be canceled externally using the provided token.
             * This ensures that if cancellation is requested, the task can be terminated.
             * On the other hand, passing the cancellation token to the Print() method allows
             * the method itself to check if cancellation has been requested and take appropriate action.
             * This enables the Print() method to respond to cancellation requests internally and exit
             * the task gracefully.
             */
            Task<int> t1 = Task.Run(() => Print(parentCts.Token), parentCts.Token);
            // This is the same as Task.Run()
            Task<int> t2 = Task.Factory.StartNew(() => Print(childCts.Token),
                childCts.Token,
                // TaskCreationOptions.LongRunning Will make the task run in a dedicated thread and not in the threads pool thread
                TaskCreationOptions.DenyChildAttach | TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            parentCts.Cancel();

            try
            {
                Console.WriteLine($"The first task processed:{t1.Result}");
                Console.WriteLine($"The second task processed:{t2.Result}");
            }
            catch (AggregateException ex) {}

            Console.WriteLine($"T1 status: {t1.Status}");
            Console.WriteLine($"T2 status: {t2.Status}");
        }

        private static int Print(CancellationToken token)
        {
            // If you need to sleep for some time within a cancelable task but finish it in case the token was cancelled.
            // if (token.WaitHandle.WaitOne(2000))
            // {
            //     token.ThrowIfCancellationRequested();
            // }

            Console.WriteLine($"Is thread pool thread:{Thread.CurrentThread.IsThreadPoolThread}");
            int total = 0;
            for (int i = 0; i < 100; i++)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Cancellation Requested!");
                }
                token.ThrowIfCancellationRequested();
                total++;
                Console.WriteLine($"Current task id={Task.CurrentId}. Value={i}");
            }

            return total;
        }
    }
}