using System;
using System.Threading;
using System.Threading.Tasks;

namespace synchronization
{
    /// <summary>
    /// A semaphore is a synchronization primitive that controls access to a shared resource by
    /// maintaining a count of permits. Think of it as a bouncer at a club, allowing only a
    /// specific number of people to enter. If the club is full, new people have to wait until
    /// someone exits.
    /// </summary>
    public static class Semaphore
    {
        public static SemaphoreSlim Bouncer { get; set; }

        public static void TestSemaphore()
        {
            // Create the semaphore with 3 slots, where 3 are available
            Bouncer = new SemaphoreSlim(3, 3);

            OpenNightClub();

            Thread.Sleep(20000);
        }

        private static void OpenNightClub()
        {
            for (int i = 0; i < 50; i++)
            {
                // Let each guest enter on an own thread
                var number = i;
                Task.Run(() => Guest(number));
            }
        }

        private static void Guest(int guestNumber)
        {
            // Wait to enter the nightclub (a semaphore to be released)
            Console.WriteLine($"Guest {guestNumber} is waiting to entering nightclub");
            Bouncer.Wait();

            // Do some dancing
            Console.WriteLine($"Guest {guestNumber} is doing some dancing");
            Thread.Sleep(500);

            // Let one guest out (release one semaphore)
            Console.WriteLine($"Guest {guestNumber} is leaving the nightclub");
            Bouncer.Release(1);
        }
    }
}