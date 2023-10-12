using System;
using System.Threading.Tasks;

namespace async_await
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            ExceptionExample.TestASyncException();

            Console.ReadKey();
        }

        private void TestAsync()
        {
            CallerMethodAsync();

            Console.WriteLine("CallerMethodAsync was called!"); // 3
        }

        /// <summary>
        /// async give us the possibility of use await in the body, but this doesn't
        /// means that the function execution is async, the caller is blocked awaiting
        /// the completion of the function, so when a real async operation is awaited
        /// </summary>
        private static async Task CallerMethodAsync()
        {
            Console.WriteLine("Caller started."); // 1
            // Will execute the Task in a synchronous way until inside a real async function
            // is awaited.
            await CalleeMethodAsync();
            Console.WriteLine("Caller finished."); // 5
        }

        private static async Task CalleeMethodAsync()
        {
            Console.WriteLine("Callee started."); // 2
            /* This await is awaiting a real async operation, so will return the
             * control to the CallerMethodAsync(), the CallerMethodAsync() await will
             * wait to the CalleeMethodAsync() completion, so it will return the control
             * to the Main()
            */
            await Task.Delay(1000); // simulates some asynchronous work
            Console.WriteLine("Callee finished."); // 4
        }
    }
}