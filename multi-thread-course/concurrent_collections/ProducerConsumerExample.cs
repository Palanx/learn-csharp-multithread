using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace concurrent_collections
{
    /// <summary>
    /// Bounded Capacity: You can set a maximum limit to the number of items the collection
    /// can hold.
    /// Blocking on Add and Take: If you try to Take an item from an empty collection,
    /// the call will block until there's an item available. If you try to Add an item
    /// to a full collection (when you've set a bounded capacity), the call will block
    /// until there's space available.
    /// </summary>
    public class ProducerConsumerExample
    {
        private readonly BlockingCollection<string> _cutleryToWash =
            new BlockingCollection<string>(new ConcurrentStack<string>(), 10);

        private readonly List<string> _cutlery = new List<string>()
        {
            "Fork",
            "Spoon",
            "Plate",
            "Knife"
        };

        private readonly Random _random = new Random();

        public static void Test()
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            ProducerConsumerExample pce = new ProducerConsumerExample();
            Task.Run(() => pce.Run(cts.Token));

            Console.Read();
            cts.Cancel();
            Console.WriteLine("End of processing");
        }

        private void Run(CancellationToken ct)
        {
            Task t1 = Task.Run(() => Eat(ct), ct);
            Task t2 = Task.Run(() => Wash(ct), ct);

            try
            {
                Task.WaitAll(t1, t2);
            }
            catch (AggregateException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // Producer
        private void Eat(CancellationToken ct)
        {
            while (true)
            {
                ct.ThrowIfCancellationRequested();

                string nextCutlery = _cutlery[_random.Next(3)];
                _cutleryToWash.Add(nextCutlery, ct);
                Console.WriteLine($"+ {nextCutlery}");
                Thread.Sleep(500);
            }
            /*
             * We have a consumer task that keeps taking items. It will block if there's no item
             * to take until the producer adds an item or completes adding
             * (_cutleryToWash.CompleteAdding();).
             */
        }

        // Consumer
        private void Wash(CancellationToken ct)
        {
            foreach (string item in _cutleryToWash.GetConsumingEnumerable())
            {
                ct.ThrowIfCancellationRequested();
                Console.WriteLine($"- {item}");
                Thread.Sleep(3000);
            }
        }
    }
}