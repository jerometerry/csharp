namespace sodium {

    public class Behavior<A> {
	    protected Event<A> evt;
	    A value;
	    A valueUpdate;
	    private Listener cleanup;

	    /**
	     * A behavior with a constant value.
	     */
        public Behavior(A value)
        {
    	    this.evt = new Event<A>();
    	    this.value = value;
        }

        protected Behavior(Event<A> evt, A initValue)
        {
    	    this.evt = evt;
    	    this.value = initValue;
    	    Transaction.run(new IHandler<Transaction>() {
    		    public void run(Transaction trans1) {
	    		    Behavior.this.cleanup = evt.listen(Node.NULL, trans1, new ITransactionHandler<A>() {
	    			    public void run(Transaction trans2, A a) {
			    		    if (Behavior.this.valueUpdate == null) {
			    			    trans2.last(new Runnable() {
			    				    public void run() {
				    				    Behavior.this.value = Behavior.this.valueUpdate;
				    				    Behavior.this.valueUpdate = null;
				    			    }
			    			    });
			    		    }
			    		    Behavior.this.valueUpdate = a;
			    	    }
	    		    }, false);
    		    }
    	    });
        }

        /**
         * @return The value including any updates that have happened in this transaction.
         */
        A newValue()
        {
    	    return valueUpdate == null ? value :  valueUpdate;
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
            return value;
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
            return Transaction.apply(new ILambda1<Transaction, Event<A>>() {
        	    public Event<A> apply(Transaction trans) {
        		    return value(trans);
        	    }
            });
        }

        Event<A> value(Transaction trans1)
        {
    	    EventSink<A> o = new EventSink<A>() {
                protected override Object[] sampleNow()
                {
                    return new Object[] { sample() };
                }
    	    };
            Listener l = evt.listen(o.node, trans1,
    		    new ITransactionHandler<A>() {
	        	    public void run(Transaction trans2, A a) { o.send(trans2, a); }
	            }, false);
            return o.addCleanup(l)
                .lastFiringOnly(trans1);  // Needed in case of an initial value and an update
    	                                  // in the same transaction.
        }

        /**
         * Transform the behavior's value according to the supplied function.
         */
	    public <B> Behavior<B> map(ILambda1<A,B> f)
	    {
		    return updates().map(f).hold(f.apply(sample()));
	    }

	    /**
	     * Lift a binary function into behaviors.
	     */
	    public <B,C> Behavior<C> lift(ILambda2<A,B,C> f, Behavior<B> b)
	    {
		    ILambda1<A, ILambda1<B,C>> ffa = new ILambda1<A, ILambda1<B,C>>() {
			    public ILambda1<B,C> apply(A aa) {
				    return new ILambda1<B,C>() {
					    public C apply(B bb) {
						    return f.apply(aa,bb);
					    }
				    };
			    }
		    };
		    Behavior<ILambda1<B,C>> bf = map(ffa);
		    return apply(bf, b);
	    }

	    /**
	     * Lift a binary function into behaviors.
	     */
	    public static <A,B,C> Behavior<C> lift(ILambda2<A,B,C> f, Behavior<A> a, Behavior<B> b)
	    {
		    return a.lift(f, b);
	    }

	    /**
	     * Lift a ternary function into behaviors.
	     */
	    public <B,C,D> Behavior<D> lift(ILambda3<A,B,C,D> f, Behavior<B> b, Behavior<C> c)
	    {
		    ILambda1<A, ILambda1<B, ILambda1<C,D>>> ffa = new ILambda1<A, ILambda1<B, ILambda1<C,D>>>() {
			    public ILambda1<B, ILambda1<C,D>> apply(A aa) {
				    return new ILambda1<B, ILambda1<C,D>>() {
					    public ILambda1<C,D> apply(B bb) {
						    return new ILambda1<C,D>() {
							    public D apply(C cc) {
								    return f.apply(aa,bb,cc);
							    }
						    };
					    }
				    };
			    }
		    };
		    Behavior<ILambda1<B, ILambda1<C, D>>> bf = map(ffa);
		    return apply(apply(bf, b), c);
	    }

	    /**
	     * Lift a ternary function into behaviors.
	     */
	    public static <A,B,C,D> Behavior<D> lift(ILambda3<A,B,C,D> f, Behavior<A> a, Behavior<B> b, Behavior<C> c)
	    {
		    return a.lift(f, b, c);
	    }

	    /**
	     * Apply a value inside a behavior to a function inside a behavior. This is the
	     * primitive for all function lifting.
	     */
	    public static <A,B> Behavior<B> apply(Behavior<ILambda1<A,B>> bf, Behavior<A> ba)
	    {
		    EventSink<B> o = new EventSink<B>();

            IHandler<Transaction> h = new IHandler<Transaction>() {
                bool fired = false;			
                public override void run(Transaction trans1) {
                    if (fired) 
                        return;

                    fired = true;
                    trans1.prioritized(o.node, new IHandler<Transaction>() {
                	    public void run(Transaction trans2) {
                            o.send(trans2, bf.newValue().apply(ba.newValue()));
                            fired = false;
                        }
            	    });
                }
            };

            Listener l1 = bf.updates().listen_(o.node, new ITransactionHandler<ILambda1<A,B>>() {
        	    public void run(Transaction trans1, ILambda1<A,B> f) {
                    h.run(trans1);
                }
            });
            Listener l2 = ba.updates().listen_(o.node, new ITransactionHandler<A>() {
        	    public void run(Transaction trans1, A a) {
	                h.run(trans1);
	            }
            });
            return o.addCleanup(l1).addCleanup(l2).hold(bf.sample().apply(ba.sample()));
	    }

	    /**
	     * Unwrap a behavior inside another behavior to give a time-varying behavior implementation.
	     */
	    public static <A> Behavior<A> switchB(Behavior<Behavior<A>> bba)
	    {
	        A za = bba.sample().sample();
	        EventSink<A> o = new EventSink<A>();
            ITransactionHandler<Behavior<A>> h = new ITransactionHandler<Behavior<A>>() {
                private Listener currentListener;
                public override void run(Transaction trans2, Behavior<A> ba) {
                    // Note: If any switch takes place during a transaction, then the
                    // value().listen will always cause a sample to be fetched from the
                    // one we just switched to. The caller will be fetching our output
                    // using value().listen, and value() throws away all firings except
                    // for the last one. Therefore, anything from the old input behaviour
                    // that might have happened during this transaction will be suppressed.
                    if (currentListener != null)
                        currentListener.unlisten();
                    currentListener = ba.value(trans2).listen(o.node, trans2, new ITransactionHandler<A>() {
                	    public void run(Transaction trans3, A a) {
	                        o.send(trans3, a);
	                    }
                    }, false);
                }

                protected override void finalize() {
                    if (currentListener != null)
                        currentListener.unlisten();
                }
            };
            Listener l1 = bba.value().listen_(o.node, h);
            return o.addCleanup(l1).hold(za);
	    }
	
	    /**
	     * Unwrap an evt inside a behavior to give a time-varying evt implementation.
	     */
	    public static <A> Event<A> switchE(Behavior<Event<A>> bea)
	    {
            return Transaction.apply(new ILambda1<Transaction, Event<A>>() {
        	    public Event<A> apply(Transaction trans) {
                    return switchE(trans, bea);
        	    }
            });
        }

	    private static <A> Event<A> switchE(Transaction trans1, Behavior<Event<A>> bea)
	    {
            EventSink<A> o = new EventSink<A>();
            ITransactionHandler<A> h2 = new ITransactionHandler<A>() {
        	    public void run(Transaction trans2, A a) {
	                o.send(trans2, a);
	            }
            };
            ITransactionHandler<Event<A>> h1 = new ITransactionHandler<Event<A>>() {
                private Listener currentListener = bea.sample().listen(o.node, trans1, h2, false);

                public override void run(Transaction trans2, Event<A> ea) {
                    trans2.last(new Runnable() {
                	    public void run() {
	                        if (currentListener != null)
	                            currentListener.unlisten();
	                        currentListener = ea.listen(o.node, trans2, h2, true);
	                    }
                    });
                }

                protected override void finalize() {
                    if (currentListener != null)
                        currentListener.unlisten();
                }
            };
            Listener l1 = bea.updates().listen(o.node, trans1, h1, false);
            return o.addCleanup(l1);
	    }

        /**
         * Transform a behavior with a generalized state loop (a mealy machine). The function
         * is passed the input and the old state and returns the new state and output value.
         */
        public <B,S> Behavior<B> collect(S initState, ILambda2<A, S, Tuple2<B, S>> f)
        {
            Event<A> ea = updates().coalesce(new ILambda2<A,A,A>() {
        	    public A apply(A fst, A snd) { return snd; }
            });
            A za = sample();
            Tuple2<B, S> zbs = f.apply(za, initState);
            EventLoop<Tuple2<B,S>> ebs = new EventLoop<Tuple2<B,S>>();
            Behavior<Tuple2<B,S>> bbs = ebs.hold(zbs);
            Behavior<S> bs = bbs.map(new ILambda1<Tuple2<B,S>,S>() {
                public S apply(Tuple2<B,S> x) {
                    return x.b;
                }
            });
            Event<Tuple2<B,S>> ebs_out = ea.snapshot(bs, f);
            ebs.loop(ebs_out);
            return bbs.map(new ILambda1<Tuple2<B,S>,B>() {
                public B apply(Tuple2<B,S> x) {
                    return x.a;
                }
            });
        }

	    protected override void finalize() {
	        if (cleanup != null)
                cleanup.unlisten();
	    }
    }
}