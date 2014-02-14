namespace sodium
{

    using System;
    using System.Collections.Generic;

    public class Event<A> : IDisposable
    {
        public readonly List<ITransactionHandler<A>> listeners = new List<ITransactionHandler<A>>();
        protected readonly List<Listener> finalizers = new List<Listener>();
        public Node node = new Node(0L);
        protected readonly List<A> firings = new List<A>();
        private bool _disposed;

        /**
         * An event that never fires.
         */
        public Event()
        {
        }

        public virtual Object[] sampleNow() { return null; }

        /**
         * Listen for firings of this event. The returned Listener has an unlisten()
         * method to cause the listener to be removed. This is the observer pattern.
         */
        public Listener listen(IHandler<A> action)
        {
            return listen_(Node.NULL, new EventHelpers.TmpTransHandler1<A>(action));
        }

        public Listener listen_(Node target, ITransactionHandler<A> action)
        {
            return Transaction.apply(new EventHelpers.ListenerApplier<A>(this, target, action));
        }

        public Listener listen(Node target, Transaction trans, ITransactionHandler<A> action, bool suppressEarlierFirings)
        {
            lock (Transaction.listenersLock)
            {
                if (node.linkTo(target))
                    trans.toRegen = true;
                listeners.Add(action);
            }
            Object[] aNow = sampleNow();
            if (aNow != null)
            {    // In cases like value(), we start with an initial value.
                for (int i = 0; i < aNow.Length; i++)
                    action.run(trans, (A)aNow[i]);  // <-- unchecked warning is here
            }
            if (!suppressEarlierFirings)
            {
                // Anything sent already in this transaction must be sent now so that
                // there's no order dependency between send and listen.
                foreach (A a in firings)
                    action.run(trans, a);
            }
            return new EventHelpers.ListenerImplementation<A>(this, action, target);
        }

        /**
         * Transform the event's value according to the supplied function.
         */
        public Event<B> map<B>(ILambda1<A, B> f)
        {
            Event<A> ev = this;
            EventSink<B> o = new EventHelpers.TmpEventtSink7<A, B>(ev, f);
            Listener l = listen_(o.node, new EventHelpers.TmpTransHandler7<A, B>(o, f));
            return o.addCleanup(l);
        }

        /**
         * Create a behavior with the specified initial value, that gets updated
         * by the values coming through the event. The 'current value' of the behavior
         * is notionally the value as it was 'at the start of the transaction'.
         * That is, state updates caused by event firings get processed at the end of
         * the transaction.
         */
        public Behavior<A> hold(A initValue)
        {
            return Transaction.apply(new EventHelpers.BehaviorBuilder<A>(this, initValue));
        }

        /**
	     * Variant of snapshot that throws away the event's value and captures the behavior's.
	     */
        public Event<B> snapshot<B>(Behavior<B> beh)
        {
            return snapshot(beh, new EventHelpers.SnapshotBehavior<A, B>());
        }

        /**
         * Sample the behavior at the time of the event firing. Note that the 'current value'
         * of the behavior that's sampled is the value as at the start of the transaction
         * before any state changes of the current transaction are applied through 'hold's.
         */
        public Event<C> snapshot<B, C>(Behavior<B> b, ILambda2<A, B, C> f)
        {
            Event<A> ev = this;
            EventSink<C> o = new EventHelpers.TmpEventSink1<A, B, C>(ev, f, b);
            Listener l = listen_(o.node, new EventHelpers.TmpTransHandler5<A, B, C>(o, f, b));
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
        public static Event<A> merge<A>(Event<A> ea, Event<A> eb)
        {
            EventSink<A> o = new EventHelpers.TmpEventSink2<A>(ea, eb);
            ITransactionHandler<A> h = new EventHelpers.TmpTransHandler2<A>(o);
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
            Listener l1 = listen_(o.node, new EventHelpers.TmpTransHandler3<A>(o));
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
        public Event<A> coalesce(ILambda2<A, A, A> f)
        {
            return Transaction.apply(new EventHelpers.Tmp2<A>(this, f));
        }

        public Event<A> coalesce(Transaction trans1, ILambda2<A, A, A> f)
        {
            Event<A> ev = this;
            EventSink<A> o = new EventHelpers.TmpEventSink3<A>(ev, f);
            ITransactionHandler<A> h = new EventHelpers.CoalesceHandler<A>(f, o);
            Listener l = listen(o.node, trans1, h, false);
            return o.addCleanup(l);
        }

        /**
         * Clean up the output by discarding any firing other than the last one. 
         */
        public Event<A> lastFiringOnly(Transaction trans)
        {
            return coalesce(trans, new EventHelpers.Tmp4<A>());
        }

        /**
         * Merge two streams of events of the same type, combining simultaneous
         * event occurrences.
         *
         * In the case where multiple event occurrences are simultaneous (i.e. all
         * within the same transaction), they are combined using the same logic as
         * 'coalesce'.
         */
        public static Event<A> mergeWith<A>(ILambda2<A, A, A> f, Event<A> ea, Event<A> eb)
        {
            return merge(ea, eb).coalesce(f);
        }

        /**
         * Only keep event occurrences for which the predicate returns true.
         */
        public Event<A> filter(ILambda1<A, Boolean> f)
        {
            Event<A> ev = this;
            EventSink<A> o = new EventHelpers.TmpEventSink5<A>(ev, f);
            Listener l = listen_(o.node, new EventHelpers.TmpTransHandler4<A>(f, o));
            return o.addCleanup(l);
        }

        /**
         * Filter out any event occurrences whose value is a Java null pointer.
         */
        public Event<A> filterNotNull()
        {
            return filter(new EventHelpers.Tmp5<A>());
        }

        /**
         * Let event occurrences through only when the behavior's value is True.
         * Note that the behavior's value is as it was at the start of the transaction,
         * that is, no state changes from the current transaction are taken into account.
         */
        public Event<A> gate(Behavior<Boolean> bPred)
        {
            return snapshot(bPred, new EventHelpers.Tmp6<A>()).filterNotNull();
        }

        /**
         * Transform an event with a generalized state loop (a mealy machine). The function
         * is passed the input and the old state and returns the new state and output value.
         */
        public Event<B> collect<B, S>(S initState, ILambda2<A, S, Tuple2<B, S>> f)
        {
            Event<A> ea = this;
            EventLoop<S> es = new EventLoop<S>();
            Behavior<S> s = es.hold(initState);
            Event<Tuple2<B, S>> ebs = ea.snapshot(s, f);
            Event<B> eb = ebs.map(new EventHelpers.Tmp7<A, B, S>());
            Event<S> es_out = ebs.map(new EventHelpers.Tmp8<A, B, S>());
            es.loop(es_out);
            return eb;
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
            EventSink<A> o = new EventHelpers.TmpEventSink4<A>(ev, la);
            la[0] = ev.listen_(o.node, new EventHelpers.TmpTransHandler8<A>(o, la));
            return o.addCleanup(la[0]);
        }

        public Event<A> addCleanup(Listener cleanup)
        {
            finalizers.Add(cleanup);
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
                    foreach (Listener l in finalizers)
                        l.unlisten();
                }

                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }
    }
}