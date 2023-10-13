using System;
using System.Threading;
using System.Threading.Tasks;

namespace signaling_constructs
{
    public static class CountdownEventExample
    {
        /*
         * When a countdown Wait() is reached the execution is blocked until the count
         * is equal to zero, the the execution continues.
         */
        private static CountdownEvent _countdown = new CountdownEvent(3);

        public static void Test()
        {
            Task.Run(DoWork);
            Task.Run(DoWork);
            Task.Run(DoWork);

            _countdown.Wait();
            Console.WriteLine("All tasks have finished their work!");

            // you can reset the countdown as well with:
            // _countdown.Reset();

            /*
             * You can increase the countdown with:
             * _countdown.TryAddCount(1);
             * but once the countdown reached the zero, this
             * function will return false and will be impossible
             * to increase the number, you just can reset it.
             */

        }

        private static void DoWork()
        {
            Thread.Sleep(1000);
            Console.WriteLine($"I'm a task with id:{Task.CurrentId}");
            // This subtract 1 from the countdown
            _countdown.Signal();
        }
    }
}