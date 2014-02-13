namespace sodium {

    //import java.util.List;
    using System.Collections.Generic;

    public class EventSink<A> : Event<A> {
        public EventSink() {}

	    public void send(final A a) {
		    Transaction.run(new Handler<Transaction>() {
			    public void run(Transaction trans) { send(trans, a); }
		    });
	    }

        void send(Transaction trans, A a) {
            if (firings.isEmpty())
                trans.last(new Runnable() {
            	    public void run() { firings.clear(); }
                });
            firings.add(a);
        
		    List<TransactionHandler<A>> listeners = (List<TransactionHandler<A>>)this.listeners.clone();
    	    foreach (TransactionHandler<A> action in listeners) {
    		    try {
                    action.run(trans, a);
    		    }
    		    catch (Throwable t) {
    		        t.printStackTrace();
    		    }
    	    }
        }
    }
}