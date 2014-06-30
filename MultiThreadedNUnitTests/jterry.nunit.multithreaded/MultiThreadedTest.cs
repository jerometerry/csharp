using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace jterry.nunit.multithreaded
{
    [TestFixture]
    public class MultiThreadedTest
    {
        readonly ThreadStart[] delegates = {
            () => {
                Console.WriteLine("Nothing to see here");
            }, () => {
                throw new InvalidOperationException("Exception in test should fail test");
            }
        };

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SimpleMultiThreading()
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

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BetterMultiThreading()
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
    }
}
