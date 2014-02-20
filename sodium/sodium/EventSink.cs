namespace sodium
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EventSink<A> : Event<A> {
        public EventSink() {}

	    public void send(A a) {
		    Transaction.run(new HandlerImpl<Transaction>(t => send(t, a)));
	    }

        internal void send(Transaction trans, A a) {
            if (!firings.Any())
                trans.last(new RunnableImpl(() => firings.Clear()));
            firings.Add(a);
        
		    List<TransactionHandler<A>> listeners = new List<TransactionHandler<A>>(this.listeners);
    	    foreach (TransactionHandler<A> action in listeners) {
    		    try {
                    action.run(trans, a);
    		    }
    		    catch (Exception t) {
    		        System.Diagnostics.Debug.WriteLine("{0}", t);
    		    }
    	    }
        }
    }
}