namespace sodium.tests
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;

    [TestFixture]
    public class EventTester : SodiumTestCase
    {
        [Test]
        public void TestSendEvent()
        {
            EventSink<Int32> e = new EventSink<Int32>();
            List<Int32> o = new List<Int32>();
            IListener l = e.Listen(x => { o.Add(x); });
            e.Send(5);
            l.Unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(5), o);
            e.Send(6);
            AssertArraysEqual(Arrays<Int32>.AsList(5), o);
        }

        [Test]
        public void TestMap()
        {
            EventSink<Int32> e = new EventSink<Int32>();
            Event<String> m = e.Map(x => x.ToString());
            List<String> o = new List<String>();
            IListener l = m.Listen((String x) => { o.Add(x); });
            e.Send(5);
            l.Unlisten();
            AssertArraysEqual(Arrays<string>.AsList("5"), o);
        }

        [Test]
        public void TestMergeNonSimultaneous()
        {
            EventSink<Int32> e1 = new EventSink<Int32>();
            EventSink<Int32> e2 = new EventSink<Int32>();
            List<Int32> o = new List<Int32>();
            IListener l = Event<Int32>.Merge(e1, e2).Listen(x => { o.Add(x); });
            e1.Send(7);
            e2.Send(9);
            e1.Send(8);
            l.Unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(7, 9, 8), o);
        }

        [Test]
        public void TestMergeSimultaneous()
        {
            EventSink<Int32> e = new EventSink<Int32>();
            List<Int32> o = new List<Int32>();
            IListener l = Event<Int32>.Merge(e, e).Listen(x => { o.Add(x); });
            e.Send(7);
            e.Send(9);
            l.Unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(7, 7, 9, 9), o);
        }

        [Test]
        public void TestCoalesce()
        {
            EventSink<Int32> e1 = new EventSink<Int32>();
            EventSink<Int32> e2 = new EventSink<Int32>();
            List<Int32> o = new List<Int32>();
            IListener l =
                 Event<Int32>.Merge(e1, Event<Int32>.Merge(e1.Map(x => x * 100), e2))
                .Coalesce((Int32 a, Int32 b) => a + b)
                .Listen((Int32 x) => { o.Add(x); });
            e1.Send(2);
            e1.Send(8);
            e2.Send(40);
            l.Unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(202, 808, 40), o);
        }

        [Test]
        public void TestFilter()
        {
            EventSink<char> e = new EventSink<char>();
            List<char> o = new List<char>();
            IListener l = e.Filter((char c) => char.IsUpper(c)).Listen((char c) => { o.Add(c); });
            e.Send('H');
            e.Send('o');
            e.Send('I');
            l.Unlisten();
            AssertArraysEqual(Arrays<char>.AsList('H', 'I'), o);
        }

        [Test]
        public void TestFilterNotNull()
        {
            EventSink<String> e = new EventSink<String>();
            List<String> o = new List<String>();
            IListener l = e.FilterNotNull().Listen(s => { o.Add(s); });
            e.Send("tomato");
            e.Send(null);
            e.Send("peach");
            l.Unlisten();
            AssertArraysEqual(Arrays<String>.AsList("tomato", "peach"), o);
        }

        [Test]
        public void TestLoopEvent()
        {
            EventSink<Int32> ea = new EventSink<Int32>();
            EventLoop<Int32> eb = new EventLoop<Int32>();
            Event<Int32> ec = Event<Int32>.MergeWith((x, y) => x + y, ea.Map(x => x % 10), eb);
            Event<Int32> eb_o = ea.Map(x => x / 10).Filter(x => x != 0);
            eb.Loop(eb_o);
            List<Int32> o = new List<Int32>();
            IListener l = ec.Listen(x => { o.Add(x); });
            ea.Send(2);
            ea.Send(52);
            l.Unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(2, 7), o);
        }

        [Test]
        public void TestGate()
        {
            EventSink<char> ec = new EventSink<char>();
            BehaviorSink<Boolean> epred = new BehaviorSink<Boolean>(true);
            List<char> o = new List<char>();
            IListener l = ec.Gate(epred).Listen(x => { o.Add(x); });
            ec.Send('H');
            epred.Send(false);
            ec.Send('O');
            epred.Send(true);
            ec.Send('I');
            l.Unlisten();
            AssertArraysEqual(Arrays<char>.AsList('H', 'I'), o);
        }

        [Test]
        public void TestCollect()
        {
            EventSink<Int32> ea = new EventSink<Int32>();
            List<Int32> o = new List<Int32>();
            Event<Int32> sum = ea.Collect(100,
                //(a,s) => new Tuple2(a+s, a+s)
                new BinaryFunction<Int32, Int32, Tuple2<Int32, Int32>>((a, s) =>
                {
                    return new Tuple2<Int32, Int32>(a + s, a + s);
                })
            );
            IListener l = sum.Listen((x) => { o.Add(x); });
            ea.Send(5);
            ea.Send(7);
            ea.Send(1);
            ea.Send(2);
            ea.Send(3);
            l.Unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(105, 112, 113, 115, 118), o);
        }

        [Test]
        public void TestAccum()
        {
            EventSink<Int32> ea = new EventSink<Int32>();
            List<Int32> o = new List<Int32>();
            Behavior<Int32> sum = ea.Accum(100, (a, s) => a + s);
            IListener l = sum.Updates().Listen((x) => { o.Add(x); });
            ea.Send(5);
            ea.Send(7);
            ea.Send(1);
            ea.Send(2);
            ea.Send(3);
            l.Unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(105, 112, 113, 115, 118), o);
        }

        [Test]
        public void TestOnce()
        {
            EventSink<char> e = new EventSink<char>();
            List<char> o = new List<char>();
            IListener l = e.Once().Listen((x) => { o.Add(x); });
            e.Send('A');
            e.Send('B');
            e.Send('C');
            l.Unlisten();
            AssertArraysEqual(Arrays<char>.AsList('A'), o);
        }

        [Test]
        public void TestDelay()
        {
            EventSink<char> e = new EventSink<char>();
            Behavior<char> b = e.Hold(' ');
            List<char> o = new List<char>();
            IListener l = e.Delay().Snapshot(b).Listen((x) => { o.Add(x); });
            e.Send('C');
            e.Send('B');
            e.Send('A');
            l.Unlisten();
            AssertArraysEqual(Arrays<char>.AsList('C', 'B', 'A'), o);
        }
    }

}