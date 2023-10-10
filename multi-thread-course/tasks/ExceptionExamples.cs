using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace tasks
{
    public class ExceptionExamples
    {
        private static void AggregateExceptionSimpleExample()
        {
            Task t1 = Task.Run(() => throw new InvalidOperationException());

            try
            {
                t1.Wait();
            }
            /*
             * AggregateException it's a wrapper of the exceptions that happened
             * inside a hierarchy of thread, because calling an async function could
             * execute operations in different threads, so you need to deal with that.
             */
            catch (AggregateException ex)
            {
                // An inner exception could be of type AggregateException
                ReadOnlyCollection<Exception> hierarchicalExs = ex.InnerExceptions;

                // So it's a good idea to flatten the all hierarchy into a 1 level hierarchy, a flat list
                ReadOnlyCollection<Exception> flattenListExs = ex.Flatten().InnerExceptions;
                foreach (Exception exception in flattenListExs)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        private static void TestAggregateException()
        {
            Task parent = Task.Factory.StartNew(() =>
            {
                int[] numbers = { 0 };
                TaskFactory childFactory = new TaskFactory(TaskCreationOptions.AttachedToParent, TaskContinuationOptions.None);
                childFactory.StartNew(() => 5 / numbers[0]); // Division by zero
                childFactory.StartNew(() => numbers[1]); // Index out of range
                childFactory.StartNew(() => throw null); // Null reference
            });

            try
            {
                parent.Wait();
            }
            catch (AggregateException ex)
            {
                ex.Flatten().Handle(e =>
                {
                    switch (e)
                    {
                        case DivideByZeroException _: // the same as if (e is DivideByZeroException)
                            Console.WriteLine("Divide by zero");
                            return true;
                        case IndexOutOfRangeException _:
                            Console.WriteLine("Index out of range");
                            return true;
                        default:
                            return false;
                    }
                });
            }
        }

        /// <summary>
        /// In .NET, exceptions within a `Task` are considered unobserved if
        /// not accessed via `Task.Result` or `Task.Wait()`, or handled via
        /// `TaskScheduler.UnobservedTaskException`. From .NET 4.5, unobserved
        /// exceptions don't terminate the application on `Task` garbage collection.
        /// It's recommended to handle exceptions immediately where they occur or
        /// propagate them to a general handler for proper management.
        ///
        /// This is generally for Fire and Forget tasks, because if we have exceptions
        /// on those type of Task, no error will be thrown, and we won't know that a
        /// exception happened.
        /// </summary>
        private static void SubscribeToUnobservedExceptions()
        {
            TaskScheduler.UnobservedTaskException += HandleTaskExceptions;
        }

        private static void HandleTaskExceptions(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Console.WriteLine(sender is Task);
            foreach (Exception error in e.Exception.InnerExceptions)
            {
                Console.WriteLine(error.Message);
            }
            e.SetObserved();
        }
    }
}