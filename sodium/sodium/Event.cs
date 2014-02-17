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

        public IListener Listen(Action<TEvent> action)
        {
            var handler = new Handler<TEvent>(action);
            return Listen(handler);
        }

        /**
         * Listen for firings of this event. The returned Listener has an unlisten()
         * method to cause the listener to be removed. This is the observer pattern.
         */
        public IListener Listen(IHandler<TEvent> action)
        {
            return Listen(Node.Null, new TransactionHandler<TEvent>(action));
        }

        public IListener Listen(Node target, ITransactionHandler<TEvent> action)
        {
            return Transaction.Apply(new ListenerInvoker<TEvent>(this, target, action));
        }

        public IListener Listen(
            Node target, 
            Transaction transaction, 
            ITransactionHandler<TEvent> action, 
            bool suppressEarlierFirings)
        {
            lock (Transaction.ListenersLock)
            {
                if (Node.LinkTo(target))
                    transaction.NeedToRegeneratePriorityQueue = true;
                Listeners.Add(action);
            }
            var aNow = SampleNow();
            if (aNow != null)
            {    // In cases like value(), we start with an initial value.
                foreach (object t in aNow)
                    action.Run(transaction, (TEvent)t); 
            }
            if (!suppressEarlierFirings)
            {
                // Anything sent already in this transaction must be sent now so that
                // there's no order dependency between send and listen.
                foreach (var firing in Firings)
                    action.Run(transaction, firing);
            }
            return new Listener<TEvent>(this, action, target);
        }

        /**
         * Transform the event's value according to the supplied function.
         */
        public Event<TNewEvent> Map<TNewEvent>(IFunction<TEvent, TNewEvent> mapFunction)
        {
            var ev = this;
            var o = new MappedEventSink<TEvent, TNewEvent>(ev, mapFunction);
            var l = Listen(o.Node, new MapSinkSender<TEvent, TNewEvent>(o, mapFunction));
            return o.AddCleanup(l);
        }

        public Event<TNewEvent> Map<TNewEvent>(Func<TEvent, TNewEvent> mapFunction)
        {
            return Map<TNewEvent>(new Function<TEvent, TNewEvent>(mapFunction));
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
            var snapshotGenerator = new BinaryFunction<TEvent, TNewEvent, TNewEvent>((a,b) => b);
            return Snapshot(behavior, snapshotGenerator);
        }

        /**
         * Sample the behavior at the time of the event firing. Note that the 'current value'
         * of the behavior that's sampled is the value as at the start of the transaction
         * before any state changes of the current transaction are applied through 'hold's.
         */
        public Event<TSnapshot> Snapshot<TBehavior, TSnapshot>(
            Behavior<TBehavior> behavior, 
            IBinaryFunction<TEvent, TBehavior, TSnapshot> snapshotFunction)
        {
            var evt = this;
            var sink = new SnapshotEventSink<TEvent, TBehavior, TSnapshot>(evt, snapshotFunction, behavior);
            var listener = Listen(sink.Node, new SnapshotSinkSender<TEvent, TBehavior, TSnapshot>(sink, snapshotFunction, behavior));
            return sink.AddCleanup(listener);
        }

        public Event<TSnapshot> Snapshot<TBehavior, TSnapshot>(
            Behavior<TBehavior> behavior,
            Func<TEvent, TBehavior, TSnapshot> snapshotFunction)
        {
            return Snapshot<TBehavior, TSnapshot>(behavior, new BinaryFunction<TEvent, TBehavior, TSnapshot>(snapshotFunction));
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
            var sink = new MergeEventSink<TEvent>(event1, event2);
            var handler = new EventSinkSender<TEvent>(sink);
            var listener1 = event1.Listen(sink.Node, handler);
            var listener2 = event2.Listen(sink.Node, handler);
            return sink.AddCleanup(listener1).AddCleanup(listener2);
        }

        /**
	     * Push each event occurrence onto a new transaction.
	     */
        public Event<TEvent> Delay()
        {
            var sink = new EventSink<TEvent>();
            var listener = Listen(sink.Node, new DelayTransactionHandler<TEvent>(sink));
            return sink.AddCleanup(listener);
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
        public Event<TEvent> Coalesce(IBinaryFunction<TEvent, TEvent, TEvent> combiningFunction)
        {
            return Transaction.Apply(new CoalesceInvoker<TEvent>(this, combiningFunction));
        }

        public Event<TEvent> Coalesce(Func<TEvent, TEvent, TEvent> combiningFunction)
        {
            return Coalesce(new BinaryFunction<TEvent, TEvent, TEvent>(combiningFunction));
        }

        public Event<TEvent> Coalesce(Transaction transaction, IBinaryFunction<TEvent, TEvent, TEvent> combiningFunction)
        {
            var evt = this;
            var sink = new CoalesceEventSink<TEvent>(evt, combiningFunction);
            var handler = new CoalesceHandler<TEvent>(combiningFunction, sink);
            var listener = Listen(sink.Node, transaction, handler, false);
            return sink.AddCleanup(listener);
        }

        /**
         * Clean up the output by discarding any firing other than the last one. 
         */
        public Event<TEvent> LastFiringOnly(Transaction transaction)
        {
            var combiningFunction = new BinaryFunction<TEvent, TEvent, TEvent>((first, second) => { return second; });
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
        public static Event<TEvent> MergeWith(
            IBinaryFunction<TEvent, TEvent, TEvent> combiningFunction, 
            Event<TEvent> event1, Event<TEvent> event2)
        {
            return Merge(event1, event2).Coalesce(combiningFunction);
        }

        public static Event<TEvent> MergeWith(
            Func<TEvent, TEvent, TEvent> combiningFunction,
            Event<TEvent> event1, Event<TEvent> event2)
        {
            return MergeWith(new BinaryFunction<TEvent, TEvent, TEvent>(combiningFunction), event1, event2);
        }

        /**
         * Only keep event occurrences for which the predicate returns true.
         */
        public Event<TEvent> Filter(IFunction<TEvent, Boolean> predicate)
        {
            var evt = this;
            var sink = new FilteredEventSink<TEvent>(evt, predicate);
            var listener = Listen(sink.Node, new FilteredEventSinkSender<TEvent>(predicate, sink));
            return sink.AddCleanup(listener);
        }

        public Event<TEvent> Filter(Func<TEvent, Boolean> predicate)
        {
            return Filter(new Function<TEvent, Boolean>(predicate));
        }

        /**
         * Filter out any event occurrences whose value is a Java null pointer.
         */
        public Event<TEvent> FilterNotNull()
        {
            var predicate = new Function<TEvent, bool>((a) => a != null);
            return Filter(predicate);
        }

        /**
         * Let event occurrences through only when the behavior's value is True.
         * Note that the behavior's value is as it was at the start of the transaction,
         * that is, no state changes from the current transaction are taken into account.
         */
        public Event<TEvent> Gate(Behavior<Boolean> behaviorPredicate)
        {
            var snapshotGenerator = new BinaryFunction<TEvent, bool, TEvent>((a,pred) => pred ? a : default(TEvent));
            return Snapshot(behaviorPredicate, snapshotGenerator).FilterNotNull();
        }

        /**
         * Transform an event with a generalized state loop (a mealy machine). The function
         * is passed the input and the old state and returns the new state and output value.
         */
        public Event<TNewEvent> Collect<TNewEvent, TState>(
            TState initState, 
            IBinaryFunction<TEvent, TState, Tuple2<TNewEvent, TState>> melayMachineFunction)
        {
            var ea = this;
            var es = new EventLoop<TState>();
            var s = es.Hold(initState);
            var ebs = ea.Snapshot(s, melayMachineFunction);
            var eb = ebs.Map(new Function<Tuple2<TNewEvent, TState>, TNewEvent>((bs) => bs.V1));
            var esOut = ebs.Map(new Function<Tuple2<TNewEvent, TState>, TState>((bs) => bs.V2));
            es.Loop(esOut);
            return eb;
        }

        public Event<TNewEvent> Collect<TNewEvent, TState>(
            TState initState,
            Func<TEvent, TState, Tuple2<TNewEvent, TState>> melayMachineFunction)
        {
            return Collect<TNewEvent, TState>(initState, new BinaryFunction<TEvent, TState, Tuple2<TNewEvent, TState>>(melayMachineFunction));
        }

        /**
         * Accumulate on input event, outputting the new state each time.
         */
        public Behavior<TState> Accumulate<TState>(TState initState, IBinaryFunction<TEvent, TState, TState> snapshotGenerator)
        {
            var ea = this;
            var es = new EventLoop<TState>();
            var s = es.Hold(initState);
            var esOut = ea.Snapshot(s, snapshotGenerator);
            es.Loop(esOut);
            return esOut.Hold(initState);
        }

        public Behavior<TState> Accumulate<TState>(TState initState, Func<TEvent, TState, TState> snapshotGenerator)
        {
            return Accumulate<TState>(initState, new BinaryFunction<TEvent, TState, TState>(snapshotGenerator));
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
            la[0] = ev.Listen(o.Node, new OnceSinkSender<TEvent>(o, la));
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