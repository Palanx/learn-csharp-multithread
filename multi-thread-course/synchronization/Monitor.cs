using System;
using System.Threading;

namespace synchronization
{
    /// <summary>
    /// Lock make another threads that are trying to execute the same locked section to await
    /// until that the current thread that locked the critical section exit the lock.
    /// </summary>
    public static class Monitor
    {
        private class BankCard
        {
            private decimal _moneyAmount;
            /*
             * It's a dummy to ensures that the lock is not contended by other unrelated
             * code that might be locking on the same public object or another commonly
             * used object.
             */
            private readonly object _sync = new object();

            public BankCard(decimal moneyAmount)
            {
                _moneyAmount = moneyAmount;
            }

            public void ReceivePayment(decimal amount)
            {
                /*
                 * This is the old fashion way to use Monitor (lock)
                 */
                bool lockTaken = false;
                try
                {
                    System.Threading.Monitor.Enter(_sync, ref lockTaken);
                    _moneyAmount += amount;
                }
                finally
                {
                    if (lockTaken)
                        System.Threading.Monitor.Exit(_sync);
                }
            }

            public void ReceivePaymentUsingKey(decimal amount)
            {
                /*
                 * It's the same than ReceivePayment, the only difference is that you can
                 * add a Timeout with the lock key.
                 */
                lock (_sync)
                {
                    _moneyAmount += amount;
                }
            }

            public void TransferToCard(decimal amount, BankCard recipient)
            {
                /*
                 * We made an extension to create a lock with timeout and Exit when the
                 * dummy sync object is destroyed.
                 */
                using (_sync.Lock(TimeSpan.FromSeconds(3)))
                {
                    _moneyAmount -= amount;
                    recipient.ReceivePayment(amount);
                }
            }
        }
    }

    public static class LockExtensions
    {
        public static Lock Lock(this object obj, TimeSpan timeout)
        {
            bool lockTaken = false;
            try
            {
                System.Threading.Monitor.TryEnter(obj, timeout, ref lockTaken);
                if (lockTaken) return new Lock(obj);

                throw new TimeoutException("Failed to acquire sync object");
            }
            catch
            {
                if (lockTaken)
                    System.Threading.Monitor.Exit(obj);
                throw;
            }
        }
    }

    public readonly struct Lock : IDisposable
    {
        private readonly object _obj;

        public Lock(object obj)
        {
            _obj = obj;
        }

        public void Dispose()
        {
            System.Threading.Monitor.Exit(_obj);
        }
    }
}