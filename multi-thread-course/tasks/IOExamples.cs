using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace tasks
{
    public class IOExamples
    {
        private const string FilePath = @"C:/tmp/hello-world.txt";

        private static void TestTaskWrite()
        {
            FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 8, true);

            string content = "Bla bla bla mister freeman, bla bla bla.";
            byte[] data = Encoding.Unicode.GetBytes(content);

            Task task = fs.WriteAsync(data, 0, data.Length);
            task.ContinueWith(t =>
            {
                fs.Close();
                Console.WriteLine($"Write completed");
                TestTaskRead();
            });
        }

        private static void TestTaskRead()
        {
            FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None, 8, true);

            byte[] data = new byte[1024];

            Task<int> readTask = fs.ReadAsync(data, 0, data.Length);
            readTask.ContinueWith(t =>
            {
                fs.Close();
                string content = Encoding.Unicode.GetString(data, 0, t.Result);
                Console.WriteLine($"Read completed, the content is {content}");
            });
        }

        /// <summary>
        /// The Task.Factory.FromAsync method is used to create a Task that represents a pair of begin and end methods
        /// that conform to the Asynchronous Programming Model (APM) pattern. This pattern was used extensively in .NET
        /// before the introduction of the async and await keywords.
        /// With this you can create async functions with sync functions.
        /// </summary>
        public static void TestFromAsyncWriteAndRead()
        {
            FileStream fs = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, 8, true);

            string content = "Bla bla bla mister freeman, bla bla bla.";
            byte[] buffer = Encoding.Unicode.GetBytes(content);

            Task writeChunk = Task.Factory.FromAsync(fs.BeginWrite, fs.EndWrite, buffer, 0, buffer.Length, null);
            writeChunk.ContinueWith(tw =>
            {
                fs.Position = 0;
                byte[] data = new byte[buffer.Length];
                Task<int> readChunk = Task<int>.Factory.FromAsync(fs.BeginRead, fs.EndRead, data, 0, data.Length, 0);
                readChunk.ContinueWith(tr =>
                {
                    string result = Encoding.Unicode.GetString(data, 0, tr.Result);
                    Console.WriteLine(result);
                });
            });
        }
    }
}