namespace sodium
{
    using System;

    public class Behavior<TBehavior> : IDisposable
    {
        private TBehavior _valueUpdate;
        private bool _disposed;

        public TBehavior Val { get; protected set; }

        public TBehavior ValueUpdate
        {
            get
            {
                return _valueUpdate;
            }
            internal set 
            { 
                _valueUpdate = value;
                ValueUpdated = true;
            }
        }

        public bool ValueUpdated { get; private set; }

        internal IListener EventListener { get; set; }

        public Event<TBehavior> Event { get; set; }

        /// <summary>
        /// A behavior with a constant value.
        /// </summary>
        /// <param name="value"></param>
        public Behavior(TBehavior value)
        {
            Event = new Event<TBehavior>();
            Val = value;
            _valueUpdate = default(TBehavior);
        }

        public Behavior(Event<TBehavior> evt, TBehavior initValue)
        {
            Event = evt;
            Val = initValue;
            var behavior = this;
            var code = new Handler<Transaction>(t => 
            {
                var handler = new BehaviorEventListener<TBehavior>(behavior);
                behavior.EventListener = evt.Listen(Node.Null, t, handler, false);
            });
            Transaction.Run(code);
        }

        public void ApplyUpdate()
        {
            Val = ValueUpdate;
            ValueUpdate = default(TBehavior);
            ValueUpdated = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>
        /// The value including any updates that have happened in this transaction.
        /// </returns>
        public TBehavior NewValue()
        {
            return ValueUpdated ? ValueUpdate : Val;
        }

        /// <summary>
        /// Sample the behavior's current value.
        ///
        /// This should generally be avoided in favour of value().listen(..) so you don't
        /// miss any updates, but in many circumstances it makes sense.
        ///
        /// It can be best to use it inside an explicit transaction (using Transaction.run()).
        /// For example, a b.Sample() inside an explicit transaction along with a
        /// b.Updates().Listen(..) will capture the current value and any updates without risk
        /// of missing any in between.
        /// </summary>
        /// <returns>
        /// </returns>
        public TBehavior Sample()
        {
            // Since pointers in Java are atomic, we don't need to explicitly create a
            // transaction.
            return Val;
        }

        /// <summary>
        /// An event that gives the updates for the behavior. If this behavior was created
        /// with a hold, then updates() gives you an evt equivalent to the one that was held.
        /// </summary>
        /// <returns>
        /// </returns>
        public Event<TBehavior> Updates()
        {
            return Event;
        }

        /// <summary>
        /// An event that is guaranteed to fire once when you listen to it, giving
        /// the current value of the behavior, and thereafter behaves like updates(),
        /// firing for each update to the behavior's value.
        /// </summary>
        /// <returns>
        /// </returns>
        public Event<TBehavior> Value()
        {
            var behavior = this;
            var action = new Function<Transaction, Event<TBehavior>>(t => 
            {
                return behavior.GetValue(t);
            });
            return Transaction.Apply(action);
        }

        public Event<TBehavior> GetValue(Transaction transaction)
        {
            var sink = new GetBehaviorValueEventSink<TBehavior>(this);
            var action = new EventSinkSender<TBehavior>(sink);
            var listener = Event.Listen(sink.Node, transaction, action, false);
            // Needed in case of an initial value and an update in the same transaction.
            return sink.RegisterListener(listener).LastFiringOnly(transaction);  
        }

        /// <summary>
        /// Transform the behavior's value according to the supplied function.
        /// </summary>
        /// <param name="f"></param>
        /// <returns>
        /// </returns>
        public Behavior<TResultBehavior> Map<TResultBehavior>(IFunction<TBehavior, TResultBehavior> f)
        {
            return Updates().Map(f).Hold(f.Apply(Sample()));
        }

        public Behavior<TResultBehavior> Map<TResultBehavior>(Func<TBehavior, TResultBehavior> f)
        {
            return Map(new Function<TBehavior, TResultBehavior>(f));
        }

        /// <summary>
        /// Lift a binary function into behaviors.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public Behavior<TResultBehavior> Lift<TBehavior2, TResultBehavior>(
            IBinaryFunction<TBehavior, TBehavior2, TResultBehavior> f, 
            Behavior<TBehavior2> behavior)
        {
            var behaviorLifter = new BinaryBehaviorLifter<TBehavior, TBehavior2, TResultBehavior>(f);
		    var behaviorMap = Map(behaviorLifter);
		    return Behavior<TBehavior2>.Apply(behaviorMap, behavior);
        }

