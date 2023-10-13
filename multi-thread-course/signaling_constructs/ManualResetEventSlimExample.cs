using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace signaling_constructs
{
    public static class ManualResetEventSlimExample
    {
        public static void Test()
        {
            BankTerminal bk = new BankTerminal(new IPEndPoint(new IPAddress(0x2414188f), 8080));

            Task purchaseTask = bk.Purchase(100);
            purchaseTask.ContinueWith(x =>
            {
                Console.WriteLine("Operation is done!");
            });
        }

        public class BankTerminal
        {
            private readonly Protocol _protocol;
            /*
             * With false, by default the state of the event is non-signaled, so a Wait will block
             * the execution.
             */
            private readonly ManualResetEventSlim _operationSignal = new ManualResetEventSlim(false);

            public BankTerminal(IPEndPoint endPoint)
            {
                _protocol = new Protocol(endPoint);
                _protocol.OnMessageReceived += OnMessageReceived;
            }

            private void OnMessageReceived(object sender, ProtocolMessage e)
            {
                if (e.Status == OperationStatus.Finished)
                {
                    Console.WriteLine("Signaling!");

                    // Set the state to signaled allow threads that are waiting to proceed
                    _operationSignal.Set();
                }
            }

            public Task Purchase(decimal amount)
            {
                return Task.Run(() =>
                {
                    const int purchaseOpCode = 1;
                    _protocol.Send(purchaseOpCode, amount);

                    /*
                     * Set the state of event to non-signaled, we made this here
                     * because the Set() operation is slower than Reset(), so,
                     * if we made the reset after the Set(), for a moment
                     * some threads could reach the Wait() and not be blocked.
                     */
                    _operationSignal.Reset();

                    Console.WriteLine("Waiting for the signal");

                    // If the state of event is unsignaled, this will block the execution
                    _operationSignal.Wait();
                });
            }
        }

        public class Protocol
        {
            private readonly IPEndPoint _endPoint;

            public event EventHandler<ProtocolMessage> OnMessageReceived;

            public Protocol(IPEndPoint endPoint)
            {
                _endPoint = endPoint;
            }

            public void Send(int opCode, object parameters)
            {
                Task.Run(() =>
                {
                    // Emulating interoperation with a bank terminal device
                    Console.WriteLine("Operation is in action");
                    Thread.Sleep(3000);

                    OnMessageReceived?.Invoke(this, new ProtocolMessage(OperationStatus.Finished));
                });
            }
        }

        public class ProtocolMessage
        {
            public OperationStatus Status { get; }

            public ProtocolMessage(OperationStatus status)
            {
                Status = status;
            }
        }

        public enum OperationStatus
        {
            Finished,
            Faulted
        }
    }
}