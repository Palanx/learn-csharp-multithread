using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace concurrent_collections
{
    public static class ImmutableCollectionsExample
    {
        public static void BuildImmutableCollectionsDemo()
        {
            List<int> largeList = GetLargeList(10000);

            ImmutableList<int>.Builder builder = ImmutableList.CreateBuilder<int>();
            foreach (var item in largeList)
            {
                builder.Add(item);
            }
            // builder.AddRange(largeList);

            ImmutableList<int> immutableList = builder.ToImmutable();
            //ImmutableList<int> immutableList = largeList.ToImmutableList();
        }

        private static List<int> GetLargeList(int amount)
        {
            List<int> largeList = new List<int>(amount);
            for (int i = 0; i < largeList.Count; i++)
            {
                largeList[i] = i;
            }

            return largeList;
        }

        public static void StackDemo()
        {
            ImmutableStack<int> stack = ImmutableStack<int>.Empty;
            stack = stack.Push(1);
            stack = stack.Push(2);

            PrintOutCollection(stack);

            int last = stack.Peek();
            Console.WriteLine($"Last item: {last}");

            stack = stack.Pop(out last);
            Console.WriteLine($"Last item: {last}; Last after Pop: {stack.Peek()}");
        }

        public static void QueueDemo()
        {
            ImmutableQueue<int> queue = ImmutableQueue<int>.Empty;
            queue = queue.Enqueue(1);
            queue = queue.Enqueue(2);

            PrintOutCollection(queue);

            int first = queue.Peek();
            Console.WriteLine($"First item: {first}");

            queue = queue.Dequeue(out first);
            Console.WriteLine($"First item: {first}; Next after Dequeue: {queue.Peek()}");
        }

        public static void ListDemo()
        {
            ImmutableList<int> list = ImmutableList<int>.Empty;
            list = list.Add(2);
            list = list.Add(3);
            list = list.Add(4);
            list = list.Add(5);

            PrintOutCollection(list);

            Console.WriteLine("Remove 4 and then RemoveAt index=2");
            list = list.Remove(4);
            list = list.RemoveAt(2);

            PrintOutCollection(list);

            Console.WriteLine("Insert 1 at 0 and 4 at 3");
            list = list.Insert(0, 1);
            list = list.Insert(3, 4);

            PrintOutCollection(list);
        }

        public static void HashSetDemo()
        {
            ImmutableHashSet<int> hashSet = ImmutableHashSet<int>.Empty;
            hashSet = hashSet.Add(5);
            hashSet = hashSet.Add(10);

            // Displays 5 and 10 in an unpredictable order (at least in multi-thread scenarios)
            PrintOutCollection(hashSet);

            Console.WriteLine("Remove 5");
            hashSet = hashSet.Remove(5);

            PrintOutCollection(hashSet);
        }

        public static void SortedSetDemo()
        {
            ImmutableSortedSet<int> sortedSet = ImmutableSortedSet<int>.Empty;
            sortedSet = sortedSet.Add(10);
            sortedSet = sortedSet.Add(5);

            PrintOutCollection(sortedSet);

            int smallest = sortedSet[0];
            Console.WriteLine($"Smallest item: {smallest}");

            Console.WriteLine("Remove 5");
            sortedSet = sortedSet.Remove(5);

            PrintOutCollection(sortedSet);
        }

        public static void DictionaryDemo()
        {
            // ImmutableSortedDictionary<> exists but it's a little bit slower
            ImmutableDictionary<int,string> dictionary = ImmutableDictionary<int, string>.Empty;
            dictionary = dictionary.Add(1, "John");
            dictionary = dictionary.Add(2, "Alex");
            dictionary = dictionary.Add(3, "April");

            // Displays the items in an unpredictable order (in multi-thread context)
            foreach (var item in dictionary)
            {
                Console.WriteLine($"{item.Key} - {item.Value}");
            }

            Console.WriteLine("Changing value of key 2 to Bob");
            dictionary = dictionary.SetItem(2, "Bob");

            PrintOutCollection(dictionary);

            var april = dictionary[3];
            Console.WriteLine($"Who is at key 3 = {april}");

            Console.WriteLine("Remove record where key = 2");
            dictionary = dictionary.Remove(2);

            PrintOutCollection(dictionary);
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