using System;
using System.Threading.Tasks;

namespace async_await
{
    public static class ExceptionExample
    {
        public static async void TestASyncException()
        {
            Task<int[]> allTask = Task.WhenAll(DivideByZero(), OutOfRange());

            try
            {
                /*
                 * As the thread pool task executions, the exceptions in the tasks won't
                 * be observer, so will be re-thrown in any synchronization context at any time
                 * causing a crash, so we need to await them to be able to re-thrown the exceptions
                 * in this context.
                 */
                await allTask;
                // It's the same effect with tasks executed in another thread, await will re-throw
                // the exceptions.
            }
            // this just will return the first exception of the stack so we'll use the tasks inner
            // exceptions.
            catch (Exception e)
            {
                foreach (Exception ex in allTask.Exception.InnerExceptions)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        private static Task<int> DivideByZero()
        {
            int[] numbers = new[] { 0 };
            return Task.FromResult(5 / numbers[0]);
        }

        private static Task<int> OutOfRange()
        {
            int[] numbers = new[] { 0 };
            return Task.FromResult(numbers[1]);
        }
    }
}