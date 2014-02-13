namespace sodium {

    //import java.util.List;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EventSink<A> : Event<A> {
        public EventSink() {}

	    public void send(A a) {
		    Transaction.run(new Handler<Transaction>() {
			    public void run(Transaction trans) { send(trans, a); }
		    });
	    }

        void send(Transaction trans, A a) {
            if (!firings.Any())
                trans.last(new Runnable() {
            	    public void run() { firings.Clear(); }
                });
            firings.Add(a);
        
		    List<TransactionHandler<A>> listeners = (List<TransactionHandler<A>>)this.listeners.clone();
    	    foreach (TransactionHandler<A> action in listeners) {
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