# learn-csharp-multithread
Repo just to learn Multi-Thread in C#

## Project <b>processes-and-threads</b>: Thread API, Thread Pool, APM & EAP
There is a [Link](https://www.udemy.com/course/parallel-csharp/learn/lecture/11126220) to the course.
- How to start a process, how to chanfe its priority, kill a process, query the OS for existing process and such.
- API of the old-school Thread class.
- The relation of thread and a very old COM technology.
- What is a thread pool and how to use thread pool threads?
- Two types of thread pool threads: workers and I/O.
- I/O-bound operations in [detail](https://blog.stephencleary.com/2013/11/there-is-no-thread.html).

## Project <b>tasks</b>: Tasks
There is a [Link](https://www.udemy.com/course/parallel-csharp/learn/lecture/11126260) to the course.
- How to create a task.
- States of a task.
- Canceling tasks.
- Chaining tasks.
- Waiting for tasks.
- Creating an I/O-bound task.
- Handling exceptions.
- Nested and Child tasks.
- Using TaskCompletionSource.

## Project <b>async_await</b>: Async & Await
There is a [Link](https://www.udemy.com/course/parallel-csharp/learn/lecture/11126304) to the course.
- What is async & await.
- How to use async & await.
- How to handle exceptions with async & await

## Project <b>synchronization</b>: Synchronization in Multithread Scenarios
There is a [Link](https://www.udemy.com/course/parallel-csharp/learn/lecture/11126328) to the course.
- Shared Resource.
- Atomicity.
- Interlocked class to defend primitives from concurrent access.
- Monitor class to acquire exclusive locks for accessing a shared resource.
- ReaderWriterLock for optimizing code which is responsible for managing concurrent access in a certain scenario.
- Semaphore synchronization construct to limit the number of threads which can have access to a particular resource simultaniously.
- What is SynchronizationContext and what role it plays in UI-apps.
- What is a deadlock and why is it so scary.

## Project <b>signaling_constructs</b>: Signaling Constructs
There is a [Link](https://www.udemy.com/course/parallel-csharp/learn/lecture/11165354) to the course.
- AutoResetEvent and ManualResetEventSlim.
- CountdownEvent for waiting several threads.
- Barrier for syncronizing threads in phases.
- Mutex to allow no more than once instace of an application to be launched.

## Project <b>spinning</b>: Spinning
There is a [Link](https://www.udemy.com/course/parallel-csharp/learn/lecture/11165390) to the course.
- The difference between blocking and spinning.
- The difference between SpinLock and SpinWait.
- How SpinWait and SpinLocl work and when to use them.
- Implement our awn synchronization primitive.

## Project <b>concurrent_collections</b>: Concurrent Collections
There is a [Link](https://www.udemy.com/course/parallel-csharp/learn/lecture/11165408) to the course.
- Implementing an immutable stack.
- ImmutableStack, ImmutableQueue, ImmutableList, ImmutableSet, and ImmutableDictionary.
- Performance characteristics of immutable collections.
- Using builders of immutable collections.
- ConcurrentStack, ConcurrentQueue, ConcurrentBag and ConcurrentDictionary.
- BlockingCollection and Produce/Cosnume.

## Project <b>parallel_programming</b>: Parallel Programming
There is a [Link](https://www.udemy.com/course/parallel-csharp/learn/lecture/11165448) to the course.
- How to use Parallel cass calling For, Foreach and Invoke.
- How to use PLINQ to rin LINQ queries in parallel.
- How to cancel units of work shich run in parallel by PLINQ or Parallel.