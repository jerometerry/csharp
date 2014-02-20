using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace sodium
{
    [TestFixture]
    public class EventTests
    {
        [Test]
        public void TestListen()
        {
            EventSink<Int32> esb = new EventSink<Int32>();
            List<Int32> results = new List<Int32>();
            Listener listener = esb.listen(results.Add);
            Assert.IsNotNull(listener);
            esb.send(123);
            Assert.AreEqual(123, results[0]);
        }

        [Test]
        public void TestFilter()
        {
            EventSink<Int32> esb = new EventSink<Int32>();
            Event<Int32> even = esb.filter(a => a%2 == 0);
            List<Int32> results = new List<Int32>();
            Listener listener = even.listen(results.Add);
            Assert.IsNotNull(listener);
            esb.send(1);
            esb.send(2);
            esb.send(3);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(2, results[0]);
        }

        [Test]
        public void TestFilterNotNull()
        {
            EventSink<Int32?> esb = new EventSink<Int32?>();
            Event<Int32?> nonNull = esb.filterNotNull();
            List<Int32> results = new List<Int32>();
            Listener listener = nonNull.listen(a => results.Add(a.Value));
            Assert.IsNotNull(listener);
            esb.send(1);
            esb.send(null);
            esb.send(3);
            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(1, results[0]);
            Assert.AreEqual(3, results[1]);
        }

        [Test]
        public void TestMap()
        {
            EventSink<Int32> esb = new EventSink<Int32>();
            Event<string> map = esb.map<string>(a => a.ToString());
            Assert.IsNotNull(map);
            List<string> results = new List<string>();
            Listener listener = map.listen(results.Add);
            Assert.IsNotNull(listener);
            Assert.IsNotNull(map);
            esb.send(123);
            Assert.AreEqual("123", results[0]);
        }
    }
}
