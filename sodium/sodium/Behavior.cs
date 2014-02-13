namespace sodium {

    using System;

    public class Behavior<A> : IDisposable {
	    protected Event<A> evt;
	    public A _value;
	    A valueUpdate;
	    private Listener cleanup;
        private bool _disposed;

	    /**
	     * A behavior with a constant value.
	     */
        public Behavior(A value)
        {
    	    this.evt = new Event<A>();
    	    this._value = value;
        }

        public Behavior(Event<A> evt, A initValue)
        {
    	    this.evt = evt;
    	    this._value = initValue;
    	    Transaction.run(new TmpTransHandler1<A>(this, evt));
        }

        private class TmpTransHandler1<A> : IHandler<Transaction>
        {
            private Behavior<A> b;
            private Event<A> evt;

            public TmpTransHandler1(Behavior<A> b, Event<A> evt)
            {
                this.b = b;
                this.evt = evt;
            }

            public void run(Transaction trans1)
            {
                b.cleanup = evt.listen(Node.NULL, trans1, new TransHandler2<A>(this.b), false);
            }
        }

        private sealed class TransHandler2<A> : ITransactionHandler<A>
        {
            private Behavior<A> b;

            public TransHandler2(Behavior<A> b)
            {
                this.b = b;
            }

            public void run(Transaction trans, A a)
            {
                if (b.valueUpdate == null)
                {
                    trans.last(new Runnable(() =>
                    {
                        b._value = b.valueUpdate;
                        b.valueUpdate = default(A);
                    }));
                    b.valueUpdate = a;
                }
            }
        }

        /**
         * @return The value including any updates that have happened in this transaction.
         */
        A newValue()
        {
    	    return valueUpdate == null ? _value :  valueUpdate;
        }

        /**
         * Sample the behavior's current value.
         *
         * This should generally be avoided in favour of value().listen(..) so you don't
         * miss any updates, but in many circumstances it makes sense.
         *
         * It can be best to use it inside an explicit transaction (using Transaction.run()).
         * For example, a b.sample() inside an explicit transaction along with a
         * b.updates().listen(..) will capture the current value and any updates without risk
         * of missing any in between.
         */
        public A sample()
        {
            // Since pointers in Java are atomic, we don't need to explicitly create a
            // transaction.
            return _value;
        }

        /**
         * An evt that gives the updates for the behavior. If this behavior was created
         * with a hold, then updates() gives you an evt equivalent to the one that was held.
         */
        public Event<A> updates()
        {
    	    return evt;
        }

        /**
         * An evt that is guaranteed to fire once when you listen to it, giving
         * the current value of the behavior, and thereafter behaves like updates(),
         * firing for each update to the behavior's value.
         */
        public Event<A> value()
        {
            return Transaction.apply(new Tmp1<A>(this));
        }

        private class Tmp1<A> : ILambda1<Transaction, Event<A>>
        {
            private Behavior<A> b;

            public Tmp1(Behavior<A> b)
            {
                this.b = b;
            }

            public Event<A> apply(Transaction trans)
            {
                return b.value(trans);
            }
        }

        Event<A> value(Transaction trans1)
        {
            EventSink<A> o = new TmpEventSink1<A>(this);

            Listener l = evt.listen(o.node, trans1,
    		    new TmpTransHandler2<A>(o), false);
            return o.addCleanup(l)
                .lastFiringOnly(trans1);  // Needed in case of an initial value and an update
    	                                  // in the same transaction.
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

        private class TmpEventSink1<A> : EventSink<A>
        {
            private Behavior<A> b;

            public TmpEventSink1(Behavior<A> b)
            {
                this.b = b;
            }

            protected override Object[] sampleNow()
            {
                return new Object[] { this.b.sample() };
            }
        }

        /**
         * Transform the behavior's value according to the supplied function.
         */
	    public Behavior<B> map<B>(ILambda1<A,B> f)
	    {
		    return updates().map(f).hold(f.apply(sample()));
	    }

	    /**
	     * Lift a binary function into behaviors.
	     */
	    public Behavior<C> lift<B,C>(ILambda2<A,B,C> f, Behavior<B> b)
	    {
	        ILambda1<A, ILambda1<B, C>> ffa = new Tmp2<A, B, C>(f);
		    Behavior<ILambda1<B,C>> bf = map(ffa);
		    return apply(bf, b);
	    }

        public class Tmp2<A, B, C> : ILambda1<A, ILambda1<B, C>>
        {
            private ILambda2<A, B, C> f;

            public Tmp2(ILambda2<A, B, C> f)
            {
                this.f = f;
            }

            public ILambda1<B, C> apply(A a)
            {
                return new Tmp3<A, B, C>(f, a);
            }
        }

        private class Tmp3<A,B,C> : ILambda1<B,C>
        {
            private ILambda2<A, B, C> f;
            private A a;

            public Tmp3(ILambda2<A, B, C> f, A a)
            {
                this.f = f;
                this.a = a;
            }

            public C apply(B b)
            {
                return f.apply(a,b);
            }
        }

        /**
	     * Lift a binary function into behaviors.
	     */
	    public static Behavior<C> lift<A,B,C>(ILambda2<A,B,C> f, Behavior<A> a, Behavior<B> b)
	    {
		    return a.lift(f, b);
	    }

	    /**
	     * Lift a ternary function into behaviors.
	     */
	    public Behavior<D> lift<B,C,D>(ILambda3<A,B,C,D> f, Behavior<B> b, Behavior<C> c)
	    {
	        ILambda1<A, ILambda1<B, ILambda1<C, D>>> ffa = new Tmp4<A, B, C, D>(f);
		    Behavior<ILambda1<B, ILambda1<C, D>>> bf = map(ffa);
		    return apply(apply(bf, b), c);
	    }

        private class Tmp4<A, B, C, D> : ILambda1<A, ILambda1<B, ILambda1<C,D>>>
        {
            private ILambda3<A, B, C, D> f;

            public Tmp4(ILambda3<A, B, C, D> f)
            {
                this.f = f;
            }

            public ILambda1<B, ILambda1<C, D>> apply(A a)
            {
                return new Tmp5<A, B, C, D>(a, f);
            }
        }

        private class Tmp5<A,B,C,D> : ILambda1<B, ILambda1<C,D>>
        {
            private A a;
            private ILambda3<A, B, C, D> f;

            public Tmp5(A a, ILambda3<A, B, C, D> f)
            {
                this.a = a;
                this.f = f;
            }

            public ILambda1<C, D> apply(B b)
            {
                return new Tmp6<A, B, C, D>(a, b, f);
            }
        }

        private class Tmp6<A,B,C,D> : ILambda1<C,D>
        {
            private A a;
            private B b;
            private ILambda3<A, B, C, D> f;

            public Tmp6(A a, B b, ILambda3<A, B, C, D> f)
            {
                this.a = a;
                this.b = b;
                this.f = f;
            }

            public D apply(C c)
            {
                return f.apply(a,b,c);
            }
        }

        /**
	     * Lift a ternary function into behaviors.
	     */
	    public static Behavior<D> lift<A,B,C,D>(ILambda3<A,B,C,D> f, Behavior<A> a, Behavior<B> b, Behavior<C> c)
	    {
		    return a.lift(f, b, c);
	    }

	    /**
	     * Apply a value inside a behavior to a function inside a behavior. This is the
	     * primitive for all function lifting.
	     */
	    public static Behavior<B> apply<A,B>(Behavior<ILambda1<A,B>> bf, Behavior<A> ba)
	    {
		    EventSink<B> o = new EventSink<B>();

	        IHandler<Transaction> h = new Tmp7<A,B>(o, bf, ba);

            Listener l1 = bf.updates().listen_(o.node, new Tmp9<A,B>(h));
            Listener l2 = ba.updates().listen_(o.node, new Tmp10<A,B>(h));
            return o.addCleanup(l1).addCleanup(l2).hold(bf.sample().apply(ba.sample()));
	    }

        private class Tmp9<A,B> : ITransactionHandler<ILambda1<A,B>>
        {
            private IHandler<Transaction> h;

            public Tmp9(IHandler<Transaction> h)
            {
                this.h = h;
            }

            public void run(Transaction trans, ILambda1<A, B> a)
            {
                h.run(trans);
            }
        }

        private class Tmp10<A,B> : ITransactionHandler<A>
        {
            private IHandler<Transaction> h;

            public Tmp10(IHandler<Transaction> h)
            {
                this.h = h;
            }

            public void run(Transaction trans, A a)
            {
                h.run(trans);
            }
        }

        private class Tmp7<A,B> : IHandler<Transaction>
        {
            public bool fired = false;
            private EventSink<B> o;
            private Behavior<ILambda1<A, B>> bf;
            private Behavior<A> ba;

            public Tmp7(EventSink<B> o, Behavior<ILambda1<A,B>> bf, Behavior<A> ba)
            {
                this.o = o;
                this.bf = bf;
                this.ba = ba;
            }

            public void run(Transaction trans)
            {
                if (fired) 
                    return;

                fired = true;
                trans.prioritized(o.node, new Tmp8<A,B>(o, bf, ba, this));
            }
        }

        private class Tmp8<A,B> : IHandler<Transaction>
        {
            private EventSink<B> o;
            private Behavior<ILambda1<A, B>> bf;
            private Behavior<A> ba;
            private Tmp7<A, B> tmp7;

            public Tmp8(EventSink<B> o, Behavior<ILambda1<A, B>> bf, Behavior<A> ba, Tmp7<A, B> tmp7)
            {
                this.o = o;
                this.bf = bf;
                this.ba = ba;
                this.tmp7 = tmp7;
            }

            public void run(Transaction trans)
            {
                o.send(trans, bf.newValue().apply(ba.newValue()));
                tmp7.fired = false;
            }
        }

        /**
	     * Unwrap a behavior inside another behavior to give a time-varying behavior implementation.
	     */
	    public static Behavior<A> switchB<A>(Behavior<Behavior<A>> bba)
	    {
	        A za = bba.sample().sample();
	        EventSink<A> o = new EventSink<A>();
	        ITransactionHandler<Behavior<A>> h = new Tmp11<A>(o);
            Listener l1 = bba.value().listen_(o.node, h);
            return o.addCleanup(l1).hold(za);
	    }
	
	    /**
	     * Unwrap an evt inside a behavior to give a time-varying evt implementation.
	     */
	    public static Event<A> switchE<A>(Behavior<Event<A>> bea)
	    {
            return Transaction.apply(new Tmp13<A>(bea));
        }

        private class Tmp13<A> : ILambda1<Transaction, Event<A>>
        {
            private Behavior<Event<A>> bea;

            public Tmp13(Behavior<Event<A>> bea)
            {
                this.bea = bea;
            }

            public Event<A> apply(Transaction trans)
            {
                return switchE(trans, bea);
            }
        }

        private class Tmp11<A> : ITransactionHandler<Behavior<A>>, IDisposable
        {
            private Listener currentListener;
            private EventSink<A> o;
            private bool _disposed;

            public Tmp11(EventSink<A> o)
            {
                this.o = o;
            }

            public void run(Transaction trans, Behavior<A> a)
            {
                // Note: If any switch takes place during a transaction, then the
                // value().listen will always cause a sample to be fetched from the
                // one we just switched to. The caller will be fetching our output
                // using value().listen, and value() throws away all firings except
                // for the last one. Therefore, anything from the old input behaviour
                // that might have happened during this transaction will be suppressed.
                if (currentListener != null)
                    currentListener.unlisten();
                currentListener = a.value(trans).listen(o.node, trans, new Tmp12<A>(o), false);
            }

            public void Dispose()
            {
                Dispose(true);

                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disponsing) 
            {
                if (!_disposed)
                {
                    if (disponsing)
                    {
                        if (currentListener != null)
                            currentListener.unlisten();
                    }

                    _disposed = true;
                }
            }
        }

        private class Tmp12<A> : ITransactionHandler<A>
        {
            private EventSink<A> o;

            public Tmp12(EventSink<A> o)
            {
                this.o = o;
            }

            public void run(Transaction trans, A a)
            {
                o.send(trans, a);
            }
        }

        private static Event<A> switchE<A>(Transaction trans1, Behavior<Event<A>> bea)
	    {
            EventSink<A> o = new EventSink<A>();
            ITransactionHandler<A> h2 = new Tmp14<A>(o);
            ITransactionHandler<Event<A>> h1 = new Tmp15<A>(o, bea, trans1, h2);
            Listener l1 = bea.updates().listen(o.node, trans1, h1, false);
            return o.addCleanup(l1);
	    }

        private class Tmp14<A> : ITransactionHandler<A>
        {
            private EventSink<A> o;

            public Tmp14(EventSink<A> o)
            {
                this.o = o;
            }

            public void run(Transaction trans, A a)
            {
                o.send(trans, a);
            }
        }

        private class Tmp15<A> : ITransactionHandler<Event<A>>, IDisposable
        {
            private EventSink<A> o;
            private Listener currentListener;
            private ITransactionHandler<A> h2;
            private bool _disposed;

            public Tmp15(EventSink<A> o, Behavior<Event<A>> bea, Transaction trans, ITransactionHandler<A> h2)
            {
                this.currentListener = bea.sample().listen(o.node, trans, h2, false);
                this.h2 = h2;
            }

            public void run(Transaction trans, Event<A> a)
            {
                trans.last(new Runnable(() =>
                {
                    if (currentListener != null)
                        currentListener.unlisten();
                    currentListener = a.listen(o.node, trans, h2, true);
                }));
            }

            public void Dispose()
            {
                Dispose(true);

                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    if (disposing)
                    {
                        if (currentListener != null)
                            currentListener.unlisten();
                    }

                    _disposed = true;
                }
            }
        }

        /**
         * Transform a behavior with a generalized state loop (a mealy machine). The function
         * is passed the input and the old state and returns the new state and output value.
         */
        public Behavior<B> collect<B,S>(S initState, ILambda2<A, S, Tuple2<B, S>> f)
        {
            Event<A> ea = updates().coalesce(new Tmp16<A>());
            A za = sample();
            Tuple2<B, S> zbs = f.apply(za, initState);
            EventLoop<Tuple2<B,S>> ebs = new EventLoop<Tuple2<B,S>>();
            Behavior<Tuple2<B,S>> bbs = ebs.hold(zbs);
            Behavior<S> bs = bbs.map(new Tmp17<A,B,S>());
            Event<Tuple2<B,S>> ebs_out = ea.snapshot(bs, f);
            ebs.loop(ebs_out);
            return bbs.map(new Tmp18<A,B,S>());
        }

        private class Tmp16<A> : ILambda2<A,A,A>
        {
            public A apply(A a, A b)
            {
                return b;
            }
        }

        private class Tmp17<A,B,S> : ILambda1<Tuple2<B,S>,S>
        {
            public S apply(Tuple2<B, S> x)
            {
                return x.b;
            }
        }

        private class Tmp18<A,B,S> : ILambda1<Tuple2<B,S>,B>
        {
            public B apply(Tuple2<B, S> x)
            {
                return x.a;
            }
        }

        public void Dispose() {
            Dispose(true);

            // Call SupressFinalize in case a subclass implements a finalizer.
            GC.SuppressFinalize(this);
	    }

        private void Dispose(bool disposing)
        {
            // If you need thread safety, use a lock around these  
              // operations, as well as in your methods that use the resource. 
              if (!_disposed) {
                 if (disposing) {
                    if (cleanup != null)
                        cleanup.unlisten();
                 }
                 
                 // Indicate that the instance has been disposed.
                 _disposed = true;   
              }
        }
    }
}