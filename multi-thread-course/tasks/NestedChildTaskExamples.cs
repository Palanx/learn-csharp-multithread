using System;
using System.Threading.Tasks;

namespace tasks
{
    public class NestedChildTaskExamples
    {
        /// <summary>
        /// It a task that runs inside a tasks but not necessary need to be completed
        /// to complete the main task.
        /// </summary>
        public void NestedTaskExample()
        {
            Task.Factory.StartNew(() =>
            {
                Task nested = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("hello world");
                });
            }).Wait();
            // We wait to the parent completion, but the nested task will be completed
            // after the application finished.
        }

        /// <summary>
        /// A child task is a task that runs in a parent task and all the child
        /// tasks needs to be completed to complete the parent.
        /// a child task is a nested task by itself.
        /// </summary>
        public void ChildTaskExample()
        {
            Task.Factory.StartNew(() =>
            {
                Task child = Task.Factory.StartNew(() =>
                {
                    Console.WriteLine("hello world");
                }, TaskCreationOptions.AttachedToParent);
            }).Wait();

            // Now the child task need to be completed to complete the parent, so
            // the Wait will wait the child task.
        }
    }
}