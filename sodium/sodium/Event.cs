namespace sodium
{
    using System;
    using System.Collections.Generic;

    public class Event<TEvent> : IDisposable
    {
        public readonly List<ITransactionHandler<TEvent>> Listeners = new List<ITransactionHandler<TEvent>>();
        protected readonly List<IListener> Finalizers = new List<IListener>();
        public Node Node = new Node(0L);
        protected readonly List<TEvent> Firings = new List<TEvent>();
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
        public IListener Listen(IHandler<TEvent> action)
        {
            return Listen(Node.Null, new ActionInvoker<TEvent>(action));
        }

        public IListener Listen(Node target, ITransactionHandler<TEvent> action)
        {
            return Transaction.Apply(new ListenerInvoker<TEvent>(this, target, action));
        }

        public IListener Listen(Node target, Transaction trans, ITransactionHandler<TEvent> action, bool suppressEarlierFirings)
        {
            lock (Transaction.ListenersLock)
            {
                if (Node.LinkTo(target))
                    trans.NeedToRegeneratePriorityQueue = true;
                Listeners.Add(action);
            }
            var aNow = SampleNow();
            if (aNow != null)
            {    // In cases like value(), we start with an initial value.
                foreach (object t in aNow)
                    action.Run(trans, (TEvent)t);  // <-- unchecked warning is here
            }
            if (!suppressEarlierFirings)
            {
                // Anything sent already in this transaction must be sent now so that
                // there's no order dependency between send and listen.
                foreach (var a in Firings)
                    action.Run(trans, a);
            }
            return new Listener<TEvent>(this, action, target);
        }

        /**
         * Transform the event's value according to the supplied function.
         */
        public Event<TNewEvent> Map<TNewEvent>(ISingleParameterFunction<TEvent, TNewEvent> mapFunction)
        {
            var ev = this;
            var o = new MapEventSink<TEvent, TNewEvent>(ev, mapFunction);
            var l = Listen(o.Node, new MapTransactionHandler<TEvent, TNewEvent>(o, mapFunction));
            return o.AddCleanup(l);
        }

        /**
         * Create a behavior with the specified initial value, that gets updated
         * by the values coming through the event. The 'current value' of the behavior
         * is notionally the value as it was 'at the start of the transaction'.
         * That is, state updates caused by event firings get processed at the end of
         * the transaction.
         */
        public Behavior<TEvent> Hold(TEvent initValue)
        {
            return Transaction.Apply(new BehaviorBuilder<TEvent>(this, initValue));
        }

        /**
	     * Variant of snapshot that throws away the event's value and captures the behavior's.
	     */
        public Event<TNewEvent> Snapshot<TNewEvent>(Behavior<TNewEvent> behavior)
        {
            var snapshotGenerator = new TwoParameterFunction<TEvent, TNewEvent, TNewEvent>((a,b) => b);
            return Snapshot(behavior, snapshotGenerator);
        }

        /**
         * Sample the behavior at the time of the event firing. Note that the 'current value'
         * of the behavior that's sampled is the value as at the start of the transaction
         * before any state changes of the current transaction are applied through 'hold's.
         */
        public Event<TSnapshot> Snapshot<TBehavior, TSnapshot>(
            Behavior<TBehavior> behavior, 
            ITwoParameterFunction<TEvent, TBehavior, TSnapshot> snapshotGenerator)
        {
            var ev = this;
            var o = new SnapshotEventSink<TEvent, TBehavior, TSnapshot>(ev, snapshotGenerator, behavior);
            var l = Listen(o.Node, new SnapshotTransactionHandler<TEvent, TBehavior, TSnapshot>(o, snapshotGenerator, behavior));
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
        public static Event<TEvent> Merge(Event<TEvent> event1, Event<TEvent> event2)
        {
            var o = new MergeEventSink<TEvent>(event1, event2);
            var h = new MergeTransactionHandler<TEvent>(o);
            var l1 = event1.Listen(o.Node, h);
            var l2 = event2.Listen(o.Node, h);
            return o.AddCleanup(l1).AddCleanup(l2);
        }

        /**
	     * Push each event occurrence onto a new transaction.
	     */
        public Event<TEvent> Delay()
        {
            var o = new EventSink<TEvent>();
            var l1 = Listen(o.Node, new DelayTransactionHandler<TEvent>(o));
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
        public Event<TEvent> Coalesce(ITwoParameterFunction<TEvent, TEvent, TEvent> combiningFunction)
        {
            return Transaction.Apply(new CoalesceInvoker<TEvent>(this, combiningFunction));
        }

        public Event<TEvent> Coalesce(Transaction transaction, ITwoParameterFunction<TEvent, TEvent, TEvent> combiningFunction)
        {
            var ev = this;
            var o = new CoalesceEventSink<TEvent>(ev, combiningFunction);
            var h = new CoalesceHandler<TEvent>(combiningFunction, o);
            var l = Listen(o.Node, transaction, h, false);
            return o.AddCleanup(l);
        }

        /**
         * Clean up the output by discarding any firing other than the last one. 
         */
        public Event<TEvent> LastFiringOnly(Transaction transaction)
        {
            var combiningFunction = new TwoParameterFunction<TEvent, TEvent, TEvent>((first, second) => { return second; });
            return Coalesce(transaction, combiningFunction);
        }

        /**
         * Merge two streams of events of the same type, combining simultaneous
         * event occurrences.
         *
         * In the case where multiple event occurrences are simultaneous (i.e. all
         * within the same transaction), they are combined using the same logic as
         * 'coalesce'.
         */
        public static Event<TEvent> MergeWith(ITwoParameterFunction<TEvent, TEvent, TEvent> combiningFunction, Event<TEvent> event1, Event<TEvent> event2)
        {
            return Merge(event1, event2).Coalesce(combiningFunction);
        }

        /**
         * Only keep event occurrences for which the predicate returns true.
         */
        public Event<TEvent> Filter(ISingleParameterFunction<TEvent, Boolean> predicate)
        {
            var ev = this;
            var o = new FilterEventSink<TEvent>(ev, predicate);
            var l = Listen(o.Node, new FilterTransactionHandler<TEvent>(predicate, o));
            return o.AddCleanup(l);
        }

        /**
         * Filter out any event occurrences whose value is a Java null pointer.
         */
        public Event<TEvent> FilterNotNull()
        {
            var predicate = new SingleParameterFunction<TEvent, bool>((a) => a != null);
            return Filter(predicate);
        }

        /**
         * Let event occurrences through only when the behavior's value is True.
         * Note that the behavior's value is as it was at the start of the transaction,
         * that is, no state changes from the current transaction are taken into account.
         */
        public Event<TEvent> Gate(Behavior<Boolean> behaviorPredicate)
        {
            var snapshotGenerator = new TwoParameterFunction<TEvent, bool, TEvent>((a,pred) => pred ? a : default(TEvent));
            return Snapshot(behaviorPredicate, snapshotGenerator).FilterNotNull();
        }

        /**
         * Transform an event with a generalized state loop (a mealy machine). The function
         * is passed the input and the old state and returns the new state and output value.
         */
        public Event<TNewEvent> Collect<TNewEvent, TState>(
            TState initState, 
            ITwoParameterFunction<TEvent, TState, Tuple2<TNewEvent, TState>> melayMachineFunction)
        {
            var ea = this;
            var es = new EventLoop<TState>();
            var s = es.Hold(initState);
            var ebs = ea.Snapshot(s, melayMachineFunction);
            var eb = ebs.Map(new SingleParameterFunction<Tuple2<TNewEvent, TState>, TNewEvent>((bs) => bs.X));
            var esOut = ebs.Map(new SingleParameterFunction<Tuple2<TNewEvent, TState>, TState>((bs) => bs.Y));
            es.Loop(esOut);
            return eb;
        }

        /**
         * Accumulate on input event, outputting the new state each time.
         */
        public Behavior<TState> Accumulate<TState>(TState initState, ITwoParameterFunction<TEvent, TState, TState> snapshotGenerator)
        {
            var ea = this;
            var es = new EventLoop<TState>();
            var s = es.Hold(initState);
            var esOut = ea.Snapshot(s, snapshotGenerator);
            es.Loop(esOut);
            return esOut.Hold(initState);
        }

        /**
         * Throw away all event occurrences except for the first one.
         */
        public Event<TEvent> Once()
        {
            // This is a bit long-winded but it's efficient because it deregisters
            // the listener.
            var ev = this;
            var la = new IListener[1];
            var o = new OnceEventSink<TEvent>(ev, la);
            la[0] = ev.Listen(o.Node, new OnceTransactionHandler<TEvent>(o, la));
            return o.AddCleanup(la[0]);
        }

        public Event<TEvent> AddCleanup(IListener cleanup)
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