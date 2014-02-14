namespace sodium
{
    using System;

    public class Behavior<A> : IDisposable
    {
        protected Event<A> evt;
        public A _value;
        public A valueUpdate;
        public Listener cleanup;
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
            Transaction.run(new BehaviorHelpers.TmpTransHandler1<A>(this, evt));
        }

        /**
         * @return The value including any updates that have happened in this transaction.
         */
        public A newValue()
        {
            return valueUpdate == null ? _value : valueUpdate;
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
            return Transaction.apply(new BehaviorHelpers.Tmp1<A>(this));
        }
        public Event<A> value(Transaction trans1)
        {
            EventSink<A> o = new BehaviorHelpers.TmpEventSink1<A>(this);

            Listener l = evt.listen(o.node, trans1,
                new BehaviorHelpers.TmpTransHandler2<A>(o), false);
            return o.addCleanup(l)
                .lastFiringOnly(trans1);  // Needed in case of an initial value and an update
            // in the same transaction.
        }

        /**
         * Transform the behavior's value according to the supplied function.
         */
        public Behavior<B> map<B>(ILambda1<A, B> f)
        {
            return updates().map(f).hold(f.apply(sample()));
        }

        /**
         * Lift a binary function into behaviors.
         */
        public Behavior<C> lift<B, C>(ILambda2<A, B, C> f, Behavior<B> b)
        {
            ILambda1<A, ILambda1<B, C>> ffa = new BehaviorHelpers.Tmp2<A, B, C>(f);
            Behavior<ILambda1<B, C>> bf = map(ffa);
            return apply(bf, b);
        }

        /**
	     * Lift a binary function into behaviors.
	     */
        public static Behavior<C> lift<A, B, C>(ILambda2<A, B, C> f, Behavior<A> a, Behavior<B> b)
        {
            return a.lift(f, b);
        }

        /**
         * Lift a ternary function into behaviors.
         */
        public Behavior<D> lift<B, C, D>(ILambda3<A, B, C, D> f, Behavior<B> b, Behavior<C> c)
        {
            ILambda1<A, ILambda1<B, ILambda1<C, D>>> ffa = new BehaviorHelpers.Tmp4<A, B, C, D>(f);
            Behavior<ILambda1<B, ILambda1<C, D>>> bf = map(ffa);
            return apply(apply(bf, b), c);
        }

        /**
	     * Lift a ternary function into behaviors.
	     */
        public static Behavior<D> lift<A, B, C, D>(ILambda3<A, B, C, D> f, Behavior<A> a, Behavior<B> b, Behavior<C> c)
        {
            return a.lift(f, b, c);
        }

        /**
         * Apply a value inside a behavior to a function inside a behavior. This is the
         * primitive for all function lifting.
         */
        public static Behavior<B> apply<A, B>(Behavior<ILambda1<A, B>> bf, Behavior<A> ba)
        {
            EventSink<B> o = new EventSink<B>();

            IHandler<Transaction> h = new BehaviorHelpers.Tmp7<A, B>(o, bf, ba);

            Listener l1 = bf.updates().listen_(o.node, new BehaviorHelpers.Tmp9<A, B>(h));
            Listener l2 = ba.updates().listen_(o.node, new BehaviorHelpers.Tmp10<A, B>(h));
            return o.addCleanup(l1).addCleanup(l2).hold(bf.sample().apply(ba.sample()));
        }

        /**
	     * Unwrap a behavior inside another behavior to give a time-varying behavior implementation.
	     */
        public static Behavior<A> switchB<A>(Behavior<Behavior<A>> bba)
        {
            A za = bba.sample().sample();
            EventSink<A> o = new EventSink<A>();
            ITransactionHandler<Behavior<A>> h = new BehaviorHelpers.Tmp11<A>(o);
            Listener l1 = bba.value().listen_(o.node, h);
            return o.addCleanup(l1).hold(za);
        }

        /**
         * Unwrap an evt inside a behavior to give a time-varying evt implementation.
         */
        public static Event<A> switchE<A>(Behavior<Event<A>> bea)
        {
            return Transaction.apply(new BehaviorHelpers.Tmp13<A>(bea));
        }
        public static Event<A> switchE<A>(Transaction trans1, Behavior<Event<A>> bea)
        {
            EventSink<A> o = new EventSink<A>();
            ITransactionHandler<A> h2 = new BehaviorHelpers.Tmp14<A>(o);
            ITransactionHandler<Event<A>> h1 = new BehaviorHelpers.Tmp15<A>(o, bea, trans1, h2);
            Listener l1 = bea.updates().listen(o.node, trans1, h1, false);
            return o.addCleanup(l1);
        }


        /**
         * Transform a behavior with a generalized state loop (a mealy machine). The function
         * is passed the input and the old state and returns the new state and output value.
         */
        public Behavior<B> collect<B, S>(S initState, ILambda2<A, S, Tuple2<B, S>> f)
        {
            Event<A> ea = updates().coalesce(new BehaviorHelpers.Tmp16<A>());
            A za = sample();
            Tuple2<B, S> zbs = f.apply(za, initState);
            EventLoop<Tuple2<B, S>> ebs = new EventLoop<Tuple2<B, S>>();
            Behavior<Tuple2<B, S>> bbs = ebs.hold(zbs);
            Behavior<S> bs = bbs.map(new BehaviorHelpers.Tmp17<A, B, S>());
            Event<Tuple2<B, S>> ebs_out = ea.snapshot(bs, f);
            ebs.loop(ebs_out);
            return bbs.map(new BehaviorHelpers.Tmp18<A, B, S>());
        }

        public void Dispose()
        {
            Dispose(true);

            // Call SupressFinalize in case a subclass implements a finalizer.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // If you need thread safety, use a lock around these  
            // operations, as well as in your methods that use the resource. 
            if (!_disposed)
            {
                if (disposing)
                {
                    if (cleanup != null)
                        cleanup.unlisten();
                }

                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }
    }
}