namespace sodium.tests
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;

    [TestFixture]
    public class BehaviorSinkTests : SodiumTestCase
    {
        [Test]
        public void TestConstruction()
        {
            BehaviorSink<Int32> sink = new BehaviorSink<int>(123);
            Assert.AreEqual(123, sink.Val);
        }
    }
}