        public Behavior<TResultBehavior> Lift<TBehavior2, TResultBehavior>(
            Func<TBehavior, TBehavior2, TResultBehavior> f,
            Behavior<TBehavior2> behavior)
        {
            return Lift(new BinaryFunction<TBehavior, TBehavior2, TResultBehavior>(f), behavior);
        }
       
        /// <summary>
        /// Lift a binary function into behaviors.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="behavior1"></param>
        /// <param name="behavior2"></param>
        /// <returns></returns>
        public static Behavior<TResultBehavior> Lift<TBehavior2, TResultBehavior>(
            IBinaryFunction<TBehavior, TBehavior2, TResultBehavior> f, 
            Behavior<TBehavior> behavior1, 
            Behavior<TBehavior2> behavior2)
        {
            return behavior1.Lift(f, behavior2);
        }

        public static Behavior<TResultBehavior> Lift<TBehavior2, TResultBehavior>(
            Func<TBehavior, TBehavior2, TResultBehavior> f,
            Behavior<TBehavior> behavior1,
            Behavior<TBehavior2> behavior2)
        {
            return Lift(new BinaryFunction<TBehavior, TBehavior2, TResultBehavior>(f), behavior1, behavior2);
        }
        
        /// <summary>
        /// Lift a ternary function into behaviors.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="behavior2"></param>
        /// <param name="behavior3"></param>
        /// <returns></returns>
        public Behavior<TResultBehavior> Lift<TBehavior2, TBehavior3, TResultBehavior>(
            ITernaryFunction<TBehavior, TBehavior2, TBehavior3, TResultBehavior> f, 
            Behavior<TBehavior2> behavior2, 
            Behavior<TBehavior3> behavior3)
        {
            var behaviorLifter = new TernaryBehaviorLifter<TBehavior, TBehavior2, TBehavior3, TResultBehavior>(f);
		    var mapFunction = Map(behaviorLifter);
            var behaviorFunction2 = Behavior<TBehavior2>.Apply(mapFunction, behavior2);
            return Behavior<TBehavior3>.Apply(behaviorFunction2, behavior3);
        }

        public Behavior<TResultBehavior> Lift<TBehavior2, TBehavior3, TResultBehavior>(
            Func<TBehavior, TBehavior2, TBehavior3, TResultBehavior> f,
            Behavior<TBehavior2> behavior2,
            Behavior<TBehavior3> behavior3)
        {
            return Lift(new TernaryFunction<TBehavior, TBehavior2, TBehavior3, TResultBehavior>(f), behavior2, behavior3);
        }

        /// <summary>
        /// Lift a ternary function into behaviors.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="behavior1"></param>
        /// <param name="behavior2"></param>
        /// <param name="behavior3"></param>
        /// <returns></returns>
        public static Behavior<TResultBehavior> Lift<TBehavior2, TBehavior3, TResultBehavior>(
            ITernaryFunction<TBehavior, TBehavior2, TBehavior3, TResultBehavior> f, 
            Behavior<TBehavior> behavior1, 
            Behavior<TBehavior2> behavior2, 
            Behavior<TBehavior3> behavior3)
        {
            return behavior1.Lift(f, behavior2, behavior3);
        }

        public static Behavior<TResultBehavior> Lift<TBehavior2, TBehavior3, TResultBehavior>(
            Func<TBehavior, TBehavior2, TBehavior3, TResultBehavior> f,
            Behavior<TBehavior> behavior1,
            Behavior<TBehavior2> behavior2,
            Behavior<TBehavior3> behavior3)
        {
            return behavior1.Lift(f, behavior2, behavior3);
        }

