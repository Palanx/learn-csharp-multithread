using System;
using System.Threading;

namespace signaling_constructs
{
    /*
     * The Program class contains logic to ensure that only one instance of the application
     * can run at a given time across the entire system. This is achieved using a
     * synchronization primitive called a Mutex (short for "mutual exclusion").
     * A Mutex is a synchronization tool that ensures that only one thread (or process,
     * in the case of a named system-wide mutex) can own it at any given time. It's
     * primarily used to protect shared resources so that concurrent access doesn't
     * produce undesirable effects.
     * In this specific application:
     * 1. An attempt is made to create and take ownership of a named Mutex called
     * Global/SignalingExamples.
     * 2. If the mutex is successfully created and owned, it indicates this is the first
     * instance of the application, and the application proceeds to wait for user input.
     * 3. If the mutex is already owned by another instance (determined by the createdNew
     * variable being false), the application terminates immediately, ensuring single-instance behavior.
     * 4. Before the first instance of the application terminates, it releases the mutex
     * to free up resources.
     *
     * In summary, the Mutex in this program acts as a gatekeeper. If the gate is already
     * claimed (by another instance of the app), any subsequent attempts to claim it will
     * fail, thus enforcing the rule of only one app instance running at a time.
     */
    internal class Program
    {
        // Declare a static Mutex variable to check/control the single instance.
        private static Mutex _instanceMutex;

        public static void Main(string[] args)
        {
            // Try to create a Mutex. The 'true' parameter indicates that this thread
            // intends to initially own the mutex if it's successfully created.
            // If the named mutex already exists, 'createdNew' will be set to false.
            _instanceMutex = new Mutex(true, @"Global/SignalingExamples", out var createdNew);

            // If 'createdNew' is false, it means another instance of the app is already running.
            if (!createdNew)
            {
                // Since this instance doesn't own the mutex, exit immediately without
                // trying to release the mutex.
                Environment.Exit(0);
            }

            // At this point, this is the first instance of the app and it owns the mutex.
            // Wait for user input.
            Console.Read();

            // Release the mutex before the application exits to clean up resources.
            // This is important to avoid possible deadlocks, although in this specific case
            // the OS would automatically release it when the app terminates.
            _instanceMutex.ReleaseMutex();
        }
    }
}