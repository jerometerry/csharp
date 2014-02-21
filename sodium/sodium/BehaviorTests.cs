using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit;

namespace sodium
{
    [TestFixture]
    public class BehaviorTests
    {
        [Test]
        public void TestSwitchB()
	    {
	        var esb = new EventSink<SB>();
	        // Split each field out_ of SB so we can update multiple behaviours in a
	        // single transaction.
	        var ba = esb.map<char?>(s => s.a).filterNotNull().hold('A');
	        var bb = esb.map(s => s.b).filterNotNull().hold('a');
	        var bsw = esb.map(s => s.sw).filterNotNull().hold(ba);
	        var bo = Behavior<char?>.switchB(bsw);
		    var out_ = new List<char?>();
	        var l = bo.value().listen(out_.Add);
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
	        EventTests.AssertArraysEqual(EventTests.Arrays<char?>.AsList('A','B','c','d','E','F','f','F','g','H','I'), out_);
	    }

        [Test]
        public void TestSwitchB2()
        {
            var esb = new EventSink<SB>();
            // Split each field out_ of SB so we can update multiple behaviours in a
            // single transaction.
            var ba = esb.map<char?>(s => s.a).filterNotNull().hold('A');
            var bb = esb.map(s => s.b).filterNotNull().hold('a');
            var bsw = esb.map(s => s.sw).filterNotNull().hold(ba);
            var bo = Behavior<char?>.switchB(bsw);
            var out_ = new List<char?>();
            var l = bo.value().listen(out_.Add);
            esb.send(new SB('B', 'b', null));
            esb.send(new SB('C', 'c', bb));
            esb.send(new SB('D', 'd', null));
            esb.send(new SB('E', 'e', ba));
            l.unlisten();
            EventTests.AssertArraysEqual(EventTests.Arrays<char?>.AsList('A', 'B', 'c', 'd', 'E'), out_);
        }

        [Test]
        public void TestSwitchB3()
        {
            var esb = new EventSink<SB>();
            // Split each field out_ of SB so we can update multiple behaviours in a
            // single transaction.
            var ba = esb.map<char?>(s => s.a).filterNotNull().hold('A');
            var bb = esb.map(s => s.b).filterNotNull().hold('a');
            var bsw = esb.map(s => s.sw).filterNotNull().hold(ba);
            var bo = Behavior<char?>.switchB(bsw);
            var out_ = new List<char?>();
            var l = bo.value().listen(out_.Add);
            esb.send(new SB('B', 'b', null));
            esb.send(new SB('C', 'c', bb));
            esb.send(new SB('D', 'd', null));
            esb.send(new SB('E', 'e', ba));
            esb.send(new SB('F', 'f', null));
            l.unlisten();
            EventTests.AssertArraysEqual(EventTests.Arrays<char?>.AsList('A', 'B', 'c', 'd', 'E', 'F'), out_);
        }

        [Test]
        public void TestSwitchB4()
        {
            var esb = new EventSink<SB>();
            // Split each field out_ of SB so we can update multiple behaviours in a
            // single transaction.
            var ba = esb.map<char?>(s => s.a).filterNotNull().hold('A');
            var bb = esb.map(s => s.b).filterNotNull().hold('a');
            var bsw = esb.map(s => s.sw).filterNotNull().hold(ba);
            var bo = Behavior<char?>.switchB(bsw);
            var out_ = new List<char?>();
            var l = bo.value().listen(out_.Add);
            esb.send(new SB('B', 'b', null));
            esb.send(new SB('C', 'c', bb));
            esb.send(new SB('D', 'd', null));
            esb.send(new SB('E', 'e', ba));
            esb.send(new SB('F', 'f', ba));
            l.unlisten();
            EventTests.AssertArraysEqual(EventTests.Arrays<char?>.AsList('A', 'B', 'c', 'd', 'E', 'F'), out_);
        }

        [Test]
        public void TestSwitchB5()
        {
            var esb = new EventSink<SB>();
            // Split each field out_ of SB so we can update multiple behaviours in a
            // single transaction.
            var ba = esb.map<char?>(s => s.a).filterNotNull().hold('A');
            var bb = esb.map(s => s.b).filterNotNull().hold('a');
            var bsw = esb.map(s => s.sw).filterNotNull().hold(ba);
            var bo = Behavior<char?>.switchB(bsw);
            var out_ = new List<char?>();
            var l = bo.value().listen(out_.Add);
            esb.send(new SB('B', 'b', null));
            esb.send(new SB('C', 'c', bb));
            esb.send(new SB('D', 'd', null));
            esb.send(new SB('E', 'e', ba));
            esb.send(new SB('F', 'f', bb));
            l.unlisten();
            EventTests.AssertArraysEqual(EventTests.Arrays<char?>.AsList('A', 'B', 'c', 'd', 'E', 'f'), out_);
        }

        class SB
        {
            public SB(char? a, char? b, Behavior<char?> sw)
            {
                this.a = a;
                this.b = b;
                this.sw = sw;
            }
            public char? a;
            public char? b;
            public Behavior<char?> sw;
        }

        [Test]
        public void TestTransactionHandlerImpl()
        {
            var results = new List<string>();
            var impl = new TransactionHandlerImpl<string>((t, a) => results.Add(a));
            impl.run(null, "this is a test");
            Assert.AreEqual("this is a test", results[0]);
        }

        [Test]
        public void TestLambda1Impl()
        {
            var impl = new Lambda1Impl<int, string>(a => a.ToString(CultureInfo.InvariantCulture));
            var results = impl.apply(1);
            Assert.AreEqual("1", results);
        }

        [Test]
        public void TestHandlerImpl()
        {
            List<string> results = new List<string>();
            var impl = new HandlerImpl<string>(results.Add);
            impl.run("hello world!");
            Assert.AreEqual("hello world!", results[0]);
        }

        [Test]
        public void TestLiftGlitch() {
		    BehaviorSink<Int32> a = new BehaviorSink<Int32>(1);
		    Behavior<Int32> a3 = a.map((x) => x * 3);
		    Behavior<Int32> a5 = a.map((x) => x * 5);
		    Behavior<String> b = Behavior<Int32>.lift<Int32, Int32, String>((x, y) => x + " " + y, a3, a5);
		    List<String> out_ = new List<String>();
            Listener l = b.value().listen(out_.Add);

		    a.send(2);
		    l.unlisten();
		    EventTests.AssertArraysEqual(EventTests.Arrays<string>.AsList("3 5", "6 10"), out_);
	    }

        class SE
        {
            public SE(char? a, char? b, Event<char?> sw)
            {
                this.a = a;
                this.b = b;
                this.sw = sw;
            }
            public char? a;
            public char? b;
            public Event<char?> sw;
        }

        [Test]
        public void TestSwitchE()
        {
            EventSink<SE> ese = new EventSink<SE>();
            Event<char?> ea = ese.map(s => s.a).filterNotNull();
            Event<char?> eb = ese.map(s => s.b).filterNotNull();
            Behavior<Event<char?>> bsw = ese.map(s => s.sw).filterNotNull().hold(ea);
            List<char?> out_ = new List<char?>();
            Event<char?> eo = Behavior<char?>.switchE(bsw);
	        Listener l = eo.listen(c => { out_.Add(c); });
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
	        EventTests.AssertArraysEqual(EventTests.Arrays<char?>.AsList('A','B','C','d','e','F','G','h','I'), out_);
        }
    }
}
