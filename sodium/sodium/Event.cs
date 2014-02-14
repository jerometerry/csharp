namespace sodium
{
    using System;
    using System.Collections.Generic;

    public class Event<TA> : IDisposable
    {
        public readonly List<ITransactionHandler<TA>> Listeners = new List<ITransactionHandler<TA>>();
        protected readonly List<IListener> Finalizers = new List<IListener>();
        public Node Node = new Node(0L);
        protected readonly List<TA> Firings = new List<TA>();
        private bool _disposed;

        /**
         * An event that never fires.
         */
        public Event()
        {
        }

        public virtual Object[] SampleNow() { return null; }

        /**
         * Listen for firings of this event. The returned Listener has an unlisten()
         * method to cause the listener to be removed. This is the observer pattern.
         */
        public IListener Listen(IHandler<TA> action)
        {
            return Listen(Node.NULL, new ActionInvoker<TA>(action));
        }

        public IListener Listen(Node target, ITransactionHandler<TA> action)
        {
            return Transaction.Apply(new ListenerInvoker<TA>(this, target, action));
        }

        public IListener Listen(Node target, Transaction trans, ITransactionHandler<TA> action, bool suppressEarlierFirings)
        {
            lock (Transaction.ListenersLock)
            {
                if (Node.linkTo(target))
                    trans.ToRegen = true;
                Listeners.Add(action);
            }
            Object[] aNow = SampleNow();
            if (aNow != null)
            {    // In cases like value(), we start with an initial value.
                for (var i = 0; i < aNow.Length; i++)
                    action.Run(trans, (TA)aNow[i]);  // <-- unchecked warning is here
            }
            if (!suppressEarlierFirings)
            {
                // Anything sent already in this transaction must be sent now so that
                // there's no order dependency between send and listen.
                foreach (TA a in Firings)
                    action.Run(trans, a);
            }
            return new Listener<TA>(this, action, target);
        }

        /**
         * Transform the event's value according to the supplied function.
         */
        public Event<TB> Map<TB>(ILambda1<TA, TB> f)
        {
            var ev = this;
            var o = new MapEventSink<TA, TB>(ev, f);
            var l = Listen(o.Node, new MapTransactionHandler<TA, TB>(o, f));
            return o.AddCleanup(l);
        }

        /**
         * Create a behavior with the specified initial value, that gets updated
         * by the values coming through the event. The 'current value' of the behavior
         * is notionally the value as it was 'at the start of the transaction'.
         * That is, state updates caused by event firings get processed at the end of
         * the transaction.
         */
        public Behavior<TA> Hold(TA initValue)
        {
            return Transaction.Apply(new BehaviorBuilder<TA>(this, initValue));
        }

        /**
	     * Variant of snapshot that throws away the event's value and captures the behavior's.
	     */
        public Event<TB> Snapshot<TB>(Behavior<TB> beh)
        {
            return Snapshot(beh, new SnapshotBehavior<TA, TB>());
        }

        /**
         * Sample the behavior at the time of the event firing. Note that the 'current value'
         * of the behavior that's sampled is the value as at the start of the transaction
         * before any state changes of the current transaction are applied through 'hold's.
         */
        public Event<TC> Snapshot<TB, TC>(Behavior<TB> b, ILambda2<TA, TB, TC> f)
        {
            var ev = this;
            var o = new SnapshotEventSink<TA, TB, TC>(ev, f, b);
            var l = Listen(o.Node, new SnapshotTransactionHandler<TA, TB, TC>(o, f, b));
            return o.AddCleanup(l);
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
        public static Event<TA> Merge(Event<TA> ea, Event<TA> eb)
        {
            var o = new MergeEventSink<TA>(ea, eb);
            var h = new MergeTransactionHandler<TA>(o);
            var l1 = ea.Listen(o.Node, h);
            var l2 = eb.Listen(o.Node, h);
            return o.AddCleanup(l1).AddCleanup(l2);
        }

        /**
	     * Push each event occurrence onto a new transaction.
	     */
        public Event<TA> Delay()
        {
            var o = new EventSink<TA>();
            var l1 = Listen(o.Node, new DelayTransactionHandler<TA>(o));
            return o.AddCleanup(l1);
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
        public Event<TA> Coalesce(ILambda2<TA, TA, TA> f)
        {
            return Transaction.Apply(new CoalesceInvoker<TA>(this, f));
        }

        public Event<TA> Coalesce(Transaction trans1, ILambda2<TA, TA, TA> f)
        {
            var ev = this;
            var o = new CoalesceEventSink<TA>(ev, f);
            var h = new CoalesceHandler<TA>(f, o);
            var l = Listen(o.Node, trans1, h, false);
            return o.AddCleanup(l);
        }

        /**
         * Clean up the output by discarding any firing other than the last one. 
         */
        public Event<TA> LastFiringOnly(Transaction trans)
        {
            return Coalesce(trans, new Lambda2<TA, TA, TA>((first, second) => { return second; }));
        }

        /**
         * Merge two streams of events of the same type, combining simultaneous
         * event occurrences.
         *
         * In the case where multiple event occurrences are simultaneous (i.e. all
         * within the same transaction), they are combined using the same logic as
         * 'coalesce'.
         */
        public static Event<TA> MergeWith(ILambda2<TA, TA, TA> f, Event<TA> ea, Event<TA> eb)
        {
            return Merge(ea, eb).Coalesce(f);
        }

        /**
         * Only keep event occurrences for which the predicate returns true.
         */
        public Event<TA> Filter(ILambda1<TA, Boolean> f)
        {
            var ev = this;
            var o = new FilterEventSink<TA>(ev, f);
            var l = Listen(o.Node, new FilterTransactionHandler<TA>(f, o));
            return o.AddCleanup(l);
        }

        /**
         * Filter out any event occurrences whose value is a Java null pointer.
         */
        public Event<TA> FilterNotNull()
        {
            return Filter(new Lambda1<TA, bool>((a) => a != null));
        }

        /**
         * Let event occurrences through only when the behavior's value is True.
         * Note that the behavior's value is as it was at the start of the transaction,
         * that is, no state changes from the current transaction are taken into account.
         */
        public Event<TA> Gate(Behavior<Boolean> bPred)
        {
            return Snapshot(bPred, new Lambda2<TA, bool, TA>((a,pred) => pred ? a : default(TA))).FilterNotNull();
        }

        /**
         * Transform an event with a generalized state loop (a mealy machine). The function
         * is passed the input and the old state and returns the new state and output value.
         */
        public Event<TB> Collect<TB, TS>(TS initState, ILambda2<TA, TS, Tuple2<TB, TS>> f)
        {
            var ea = this;
            var es = new EventLoop<TS>();
            var s = es.Hold(initState);
            var ebs = ea.Snapshot(s, f);

            var eb = ebs.Map(new Lambda1<Tuple2<TB, TS>, TB>((bs) => bs.X));
            var esOut = ebs.Map(new Lambda1<Tuple2<TB, TS>, TS>((bs) => bs.Y));
            es.loop(esOut);
            return eb;
        }

        /**
         * Accumulate on input event, outputting the new state each time.
         */
        public Behavior<TS> Accum<TS>(TS initState, ILambda2<TA, TS, TS> f)
        {
            var ea = this;
            var es = new EventLoop<TS>();
            var s = es.Hold(initState);
            var esOut = ea.Snapshot(s, f);
            es.loop(esOut);
            return esOut.Hold(initState);
        }

        /**
         * Throw away all event occurrences except for the first one.
         */
        public Event<TA> Once()
        {
            // This is a bit long-winded but it's efficient because it deregisters
            // the listener.
            var ev = this;
            var la = new IListener[1];
            var o = new OnceEventSink<TA>(ev, la);
            la[0] = ev.Listen(o.Node, new OnceTransactionHandler<TA>(o, la));
            return o.AddCleanup(la[0]);
        }

        public Event<TA> AddCleanup(IListener cleanup)
        {
            Finalizers.Add(cleanup);
            return this;
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
                    foreach (var l in Finalizers)
                        l.Unlisten();
                }

                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }
    }
}