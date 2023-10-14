using System;
using System.Collections;
using System.Collections.Generic;

namespace concurrent_collections
{
    /// <summary>
    /// An interface for an immutable stack.
    /// Immutable means the data structure cannot be altered once created.
    /// Instead of altering the original structure, every operation returns a new modified version of the stack.
    /// </summary>
    /// <typeparam name="T">The type of elements in the stack.</typeparam>
    public interface IStack<T> : IEnumerable<T>
    {
        IStack<T> Push(T value);
        IStack<T> Pop();
        T Peek();
        bool IsEmpty { get; }
    }

    public sealed class Stack<T> : IStack<T>
    {
        /// <summary>
        /// A private implementation of an empty stack.
        /// This allows for the creation of an immutable stack with a sentinel empty node.
        /// </summary>
        private sealed class EmptyStack : IStack<T>
        {
            public bool IsEmpty => true;

            public IStack<T> Push(T value)
            {
                return new Stack<T>(value, this);
            }

            public IStack<T> Pop()
            {
                throw new Exception("Empty stack");
            }

            public T Peek()
            {
                throw new Exception("Empty stack");
            }

            public IEnumerator<T> GetEnumerator()
            {
                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        // Singleton empty stack instance. This is shared among all instances.
        private static readonly EmptyStack empty = new EmptyStack();

        public static IStack<T> Empty => empty;

        private readonly T _head;  // The top item of the stack
        private readonly IStack<T> _tail;  // The rest of the stack after the top item

        public bool IsEmpty => false;

        private Stack(T head, IStack<T> tail)
        {
            _head = head;
            _tail = tail;
        }

        public IStack<T> Push(T value)
        {
            // Return a new stack with the added item as the head, and the current stack as its tail.
            return new Stack<T>(value, this);
        }

        public IStack<T> Pop()
        {
            // Return the tail, effectively removing the head.
            return _tail;
        }

        public T Peek()
        {
            return _head;
        }

        public IEnumerator<T> GetEnumerator()
        {
            // Yield each item in the stack for enumeration.
            for (IStack<T> stack = this; !stack.IsEmpty; stack = stack.Pop())
                yield return stack.Peek();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    /// <summary>
    /// A class to demonstrate the usage of the custom immutable stack.
    /// </summary>
    public class CustomImmutableStackExample
    {
        public void Test()
        {
            // Create empty stack
            IStack<int> stack1 = Stack<int>.Empty;

            // Push 10 onto the stack
            IStack<int> stack2 = stack1.Push(10);

            // Push 20 onto the stack, above 10
            IStack<int> stack3 = stack2.Push(20);

            // Display all items in the stack.
            foreach (var cur in stack3)
            {
                Console.WriteLine(cur);
            }
        }
    }
}
