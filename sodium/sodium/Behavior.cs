namespace sodium
{
    using System;

    public class Behavior<TA> : IDisposable
    {
        protected Event<TA> Evt;
        public TA Value;
        public TA ValueUpdate;
        public IListener Cleanup;
        private bool _disposed;

        /**
         * A behavior with a constant value.
         */
        public Behavior(TA value)
        {
            Evt = new Event<TA>();
            Value = value;
        }

        public Behavior(Event<TA> evt, TA initValue)
        {
            Evt = evt;
            Value = initValue;
            Transaction.Run(new BehaviorListenInvoker<TA>(this, evt));
        }

        /**
         * @return The value including any updates that have happened in this transaction.
         */
        public TA NewValue()
        {
            return ValueUpdate == null ? Value : ValueUpdate;
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
        public TA Sample()
        {
            // Since pointers in Java are atomic, we don't need to explicitly create a
            // transaction.
            return Value;
        }

        /**
         * An evt that gives the updates for the behavior. If this behavior was created
         * with a hold, then updates() gives you an evt equivalent to the one that was held.
         */
        public Event<TA> Updates()
        {
            return Evt;
        }

        /**
         * An evt that is guaranteed to fire once when you listen to it, giving
         * the current value of the behavior, and thereafter behaves like updates(),
         * firing for each update to the behavior's value.
         */
        public Event<TA> GetValue()
        {
            return Transaction.Apply(new GetBehaviorValueInvoker<TA>(this));
        }

        public Event<TA> GetValue(Transaction trans1)
        {
            var o = new GetBehaviorValueEventSink<TA>(this);

            var l = Evt.Listen(o.Node, trans1,
                new GetBehaviorValueTransactionHandler<TA>(o), false);
            return o.AddCleanup(l)
                .LastFiringOnly(trans1);  // Needed in case of an initial value and an update
            // in the same transaction.
        }

        /**
         * Transform the behavior's value according to the supplied function.
         */
        public Behavior<TB> Map<TB>(ILambda1<TA, TB> f)
        {
            return Updates().Map(f).Hold(f.Apply(Sample()));
        }

        /**
         * Lift a binary function into behaviors.
         */
        public Behavior<TC> Lift<TB, TC>(ILambda2<TA, TB, TC> f, Behavior<TB> b)
        {
            ILambda1<TA, ILambda1<TB, TC>> ffa = new BehaviorLifter2<TA, TB, TC>(f);
		    Behavior<ILambda1<TB,TC>> bf = Map(ffa);
		    return Behavior<TB>.Apply(bf, b);
        }

        /**
	     * Lift a binary function into behaviors.
	     */
        public static Behavior<TC> Lift<TB, TC>(ILambda2<TA, TB, TC> f, Behavior<TA> a, Behavior<TB> b)
        {
            return a.Lift(f, b);
        }

        /**
         * Lift a ternary function into behaviors.
         */
        public Behavior<TD> Lift<TB, TC, TD>(ILambda3<TA, TB, TC, TD> f, Behavior<TB> b, Behavior<TC> c)
        {
            ILambda1<TA, ILambda1<TB, ILambda1<TC, TD>>> ffa = new BehaviorLifter3<TA, TB, TC, TD>(f);
		    Behavior<ILambda1<TB, ILambda1<TC, TD>>> bf = Map(ffa);
            Behavior<ILambda1<TC,TD>> r1 = Behavior<TB>.Apply(bf, b);
            Behavior<TD> res = Behavior<TC>.Apply(r1, c);
            return res;
        }

        /**
	     * Lift a ternary function into behaviors.
	     */
        public static Behavior<TD> Lift<TB, TC, TD>(ILambda3<TA, TB, TC, TD> f, Behavior<TA> a, Behavior<TB> b, Behavior<TC> c)
        {
            return a.Lift(f, b, c);
        }

        /**
         * Apply a value inside a behavior to a function inside a behavior. This is the
         * primitive for all function lifting.
         */
        public static Behavior<TB> Apply<TB>(Behavior<ILambda1<TA, TB>> bf, Behavior<TA> ba)
        {
            var o = new EventSink<TB>();
            var h = new BehaviorPrioritizedInvoker<TA, TB>(o, bf, ba);
            var l1 = bf.Updates().Listen(o.Node, new ApplyBehaviorTransactionHandler<TA, TB>(h));
            var l2 = ba.Updates().Listen(o.Node, new ApplyBehaviorTransactionHandler2<TA, TB>(h));
            return o.AddCleanup(l1).AddCleanup(l2).Hold(bf.Sample().Apply(ba.Sample()));
        }

        /**
	     * Unwrap a behavior inside another behavior to give a time-varying behavior implementation.
	     */
        public static Behavior<TA> SwitchB(Behavior<Behavior<TA>> bba)
        {
            var za = bba.Sample().Sample();
            var o = new EventSink<TA>();
            var h = new SwitchToBehaviorTransactionHandler<TA>(o);
            var l1 = bba.GetValue().Listen(o.Node, h);
            return o.AddCleanup(l1).Hold(za);
        }

        /**
         * Unwrap an evt inside a behavior to give a time-varying evt implementation.
         */
        public static Event<TA> SwitchE(Behavior<Event<TA>> bea)
        {
            return Transaction.Apply(new SwitchToEventInvoker<TA>(bea));
        }

        public static Event<TA> SwitchE(Transaction trans1, Behavior<Event<TA>> bea)
        {
            var o = new EventSink<TA>();
            var h2 = new SwitchToEventTransactionHandler2<TA>(o);
            var h1 = new SwitchToEventTransactionHandler<TA>(o, bea, trans1, h2);
            var l1 = bea.Updates().Listen(o.Node, trans1, h1, false);
            return o.AddCleanup(l1);
        }

        /**
         * Transform a behavior with a generalized state loop (a mealy machine). The function
         * is passed the input and the old state and returns the new state and output value.
         */
        public Behavior<TB> Collect<TB, TS>(TS initState, ILambda2<TA, TS, Tuple2<TB, TS>> f)
        {
            var ea = Updates().Coalesce(new Lambda2<TA, TA, TA>((a, b) => b));
            var za = Sample();
            var zbs = f.Apply(za, initState);
            var ebs = new EventLoop<Tuple2<TB, TS>>();
            var bbs = ebs.Hold(zbs);
            var bs = bbs.Map(new Lambda1<Tuple2<TB, TS>, TS>((x) => x.Y));
            var ebsOut = ea.Snapshot(bs, f);
            ebs.loop(ebsOut);
            return bbs.Map(new Lambda1<Tuple2<TB, TS>, TB>((x) => x.X));
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
                    if (Cleanup != null)
                        Cleanup.Unlisten();
                }

                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }
    }
}