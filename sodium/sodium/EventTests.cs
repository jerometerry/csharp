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
        public void TestFilter2()
        {
            var e = new EventSink<char>();
            var out_ = new List<char>();
            var l = e.filter(char.IsUpper).listen(out_.Add);
            e.send('H');
            e.send('o');
            e.send('I');
            l.unlisten();
            AssertArraysEqual(Arrays<char>.AsList('H','I'), out_);
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

        [Test]
        public void TestCoalesce()
        {
            EventSink<Int32> e1 = new EventSink<Int32>();
            EventSink<Int32> e2 = new EventSink<Int32>();
            List<Int32> out_ = new List<Int32>();
            Listener l =
                 Event<Int32>.merge(e1,Event<Int32>.merge(e1.map(x => x * 100), e2))
                .coalesce((a,b) => a+b)
                .listen((x) => { out_.Add(x); });
            e1.send(2);
            e1.send(8);
            e2.send(40);
            l.unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(202, 808, 40), out_);
        }

        [Test]
        public void testDelay()
        {
            EventSink<char> e = new EventSink<char>();
            Behavior<char> b = e.hold(' ');
            List<char> out_ = new List<char>();
            Listener l = e.delay().snapshot(b).listen((x) => { out_.Add(x); });
            e.send('C');
            e.send('B');
            e.send('A');
            l.unlisten();
            AssertArraysEqual(Arrays<char>.AsList('C','B','A'), out_);
        }

        public static void AssertArraysEqual<TA>(List<TA> l1, List<TA> l2)
        {
            Assert.True(Arrays<TA>.AreArraysEqual(l1, l2));
        }

        internal static class Arrays<TA>
        {

            public static List<TA> AsList(params TA[] items)
            {
                return new List<TA>(items);
            }

            public static bool AreArraysEqual(List<TA> l1, List<TA> l2)
            {
                if (l1.Count != l2.Count)
                    return false;

                l1.Sort();
                l2.Sort();

                for (int i = 0; i < l1.Count; i++)
                {
                    TA item1 = l1[i];
                    TA item2 = l2[i];
                    if (!item1.Equals(item2))
                        return false;
                }

                return true;
            }

            public static void AssertArraysEqual(List<TA> l1, List<TA> l2)
            {
                Assert.True(Arrays<TA>.AreArraysEqual(l1, l2));
            }
        }
    }
}
