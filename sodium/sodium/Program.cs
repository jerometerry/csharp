using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sodium
{
    class Program
    {
        static void Main(string[] args)
        {
            testSwitchB();
        }

        public static void testSwitchB()
	    {
	        EventSink<SB> esb = new EventSink<SB>();
	        // Split each field out_ of SB so we can update multiple behaviours in a
	        // single transaction.
	        Behavior<char?> ba = esb.map(s => s.a).filterNotNull().hold('A');
	        Behavior<char?> bb = esb.map(s => s.b).filterNotNull().hold('a');
	        Behavior<Behavior<char?>> bsw = esb.map(s => s.sw).filterNotNull().hold(ba);
	        Behavior<char?> bo = Behavior<char?>.switchB(bsw);
		    List<char?> out_ = new List<char?>();
	        Listener l = bo.value().listen(c => { out_.Add(c); });
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
	        //assertEquals(Arrays.asList('A','B','c','d','E','F','f','F','g','H','I'), out_);
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
    }
}
