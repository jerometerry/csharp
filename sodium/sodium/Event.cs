namespace sodium {

    using System;
    using System.Collections.Generic;
    //import java.util.ArrayList;
    //import java.util.List;

    public class Event<A> {
	    private sealed class ListenerImplementation<A> : Listener {
		    /**
		     * It's essential that we keep the listener alive while the caller holds
		     * the Listener, so that the finalizer doesn't get triggered.
		     */
		    private readonly Event<A> evt;
		    private readonly ITransactionHandler<A> action;
		    private readonly Node target;

		    private ListenerImplementation(Event<A> evt, ITransactionHandler<A> action, Node target) {
			    this.evt = evt;
			    this.action = action;
			    this.target = target;
		    }

		    public void unlisten() {
		        lock (Transaction.listenersLock) {
                    evt.listeners.Remove(action);
                    evt.node.unlinkTo(target);
                }
		    }

		    protected void finalize() {
			    unlisten();
		    }
	    }

	    protected readonly List<ITransactionHandler<A>> listeners = new List<ITransactionHandler<A>>();
	    protected readonly List<Listener> finalizers = new List<Listener>();
	    Node node = new Node(0L);
	    protected readonly List<A> firings = new List<A>();

	    /**
	     * An event that never fires.
	     */
	    public Event() {
	    }

	    protected Object[] sampleNow() { return null; }

	    /**
	     * Listen for firings of this event. The returned Listener has an unlisten()
	     * method to cause the listener to be removed. This is the observer pattern.
         */
	    public Listener listen(IHandler<A> action) {
		    return listen_(Node.NULL, new ITransactionHandler<A>() {
			    public void run(Transaction trans2, A a) {
				    action.run(a);
			    }
		    });
	    }

	    Listener listen_(Node target, ITransactionHandler<A> action) {
		    return Transaction.apply(new Lambda1<Transaction, Listener>() {
			    public Listener apply(Transaction trans1) {
				    return listen(target, trans1, action, false);
			    }
		    });
	    }

	    Listener listen(Node target, Transaction trans, ITransactionHandler<A> action, bool suppressEarlierFirings) {
            lock (Transaction.listenersLock) {
                if (node.linkTo(target))
                    trans.toRegen = true;
                listeners.Add(action);
            }
		    Object[] aNow = sampleNow();
		    if (aNow != null) {    // In cases like value(), we start with an initial value.
		        for (int i = 0; i < aNow.Length; i++)
                    action.run(trans, (A)aNow[i]);  // <-- unchecked warning is here
            }
		    if (!suppressEarlierFirings) {
                // Anything sent already in this transaction must be sent now so that
                // there's no order dependency between send and listen.
                foreach (A a in firings)
                    action.run(trans, a);
            }
		    return new ListenerImplementation<A>(this, action, target);
	    }

        /**
         * Transform the event's value according to the supplied function.
         */
	    public <B> Event<B> map(Lambda1<A,B> f)
	    {
	        Event<A> ev = this;
	        EventSink<B> o = new EventSink<B>() {
                protected override Object[] sampleNow()
                {
                    Object[] oi = ev.sampleNow();
                    if (oi != null) {
                        Object[] oo = new Object[oi.Length];
                        for (int i = 0; i < oo.Length; i++)
                            oo[i] = f.apply((A)oi[i]);
                        return oo;
                    }
                    else
                        return null;
                }
	        };
            Listener l = listen_(o.node, new ITransactionHandler<A>() {
        	    public void run(Transaction trans2, A a) {
	                o.send(trans2, f.apply(a));
	            }
            });
            return o.addCleanup(l);
	    }

	    /**
	     * Create a behavior with the specified initial value, that gets updated
         * by the values coming through the event. The 'current value' of the behavior
         * is notionally the value as it was 'at the start of the transaction'.
         * That is, state updates caused by event firings get processed at the end of
         * the transaction.
         */
	    public Behavior<A> hold(A initValue) {
		    return Transaction.apply(new Lambda1<Transaction, Behavior<A>>() {
			    public Behavior<A> apply(Transaction trans) {
			        return new Behavior<A>(lastFiringOnly(trans), initValue);
			    }
		    });
	    }

	    /**
	     * Variant of snapshot that throws away the event's value and captures the behavior's.
	     */
	    public <B> Event<B> snapshot(Behavior<B> beh)
	    {
	        return snapshot(beh, new Lambda2<A,B,B>() {
	    	    public B apply(A a, B b) {
	    		    return b;
	    	    }
	        });
	    }

	    /**
	     * Sample the behavior at the time of the event firing. Note that the 'current value'
         * of the behavior that's sampled is the value as at the start of the transaction
         * before any state changes of the current transaction are applied through 'hold's.
         */
	    public <B,C> Event<C> snapshot(Behavior<B> b, Lambda2<A,B,C> f)
	    {
	        Event<A> ev = this;
		    EventSink<C> o = new EventSink<C>() {
                protected override Object[] sampleNow()
                {
                    Object[] oi = ev.sampleNow();
                    if (oi != null) {
                        Object[] oo = new Object[oi.Length];
                        for (int i = 0; i < oo.Length; i++)
                            oo[i] = f.apply((A)oi[i], b.sample());
                        return oo;
                    }
                    else
                        return null;
                }
		    };
            Listener l = listen_(o.node, new ITransactionHandler<A>() {
        	    public void run(Transaction trans2, A a) {
	                o.send(trans2, f.apply(a, b.sample()));
	            }
            });
            return o.addCleanup(l);
	    }

        /**
         * Merge two streams of events of the same type.
         *
         * In the case where two event occurrences are simultaneous (i.e. both
         * within the same transaction), both will be delivered in the same
         * transaction. If the event firings are ordered for some reason, then
         * their ordering is retained. In many common cases the ordering will
         * be undefined.
         */
	    public static <A> Event<A> merge(Event<A> ea, Event<A> eb)
	    {
	        EventSink<A> o = new EventSink<A>() {
                protected override Object[] sampleNow()
                {
                    Object[] oa = ea.sampleNow();
                    Object[] ob = eb.sampleNow();
                    if (oa != null && ob != null) {
                        Object[] oo = new Object[oa.Length + ob.Length];
                        int j = 0;
                        for (int i = 0; i < oa.Length; i++) oo[j++] = oa[i];
                        for (int i = 0; i < ob.Length; i++) oo[j++] = ob[i];
                        return oo;
                    }
                    else
                    if (oa != null)
                        return oa;
                    else
                        return ob;
                }
	        };
            ITransactionHandler<A> h = new ITransactionHandler<A>() {
        	    public void run(Transaction trans, A a) {
	                o.send(trans, a);
	            }
            };
            Listener l1 = ea.listen_(o.node, h);
            Listener l2 = eb.listen_(o.node, h);
            return o.addCleanup(l1).addCleanup(l2);
	    }

	    /**
	     * Push each event occurrence onto a new transaction.
	     */
	    public Event<A> delay()
	    {
	        EventSink<A> o = new EventSink<A>();
	        Listener l1 = listen_(o.node, new ITransactionHandler<A>() {
	            public void run(Transaction trans, A a) {
	                trans.post(new Runnable() {
                        public void run() {
                            Transaction trans = new Transaction();
                            try {
                                o.send(trans, a);
                            } finally {
                                trans.close();
                            }
                        }
	                });
	            }
	        });
	        return o.addCleanup(l1);
	    }

        /**
         * If there's more than one firing in a single transaction, combine them into
         * one using the specified combining function.
         *
         * If the event firings are ordered, then the first will appear at the left
         * input of the combining function. In most common cases it's best not to
         * make any assumptions about the ordering, and the combining function would
         * ideally be commutative.
         */
	    public Event<A> coalesce(Lambda2<A,A,A> f)
	    {
	        return Transaction.apply(new Lambda1<Transaction, Event<A>>() {
	    	    public Event<A> apply(Transaction trans) {
	    		    return coalesce(trans, f);
	    	    }
	        });
	    }

	    Event<A> coalesce(Transaction trans1, Lambda2<A,A,A> f)
	    {
	        Event<A> ev = this;
	        EventSink<A> o = new EventSink<A>() {
                protected override Object[] sampleNow()
                {
                    Object[] oi = ev.sampleNow();
                    if (oi != null) {
					    A o = (A)oi[0];
                        for (int i = 1; i < oi.Length; i++)
                            o = f.apply(o, (A)oi[i]);
                        return new Object[] { o };
                    }
                    else
                        return null;
                }
	        };
            ITransactionHandler<A> h = new CoalesceHandler<A>(f, o);
            Listener l = listen(o.node, trans1, h, false);
            return o.addCleanup(l);
        }

        /**
         * Clean up the output by discarding any firing other than the last one. 
         */
        Event<A> lastFiringOnly(Transaction trans)
        {
            return coalesce(trans, new Lambda2<A,A,A>() {
        	    public A apply(A first, A second) { return second; }
            });
        }

        /**
         * Merge two streams of events of the same type, combining simultaneous
         * event occurrences.
         *
         * In the case where multiple event occurrences are simultaneous (i.e. all
         * within the same transaction), they are combined using the same logic as
         * 'coalesce'.
         */
        public static <A> Event<A> mergeWith(Lambda2<A,A,A> f, Event<A> ea, Event<A> eb)
        {
            return merge(ea, eb).coalesce(f);
        }

        /**
         * Only keep event occurrences for which the predicate returns true.
         */
        public Event<A> filter(Lambda1<A,Boolean> f)
        {
            Event<A> ev = this;
            EventSink<A> o = new EventSink<A>() {
                protected override Object[] sampleNow()
                {
                    Object[] oi = ev.sampleNow();
                    if (oi != null) {
                        Object[] oo = new Object[oi.Length];
                        int j = 0;
                        for (int i = 0; i < oi.Length; i++)
                            if (f.apply((A)oi[i]))
                                oo[j++] = oi[i];
                        if (j == 0)
                            oo = null;
                        else
                        if (j < oo.Length) {
                            Object[] oo2 = new Object[j];
                            for (int i = 0; i < j; i++)
                                oo2[i] = oo[i];
                            oo = oo2;
                        }
                        return oo;
                    }
                    else
                        return null;
                }
            };
            Listener l = listen_(o.node, new ITransactionHandler<A>() {
        	    public void run(Transaction trans2, A a) {
	                if (f.apply(a)) o.send(trans2, a);
	            }
            });
            return o.addCleanup(l);
        }

        /**
         * Filter out any event occurrences whose value is a Java null pointer.
         */
        public Event<A> filterNotNull()
        {
            return filter(new Lambda1<A,Boolean>() {
        	    public Boolean apply(A a) { return a != null; }
            });
        }

        /**
         * Let event occurrences through only when the behavior's value is True.
         * Note that the behavior's value is as it was at the start of the transaction,
         * that is, no state changes from the current transaction are taken into account.
         */
        public Event<A> gate(Behavior<Boolean> bPred)
        {
            return snapshot(bPred, new Lambda2<A,Boolean,A>() {
        	    public A apply(A a, Boolean pred) { return pred ? a : null; }
            }).filterNotNull();
        }

        /**
         * Transform an event with a generalized state loop (a mealy machine). The function
         * is passed the input and the old state and returns the new state and output value.
         */
        public <B,S> Event<B> collect(S initState, Lambda2<A, S, Tuple2<B, S>> f)
        {
            Event<A> ea = this;
            EventLoop<S> es = new EventLoop<S>();
            Behavior<S> s = es.hold(initState);
            Event<Tuple2<B,S>> ebs = ea.snapshot(s, f);
            Event<B> eb = ebs.map(new Lambda1<Tuple2<B,S>,B>() {
                public B apply(Tuple2<B,S> bs) { return bs.a; }
            });
            Event<S> es_out = ebs.map(new Lambda1<Tuple2<B,S>,S>() {
                public S apply(Tuple2<B,S> bs) { return bs.b; }
            });
            es.loop(es_out);
            return eb;
        }

        /**
         * Accumulate on input event, outputting the new state each time.
         */
        public <S> Behavior<S> accum(S initState, Lambda2<A, S, S> f)
        {
            Event<A> ea = this;
            EventLoop<S> es = new EventLoop<S>();
            Behavior<S> s = es.hold(initState);
            Event<S> es_out = ea.snapshot(s, f);
            es.loop(es_out);
            return es_out.hold(initState);
        }

        /**
         * Throw away all event occurrences except for the first one.
         */
        public Event<A> once()
        {
            // This is a bit long-winded but it's efficient because it deregisters
            // the listener.
            Event<A> ev = this;
            Listener[] la = new Listener[1];
            EventSink<A> o = new EventSink<A>() {
                protected override Object[] sampleNow()
                {
                    Object[] oi = ev.sampleNow();
                    Object[] oo = oi;
                    if (oo != null) {
                        if (oo.Length > 1)
                            oo = new Object[] { oi[0] };
                        if (la[0] != null) {
                            la[0].unlisten();
                            la[0] = null;
                        }
                    }
                    return oo;
                }
            };
            la[0] = ev.listen_(o.node, new ITransactionHandler<A>() {
        	    public void run(Transaction trans, A a) {
	                o.send(trans, a);
	                if (la[0] != null) {
	                    la[0].unlisten();
	                    la[0] = null;
	                }
	            }
            });
            return o.addCleanup(la[0]);
        }

        Event<A> addCleanup(Listener cleanup)
        {
            finalizers.Add(cleanup);
            return this;
        }

	    protected override void finalize() {
		    foreach (Listener l in finalizers)
			    l.unlisten();
	    }
    }

    class CoalesceHandler<A> : ITransactionHandler<A>
    {
	    public CoalesceHandler(Lambda2<A,A,A> f, EventSink<A> o)
	    {
	        this.f = f;
	        this.o = o;
	    }
	    private Lambda2<A,A,A> f;
	    private EventSink<A> o;
        private bool accumValid = false;
        private A accum;
        public override void run(Transaction trans1, A a) {
            if (accumValid)
                accum = f.apply(accum, a);
            else {
        	    CoalesceHandler<A> thiz = this;
                trans1.prioritized(o.node, new IHandler<Transaction>() {
            	    public void run(Transaction trans2) {
                        o.send(trans2, thiz.accum);
                        thiz.accumValid = false;
                        thiz.accum = null;
                    }
                });
                accum = a;
                accumValid = true;
            }
        }
    }

}