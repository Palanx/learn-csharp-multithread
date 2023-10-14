using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace concurrent_collections
{
    public static class ConcurrentCollectionsExample
    {
        public static void QueueDemo()
        {
            ConcurrentQueue<string> names = new ConcurrentQueue<string>();
            // Enqueue in normal Queues is thread-safe, never will fails
            names.Enqueue("Bob");
            names.Enqueue("Alex");
            names.Enqueue("Rob");
            // But Peek and Dequeue is a different case in multi-thread

            Console.WriteLine($"After enqueuing, count = {names.Count}");

            bool success = names.TryDequeue(out string item1);
            if (success)
                Console.WriteLine($"\nRemoving {item1}");
            else
                Console.WriteLine("Queue was empty");

            success = names.TryPeek(out string item2);
            if (success)
                Console.WriteLine($"\nPeeking {item2}");
            else
                Console.WriteLine("Queue was empty");

            Console.WriteLine("Enumerating:");

            PrintOutCollection(names);

            Console.WriteLine($"\nAfter enumerating, count = {names.Count}");
        }

        public static void StackDemo()
        {
            ConcurrentStack<string> names = new ConcurrentStack<string>();
            // Push in normal Stack is thread-safe, never will fails
            names.Push("Bob");
            names.Push("Alex");
            names.Push("Rob");
            // But Peek and Pop is a different case in multi-thread

            Console.WriteLine($"After Pop, count = {names.Count}");

            bool success = names.TryPop(out string item1);
            if (success)
                Console.WriteLine($"\nRemoving {item1}");
            else
                Console.WriteLine("Stack was empty");

            success = names.TryPeek(out string item2);
            if (success)
                Console.WriteLine($"\nPeeking {item2}");
            else
                Console.WriteLine("Stack was empty");

            Console.WriteLine("Enumerating:");

            PrintOutCollection(names);

            Console.WriteLine($"\nAfter enumerating, count = {names.Count}");
        }

        /// <summary>
        /// ConcurrentBag doesn't guarantee any particular sequence of taking elements out.
        /// Absence of guarantees allowed to build ConcurrentBag in a way that in demonstrates
        /// very good performance.
        /// It keeps items in separate collections, one separate collection per thread.
        /// No thread synchronization at all in ConcurrentBag since there is no contention
        /// (dedicated collections).
        /// When "personal" collection is empty, a thread tries to remove an item from
        /// collection which belongs to another thread (steal an item).
        /// </summary>
        public static void BagDemo()
        {
            ConcurrentBag<string> names = new ConcurrentBag<string>();
            names.Add("Bob");
            names.Add("Alex");
            names.Add("Rob");

            Console.WriteLine($"After take, count = {names.Count}");

            bool success = names.TryTake(out string item1);
            if (success)
                Console.WriteLine($"\nRemoving {item1}");
            else
                Console.WriteLine("Bag was empty");

            success = names.TryPeek(out string item2);
            if (success)
                Console.WriteLine($"\nPeeking {item2}");
            else
                Console.WriteLine("Bag was empty");

            Console.WriteLine("Enumerating:");

            PrintOutCollection(names);

            Console.WriteLine($"\nAfter enumerating, count = {names.Count}");
        }

        /// <summary>
        /// In case of remove item in iteration you'll need to validate TryRemove() method to know if
        /// it returns false, to know that the remove operation was unsuccessful.
        /// </summary>
        public static void DictionaryDemo()
        {
            StockController controller = new StockController();
            TimeSpan workDay = new TimeSpan(0, 0, 1);
            Task t1 = Task.Run(() => new SalesManager("Bob").StartWork(controller, workDay));
            Task t2 = Task.Run(() => new SalesManager("Alice").StartWork(controller, workDay));
            Task t3 = Task.Run(() => new SalesManager("Rob").StartWork(controller, workDay));

            Task.WaitAll(t1, t2, t3);

            PrintOutCollection(controller.Stock);
        }

        public class StockController
        {
            readonly ConcurrentDictionary<string, int> _stock = new ConcurrentDictionary<string, int>();
            public ConcurrentDictionary<string, int> Stock => _stock;

            public void BuyBook(string item, int quantity)
            {
                _stock.AddOrUpdate(item, quantity, (key, oldValue) => oldValue + quantity);
            }

            public bool TryRemoveBookFromStock(string item)
            {
                if (_stock.TryRemove(item, out int val))
                {
                    Console.WriteLine($"How much was removed: {val}");
                    return true;
                }

                return false;
            }

            public bool TrySellBook(string item)
            {
                bool success = false;

                _stock.AddOrUpdate(item,
                    itemName => // When the key doesn't exist
                    {
                        /*
                         * Remember one extremely important this about concurrent dictionary
                         * lambdas that have been passed as argument to any mutating method
                         * can be called an unpredictable number of times, that means that all
                         * the side effects (success = false;) of lambdas have to be explicitly
                         * overriden.
                         */
                        success = false;
                        return 0;
                    },
                    (itemName, oldValue) => // When the key exist
                    {
                        if (oldValue == 0)
                        {
                            success = false;
                            return 0;
                        }

                        success = true;
                        return oldValue - 1;
                    });

                return success;
            }
        }

        public class SalesManager
        {
            public string Name { get; }

            public SalesManager(string id)
            {
                Name = id;
            }

            public void StartWork(StockController stockController, TimeSpan workDay)
            {
                Random rand = new Random((int)DateTime.UtcNow.Ticks);
                DateTime start = DateTime.UtcNow;
                while (DateTime.UtcNow - start < workDay)
                {
                    Thread.Sleep(rand.Next(50));
                    int generatedNumber = rand.Next(10);
                    bool shouldPurchase = generatedNumber % 2 == 0;
                    bool shouldRemove = generatedNumber == 9;
                    string itemName = RemoteBookStock.Books[rand.Next(RemoteBookStock.Books.Count)];

                    if (shouldPurchase)
                    {
                        int quantity = rand.Next(9) + 1;
                        stockController.BuyBook(itemName, quantity);
                        DisplayPurchase(itemName, quantity);
                    }
                    else if (shouldRemove)
                    {
                        stockController.TryRemoveBookFromStock(itemName);
                        DisplayRemoveAttempt(itemName);
                    }
                    else
                    {
                        bool success = stockController.TrySellBook(itemName);
                        DisplaySaleAttempt(success, itemName);
                    }
                }
            }

            private void DisplaySaleAttempt(bool success, string itemName)
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;
                Console.WriteLine(success
                ? $"Thread {threadId}: {Name} sold {itemName}"
                : $"Thread {threadId}: {Name}: Out of stock of {itemName}");
            }

            private void DisplayRemoveAttempt(string itemName)
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;
                Console.WriteLine($"Thread {threadId}: {Name} removed {itemName}");
            }

            private void DisplayPurchase(string itemName, int quantity)
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;
                Console.WriteLine($"Thread {threadId}: {Name} bought {quantity} of {itemName}");
            }
        }

        public static class RemoteBookStock
        {
            public static readonly List<string> Books =
                new List<string>
                {
                    "Clean Code",
                    "C# in Depth",
                    "C++ for Beginners",
                    "Design Patterns in C#",
                    "Marvel Heroes"
                };
        }

        private static void PrintOutCollection<T>(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                Console.WriteLine(item);
            }
        }

        private static void PrintOutCollection<T1, T2>(IEnumerable<KeyValuePair<T1,T2>> dictionary)
        {
            foreach (var item in dictionary)
            {
                Console.WriteLine($"{item.Key} - {item.Value}");
            }
        }
    }
}