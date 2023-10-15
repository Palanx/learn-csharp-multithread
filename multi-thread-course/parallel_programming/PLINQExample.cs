using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace parallel_programming
{
    public static class PLINQExample
    {
        public static void Demo()
        {
            IEnumerable<int> numbers = Enumerable.Range(3, 10000 - 3);
            // Calling AsOrder() is the only way to preserve the order, because the
            // outcome can be come in any order.
            ParallelQuery<int> parallelQuery = from n in numbers.AsParallel().AsOrdered()
                where Enumerable.Range(2, (int)Math.Sqrt(n)).All(i => n % i > 0)
                select n;

            int[] primes = parallelQuery.ToArray();



            primes.ToList().AsParallel() // Wraps sequence in ParallelQuery<int>
                .Where(n => n > 100) // Outputs another ParallelQuery<int>
                .AsParallel()           // Unnecessary = and inefficient!
                .Select(n => n * n);

            /*
             * Take, TakeWhile, SKip, SkipWhile, indexed Select, SelectManu, ElementAt
             * prevent a query from being parallelized, unless the source elements
             * are in there original indexing position.
             */

            /*
             * Join, GroupBy, GroupJoin, Distinct, Union, Intersect, Except
             * can be parallelized, but they use an expensive partitioning strategy
             * that can some times be slower than a sequential processing.
             */

            // Make 6 pings at the same time
            var result = from site in new[]
                {
                    "www.engineerspock.com",
                    "www.udemy.com",
                    "www.reddit.com",
                    "www.facebook.com",
                    "www.stackoverflow.com",
                    "www.proralsight.com"
                }.AsParallel().WithDegreeOfParallelism(6) // WithDegreeOfParallelism forces to execute in X concurrent tasks
                let p = new System.Net.NetworkInformation.Ping().Send(site)
                select new
                {
                    site,
                    Result = p.Status,
                    Time = p.RoundtripTime
                };
        }

        public static void CancellaingParallelAndPLINQDemo()
        {
            List<CreditCard> cards = new List<CreditCard>()
            {
                new CreditCard() { Liabilities = 1200, Id = 1 },
                new CreditCard() { Liabilities = 80, Id = 2 },
                new CreditCard() { Liabilities = 1100, Id = 3 },
                new CreditCard() { Liabilities = 100, Id = 4 },
                new CreditCard() { Liabilities = 3000, Id = 5 },
                new CreditCard() { Liabilities = 800, Id = 6 },
                new CreditCard() { Liabilities = 1450, Id = 7 }
            };

            CancellationTokenSource cts = new CancellationTokenSource();

            Task task = Task.Run(() =>
            {
                try
                {
                    // With parallel body we need to break the execution by us under a logic.
                    Parallel.ForEach(cards, (curItem, loopState, curIdx) =>
                    {
                        if (new Random().Next() % 2 == 0)
                        {
                            loopState.Break();
                        }

                        if (curItem.Liabilities > 1000)
                        {
                            curItem.Block(cts.Token);
                        }
                    });

                    // Parallel.ForEach(cards, new ParallelOptions() { CancellationToken = cts.Token },
                    //     x =>
                    //     {
                    //         if (x.Liabilities > 1000)
                    //         {
                    //             x.Block(cts.Token);
                    //         }
                    //     });

                    // cards.AsParallel().WithCancellation(cts.Token)
                    //     .ForAll(x =>
                    //     {
                    //         if (x.Liabilities > 1000)
                    //         {
                    //             x.Block(cts.Token);
                    //         }
                    //     });
                }
                catch (OperationCanceledException e)
                {
                    Console.WriteLine("Operation cancelled");
                }
            });
            Thread.Sleep(1500);
            Console.WriteLine("Canceling");

            /* It doesn't instantly cancel all the operations, just  it'll stop new iterations but
             * the current operations will continue until completion, unless you define a cancellation
             * logic inside the operation, like in this example.
             */
            cts.Cancel();
        }

        public class CreditCard
        {
            public decimal Liabilities { get; set; }
            public int Id { get; set; }

            public void Block(CancellationToken ct)
            {
                bool blocked = false;
                for (int i = 0; i < 3; i++)
                {
                    ct.ThrowIfCancellationRequested();


                    // connecting to a server
                    Console.WriteLine($"Connecting {Id}. Iteration:{i}");
                    Thread.Sleep(1000);

                    // idiotic condition
                    if (i == 3)
                    {
                        blocked = true;
                    }
                }
                if (blocked)
                    Console.WriteLine($"Blocked credit card. ID:{Id}");
            }
        }
    }
}