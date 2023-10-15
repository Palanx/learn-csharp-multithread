using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace parallel_programming
{
    public static class ParallelExample
    {
        private static void RunLoop()
        {
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine($"ThreadID:{Thread.CurrentThread.ManagedThreadId}; Iteration:{i}");
            }
        }

        private static IEnumerable<int> RunEnumerableLoop()
        {
            for (int i = 0; i < 100; i++)
            {
                yield return i;
            }
        }

        public static void Test()
        {
            /*
             * Run 2 functions at the same time in different threads and wait for both
             * completion.
             */
            Parallel.Invoke(RunLoop, RunLoop);


            /*
             * The code you provided isn't thread-safe due to concurrent access to the
             * data list, which is not inherently thread-safe. Specifically, List<T> is
             * not designed to handle simultaneous modifications from multiple threads.
             *
             * Here's a breakdown of the problems:
             * 1. Internal Array Resizing: List<T> internally uses an array to store its
             * elements. When elements are added beyond its current capacity, it resizes
             * this internal array. If two threads attempt to resize the internal array
             * simultaneously or if one thread is reading while another one is resizing,
             * it can lead to undefined behavior or exceptions.
             * 2. Race Conditions: Even if you were just adding items without triggering
             * a resize, two threads could still conflict in terms of which one gets to
             * add its item next, leading to race conditions.
             * 3. Index Corruption: Since the Add method (and by extension, AddRange)
             * modifies the internal index of the last item, concurrent additions can lead
             * to index corruption where some items might be overwritten or skipped.
             */
            List<int> data = new List<int>();
            Parallel.Invoke(() => data.AddRange(RunEnumerableLoop()),
                () => data.AddRange(RunEnumerableLoop()));

            // Parallel iteration between 1 and 10
            Parallel.For(1, 11, i => { Console.WriteLine($"{i * i * i}"); });

            // You can add options to "how" execute the parallel operation
            ParallelOptions po = new ParallelOptions();
            po.MaxDegreeOfParallelism = 8; // It's common to use the max number of cores

            // process word per word
            string sentence =
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cras non semper quam. Nam sagittis ante non efficitur dignissim. Nunc non dapibus tortor. Praesent sodales eros lobortis neque congue hendrerit. Aliquam nibh mauris, hendrerit eget purus sit amet, lacinia vulputate quam. Fusce vel sodales arcu. Donec feugiat ex vitae dolor rhoncus egestas. Aenean consequat erat eget nibh lacinia imperdiet. Quisque volutpat lectus a odio lacinia, tincidunt lacinia libero facilisis. Donec facilisis tellus vel nisl imperdiet condimentum.";
            string[] words = sentence.Split(' ');
            Parallel.ForEach(words, word =>
            {
                Console.WriteLine($"\"{word}\" is of {word.Length} length = thread {Thread.CurrentThread.ManagedThreadId}");
            });
        }
    }
}