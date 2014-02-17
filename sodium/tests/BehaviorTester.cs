namespace sodium.tests
{
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;

    [TestFixture]
    public class BehaviorTester
    {
        [TearDown]
        public void TearDown()
        {
            // All tests are being run on a single thread.
            // Is this really necessary?
            //GC.Collect();
            //Thread.Sleep(100);
        }

        [Test]
        public void TestHold()
        {
            var evt = new EventSink<Int32>();
            var behavior = evt.Hold(0);
            var results = new List<Int32>();
            var listener = behavior.Updates().Listen(results.Add);
            evt.Send(2);
            evt.Send(9);
            listener.Unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(2, 9), results);
        }

        [Test]
        public void TestSnapshot()
        {
            var behavior = new BehaviorSink<Int32>(0);
            var evt = new EventSink<Int64>();
            var results = new List<String>();
            var snapshotFunction = new BinaryFunction<long, int, string>(
                (x, y) => string.Format("{0} {1}", x, y));
            var listener = evt.Snapshot(behavior, snapshotFunction).Listen(results.Add);

            evt.Send(100L);
            behavior.Send(2);
            evt.Send(200L);
            behavior.Send(9);
            behavior.Send(1);
            evt.Send(300L);
            listener.Unlisten();
            AssertArraysEqual(Arrays<string>.AsList("100 0", "200 2", "300 1"), results);
        }

        [Test]
        public void TestValues()
        {
            var behavior = new BehaviorSink<Int32>(9);
            var results = new List<Int32>();
            var listener = behavior.GetValue().Listen(x => { results.Add(x); });
            behavior.Send(2);
            behavior.Send(7);
            listener.Unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(9, 2, 7), results);
        }

        [Test]
        public void TestConstantBehavior()
        {
            var behavior = new Behavior<Int32>(12);
            var results = new List<Int32>();
            var listener = behavior.GetValue().Listen(x => { results.Add(x); });
            listener.Unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(12), results);
        }

        [Test]
        public void TestValuesThenMap()
        {
            var behavior = new BehaviorSink<Int32>(9);
            var results = new List<Int32>();
            var map = new Function<Int32, Int32>((x) => { return x + 100; });
            var l = behavior.GetValue().Map(map).Listen(x => { results.Add(x); });
            behavior.Send(2);
            behavior.Send(7);
            l.Unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(109, 102, 107), results);
        }

        [Test]
        public void TestValuesTwiceThenMap()
        {
            var behavior = new BehaviorSink<Int32>(9);
            var results = new List<Int32>();
            var map = new Function<Int32, Int32>((x) => { return x + 100; });
            var listener = DoubleUp(behavior.GetValue()).Map(map).Listen(x => { results.Add(x); });
            behavior.Send(2);
            behavior.Send(7);
            listener.Unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(109, 109, 102, 102, 107, 107), results);
        }


        [Test]
        public void TestValuesThenCoalesce()
        {
            var behavior = new BehaviorSink<Int32>(9);
            var results = new List<Int32>();
            var coaleseFunction = new BinaryFunction<Int32, Int32, Int32>((fst, snd) => snd);
            var listener = behavior.GetValue().Coalesce(coaleseFunction).Listen(x => { results.Add(x); });
            behavior.Send(2);
            behavior.Send(7);
            listener.Unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(9, 2, 7), results);
        }

        
        [Test]
	    public void TestValuesTwiceThenCoalesce() 
        {
		    var behavior = new BehaviorSink<Int32>(9);
		    var results = new List<Int32>();
            var coaleseFunction = new BinaryFunction<Int32, Int32, Int32>((fst, snd) => { return fst + snd; });
            var listener = DoubleUp(behavior.GetValue()).Coalesce(coaleseFunction).Listen(x => { results.Add(x); });
		    behavior.Send(2);
		    behavior.Send(7);
		    listener.Unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(18, 4, 14), results);
	    }
        
        [Test]
	    public void TestValuesThenSnapshot() 
        {
		    var behaviorInt32 = new BehaviorSink<Int32>(9);
		    var behaviorChar = new BehaviorSink<char>('a');
		    var results = new List<char>();
            var listener = behaviorInt32.GetValue().Snapshot(behaviorChar).Listen(x => { results.Add(x); });
		    behaviorChar.Send('b');
		    behaviorInt32.Send(2);
		    behaviorChar.Send('c');
		    behaviorInt32.Send(7);
		    listener.Unlisten();
            AssertArraysEqual(Arrays<char>.AsList('a', 'b', 'c'), results);
	    }
        
        [Test]
	    public void TestValuesTwiceThenSnapshot() 
        {
		    var behaviorInt32 = new BehaviorSink<Int32>(9);
		    var behaviorChar = new BehaviorSink<char>('a');
		    var results = new List<char>();
            var listener = DoubleUp(behaviorInt32.GetValue()).Snapshot(behaviorChar).Listen(x => { results.Add(x); });
		    behaviorChar.Send('b');
		    behaviorInt32.Send(2);
		    behaviorChar.Send('c');
		    behaviorInt32.Send(7);
		    listener.Unlisten();
            AssertArraysEqual(Arrays<char>.AsList('a', 'a', 'b', 'b', 'c', 'c'), results);
	    }

        
        [Test]
	    public void TestValuesThenMerge() 
        {
		    var behavior1 = new BehaviorSink<Int32>(9);
		    var behavior2 = new BehaviorSink<Int32>(2);
		    var results = new List<Int32>();
            var combiningFunction = new BinaryFunction<Int32, Int32, Int32>((x, y) => x+y);
            var listener = Event<Int32>.MergeWith(combiningFunction, behavior1.GetValue(), behavior2.GetValue()).Listen(x => { results.Add(x); });
		    behavior1.Send(1);
		    behavior2.Send(4);
		    listener.Unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(11, 1, 4), results);
	    }
        
        [Test]
	    public void TestValuesThenFilter() 
        {
		    var behavior = new BehaviorSink<Int32>(9);
		    var results = new List<Int32>();
            var predicate = new Function<int, bool>(a => true);
            var listener = behavior.GetValue().Filter(predicate).Listen(x => { results.Add(x); });
		    behavior.Send(2);
		    behavior.Send(7);
		    listener.Unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(9, 2, 7), results);
	    }
        
        [Test]
	    public void TestValuesTwiceThenFilter() 
        {
		    var behavior = new BehaviorSink<Int32>(9);
		    var results = new List<Int32>();
            var predicate = new Function<Int32, bool>(a => true);
            var listener = DoubleUp(behavior.GetValue()).Filter(predicate).Listen(x => { results.Add(x); });
		    behavior.Send(2);
		    behavior.Send(7);
		    listener.Unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(9, 9, 2, 2, 7, 7), results);
	    }
        
        [Test]
	    public void TestValuesThenOnce() 
        {
		    var behavior = new BehaviorSink<Int32>(9);
		    var results = new List<Int32>();
            var listener = behavior.GetValue().Once().Listen(x => { results.Add(x); });
		    behavior.Send(2);
		    behavior.Send(7);
		    listener.Unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(9), results);
	    }
        
        [Test]
	    public void TestValuesTwiceThenOnce() 
        {
		    var behavior = new BehaviorSink<Int32>(9);
		    var results = new List<Int32>();
            var listener = DoubleUp(behavior.GetValue()).Once().Listen(x => { results.Add(x); });
		    behavior.Send(2);
		    behavior.Send(7);
		    listener.Unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(9), results);
	    }

        [Test]
	    public void TestValuesLateListen() 
        {
		    BehaviorSink<Int32> b = new BehaviorSink<Int32>(9);
		    List<Int32> o = new List<Int32>();
		    Event<Int32> value = b.GetValue();
		    b.Send(8);
		    IListener l = value.Listen(x => { o.Add(x); });
		    b.Send(2);
		    l.Unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(8, 2), o);
	    }
	
        [Test]
	    public void TestMapB() 
        {
		    BehaviorSink<Int32> b = new BehaviorSink<Int32>(6);
		    List<String> o = new List<String>();
		    IListener l = b.Map(x => x.ToString()).GetValue().Listen(x => { o.Add(x); });
		    b.Send(8);
		    l.Unlisten();
		    AssertArraysEqual(Arrays<string>.AsList("6", "8"), o);
	    }
	
        [Test]
	    public void TestMapBLateListen() 
        {
		    BehaviorSink<Int32> b = new BehaviorSink<Int32>(6);
		    List<String> o = new List<String>();
		    Behavior<String> bm = b.Map(x => x.ToString());
		    b.Send(2);
		    IListener l = bm.GetValue().Listen(x => { o.Add(x); });
		    b.Send(8);
		    l.Unlisten();
		    AssertArraysEqual(Arrays<string>.AsList("2", "8"), o);
	    }
	
        [Test]
	    public void TestTransaction() 
        {
            bool[] calledBack = new bool[1];
            Transaction.Run((trans) => { trans.Prioritized(Node.Null, (trans2) => { calledBack[0] = true; }); });
           Assert.AreEqual(true, calledBack[0]);
	    }

        [Test]
	    public void TestApply() 
        {
		    BehaviorSink<IFunction<Int64, String>> bf = new BehaviorSink<IFunction<Int64, String>>(
				    new Function<Int64, String>((Int64 b) => "1 "+b));
		    BehaviorSink<Int64> ba = new BehaviorSink<Int64>(5L);
		    List<String> o = new List<String>();
		    IListener l = Behavior<Int64>.Apply<string>(bf,ba).GetValue().Listen(x => { o.Add(x); });
		    bf.Send(new Function<Int64, String>((Int64 b) => "12 "+b));
		    ba.Send(6L);
            l.Unlisten();
            AssertArraysEqual(Arrays<string>.AsList("1 5", "12 5", "12 6"), o);
	    }

        [Test]
	    public void TestLift() 
        {
		    BehaviorSink<Int32> a = new BehaviorSink<Int32>(1);
		    BehaviorSink<Int64> b = new BehaviorSink<Int64>(5L);
		    List<String> o = new List<String>();
		    IListener l = Behavior<Int32>.Lift(
			    (x, y) => x + " " + y,
			    a,
			    b
		    ).GetValue().Listen((String x) => { o.Add(x); });
		    a.Send(12);
		    b.Send(6L);
            l.Unlisten();
            AssertArraysEqual(Arrays<string>.AsList("1 5", "12 5", "12 6"), o);
	    }
	
        [Test]
	    public void TestLiftGlitch() 
        {
		    BehaviorSink<Int32> a = new BehaviorSink<Int32>(1);
		    Behavior<Int32> a3 = a.Map((Int32 x) => x * 3);
		    Behavior<Int32> a5 = a.Map((Int32 x) => x * 5);
		    Behavior<String> b = Behavior<Int32>.Lift((x, y) => x + " " + y, a3, a5);
		    List<String> o = new List<String>();
		    IListener l = b.GetValue().Listen((String x) => { o.Add(x); });
		    a.Send(2);
		    l.Unlisten();
		    AssertArraysEqual(Arrays<string>.AsList("3 5", "6 10"), o);
	    }

        [Test]
	    public void TestHoldIsDelayed() 
        {
	        EventSink<Int32> e = new EventSink<Int32>();
	        Behavior<Int32> h = e.Hold(0);
	        Event<String> pair = e.Snapshot(h, (a, b) => a + " " + b);
		    List<String> o = new List<String>();
		    IListener l = pair.Listen((String x) => { o.Add(x); });
		    e.Send(2);
		    e.Send(3);
		    l.Unlisten();
		    AssertArraysEqual(Arrays<string>.AsList("2 0", "3 2"), o);
	    }

        [Test]
	    public void TestSwitchB()
	    {
	        EventSink<SB> esb = new EventSink<SB>();
	        // Split each field o of SB so we can update multiple behaviours in a
	        // single transaction.
	        Behavior<char> ba = esb.Map(s => s.a).FilterNotNull().Hold('A');
	        Behavior<char> bb = esb.Map(s => s.b).FilterNotNull().Hold('a');
	        Behavior<Behavior<char>> bsw = esb.Map(s => s.sw).FilterNotNull().Hold(ba);
	        Behavior<char> bo = Behavior<char>.SwitchB(bsw);
		    List<char> o = new List<char>();
	        IListener l = bo.GetValue().Listen(c => { o.Add(c); });
	        esb.Send(new SB('B','b',null));
	        esb.Send(new SB('C','c',bb));
	        esb.Send(new SB('D','d',null));
	        esb.Send(new SB('E','e',ba));
	        esb.Send(new SB('F','f',null));
	        //esb.Send(new SB(null,null,bb));
	        //esb.Send(new SB(null,null,ba));
	        esb.Send(new SB('G','g',bb));
	        esb.Send(new SB('H','h',ba));
	        esb.Send(new SB('I','i',ba));
	        l.Unlisten();
	        AssertArraysEqual(Arrays<char>.AsList('A','B','c','d','E','F','f','F','g','H','I'), o);
	    }

        [Test]
        public void TestSwitchE()
        {
            EventSink<SE> ese = new EventSink<SE>();
            Event<char> ea = ese.Map(s => s.a).FilterNotNull();
            Event<char> eb = ese.Map(s => s.b).FilterNotNull();
            Behavior<Event<char>> bsw = ese.Map(s => s.sw).FilterNotNull().Hold(ea);
            List<char> o = new List<char>();
            Event<char> eo = Behavior<char>.SwitchE(bsw);
	        IListener l = eo.Listen(c => { o.Add(c); });
	        ese.Send(new SE('A','a',null));
	        ese.Send(new SE('B','b',null));
	        ese.Send(new SE('C','c',eb));
	        ese.Send(new SE('D','d',null));
	        ese.Send(new SE('E','e',ea));
	        ese.Send(new SE('F','f',null));
	        ese.Send(new SE('G','g',eb));
	        ese.Send(new SE('H','h',ea));
	        ese.Send(new SE('I','i',ea));
	        l.Unlisten();
	        AssertArraysEqual(Arrays<char>.AsList('A','B','C','d','e','F','G','h','I'), o);
        }

        [Test]
        public void TestLoopBehavior()
        {
            EventSink<Int32> ea = new EventSink<Int32>();
            BehaviorLoop<Int32> sum = new BehaviorLoop<Int32>();
            Behavior<Int32> sum_out = ea.Snapshot(sum, (x, y) => x+y).Hold(0);
            sum.Loop(sum_out);
            List<Int32> o = new List<Int32>();
            IListener l = sum_out.GetValue().Listen(x => { o.Add(x); });
            ea.Send(2);
            ea.Send(3);
            ea.Send(1);
            l.Unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(0,2,5,6), o);
            Assert.AreEqual((int)6, (int)sum.Sample());
        }

        [Test]
        public void TestCollect()
        {
            EventSink<Int32> ea = new EventSink<Int32>();
            List<Int32> o = new List<Int32>();
            Behavior<Int32> sum = null; // TODO
            //Behavior<Int32> sum = ea.Hold(100).Collect(0,
            //    (a,s) => new Tuple2(a+s, a+s)
            //    new Lambda2<Int32, Int32, Tuple2<Int32,Int32>>() {
            //        public Tuple2<Int32,Int32> apply(Int32 a, Int32 s) {
            //            return new Tuple2<Int32,Int32>(a+s, a+s);
            //        }
            //    }
            //);
            IListener l = sum.GetValue().Listen((x) => { o.Add(x); });
            ea.Send(5);
            ea.Send(7);
            ea.Send(1);
            ea.Send(2);
            ea.Send(3);
            l.Unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(100,105,112,113,115,118), o);
        }

        [Test]
        public void TestAccum()
        {
            EventSink<Int32> ea = new EventSink<Int32>();
            List<Int32> o = new List<Int32>();
            Behavior<Int32> sum = ea.Accumulate(100, (a,s)=>a+s);
            IListener l = sum.GetValue().Listen((x) => { o.Add(x); });
            ea.Send(5);
            ea.Send(7);
            ea.Send(1);
            ea.Send(2);
            ea.Send(3);
            l.Unlisten();
            AssertArraysEqual(Arrays<Int32>.AsList(100,105,112,113,115,118), o);
        }

        /**
         * This is used for tests where value() produces a single initial value on listen,
         * and then we double that up by causing that single initial event to be repeated.
         * This needs testing separately, because the code must be done carefully to achieve
         * this.
         */
        private static Event<Int32> DoubleUp(Event<Int32> ev)
        {
            return Event<Int32>.Merge(ev, ev);
        }

        private void AssertArraysEqual<TA>(List<TA> l1, List<TA> l2)
        {
            Assert.True(Arrays<TA>.AreArraysEqual(l1, l2));
        }

        private static class Arrays<TA>
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
        }

        class SE
	    {
	        public SE(char a, char b, Event<char> sw)
	        {
	            this.a = a;
	            this.b = b;
	            this.sw = sw;
	        }
	        public char a;
	        public char b;
            public Event<char> sw;
	    }

        class SB
	    {
	        public SB(char a, char b, Behavior<char> sw)
	        {
	            this.a = a;
	            this.b = b;
	            this.sw = sw;
	        }
	        public char a;
            public char b;
            public Behavior<char> sw;
	    }
    }
}