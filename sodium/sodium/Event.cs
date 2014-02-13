namespace sodium {

    using System;
    using System.Collections.Generic;

    public class Event<A> : IDisposable {
	    private sealed class ListenerImplementation<A> : Listener {
		    /**
		     * It's essential that we keep the listener alive while the caller holds
		     * the Listener, so that the finalizer doesn't get triggered.
		     */
		    private readonly Event<A> evt;
		    private readonly ITransactionHandler<A> action;
		    private readonly Node target;

		    public ListenerImplementation(Event<A> evt, ITransactionHandler<A> action, Node target) {
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
	    public Node node = new Node(0L);
	    protected readonly List<A> firings = new List<A>();
        private bool _disposed;

	    /**
	     * An event that never fires.
	     */
	    public Event() {
	    }

	    protected virtual Object[] sampleNow() { return null; }

	    /**
	     * Listen for firings of this event. The returned Listener has an unlisten()
	     * method to cause the listener to be removed. This is the observer pattern.
         */
	    public Listener listen(IHandler<A> action) {
            return listen_(Node.NULL, new TmpTransHandler1<A>(action));
	    }

        private class TmpTransHandler1<A> : ITransactionHandler<A>
        {
            private IHandler<A> action;

            public TmpTransHandler1(IHandler<A> action)
            {
                this.action = action;
            }

            public void run(Transaction trans, A a)
            {
                action.run(a);
            }
        }

        public Listener listen_(Node target, ITransactionHandler<A> action) {
		    return Transaction.apply(new ListenerApplier<A>(this, target, action));
	    }

        private class ListenerApplier<A> : ILambda1<Transaction, Listener>
        {
            private Event<A> listener;
            private Node target;
            private ITransactionHandler<A> action;

            public ListenerApplier(Event<A> listener, Node target, ITransactionHandler<A> action)
            {
                this.listener = listener;
                this.target = target;
                this.action = action;
            }

            public Listener apply(Transaction trans)
            {
                return listener.listen(target, trans, action, false);
            }
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
	    public Event<B> map<B>(ILambda1<A,B> f)
	    {
	        Event<A> ev = this;
	        EventSink<B> o = new TmpEventtSink7<B>(ev, f);
            Listener l = listen_(o.node, new TmpTransHandler7<A, B>(o,f));
            return o.addCleanup(l);
	    }

        private class TmpTransHandler7<A, B> : ITransactionHandler<A>
        {
            private EventSink<B> o;
            private ILambda1<A, B> f;

            public TmpTransHandler7(EventSink<B> o, ILambda1<A, B> f)
            {
                this.o = o;
                this.f = f;
            }

            public void run(Transaction trans, A a)
            {
                o.send(trans, f.apply(a));
            }
        }

        private class TmpEventtSink7<B> : EventSink<B>
        {
            private Event<A> ev;
            private ILambda1<A, B> f;

            public TmpEventtSink7(Event<A> ev, ILambda1<A,B> f)
            {
                this.ev = ev;
                this.f = f;
            }

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
        }

	    /**
	     * Create a behavior with the specified initial value, that gets updated
         * by the values coming through the event. The 'current value' of the behavior
         * is notionally the value as it was 'at the start of the transaction'.
         * That is, state updates caused by event firings get processed at the end of
         * the transaction.
         */
	    public Behavior<A> hold(A initValue) {
		    return Transaction.apply(new BehaviorBuilder<A>(this, initValue));
	    }

        private class BehaviorBuilder<A> : ILambda1<Transaction, Behavior<A>>
        {
            private Event<A> evt;
            private A initValue;

            public BehaviorBuilder(Event<A> evt, A initValue)
            {
                this.evt = evt;
                this.initValue = initValue;
            }

            public Behavior<A> apply(Transaction trans)
            {
                return new Behavior<A>(evt.lastFiringOnly(trans), initValue);
            }
        }

        /**
	     * Variant of snapshot that throws away the event's value and captures the behavior's.
	     */
	    public Event<B> snapshot<B>(Behavior<B> beh)
	    {
	        return snapshot(beh, new SnapshotBehavior<A,B>());
	    }

        private class SnapshotBehavior<A,B> : ILambda2<A,B,B>
        {
            public B apply(A a, B b)
            {
                return b;
            }
        }

	    /**
	     * Sample the behavior at the time of the event firing. Note that the 'current value'
         * of the behavior that's sampled is the value as at the start of the transaction
         * before any state changes of the current transaction are applied through 'hold's.
         */
	    public Event<C> snapshot<B,C>(Behavior<B> b, ILambda2<A,B,C> f)
	    {
	        Event<A> ev = this;
		    EventSink<C> o = new TmpEventSink1<A,B,C>(ev, f, b);
            Listener l = listen_(o.node, new TmpTransHandler5<A,B,C>(o, f, b));
            return o.addCleanup(l);
	    }

        private class TmpEventSink1<A,B,C> : EventSink<C>
        {
            private Event<A> ev;
            private ILambda2<A, B, C> f;
            private Behavior<B> b;

            public TmpEventSink1(Event<A> ev, ILambda2<A, B, C> f, Behavior<B> b)
            {
                this.ev = ev;
                this.f = f;
                this.b = b;
            }

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
        }

        private class TmpTransHandler5<A,B,C> : ITransactionHandler<A>
        {
            private EventSink<C> o;
            private ILambda2<A, B, C> f;
            private Behavior<B> b;

            public TmpTransHandler5(EventSink<C> o, ILambda2<A, B, C> f, Behavior<B> b)
            {
                this.o = o;
                this.f = f;
                this.b = b;
            }

            public void run(Transaction trans, A a)
            {
                o.send(trans, f.apply(a, b.sample()));
            }
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
	    public static Event<A> merge<A>(Event<A> ea, Event<A> eb)
	    {
	        EventSink<A> o = new TmpEventSink2<A>(ea, eb);
            ITransactionHandler<A> h = new TmpTransHandler2<A>(o);
            Listener l1 = ea.listen_(o.node, h);
            Listener l2 = eb.listen_(o.node, h);
            return o.addCleanup(l1).addCleanup(l2);
	    }

        private class TmpEventSink2<A> : EventSink<A>
        {
            private Event<A> ea; 
            private Event<A> eb;

            public TmpEventSink2(Event<A> ea, Event<A> eb)
            {
                this.ea = ea;
                this.eb = eb;
            }

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
        }

        private class TmpTransHandler2<A> : ITransactionHandler<A>
        {
            private EventSink<A> o; 

            public TmpTransHandler2(EventSink<A> o)
            {
                this.o = o;
            }

            public void run(Transaction trans, A a)
            {
                o.send(trans, a);
            }
        }

        /**
	     * Push each event occurrence onto a new transaction.
	     */
	    public Event<A> delay()
	    {
	        EventSink<A> o = new EventSink<A>();
	        Listener l1 = listen_(o.node, new TmpTransHandler3<A>(o));
	        return o.addCleanup(l1);
	    }

        private class TmpTransHandler3<A> : ITransactionHandler<A>
        {
            private EventSink<A> o;

            public TmpTransHandler3(EventSink<A> o)
            {
                this.o = o;
            }

            public void run(Transaction trans, A a)
            {
                trans.post(new Runnable(() =>
                {
                    Transaction trans2 = new Transaction();
                    try
                    {
                        o.send(trans2, a);
                    }
                    finally
                    {
                        trans2.close();
                    }
                }));
            }

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
	    public Event<A> coalesce(ILambda2<A,A,A> f)
	    {
	        return Transaction.apply(new Tmp2<A>(this, f));
	    }

        private class Tmp2<A> : ILambda1<Transaction, Event<A>>
        {
            private Event<A> evt;
            private ILambda2<A, A, A> f;

            public Tmp2(Event<A> evt, ILambda2<A, A, A> f)
            {
                this.evt = evt;
                this.f = f;
            }

            public Event<A> apply(Transaction trans)
            {
                return evt.coalesce(trans, f);
            }
        }

        Event<A> coalesce(Transaction trans1, ILambda2<A,A,A> f)
	    {
	        Event<A> ev = this;
	        EventSink<A> o = new TmpEventSink3<A>(ev, f);
            ITransactionHandler<A> h = new CoalesceHandler<A>(f, o);
            Listener l = listen(o.node, trans1, h, false);
            return o.addCleanup(l);
        }

        private class TmpEventSink3<A> : EventSink<A>
        {
            private Event<A> ev;
            private ILambda2<A, A, A> f;

            public TmpEventSink3(Event<A> ev, ILambda2<A,A,A> f)
            {
                this.ev = ev;
                this.f = f;
            }

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
        }

        /**
         * Clean up the output by discarding any firing other than the last one. 
         */
        Event<A> lastFiringOnly(Transaction trans)
        {
            return coalesce(trans, new Tmp4<A>());
        }

        private class Tmp4<a> : ILambda2<A,A,A>
        {
            public A apply(A first, A second)
            {
                return second;
            }
        }

        /**
         * Merge two streams of events of the same type, combining simultaneous
         * event occurrences.
         *
         * In the case where multiple event occurrences are simultaneous (i.e. all
         * within the same transaction), they are combined using the same logic as
         * 'coalesce'.
         */
        public static Event<A> mergeWith<A>(ILambda2<A,A,A> f, Event<A> ea, Event<A> eb)
        {
            return merge(ea, eb).coalesce(f);
        }

        /**
         * Only keep event occurrences for which the predicate returns true.
         */
        public Event<A> filter(ILambda1<A,Boolean> f)
        {
            Event<A> ev = this;
            EventSink<A> o = new TmpEventSink5<A>(ev, f);
            Listener l = listen_(o.node, new TmpTransHandler4<A>(f, o));
            return o.addCleanup(l);
        }

        private class TmpEventSink5<A> : EventSink<A>
        {
            private Event<A> ev;
            private ILambda1<A, Boolean> f;

            public TmpEventSink5(Event<A> ev, ILambda1<A, Boolean> f)
            {
                this.ev = ev;
                this.f = f;
            }

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
        }

        private class TmpTransHandler4<A> : ITransactionHandler<A>
        {
            private ILambda1<A, Boolean> f;
            private EventSink<A> o;

            public TmpTransHandler4(ILambda1<A, Boolean> f, EventSink<A> o)
            {
                this.f = f;
                this.o = o;
            }

            public void run(Transaction trans, A a)
            {
                if (f.apply(a)) o.send(trans, a);
            }
        }

        /**
         * Filter out any event occurrences whose value is a Java null pointer.
         */
        public Event<A> filterNotNull()
        {
            return filter(new Tmp5<A>());
        }

        private class Tmp5<A> : ILambda1<A,Boolean>
        {
            public bool apply(A a)
            {
                return a != null;
            }
        }

        /**
         * Let event occurrences through only when the behavior's value is True.
         * Note that the behavior's value is as it was at the start of the transaction,
         * that is, no state changes from the current transaction are taken into account.
         */
        public Event<A> gate(Behavior<Boolean> bPred)
        {
            return snapshot(bPred, new Tmp6<A>()).filterNotNull();
        }

        private class Tmp6<A> :  ILambda2<A,Boolean,A>
        {
            public A apply(A a, bool pred)
            {
                return pred ? a : default(A);
            }
        }

        /**
         * Transform an event with a generalized state loop (a mealy machine). The function
         * is passed the input and the old state and returns the new state and output value.
         */
        public Event<B> collect<B,S>(S initState, ILambda2<A, S, Tuple2<B, S>> f)
        {
            Event<A> ea = this;
            EventLoop<S> es = new EventLoop<S>();
            Behavior<S> s = es.hold(initState);
            Event<Tuple2<B,S>> ebs = ea.snapshot(s, f);
            Event<B> eb = ebs.map(new Tmp7<A,B,S>());
            Event<S> es_out = ebs.map(new Tmp8<A,B,S>());
            es.loop(es_out);
            return eb;
        }

        private class Tmp7<A,B,S> : ILambda1<Tuple2<B,S>,B>
        {
            public B apply(Tuple2<B, S> bs)
            {
                return bs.a;
            }
        }

        private class Tmp8<A,B,S> : ILambda1<Tuple2<B,S>,S>
        {
            public S apply(Tuple2<B, S> bs)
            {
                return bs.b;
            }
        }

        /**
         * Accumulate on input event, outputting the new state each time.
         */
        public Behavior<S> accum<S>(S initState, ILambda2<A, S, S> f)
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
            EventSink<A> o = new TmpEventSink4<A>(ev, la);
            la[0] = ev.listen_(o.node, new TmpTransHandler8<A>(o, la));
            return o.addCleanup(la[0]);
        }

        private class TmpEventSink4<A> : EventSink<A>
        {
            private Event<A> ev;
            private Listener[] la;

            public TmpEventSink4(Event<A> ev, Listener[] la)
            {
                this.ev = ev;
                this.la = la;
            }

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
        }

        private class TmpTransHandler8<A> : ITransactionHandler<A>
        {
            private EventSink<A> o;
            private Listener[] la;

            public TmpTransHandler8(EventSink<A> o, Listener[] la)
            {
                this.o = o;
                this.la = la;
            }

            public void run(Transaction trans, A a)
            {
                o.send(trans, a);
	                if (la[0] != null) {
	                    la[0].unlisten();
	                    la[0] = null;
	                }
            }
        }

        protected Event<A> addCleanup(Listener cleanup)
        {
            finalizers.Add(cleanup);
            return this;
        }

	    public void Dispose() {
            Dispose(true);

            // Call SupressFinalize in case a subclass implements a finalizer.
            GC.SuppressFinalize(this);
	    }

        protected virtual void Dispose(bool disposing)
        {
            // If you need thread safety, use a lock around these  
              // operations, as well as in your methods that use the resource. 
              if (!_disposed) {
                 if (disposing) {
                    foreach (Listener l in finalizers)
			            l.unlisten();
                 }
                 
                 // Indicate that the instance has been disposed.
                 _disposed = true;   
              }
        }
    }

    class CoalesceHandler<A> : ITransactionHandler<A>
    {
        private ILambda2<A, A, A> f;
        private EventSink<A> o;
        private bool accumValid = false;
        private A accum;

	    public CoalesceHandler(ILambda2<A,A,A> f, EventSink<A> o)
	    {
	        this.f = f;
	        this.o = o;
	    }

        public void run(Transaction trans1, A a) {
            if (accumValid)
                accum = f.apply(accum, a);
            else {
        	    CoalesceHandler<A> thiz = this;
                trans1.prioritized(o.node, new TransHandler<A>(thiz, o));
                accum = a;
                accumValid = true;
            }
        }

        private class TransHandler<A> : IHandler<Transaction>
        {
            private CoalesceHandler<A> h;
            private EventSink<A> o;

            public TransHandler(CoalesceHandler<A> h, EventSink<A> o)
            {
                this.h = h;
                this.o = o;
            }

            public void run(Transaction trans)
            {
                o.send(trans, h.accum);
                        h.accumValid = false;
                        h.accum = default(A);
            }
        }
    }
}