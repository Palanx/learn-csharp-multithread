using System;
using System.Threading;
using System.Threading.Tasks;

namespace signaling_constructs
{
    public class BarrierExample
    {
        /*
         * Barrier wait until a specific amount of threads reach the same await and then
         * allow them to continue, is useful to process steps in different threads
         * at the same time.
         */
        private static readonly Barrier _barrier = new Barrier(0);

        public static void Test()
        {
            int numberOfRecords = GetNumberOfRecords();
            // We define the amount of participants that needs to reach the Wait to continue
            _barrier.AddParticipants(numberOfRecords);

            Task[] tasks = new Task[numberOfRecords];
            for (int i = 0; i < tasks.Length; i++)
            {
                int j = i;
                tasks[j] = Task.Factory.StartNew(() => GetDataAndStoreData(j));
            }

            Task.WaitAll(tasks);

            Console.WriteLine("Backup was completed");

        }

        private static int GetNumberOfRecords()
        {
            return 10;
        }

        private static void GetDataAndStoreData(int i)
        {
            Console.WriteLine($"Getting data from server: {i}");
            Thread.Sleep(TimeSpan.FromSeconds(2));

            /*
             * The thread signal its participation and wait until the number of participants
             * is reached in the barrier.
             */
            _barrier.SignalAndWait();

            Console.WriteLine($"Send data to backup server: {i}");

            _barrier.SignalAndWait();
        }
    }
}