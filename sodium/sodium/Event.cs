namespace sodium
{
    using System;
    using System.Collections.Generic;

    public class Event<TEvent> : IDisposable
    {
        protected readonly List<ITransactionHandler<TEvent>> Actions = new List<ITransactionHandler<TEvent>>();
        protected readonly List<IListener> Listeners = new List<IListener>();
        internal Node Node = new Node(0L);
        protected readonly List<TEvent> Firings = new List<TEvent>();
        private bool _disposed;

        public virtual Object[] SampleNow() 
        { 
            return null; 
        }

        public IListener Listen(Action<TEvent> action)
        {
            var handler = new Handler<TEvent>(action);
            return Listen(handler);
        }
        
        /// <summary>
        /// Method to cause the listener to be removed. This is the observer pattern.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
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
                Actions.Add(action);
            }

            var events = SampleNow();
            if (events != null)
            {    // In cases like value(), we start with an initial value.
                foreach (object evt in events)
                    action.Run(transaction, (TEvent)evt); 
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
        
        /// <summary>
        /// Transform the event's value according to the supplied function.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public Event<TResultEvent> Map<TResultEvent>(IFunction<TEvent, TResultEvent> f)
        {
            var sink = new MappedEventSink<TEvent, TResultEvent>(this, f);
            var listener = Listen(sink.Node, new MapSinkSender<TEvent, TResultEvent>(sink, f));
            return sink.RegisterListener(listener);
        }

        public Event<TResultEvent> Map<TResultEvent>(Func<TEvent, TResultEvent> f)
        {
            return Map(new Function<TEvent, TResultEvent>(f));
        }

        /// <summary>
        /// Create a behavior with the specified initial value, that gets updated
        /// by the values coming through the event. The 'current value' of the behavior
        /// is notionally the value as it was 'at the start of the transaction'.
        /// That is, state updates caused by event firings get processed at the end of
        /// the transaction.
        /// </summary>
        /// <param name="initValue"></param>
        /// <returns></returns>
        public Behavior<TEvent> Hold(TEvent initValue)
        {
            return Transaction.Apply(new BehaviorBuilder<TEvent>(this, initValue));
        }

        /// <summary>
        /// Variant of snapshot that throws away the event's value and captures the behavior's.
        /// </summary>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public Event<TResultEvent> Snapshot<TResultEvent>(Behavior<TResultEvent> behavior)
        {
            var snapshotGenerator = new BinaryFunction<TEvent, TResultEvent, TResultEvent>((a,b) => b);
            return Snapshot(behavior, snapshotGenerator);
        }

        /// <summary>
        /// Sample the behavior at the time of the event firing. Note that the 'current value'
        /// of the behavior that's sampled is the value as at the start of the transaction
        /// before any state changes of the current transaction are applied through 'hold's.
        /// </summary>
        /// <param name="behavior"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public Event<TSnapshot> Snapshot<TBehavior, TSnapshot>(
            Behavior<TBehavior> behavior, 
            IBinaryFunction<TEvent, TBehavior, TSnapshot> f)
        {
            var sink = new SnapshotEventSink<TEvent, TBehavior, TSnapshot>(this, f, behavior);
            var listener = Listen(sink.Node, new SnapshotSinkSender<TEvent, TBehavior, TSnapshot>(sink, f, behavior));
            return sink.RegisterListener(listener);
        }

        public Event<TSnapshot> Snapshot<TBehavior, TSnapshot>(
            Behavior<TBehavior> behavior,
            Func<TEvent, TBehavior, TSnapshot> f)
        {
            return Snapshot(behavior, new BinaryFunction<TEvent, TBehavior, TSnapshot>(f));
        }

        /// <summary>
        /// Merge two streams of events of the same type.
        ///
        /// In the case where two event occurrences are simultaneous (i.e. both
        /// within the same transaction), both will be delivered in the same
        /// transaction. If the event firings are ordered for some reason, then
        /// their ordering is retained. In many common cases the ordering will
        /// be undefined.
        /// </summary>
        /// <param name="event1"></param>
        /// <param name="event2"></param>
        /// <returns></returns>
        public static Event<TEvent> Merge(Event<TEvent> event1, Event<TEvent> event2)
        {
            var sink = new MergeEventSink<TEvent>(event1, event2);
            var handler = new EventSinkSender<TEvent>(sink);
            var listener1 = event1.Listen(sink.Node, handler);
            var listener2 = event2.Listen(sink.Node, handler);
            return sink.RegisterListener(listener1).RegisterListener(listener2);
        }

        /// <summary>
        /// Push each event occurrence onto a new transaction.
        /// </summary>
        /// <returns></returns>
        public Event<TEvent> Delay()
        {
            var sink = new EventSink<TEvent>();
            var listener = Listen(sink.Node, new DelayTransactionHandler<TEvent>(sink));
            return sink.RegisterListener(listener);
        }

        /// <summary>
        /// If there's more than one firing in a single transaction, combine them into
        /// one using the specified combining function.
        ///
        /// If the event firings are ordered, then the first will appear at the left
        /// input of the combining function. In most common cases it's best not to
        /// make any assumptions about the ordering, and the combining function would
        /// ideally be commutative.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public Event<TEvent> Coalesce(IBinaryFunction<TEvent, TEvent, TEvent> f)
        {
            return Transaction.Apply(new CoalesceInvoker<TEvent>(this, f));
        }

        public Event<TEvent> Coalesce(Func<TEvent, TEvent, TEvent> f)
        {
            return Coalesce(new BinaryFunction<TEvent, TEvent, TEvent>(f));
        }

        public Event<TEvent> Coalesce(Transaction transaction, IBinaryFunction<TEvent, TEvent, TEvent> f)
        {
            var sink = new CoalesceEventSink<TEvent>(this, f);
            var handler = new CoalesceHandler<TEvent>(f, sink);
            var listener = Listen(sink.Node, transaction, handler, false);
            return sink.RegisterListener(listener);
        }

        /// <summary>
        /// Clean up the output by discarding any firing other than the last one. 
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public Event<TEvent> LastFiringOnly(Transaction transaction)
        {
            var combiningFunction = new BinaryFunction<TEvent, TEvent, TEvent>((first, second) => second);
            return Coalesce(transaction, combiningFunction);
        }

        /// <summary>
        /// Merge two streams of events of the same type, combining simultaneous
        /// event occurrences.
        ///
        /// In the case where multiple event occurrences are simultaneous (i.e. all
        /// within the same transaction), they are combined using the same logic as
        /// 'coalesce'.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="event1"></param>
        /// <param name="event2"></param>
        /// <returns></returns>
        public static Event<TEvent> MergeWith(
            IBinaryFunction<TEvent, TEvent, TEvent> f, 
            Event<TEvent> event1, 
            Event<TEvent> event2)
        {
            return Merge(event1, event2).Coalesce(f);
        }

        public static Event<TEvent> MergeWith(
            Func<TEvent, TEvent, TEvent> f,
            Event<TEvent> event1, 
            Event<TEvent> event2)
        {
            return MergeWith(new BinaryFunction<TEvent, TEvent, TEvent>(f), event1, event2);
        }

        /// <summary>
        /// Only keep event occurrences for which the predicate returns true.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public Event<TEvent> Filter(IFunction<TEvent, Boolean> predicate)
        {
            var sink = new FilteredEventSink<TEvent>(this, predicate);
            var listener = Listen(sink.Node, new FilteredEventSinkSender<TEvent>(predicate, sink));
            return sink.RegisterListener(listener);
        }

        public Event<TEvent> Filter(Func<TEvent, Boolean> predicate)
        {
            return Filter(new Function<TEvent, Boolean>(predicate));
        }

        /// <summary>
        /// Filter out any event occurrences whose value is a Java null pointer.
        /// </summary>
        /// <returns></returns>
        public Event<TEvent> FilterNotNull()
        {
            var predicate = new Function<TEvent, bool>((a) => a != null);
            return Filter(predicate);
        }

        /// <summary>
        /// Let event occurrences through only when the behavior's value is True.
        /// Note that the behavior's value is as it was at the start of the transaction,
        /// that is, no state changes from the current transaction are taken into account.
        /// </summary>
        /// <param name="behaviorPredicate"></param>
        /// <returns></returns>
        public Event<TEvent> Gate(Behavior<Boolean> behaviorPredicate)
        {
            // TODO - default(TEvent) isn't correct for value types such as char, int, etc.
            // To fix this, we would need to use something like the Maybe monad
            var f = new BinaryFunction<TEvent, bool, Maybe<TEvent>>((a,pred) => pred ? new Maybe<TEvent>(a) : null);
            return Snapshot<Boolean, Maybe<TEvent>>(behaviorPredicate, f).FilterNotNull().Map<TEvent>(a => a.Value);
        }

        /// <summary>
        /// Transform an event with a generalized state loop (a mealy machine). The function
        /// is passed the input and the old state and returns the new state and output value.
        /// </summary>
        /// <param name="initState"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public Event<TResultEvent> Collect<TResultEvent, TState>(
            TState initState, 
            IBinaryFunction<TEvent, TState, Tuple2<TResultEvent, TState>> f)
        {
            var loop = new EventLoop<TState>();
            var behavior = loop.Hold(initState);
            var snapshot = Snapshot(behavior, f);
            var event1 = snapshot.Map(new Function<Tuple2<TResultEvent, TState>, TResultEvent>(bs => bs.V1));
            var event2 = snapshot.Map(new Function<Tuple2<TResultEvent, TState>, TState>(bs => bs.V2));
            loop.Loop(event2);
            return event1;
        }

        public Event<TResultEvent> Collect<TResultEvent, TState>(
            TState initState,
            Func<TEvent, TState, Tuple2<TResultEvent, TState>> f)
        {
            var function = new BinaryFunction<TEvent, TState, Tuple2<TResultEvent, TState>>(f);
            return Collect(initState, function);
        }

        /// <summary>
        /// Accumulate on input event, outputting the new state each time.
        /// </summary>
        /// <param name="initState"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public Behavior<TState> Accum<TState>(
            TState initState, 
            IBinaryFunction<TEvent, TState, TState> f)
        {
            var loop = new EventLoop<TState>();
            var behavior = loop.Hold(initState);
            var snapshot = Snapshot(behavior, f);
            loop.Loop(snapshot);
            return snapshot.Hold(initState);
        }

        public Behavior<TState> Accum<TState>(TState initState, Func<TEvent, TState, TState> f)
        {
            return Accum(initState, new BinaryFunction<TEvent, TState, TState>(f));
        }

        /// <summary>
        /// Throw away all event occurrences except for the first one.
        /// </summary>
        /// <returns></returns>
        public Event<TEvent> Once()
        {
            // This is a bit long-winded but it's efficient because it deregisters
            // the listener.
            var listeners = new IListener[1];
            var sink = new OnceEventSink<TEvent>(this, listeners);
            listeners[0] = Listen(sink.Node, new OnceSinkSender<TEvent>(sink, listeners));
            return sink.RegisterListener(listeners[0]);
        }

        public Event<TEvent> RegisterListener(IListener listener)
        {
            Listeners.Add(listener);
            return this;
        }

        public void RemoveAction(ITransactionHandler<TEvent> action)
        {
            Actions.Remove(action);
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
                    foreach (var listener in Listeners)
                        listener.Unlisten();
                }

                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }
    }
}