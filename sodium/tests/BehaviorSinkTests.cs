namespace sodium.tests
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;

    [TestFixture]
    public class BehaviorSinkTests : SodiumTestCase
    {
        [Test]
        public void Constructor_PassIntValue_ExpectValueSet()
        {
            var sink = new BehaviorSink<int>(123);
            Assert.AreEqual(123, sink.Val);
            Assert.False(sink.ValueUpdated);
            Assert.AreEqual(0, sink.ValueUpdate);
        }

        [Test]
        public void Constructor_PassNullableIntValue_ExpectNullValue()
        {
            var sink = new BehaviorSink<int?>(null);
            Assert.AreEqual(null, sink.Val);
            Assert.False(sink.ValueUpdated);
            Assert.AreEqual(null, sink.ValueUpdate);
        }

        [Test]
        public void Send_PassIntValue_ExpectValueUpdated()
        {
            var sink = new BehaviorSink<int>(0);
            sink.Send(1);
            Assert.False(sink.ValueUpdated);
            Assert.AreEqual(default(Int32), sink.ValueUpdate);
            Assert.AreEqual(1, sink.NewValue());
            Assert.AreEqual(1, sink.Val);
        }

        [Test]
        public void Send_PassIntValueTwice_ExpectSecondValue()
        {
            var sink = new BehaviorSink<int>(0);
            sink.Send(1);
            sink.Send(2);
            Assert.False(sink.ValueUpdated);
            Assert.AreEqual(default(Int32), sink.ValueUpdate);
            Assert.AreEqual(2, sink.NewValue());
            Assert.AreEqual(2, sink.Val);
        }
    }
}
