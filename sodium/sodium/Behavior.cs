namespace sodium
{
    using System;

    public class Behavior<A> {
	    protected Event<A> _event;
	    A _value;
	    A valueUpdate;
	    private Listener cleanup;

	    ///
	    /// A behavior with a constant value.
	    ///
        public Behavior(A value)
        {
    	    this._event = new Event<A>();
    	    this._value = value;
        }

        internal Behavior(Event<A> evt, A initValue)
        {
    	    this._event = evt;
    	    this._value = initValue;
            Behavior<A> thiz = this;

            Transaction.run(new HandlerImpl<Transaction>(t1 =>
            {
                var handler = new TransactionHandlerImpl<A>((t2, a) =>
                { 
                    if (thiz.valueUpdate == null)
                    {
                        t2.last(new RunnableImpl(() =>
                        {
                            thiz._value = thiz.valueUpdate;
                            var v = default(A); // TODO - used to be set to null
                            thiz.valueUpdate = v;
                        }));
                    }
                    this.valueUpdate = a;

                });
                this.cleanup = evt.listen(Node.NULL, t1, handler, false);
            }));
        }

        ///
        /// @return The value including any updates that have happened in this transaction.
        ///
        A newValue()
        {
    	    return valueUpdate == null ? _value :  valueUpdate;
        }

        ///
        /// Sample the behavior's current value.
        ///
        /// This should generally be avoided in favour of value().listen(..) so you don't
        /// miss any updates, but in many circumstances it makes sense.
        ///
        /// It can be best to use it inside an explicit transaction (using Transaction.run()).
        /// For example, a b.sample() inside an explicit transaction along with a
        /// b.updates().listen(..) will capture the current value and any updates without risk
        /// of missing any in between.
        ///
        public A sample()
        {
            // Since pointers in Java are atomic, we don't need to explicitly create a
            // transaction.
            return _value;
        }

        ///
        /// An event that gives the updates for the behavior. If this behavior was created
        /// with a hold, then updates() gives you an event equivalent to the one that was held.
        ///
        public Event<A> updates()
        {
    	    return _event;
        }

        ///
        /// An event that is guaranteed to fire once when you listen to it, giving
        /// the current value of the behavior, and thereafter behaves like updates(),
        /// firing for each update to the behavior's value.
        ///
        public Event<A> value()
        {
            return Transaction.apply(new Lambda1Impl<Transaction, Event<A>>((t) => { return value(t); }));
        }

        Event<A> value(Transaction trans1)
        {
    	    EventSink<A> out_ = new ValueEventSink<A>(this);
            Listener l = _event.listen(out_.node, trans1,
    		    new TransactionHandlerImpl<A>((t2,a) => { out_.send(t2, a); }), false);
            return out_.addCleanup(l)
                .lastFiringOnly(trans1);  // Needed in case of an initial value and an update
    	                                  // in the same transaction.
        }

        private class ValueEventSink<A> : EventSink<A>
        {
            private Behavior<A> _behavior; 

            public ValueEventSink(Behavior<A> behavior)
            {
                _behavior = behavior;
            }

            protected internal override Object[] sampleNow()
            {
                return new Object[] { _behavior.sample() };
            }
        }

        /// <summary>
        /// Overload of map that accepts a Func<A,B> to support C# lambdas
        /// </summary>
        /// <typeparam name="B"></typeparam>
        /// <param name="f"></param>
        /// <returns></returns>
        public Behavior<B> map<B>(Func<A,B> f)
        {
            return map(new Lambda1Impl<A, B>(f));
        }
        
        ///
        /// Transform the behavior's value according to the supplied function.
        ///
        public Behavior<B> map<B>(Lambda1<A, B> f)
	    {
		    return updates().map(f).hold(f.apply(sample()));
	    }

	    ///
	    /// Lift a binary function into behaviors.
	    ///
	    public  Behavior<C> lift<B,C>(Lambda2<A,B,C> f, Behavior<B> b)
	    {
	        Lambda1<A, Lambda1<B, C>> ffa = null;
            //Lambda1<A, Lambda1<B,C>> ffa = new Lambda1<A, Lambda1<B,C>>() {
            //    public Lambda1<B,C> apply(final A aa) {
            //        return new Lambda1<B,C>() {
            //            public C apply(B bb) {
            //                return f.apply(aa,bb);
            //            }
            //        };
            //    }
            //};
		    Behavior<Lambda1<B,C>> bf = map(ffa);
		    return apply(bf, b);
	    }

        /// <summary>
        /// Overload of lift that accepts binary function Func<A,B,C> f and two behaviors, to enable C# lambdas
        /// </summary>
        /// <typeparam name="A"></typeparam>
        /// <typeparam name="B"></typeparam>
        /// <typeparam name="C"></typeparam>
        /// <param name="f"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Behavior<C> lift<A,B,C>(Func<A,B,C> f, Behavior<A> a, Behavior<B> b)
        {
            return lift<A,B,C>(new Lambda2Impl<A,B,C>(f), a, b);
        }

        ///
	    /// Lift a binary function into behaviors.
	    ///
	    public static Behavior<C> lift<A,B,C>(Lambda2<A,B,C> f, Behavior<A> a, Behavior<B> b)
	    {
		    return a.lift(f, b);
	    }

	    ///
	    /// Lift a ternary function into behaviors.
	    ///
	    public Behavior<D> lift<B,C,D>(Lambda3<A,B,C,D> f, Behavior<B> b, Behavior<C> c)
	    {
	        Lambda1<A, Lambda1<B, Lambda1<C, D>>> ffa = null;
            //Lambda1<A, Lambda1<B, Lambda1<C,D>>> ffa = new Lambda1<A, Lambda1<B, Lambda1<C,D>>>() {
            //    public Lambda1<B, Lambda1<C,D>> apply(final A aa) {
            //        return new Lambda1<B, Lambda1<C,D>>() {
            //            public Lambda1<C,D> apply(final B bb) {
            //                return new Lambda1<C,D>() {
            //                    public D apply(C cc) {
            //                        return f.apply(aa,bb,cc);
            //                    }
            //                };
            //            }
            //        };
            //    }
            //};
		    Behavior<Lambda1<B, Lambda1<C, D>>> bf = map(ffa);
		    return apply(apply(bf, b), c);
	    }

	    ///
	    /// Lift a ternary function into behaviors.
	    ///
	    public static  Behavior<D> lift<A,B,C,D>(Lambda3<A,B,C,D> f, Behavior<A> a, Behavior<B> b, Behavior<C> c)
	    {
		    return a.lift(f, b, c);
	    }

	    ///
	    /// Apply a value inside a behavior to a function inside a behavior. This is the
	    /// primitive for all function lifting.
	    ///
	    public static Behavior<B> apply<A,B>(Behavior<Lambda1<A,B>> bf, Behavior<A> ba)
	    {
            //final EventSink<B> out = new EventSink<B>();

            //final Handler<Transaction> h = new Handler<Transaction>() {
            //    boolean fired = false;			
            //    @Override
            //    public void run(Transaction trans1) {
            //        if (fired) 
            //            return;

            //        fired = true;
            //        trans1.prioritized(out.node, new Handler<Transaction>() {
            //            public void run(Transaction trans2) {
            //                out.send(trans2, bf.newValue().apply(ba.newValue()));
            //                fired = false;
            //            }
            //        });
            //    }
            //};

            //Listener l1 = bf.updates().listen_(out.node, new TransactionHandler<Lambda1<A,B>>() {
            //    public void run(Transaction trans1, Lambda1<A,B> f) {
            //        h.run(trans1);
            //    }
            //});
            //Listener l2 = ba.updates().listen_(out.node, new TransactionHandler<A>() {
            //    public void run(Transaction trans1, A a) {
            //        h.run(trans1);
            //    }
            //});
            //return out.addCleanup(l1).addCleanup(l2).hold(bf.sample().apply(ba.sample()));
	        return null;
	    }

	    ///
	    /// Unwrap a behavior inside another behavior to give a time-varying behavior implementation.
	    ///
	    public static Behavior<A> switchB<A> (Behavior<Behavior<A>> bba)
	    {
	        A za = bba.sample().sample();
	        EventSink<A> out_ = new EventSink<A>();
	        SwitchHandler<A> h = new SwitchHandler<A>(out_);
            Listener l1 = bba.value().listen_(out_.node, h);
            return out_.addCleanup(l1).hold(za);
	    }

        private class SwitchHandler<A> : TransactionHandler<Behavior<A>>
        {
            private Listener currentListener;
            private EventSink<A> out_;

            public SwitchHandler(EventSink<A> o)
            {
                out_ = o;
            }

            public void run(Transaction trans2, Behavior<A> ba)
            {
                // Note: If any switch takes place during a transaction, then the
                // value().listen will always cause a sample to be fetched from the
                // one we just switched to. The caller will be fetching our output
                // using value().listen, and value() throws away all firings except
                // for the last one. Therefore, anything from the old input behaviour
                // that might have happened during this transaction will be suppressed.
                if (currentListener != null)
                    currentListener.unlisten();

                Event<A> ev = ba.value(trans2);
                currentListener = ev.listen(out_.node, trans2, new TransactionHandlerImpl<A>(Handler), false);
            }

            private void Handler(Transaction t3, A a)
            {
                out_.send(t3, a);
            }


            ~SwitchHandler()
            {
                if (currentListener != null)
                    currentListener.unlisten();
            }
        }
	
        /*
	    ///
	    /// Unwrap an event inside a behavior to give a time-varying event implementation.
	    ///
	    public static <A> Event<A> switchE(final Behavior<Event<A>> bea)
	    {
            return Transaction.apply(new Lambda1<Transaction, Event<A>>() {
        	    public Event<A> apply(final Transaction trans) {
                    return switchE(trans, bea);
        	    }
            });
        }

	    private static <A> Event<A> switchE(final Transaction trans1, final Behavior<Event<A>> bea)
	    {
            final EventSink<A> out = new EventSink<A>();
            final TransactionHandler<A> h2 = new TransactionHandler<A>() {
        	    public void run(Transaction trans2, A a) {
	                out.send(trans2, a);
	            }
            };
            TransactionHandler<Event<A>> h1 = new TransactionHandler<Event<A>>() {
                private Listener currentListener = bea.sample().listen(out.node, trans1, h2, false);

                @Override
                public void run(final Transaction trans2, final Event<A> ea) {
                    trans2.last(new Runnable() {
                	    public void run() {
	                        if (currentListener != null)
	                            currentListener.unlisten();
	                        currentListener = ea.listen(out.node, trans2, h2, true);
	                    }
                    });
                }

                @Override
                protected void finalize() throws Throwable {
                    if (currentListener != null)
                        currentListener.unlisten();
                }
            };
            Listener l1 = bea.updates().listen(out.node, trans1, h1, false);
            return out.addCleanup(l1);
	    }
        */

        ///
        /// Transform a behavior with a generalized state loop (a mealy machine). The function
        /// is passed the input and the old state and returns the new state and output value.
        ///
        public  Behavior<B> collect<B,S>(S initState, Lambda2<A, S, Tuple2<B, S>> f)
        {
            Event<A> ea = updates().coalesce(new Lambda2Impl<A, A, A>((a, b) => b));
            A za = sample();
            Tuple2<B, S> zbs = f.apply(za, initState);
            EventLoop<Tuple2<B,S>> ebs = new EventLoop<Tuple2<B,S>>();
            Behavior<Tuple2<B,S>> bbs = ebs.hold(zbs);
            Behavior<S> bs = bbs.map(new Lambda1Impl<Tuple2<B, S>, S>(x => x.b));
            Event<Tuple2<B,S>> ebs_out = ea.snapshot(bs, f);
            ebs.loop(ebs_out);
            return bbs.map(new Lambda1Impl<Tuple2<B, S>, B>(x => x.a));
        }

	    ~Behavior() {
	        if (cleanup != null)
                cleanup.unlisten();
	    }
        
    }
}