        /// <summary>
        /// Apply a value inside a behavior to a function inside a behavior. This is the
        /// primitive for all function lifting.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public static Behavior<TResultBehavior> Apply<TResultBehavior>(
            Behavior<IFunction<TBehavior, TResultBehavior>> f, 
            Behavior<TBehavior> behavior)
        {
            var sink = new EventSink<TResultBehavior>();
            var invoker = new BehaviorPrioritizedInvoker<TBehavior, TResultBehavior>(sink, f, behavior);
            var handler1 = new BehaviorFunctionUpdateHandler<TBehavior, TResultBehavior>(invoker);
            var listener1 = f.Updates().Listen(sink.Node, handler1);
            var handler2 = new BehaviorUpdateHandler<TBehavior>(invoker);
            var listener2 = behavior.Updates().Listen(sink.Node, handler2);
            var initValue = f.Sample().Apply(behavior.Sample());
            return sink.RegisterListener(listener1).RegisterListener(listener2).Hold(initValue);
        }

        /// <summary>
        /// Unwrap a behavior inside another behavior to give a time-varying behavior implementation.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Behavior<TBehavior> SwitchB(Behavior<Behavior<TBehavior>> f)
        {
            var value = f.Sample().Sample();
            var sink = new EventSink<TBehavior>();
            var handler = new BehaviorUnwrapper<TBehavior>(sink);
            var listener = f.Value().Listen(sink.Node, handler);
            return sink.RegisterListener(listener).Hold(value);
        }

        /// <summary>
        /// Unwrap an evt inside a behavior to give a time-varying evt implementation.
        /// </summary>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public static Event<TBehavior> SwitchE(Behavior<Event<TBehavior>> behavior)
        {
            var action = new Function<Transaction, Event<TBehavior>>(t => 
            {
                return Behavior<TBehavior>.SwitchE(t, behavior);
            });
            return Transaction.Apply(action);
        }

        public static Event<TBehavior> SwitchE(Transaction transaction, Behavior<Event<TBehavior>> behavior)
        {
            var sink = new EventSink<TBehavior>();
            var handler2 = new EventSinkSender<TBehavior>(sink);
            var handler1 = new SwitchToEventTransactionHandler<TBehavior>(sink, behavior, transaction, handler2);
            var listener = behavior.Updates().Listen(sink.Node, transaction, handler1, false);
            return sink.RegisterListener(listener);
        }
        
        /// <summary>
        /// Transform a behavior with a generalized state loop (a mealy machine). The function
        /// is passed the input and the old state and returns the new state and output value.
        /// </summary>
        /// <param name="initState"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public Behavior<TResultBehavior> Collect<TResultBehavior, TState>(
            TState initState, 
            IBinaryFunction<TBehavior, TState, Tuple2<TResultBehavior, TState>> f)
        {
            var combiningFunction = new BinaryFunction<TBehavior, TBehavior, TBehavior>((a, b) => b);
            var evt = Updates().Coalesce(combiningFunction);
            var value = Sample();
            var zbs = f.Apply(value, initState);
            var loop = new EventLoop<Tuple2<TResultBehavior, TState>>();
            var bbs = loop.Hold(zbs);
            var mapFunction1 = new Function<Tuple2<TResultBehavior, TState>, TState>(x => x.V2);
            var bs = bbs.Map(mapFunction1);
            var ebsOut = evt.Snapshot(bs, f);
            loop.Loop(ebsOut);
            var mapFunction2 = new Function<Tuple2<TResultBehavior, TState>, TResultBehavior>(x => x.V1);
            return bbs.Map(mapFunction2);
        }

        public Behavior<TResultBehavior> Collect<TResultBehavior, TState>(
            TState initState,
            Func<TBehavior, TState, Tuple2<TResultBehavior, TState>> f)
        {
            return Collect(initState, new BinaryFunction<TBehavior, TState, Tuple2<TResultBehavior, TState>>(f));
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
                    if (EventListener != null)
                        EventListener.Unlisten();
                }

                // Indicate that the instance has been disposed.
                _disposed = true;
            }
        }
    }
}