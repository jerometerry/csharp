namespace sodium.tests
{
    using NUnit.Framework;
    using NUnit;
    using System;
    using System.Threading;
    using System.Collections.Generic;

    [TestFixture]
    public class BehaviorTester
    {
        [TearDown]
        public void TearDown()
        {
            GC.Collect();
            Thread.Sleep(100);
        }

        [Test]
        public void TestHold()
        {
            var e = new EventSink<Int32>();
            var b = e.Hold(0);
            var o = new List<Int32>();
            var l = b.Updates().Listen(new Handler<Int32>(o.Add));
            e.Send(2);
            e.Send(9);
            l.Unlisten();
            AssertArraysEqual(Arrays<Int32>.asList(2, 9), o);
        }

        [Test]
        public void TestSnapshot()
        {
            var b = new BehaviorSink<Int32>(0);
            var trigger = new EventSink<Int64>();
            var o = new List<String>();

            var l = trigger
                .Snapshot(b, new TwoParameterFunction<long, int, string>((x, y) => string.Format("{0} {1}", x, y)))
                .Listen(new Handler<string>(o.Add));

            trigger.Send(100L);
            b.Send(2);
            trigger.Send(200L);
            b.Send(9);
            b.Send(1);
            trigger.Send(300L);
            l.Unlisten();
            AssertArraysEqual(Arrays<string>.asList("100 0", "200 2", "300 1"), o);
        }

        /*
        [Test]
        public void testValues() {
            BehaviorSink<Int32> b = new BehaviorSink<Int32>(9);
            List<Int32> o = new List<Int32>();
            Listener l = b.value().listen(x => { o.add(x); });
            b.send(2);
            b.send(7);
            l.unlisten();
            Assert.AreEqual(Arrays.asList(9,2,7), o);
        }
	
        [Test]
        public void testConstantBehavior() {
            Behavior<Int32> b = new Behavior<Int32>(12);
            List<Int32> o = new List();
            Listener l = b.value().listen(x => { o.add(x); });
            l.unlisten();
            Assert.AreEqual(Arrays.asList(12), o);
        }

        [Test]
        public void testValuesThenMap() {
            BehaviorSink<Int32> b = new BehaviorSink<Int32>(9);
            List<Int32> o = new List<Int32>();
            Listener l = b.value().map(x => x+100).listen(x => { o.add(x); });
            b.send(2);
            b.send(7);
            l.unlisten();
            Assert.AreEqual(Arrays.asList(109,102,107), o);
        }
        */

        /**
         * This is used for tests where value() produces a single initial value on listen,
         * and then we double that up by causing that single initial event to be repeated.
         * This needs testing separately, because the code must be done carefully to achieve
         * this.
         */
        private static Event<Int32> doubleUp(Event<Int32> ev)
        {
            return Event<Int32>.Merge(ev, ev);
        }

        /*
        [Test]
	    public void testValuesTwiceThenMap() {
		    BehaviorSink<Int32> b = new BehaviorSink<Int32>(9);
		    List<Int32> o = new List<Int32>();
		    Listener l = doubleUp(b.value()).map(x => x+100).listen(x => { o.add(x); });
		    b.send(2);
		    b.send(7);
		    l.unlisten();
		    Assert.AreEqual(Arrays.asList(109,109,102,102,107,107), o);
	    }

        [Test]
	    public void testValuesThenCoalesce() {
		    BehaviorSink<Int32> b = new BehaviorSink<Int32>(9);
		    List<Int32> o = new List<Int32>();
		    Listener l = b.value().coalesce((fst, snd) => snd).listen(x => { o.add(x); });
		    b.send(2);
		    b.send(7);
		    l.unlisten();
		    Assert.AreEqual(Arrays.asList(9,2,7), o);
	    }

        [Test]
	    public void testValuesTwiceThenCoalesce() {
		    BehaviorSink<Int32> b = new BehaviorSink<Int32>(9);
		    List<Int32> o = new List<Int32>();
		    Listener l = doubleUp(b.value()).coalesce((fst, snd) => fst+snd).listen(x => { o.add(x); });
		    b.send(2);
		    b.send(7);
		    l.unlisten();
		    Assert.AreEqual(Arrays.asList(18,4,14), o);
	    }

        [Test]
	    public void testValuesThenSnapshot() {
		    BehaviorSink<Int32> bi = new BehaviorSink<Int32>(9);
		    BehaviorSink<char> bc = new BehaviorSink<char>('a');
		    List<char> o = new List<char>();
		    Listener l = bi.value().snapshot(bc).listen(x => { o.add(x); });
		    bc.send('b');
		    bi.send(2);
		    bc.send('c');
		    bi.send(7);
		    l.unlisten();
		    Assert.AreEqual(Arrays.asList('a','b','c'), o);
	    }

        [Test]
	    public void testValuesTwiceThenSnapshot() {
		    BehaviorSink<Int32> bi = new BehaviorSink<Int32>(9);
		    BehaviorSink<char> bc = new BehaviorSink<char>('a');
		    List<char> o = new List<char>();
		    Listener l = doubleUp(bi.value()).snapshot(bc).listen(x => { o.add(x); });
		    bc.send('b');
		    bi.send(2);
		    bc.send('c');
		    bi.send(7);
		    l.unlisten();
		    Assert.AreEqual(Arrays.asList('a','a','b','b','c','c'), o);
	    }

        [Test]
	    public void testValuesThenMerge() {
		    BehaviorSink<Int32> bi = new BehaviorSink<Int32>(9);
		    BehaviorSink<Int32> bj = new BehaviorSink<Int32>(2);
		    List<Int32> o = new List<Int32>();
		    Listener l = Event.mergeWith((x, y) => x+y, bi.value(),bj.value())
		        .listen(x => { o.add(x); });
		    bi.send(1);
		    bj.send(4);
		    l.unlisten();
		    Assert.AreEqual(Arrays.asList(11,1,4), o);
	    }

        [Test]
	    public void testValuesThenFilter() {
		    BehaviorSink<Int32> b = new BehaviorSink<Int32>(9);
		    List<Int32> o = new List<Int32>();
		    Listener l = b.value().filter(a => true).listen(x => { o.add(x); });
		    b.send(2);
		    b.send(7);
		    l.unlisten();
		    Assert.AreEqual(Arrays.asList(9,2,7), o);
	    }

        [Test]
	    public void testValuesTwiceThenFilter() {
		    BehaviorSink<Int32> b = new BehaviorSink<Int32>(9);
		    List<Int32> o = new List<Int32>();
		    Listener l = doubleUp(b.value()).filter(a => true).listen(x => { o.add(x); });
		    b.send(2);
		    b.send(7);
		    l.unlisten();
		    Assert.AreEqual(Arrays.asList(9,9,2,2,7,7), o);
	    }

        [Test]
	    public void testValuesThenOnce() {
		    BehaviorSink<Int32> b = new BehaviorSink<Int32>(9);
		    List<Int32> o = new List<Int32>();
		    Listener l = b.value().once().listen(x => { o.add(x); });
		    b.send(2);
		    b.send(7);
		    l.unlisten();
		    Assert.AreEqual(Arrays.asList(9), o);
	    }

        [Test]
	    public void testValuesTwiceThenOnce() {
		    BehaviorSink<Int32> b = new BehaviorSink<Int32>(9);
		    List<Int32> o = new List<Int32>();
		    Listener l = doubleUp(b.value()).once().listen(x => { o.add(x); });
		    b.send(2);
		    b.send(7);
		    l.unlisten();
		    Assert.AreEqual(Arrays.asList(9), o);
	    }

        [Test]
	    public void testValuesLateListen() {
		    BehaviorSink<Int32> b = new BehaviorSink<Int32>(9);
		    List<Int32> o = new List<Int32>();
		    Event<Int32> value = b.value();
		    b.send(8);
		    Listener l = value.listen(x => { o.add(x); });
		    b.send(2);
		    l.unlisten();
		    Assert.AreEqual(Arrays.asList(8,2), o);
	    }
	
        [Test]
	    public void testMapB() {
		    BehaviorSink<Int32> b = new BehaviorSink<Int32>(6);
		    List<String> o = new List<String>();
		    Listener l = b.map(x => x.toString())
				    .value().listen(x => { o.add(x); });
		    b.send(8);
		    l.unlisten();
		    Assert.AreEqual(Arrays.asList("6", "8"), o);
	    }
	
        [Test]
	    public void testMapBLateListen() {
		    BehaviorSink<Int32> b = new BehaviorSink<Int32>(6);
		    List<String> o = new List<String>();
		    Behavior<String> bm = b.map(x => x.toString());
		    b.send(2);
		    Listener l = bm.value().listen(x => { o.add(x); });
		    b.send(8);
		    l.unlisten();
		    Assert.AreEqual(Arrays.asList("2", "8"), o);
	    }
	
        [Test]
	    public void testTransaction() {
		    bool[] calledBack = new bool[1];
	        Transaction.run((Transaction trans) => {
	    	    trans.prioritized(Node.NULL, trans2 => { calledBack[0] = true; });
	        });
	        Assert.AreEqual(true, calledBack[0]);
	    }

        [Test]
	    public void testApply() {
		    BehaviorSink<Lambda1<Int64, String>> bf = new BehaviorSink<Lambda1<Int64, String>>(
				    (Int64 b) => "1 "+b);
		    BehaviorSink<Int64> ba = new BehaviorSink<Int64>(5L);
		    List<String> o = new List<String>();
		    Listener l = Behavior.apply(bf,ba).value().listen(x => { o.add(x); });
		    bf.send((Int64 b) => "12 "+b);
		    ba.send(6L);
            l.unlisten();
            Assert.AreEqual(Arrays.asList("1 5", "12 5", "12 6"), o);
	    }

        [Test]
	    public void testLift() {
		    BehaviorSink<Int32> a = new BehaviorSink<Int32>(1);
		    BehaviorSink<Int64> b = new BehaviorSink<Int64>(5L);
		    List<String> o = new List<String>();
		    Listener l = Behavior.lift(
			    (x, y) => x + " " + y,
			    a,
			    b
		    ).value().listen((String x) => { o.add(x); });
		    a.send(12);
		    b.send(6L);
            l.unlisten();
            Assert.AreEqual(Arrays.asList("1 5", "12 5", "12 6"), o);
	    }
	
        [Test]
	    public void testLiftGlitch() {
		    BehaviorSink<Int32> a = new BehaviorSink<Int32>(1);
		    Behavior<Int32> a3 = a.map((Int32 x) => x * 3);
		    Behavior<Int32> a5 = a.map((Int32 x) => x * 5);
		    Behavior<String> b = Behavior.lift((x, y) => x + " " + y, a3, a5);
		    List<String> o = new List<String>();
		    Listener l = b.value().listen((String x) => { o.add(x); });
		    a.send(2);
		    l.unlisten();
		    Assert.AreEqual(Arrays.asList("3 5", "6 10"), o);
	    }

        [Test]
	    public void testHoldIsDelayed() {
	        EventSink<Int32> e = new EventSink<Int32>();
	        Behavior<Int32> h = e.hold(0);
	        Event<String> pair = e.snapshot(h, (a, b) => a + " " + b);
		    List<String> o = new List<String>();
		    Listener l = pair.listen((String x) => { o.add(x); });
		    e.send(2);
		    e.send(3);
		    l.unlisten();
		    Assert.AreEqual(Arrays.asList("2 0", "3 2"), o);
	    }

	    static class SB
	    {
	        SB(char a, char b, Behavior<char> sw)
	        {
	            this.a = a;
	            this.b = b;
	            this.sw = sw;
	        }
	        char a;
	        char b;
	        Behavior<char> sw;
	    }

        [Test]
	    public void testSwitchB()
	    {
	        EventSink<SB> esb = new EventSink();
	         Split each field o of SB so we can update multiple behaviours in a
	         single transaction.
	        Behavior<char> ba = esb.map(s => s.a).filterNotNull().hold('A');
	        Behavior<char> bb = esb.map(s => s.b).filterNotNull().hold('a');
	        Behavior<Behavior<char>> bsw = esb.map(s => s.sw).filterNotNull().hold(ba);
	        Behavior<char> bo = Behavior.switchB(bsw);
		    List<char> o = new List<char>();
	        Listener l = bo.value().listen(c => { o.add(c); });
	        esb.send(new SB('B','b',null));
	        esb.send(new SB('C','c',bb));
	        esb.send(new SB('D','d',null));
	        esb.send(new SB('E','e',ba));
	        esb.send(new SB('F','f',null));
	        esb.send(new SB(null,null,bb));
	        esb.send(new SB(null,null,ba));
	        esb.send(new SB('G','g',bb));
	        esb.send(new SB('H','h',ba));
	        esb.send(new SB('I','i',ba));
	        l.unlisten();
	        Assert.AreEqual(Arrays.asList('A','B','c','d','E','F','f','F','g','H','I'), o);
	    }

	    static class SE
	    {
	        SE(char a, char b, Event<char> sw)
	        {
	            this.a = a;
	            this.b = b;
	            this.sw = sw;
	        }
	        char a;
	        char b;
	        Event<char> sw;
	    }

        [Test]
        public void testSwitchE()
        {
            EventSink<SE> ese = new EventSink();
            Event<char> ea = ese.map(s => s.a).filterNotNull();
            Event<char> eb = ese.map(s => s.b).filterNotNull();
            Behavior<Event<char>> bsw = ese.map(s => s.sw).filterNotNull().hold(ea);
            List<char> o = new List();
            Event<char> eo = Behavior.switchE(bsw);
	        Listener l = eo.listen(c => { o.add(c); });
	        ese.send(new SE('A','a',null));
	        ese.send(new SE('B','b',null));
	        ese.send(new SE('C','c',eb));
	        ese.send(new SE('D','d',null));
	        ese.send(new SE('E','e',ea));
	        ese.send(new SE('F','f',null));
	        ese.send(new SE('G','g',eb));
	        ese.send(new SE('H','h',ea));
	        ese.send(new SE('I','i',ea));
	        l.unlisten();
	        Assert.AreEqual(Arrays.asList('A','B','C','d','e','F','G','h','I'), o);
        }

        [Test]
        public void testLoopBehavior()
        {
            EventSink<Int32> ea = new EventSink();
            BehaviorLoop<Int32> sum = new BehaviorLoop<Int32>();
            Behavior<Int32> sum_out = ea.snapshot(sum, (x, y) => x+y).hold(0);
            sum.loop(sum_out);
            List<Int32> o = new List();
            Listener l = sum_out.value().listen(x => { o.add(x); });
            ea.send(2);
            ea.send(3);
            ea.send(1);
            l.unlisten();
            Assert.AreEqual(Arrays.asList(0,2,5,6), o);
            Assert.AreEqual((int)6, (int)sum.sample());
        }

        [Test]
        public void testCollect()
        {
            EventSink<Int32> ea = new EventSink();
            List<Int32> o = new List();
            Behavior<Int32> sum = ea.hold(100).collect(0,
                (a,s) => new Tuple2(a+s, a+s)
                new Lambda2<Int32, Int32, Tuple2<Int32,Int32>>() {
                    public Tuple2<Int32,Int32> apply(Int32 a, Int32 s) {
                        return new Tuple2<Int32,Int32>(a+s, a+s);
                    }
                }
            );
            Listener l = sum.value().listen((x) => { o.add(x); });
            ea.send(5);
            ea.send(7);
            ea.send(1);
            ea.send(2);
            ea.send(3);
            l.unlisten();
            Assert.AreEqual(Arrays.asList(100,105,112,113,115,118), o);
        }

        [Test]
        public void testAccum()
        {
            EventSink<Int32> ea = new EventSink();
            List<Int32> o = new List();
            Behavior<Int32> sum = ea.accum(100, (a,s)=>a+s);
            Listener l = sum.value().listen((x) => { o.add(x); });
            ea.send(5);
            ea.send(7);
            ea.send(1);
            ea.send(2);
            ea.send(3);
            l.unlisten();
            Assert.AreEqual(Arrays.asList(100,105,112,113,115,118), o);
        }
        */

        private void AssertArraysEqual<TA>(List<TA> l1, List<TA> l2)
        {
            Assert.True(Arrays<TA>.AreArraysEqual(l1, l2));
        }

        private static class Arrays<TA>
        {
            public static List<TA> asList(params TA[] items)
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
        }
    }
}