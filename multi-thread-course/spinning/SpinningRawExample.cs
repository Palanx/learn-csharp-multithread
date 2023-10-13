using System;
using System.Threading;
using System.Threading.Tasks;

namespace spinning
{
    /// <summary>
    /// "Spinning" in multi-threaded scenarios refers to a thread continuously checking
    /// a condition rather than yielding or sleeping when it's waiting for a resource.
    /// This technique is called "spin-wait" or "busy-wait," and the basic idea is to
    /// have the thread continuously poll until a certain condition is met.
    /// </summary>
    public class SpinningRawExample
    {
        private static bool _done;

        public static void Test()
        {
            Task.Run(() =>
            {
                try
                {
                    Console.WriteLine("Task started");
                    Thread.Sleep(1000);
                    Console.WriteLine("Task is done");
                }
                finally
                {
                    _done = true;
                }
            });

            while (!_done)
            {
                Thread.Sleep(10);
            }

            Console.WriteLine("This is the end of program");
        }
    }
}