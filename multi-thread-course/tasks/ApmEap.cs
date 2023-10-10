using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace tasks
{
    /// <summary>
    ///  APM = Asynchronous Programing Model
    /// </summary>
    public class ApmEap
    {
        public static void TestEap()
        {
            WebClient wc = new WebClient();

            // This doesn't use threads, is a I/O-based pure operation
            Task<byte[]> task = wc.DownloadDataTaskAsync(new Uri("http://www.engineerspock.com"));
            // This use a working thread from the thread pool, is required to process the result
            // of the I/O async operation result
            task.ContinueWith(t => Console.WriteLine(Encoding.UTF8.GetString(t.Result)));

            Console.ReadKey();
        }
    }
}