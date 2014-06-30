using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace jterry.nunit.multithreaded
{
    [TestFixture]
    public class MultiThreadedTest
    {
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NaieveMultiThreading()
        {
            SimpleRunInParallel(GetDelegates());
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BetterMultiThreading()
        {
            RunInParallel(GetDelegates());
        }

        protected static void RunInParallel(params ThreadStart[] delegates)
        {
            var threads = delegates.Select(d => new CrossThreadTestRunner(d)).ToList();
            foreach (var t in threads)
            {
                t.Start();
            }

            foreach (var t in threads)
            {
                t.Join();
            }
        }

        protected static void SimpleRunInParallel(params ThreadStart[] delegates)
        {
            var threads = delegates.Select(d => new Thread(d)).ToList();
            foreach (var t in threads)
            {
                t.Start();
            }

            foreach (var t in threads)
            {
                t.Join();
            }
        }

        private static ThreadStart[] GetDelegates()
        {
            return new ThreadStart[]{() =>
            {
                Console.WriteLine("Nothing to see here");
            }, () => 
            {
                throw new InvalidOperationException("Unhandled exceptions should fail the test, but they just silently terminate the running thread");
            }};
        }
    }
}
