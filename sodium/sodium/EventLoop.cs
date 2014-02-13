namespace sodium {

    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EventLoop<A> : Event<A> {
        private Event<A> ea_out;

        public EventLoop()
        {
        }

        // TO DO: Copy & paste from EventSink. Can we improve this?
        private void send(Transaction trans, A a) {
            if (!firings.Any())
                trans.last(new Runnable(() => firings.Clear()) {
                });
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

        public void loop(Event<A> ea_out)
        {
            if (this.ea_out != null)
                throw new ApplicationException("EventLoop looped more than once");
            this.ea_out = ea_out;
            EventLoop<A> me = this;
            ITransactionHandler<A> action = new TransactionHandler(me);
            addCleanup(ea_out.listen_(this.node, action));
        }

        private class TransactionHandler : ITransactionHandler<A>
        {
            private EventLoop<A> me;

            public TransactionHandler(EventLoop<A> me)
            {
                this.me = me;
            }

            public void run(Transaction trans, A a)
            {
                me.send(trans, a);
            }
        }
    }

}