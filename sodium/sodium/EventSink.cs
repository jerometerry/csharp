namespace sodium {

    //import java.util.List;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EventSink<A> : Event<A>
    {
        public EventSink() {}

	    public void send(A a)
	    {
            Transaction.run(new EventSinkRunner<A>(this, a));
	    }

        private class EventSinkRunner<A> : IHandler<Transaction>
        {
            private A a;
            private EventSink<A> sink;

            public EventSinkRunner(EventSink<A> sink, A a)
            {
                this.sink = sink;
                this.a = a;
            }

            public void run(Transaction trans)
            {
                sink.send(trans, a);
            }
        }

        public void send(Transaction trans, A a) {
            if (!firings.Any())
                trans.last(new Runnable(() => firings.Clear()));
            firings.Add(a);
        
            List<ITransactionHandler<A>> listeners = new List<ITransactionHandler<A>>(this.listeners);

    	    foreach (ITransactionHandler<A> action in listeners) {
    		    try {
                    action.run(trans, a);
    		    }
    		    catch (Exception t) {
    		        Console.WriteLine("{0}", t);
    		    }
    	    }
        }
    }
}