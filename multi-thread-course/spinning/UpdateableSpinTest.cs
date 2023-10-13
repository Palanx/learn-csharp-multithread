using System;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace spinning
{
    [TestFixture]
    public class UpdateableSPpinTest
    {
        [Test]
        public void Wait_NoPulse_ReturnsFalse()
        {
            UpdateableSpin spin = new UpdateableSpin();
            bool wasPulsed = spin.Wait(TimeSpan.FromMilliseconds(10));
            Assert.IsFalse(wasPulsed);
        }

        [Test]
        public void Wait_Pulse_ReturnsTrue()
        {
            UpdateableSpin spin = new UpdateableSpin();

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                spin.Set();
            });
            bool wasPulsed = spin.Wait(TimeSpan.FromSeconds(10));
            Assert.IsTrue(wasPulsed);
        }

        [Test]
        public void Wait20Millisec_CallIsActuallyWaitingFor50Millisec()
        {
            UpdateableSpin spin = new UpdateableSpin();

            Stopwatch watcher = new Stopwatch();
            watcher.Start();

            spin.Wait(TimeSpan.FromMilliseconds(50));

            watcher.Stop();

            TimeSpan actual = TimeSpan.FromMilliseconds(watcher.ElapsedMilliseconds);
            TimeSpan leftEpsilon = TimeSpan.FromMilliseconds(50 - (50 * 0.15));
            TimeSpan rightEpsilon = TimeSpan.FromMilliseconds(50 + (50 * 0.15));

            Assert.IsTrue(actual > leftEpsilon && actual < rightEpsilon);
        }

        [Test]
        public void Wait50Millisec_UpdateAfter300Millisec_TotalWaitingIsApprox800Millisec()
        {
            UpdateableSpin spin = new UpdateableSpin();

            Stopwatch watcher = new Stopwatch();
            watcher.Start();

            const int timeout = 500;
            const int spanBeforeUpdate = 300;

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(spanBeforeUpdate);
                spin.UpdateTimeout();
            });

            spin.Wait(TimeSpan.FromMilliseconds(timeout));

            watcher.Stop();

            TimeSpan actual = TimeSpan.FromMilliseconds(watcher.ElapsedMilliseconds);
            const int expected = timeout + spanBeforeUpdate;

            TimeSpan leftEpsilon = TimeSpan.FromMilliseconds(expected - (expected * 0.1));
            TimeSpan rightEpsilon = TimeSpan.FromMilliseconds(expected + (expected * 0.1));

            Assert.IsTrue(actual > leftEpsilon && actual < rightEpsilon);
        }
    }

    /// <summary>
    /// Custom SpinLock that has a time out that can be reset
    /// </summary>
    public class UpdateableSpin
    {
        private readonly object lockObj = new object();
        private bool shouldWait = true;
        private long executionStartingTime;

        public bool Wait(TimeSpan timeout, int spinDuration = 0)
        {
            UpdateTimeout();
            while (true)
            {
                lock (lockObj)
                {
                    if (!shouldWait)
                        return true;

                    if (DateTime.UtcNow.Ticks - executionStartingTime > timeout.Ticks)
                        return false;
                }
                Thread.Sleep(spinDuration);
            }
        }

        public void Set()
        {
            lock (lockObj)
            {
                shouldWait = false;
            }
        }

        public void UpdateTimeout()
        {
            lock (lockObj)
            {
                executionStartingTime = DateTime.UtcNow.Ticks;
            }
        }
    }
}