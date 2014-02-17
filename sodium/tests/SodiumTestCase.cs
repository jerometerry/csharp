namespace sodium.tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NUnit.Framework;

    public class SodiumTestCase
    {
        [TearDown]
        public void TearDown()
        {
            // All tests are being run on a single thread.
            // Is this really necessary?
            //GC.Collect();
            //Thread.Sleep(100);
        }

        public static void AssertArraysEqual<TA>(List<TA> l1, List<TA> l2)
        {
            Assert.True(Arrays<TA>.AreArraysEqual(l1, l2));
        }
    }
}
