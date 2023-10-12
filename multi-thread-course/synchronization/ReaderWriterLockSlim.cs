using System;
using System.Threading;

namespace synchronization
{
    /// <summary>
    /// This is an example of how to use locks but just for write or read, it's optimal.
    /// </summary>
    public class ReaderWriterLockSlim
    {
        private class BankCard
        {
            private decimal _moneyAmount;
            private decimal _credit;
            private readonly object _sync = new object();
            private System.Threading.ReaderWriterLockSlim _rwLock = new System.Threading.ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

            public decimal TotalMoneyAmount
            {
                get
                {
                    using (_rwLock.TakeReaderLock(TimeSpan.FromSeconds(3)))
                    {
                        return _moneyAmount + _credit;
                    }
                }
            }

            public BankCard(decimal moneyAmount)
            {
                _moneyAmount = moneyAmount;
            }

            public void ReceivePayment(decimal amount)
            {
                _rwLock.EnterWriteLock();
                try
                {
                    _moneyAmount += amount;
                }
                finally
                {
                    _rwLock.ExitWriteLock();
                }
            }

            public void TransferToCard(decimal amount, BankCard recipient)
            {
                _rwLock.EnterWriteLock();
                try
                {
                    _moneyAmount -= amount;
                }
                finally
                {
                    _rwLock.ExitWriteLock();
                }

                recipient.ReceivePayment(amount);
            }
        }
    }

    public static class ReaderWriterLockSlimExtensions
    {
        public static ReaderLockSlimWrapper TakeReaderLock(this System.Threading.ReaderWriterLockSlim rwl, TimeSpan timeout)
        {
            bool lockTaken = false;
            try
            {
                lockTaken = rwl.TryEnterReadLock(timeout);
                if (lockTaken) return new ReaderLockSlimWrapper(rwl);

                throw new TimeoutException("Failed to acquire ReaderWriterLockSlim control");
            }
            catch
            {
                if (lockTaken)
                    rwl.ExitReadLock();
                throw;
            }
        }

        public static WriterLockSlimWrapper TakeWriterLock(this System.Threading.ReaderWriterLockSlim rwl, TimeSpan timeout)
        {
            bool lockTaken = false;
            try
            {
                lockTaken = rwl.TryEnterWriteLock(timeout);
                if (lockTaken) return new WriterLockSlimWrapper(rwl);

                throw new TimeoutException("Failed to acquire ReaderWriterLockSlim control");
            }
            catch
            {
                if (lockTaken)
                    rwl.ExitWriteLock();
                throw;
            }
        }
    }

    public readonly struct ReaderLockSlimWrapper : IDisposable
    {
        private readonly System.Threading.ReaderWriterLockSlim _rwl;

        public ReaderLockSlimWrapper(System.Threading.ReaderWriterLockSlim rwl)
        {
            _rwl = rwl;
        }

        public void Dispose()
        {
            _rwl.ExitReadLock();
        }
    }

    public readonly struct WriterLockSlimWrapper : IDisposable
    {
        private readonly System.Threading.ReaderWriterLockSlim _rwl;

        public WriterLockSlimWrapper(System.Threading.ReaderWriterLockSlim rwl)
        {
            _rwl = rwl;
        }

        public void Dispose()
        {
            _rwl.ExitWriteLock();
        }
    }
}