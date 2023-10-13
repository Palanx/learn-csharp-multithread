using System;
using System.Threading;
using System.Threading.Tasks;

namespace spinning
{
    /// <summary>
    /// SpinWait is a struct provided by the .NET Framework that offers support for
    /// spin-based waiting. Rather than putting a thread to sleep or yielding its
    /// time slice immediately when it encounters a condition where it needs to wait,
    /// a spinning thread will stay active for a period and continuously check the
    /// condition in the hope that it will soon be satisfied.
    ///
    /// SpinWait doesn't just spin indefinitely. It spins for a certain number of
    /// iterations and then begins to yield, increasing the duration of yielding
    /// over time. This makes it adaptive to the situation, combining the benefits
    /// of spinning and yielding.
    /// </summary>
    public class SpinWaitExample
    {
        private static bool _done;

        public static void Test()
        {
            Task.Run(() =>
            {
                try
                {
                    Console.WriteLine("Task started");
                    Thread.Sleep(1);
                    Console.WriteLine("Task is done");
                }
                finally
                {
                    _done = true;
                }
            });

            // This static method will spin until the provided delegate returns true.
            SpinWait.SpinUntil(() =>
            {
                /*
                 * In this specific example, the use of Thread.MemoryBarrier() is ensuring that the
                 * reading thread always checks the latest value of _done from the memory, instead
                 * of relying on potentially stale or optimized-away local caches. However, in
                 * modern .NET, using the volatile keyword or Volatile class methods would be a
                 * clearer way to achieve this memory consistency
                 */
                Thread.MemoryBarrier();
                return _done;
            });

            Console.WriteLine("This is the end of program");
        }

        private static void MultiplyXByY(ref int val, int factor)
        {
            SpinWait spinWait = new SpinWait();
            while (true)
            {
                int snapshot1 = val;
                int calc = snapshot1 * factor;
                int snapshot2 = Interlocked.CompareExchange(ref val, calc, snapshot1);
                if (snapshot1 == snapshot2)
                    return; // No one preempted us

                /*
                 * This method performs a single spin, adapting its behavior over
                 * multiple calls. On initial calls, it will spin. As you call it
                 * repeatedly, it will start yielding.
                 */
                spinWait.SpinOnce();
            }
        }
    }
}