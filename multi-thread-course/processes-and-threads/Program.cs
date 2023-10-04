using System;
using System.Diagnostics;
using System.Threading;

namespace oricesses_and_threads
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            PrintNumbersUsingThread();

            Console.Read();
        }

        #region Old Thread Example
        private static void PrintNumbersUsingThread()
        {
            Console.WriteLine("Even:");
            Thread t1 = new Thread(PrintNumbers);
            t1.IsBackground = true; // If the app stops all the background threads will stop
            t1.Start(true);
            t1.Priority = ThreadPriority.Highest;

            if (t1.Join(TimeSpan.FromSeconds(5))) // Block the calling thread (main in this case) until time-out or thread terminates
            {
                Console.WriteLine("Thread terminates at time!");
            }
            else
            {
                Console.WriteLine("Thread time-out!");
            }

            Console.WriteLine("Odd:");
            PrintNumbers(false);
        }

        private static void PrintNumbers(object arg)
        {
            Console.WriteLine($"Current Thread ID: {Thread.CurrentThread.ManagedThreadId}");
            bool isEven = (bool) arg;
            for (int i = 0; i < 100; i++)
            {
                if ((i%2 == 0) != isEven) continue;

                Console.WriteLine(i);
            }
        }
        #endregion Old Thread Example

        #region Process Example
        private static void OpenAndCloseNotepad()
        {
            Process notepadApp = OpenNotepad();
            KillProcessByName(notepadApp.ProcessName);
        }

        private static Process OpenNotepad()
        {
            Process.Start("notepad.exe");

            Process app = new Process()
            {
                StartInfo =
                {
                    FileName = @"notepad.exe",
                    Arguments = @"C:/tmp/hello-world.txt"
                }
            };
            app.Start();

            app.PriorityClass = ProcessPriorityClass.RealTime;
            return app;
        }

        private static void WaitProcessForExit(Process process)
        {
            process.WaitForExit();
            Console.WriteLine("Process closed!");
        }

        private static void KillProcessByName(string name)
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                if (process.ProcessName != name) continue;

                process.Kill();
            }
        }
        #endregion Process Example
    }
}