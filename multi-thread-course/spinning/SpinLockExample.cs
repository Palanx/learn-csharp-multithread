using System;
using System.Threading;
using System.Threading.Tasks;

namespace spinning
{
    /// <summary>
    /// 1. Purpose: SpinLock is a full-blown synchronization primitive that provides mutual
    /// exclusion, similar to other locks like Monitor (used with the lock keyword in C#).
    /// It's meant to protect critical sections of code so that only one thread can enter
    /// at a time.
    ///
    /// 2. Usage: SpinLock is a struct that you use to lock and unlock around a critical
    /// section. You typically use its Enter(ref bool lockTaken) and Exit() methods to
    /// acquire and release the lock.
    ///
    /// 3. Doesn't Yield Initially: When a thread tries to acquire a SpinLock and it's
    /// already held, the thread will spin for a while, hoping that the lock will be
    /// released soon. If the lock isn't released within a certain time, the thread might
    /// then yield or sleep, giving up its time slice.
    ///
    /// 4. Avoids Recursion: Unlike some other locking primitives, SpinLock is non-reentrant,
    /// meaning a thread cannot acquire it again if it already holds it. Attempting to do so
    /// will result in an exception. This behavior is intentional to avoid potential
    /// deadlocks and to keep the SpinLock lightweight.
    ///
    /// 5. Low-Level Primitive: SpinLock provides a minimal feature set compared to other
    /// locks. It doesn't support timeouts, recursion, or thread ownership identification,
    /// making it a more raw tool but also potentially faster in the right circumstances.
    /// </summary>
    public class SpinLockExample
    {
        private static int _counter = 0;
        private static SpinLock _spinLock = new SpinLock();

        /// <summary>
        /// 1. We have a static class SpinLockExample that contains a static integer _counter
        /// and a static SpinLock _spinLock.
        /// 2. The IncrementCounter method safely increments the counter using the SpinLock
        /// to ensure mutual exclusion.
        /// 3. In the SpinLockExample class's Test method, we use the Parallel.For method to try and
        /// increment the counter 10,000 times concurrently from multiple threads.
        /// 4. After all increments are done, we print the counter's value, which should be
        /// 10,000 since we've ensured thread safety with our SpinLock.
        /// </summary>
        public static void Test()
        {
            Parallel.For(0, 10000, _ =>
            {
                IncrementCounter();
            });

            Console.WriteLine($"Counter Value: {GetCounter()}");
        }

        private static void IncrementCounter()
        {
            bool lockTaken = false;
            try
            {
                _spinLock.Enter(ref lockTaken);
                _counter++;
            }
            finally
            {
                if (lockTaken)
                {
                    _spinLock.Exit();
                }
            }
        }

        private static int GetCounter()
        {
            return _counter; // Reading an int is atomic in C#, so no lock is needed here
        }
    }
}