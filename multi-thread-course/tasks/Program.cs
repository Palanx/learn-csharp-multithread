using System;
using System.Threading;
using System.Threading.Tasks;

namespace tasks
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            ExecuteTwoTasksInDifferentThreads();

            Console.Read();
        }

        private static void ExecuteTwoTasksInDifferentThreads()
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