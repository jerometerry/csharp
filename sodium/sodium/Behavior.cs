namespace sodium
{
    using System;

    public class Behavior<TBehavior> : IDisposable
    {
        private Event<TBehavior> _event;
        private TBehavior _value;
        private TBehavior _valueUpdate;
        private IListener _cleanup;
        private bool _disposed;
        private bool _valueUpdated = false;

        public TBehavior Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public TBehavior ValueUpdate
        {
            get
            {
                return _valueUpdate;
            }
            set 
            { 
                _valueUpdate = value;
                _valueUpdated = true;
            }
        }

        public bool HasValueUpdate
        {
            get
            {
                return _valueUpdated;
            }
        }

        public IListener Cleanup
        {
            get
            {
                return _cleanup;
            }
            set
            {
                _cleanup = value;
            }
        }

        public Event<TBehavior> Event
        {
            get
            {
                return _event;
            }
            set
            {
                _event = value;
            }
        }

        /**
         * A behavior with a constant value.
         */
        public Behavior(TBehavior value)
        {
            Event = new Event<TBehavior>();
            Value = value;
        }

        public Behavior(Event<TBehavior> evt, TBehavior initValue)
        {
            Event = evt;
            Value = initValue;
            var converter = new EventToBehaviorConverter<TBehavior>(this, evt);
            Transaction.Run(converter);
        }

        /**
         * @return The value including any updates that have happened in this transaction.
         */
        public TBehavior NewValue()
        {
            return HasValueUpdate ? ValueUpdate : Value;
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
        public TBehavior Sample()
        {
            // Since pointers in Java are atomic, we don't need to explicitly create a
            // transaction.
            return Value;
        }

        /**
         * An event that gives the updates for the behavior. If this behavior was created
         * with a hold, then updates() gives you an evt equivalent to the one that was held.
         */
        public Event<TBehavior> Updates()
        {
            return Event;
        }

        /**
         * An event that is guaranteed to fire once when you listen to it, giving
         * the current value of the behavior, and thereafter behaves like updates(),
         * firing for each update to the behavior's value.
         */
        public Event<TBehavior> GetValue()
        {
            var code = new GetBehaviorValueInvoker<TBehavior>(this);
            return Transaction.Apply<Event<TBehavior>>(code);
        }

        public Event<TBehavior> GetValue(Transaction transaction)
        {
            var sink = new GetBehaviorValueEventSink<TBehavior>(this);
            var action = new EventSinkSender<TBehavior>(sink);
            var listener = Event.Listen(sink.Node, transaction, action, false);
            // Needed in case of an initial value and an update in the same transaction.
            return sink.AddCleanup(listener).LastFiringOnly(transaction);  
        }

        /**
         * Transform the behavior's value according to the supplied function.
         */
        public Behavior<TNewBehavior> Map<TNewBehavior>(IFunction<TBehavior, TNewBehavior> mapFunction)
        {
            return Updates().Map(mapFunction).Hold(mapFunction.Apply(Sample()));
        }

        public Behavior<TNewBehavior> Map<TNewBehavior>(Func<TBehavior, TNewBehavior> mapFunction)
        {
            return Map<TNewBehavior>(new Function<TBehavior, TNewBehavior>(mapFunction));
        }

        /**
         * Lift a binary function into behaviors.
         */
        public Behavior<TResultBehavior> Lift<TSecondBehavior, TResultBehavior>(
            IBinaryFunction<TBehavior, TSecondBehavior, TResultBehavior> liftFunction, 
            Behavior<TSecondBehavior> behavior)
        {
            var behaviorLifter = new BehaviorLifter2<TBehavior, TSecondBehavior, TResultBehavior>(liftFunction);
		    var behaviorMap = Map(behaviorLifter);
		    return Behavior<TSecondBehavior>.Apply(behaviorMap, behavior);
        }

        public Behavior<TResultBehavior> Lift<TSecondBehavior, TResultBehavior>(
            Func<TBehavior, TSecondBehavior, TResultBehavior> liftFunction,
            Behavior<TSecondBehavior> behavior)
        {
            return Lift<TSecondBehavior, TResultBehavior>(new BinaryFunction<TBehavior, TSecondBehavior, TResultBehavior>(liftFunction), behavior);
        }

        /**
	     * Lift a binary function into behaviors.
	     */
        public static Behavior<TResultBehavior> Lift<TSecondBehavior, TResultBehavior>(
            IBinaryFunction<TBehavior, TSecondBehavior, TResultBehavior> liftFunction, 
            Behavior<TBehavior> behavior1, 
            Behavior<TSecondBehavior> behavior2)
        {
            return behavior1.Lift(liftFunction, behavior2);
        }

        public static Behavior<TResult> Lift<TNewBehavior, TResult>(
            Func<TBehavior, TNewBehavior, TResult> liftFunction,
            Behavior<TBehavior> behavior1,
            Behavior<TNewBehavior> behavior2)
        {
            return Lift(new BinaryFunction<TBehavior, TNewBehavior, TResult>(liftFunction), behavior1, behavior2);
        }

        /**
         * Lift a ternary function into behaviors.
         */
        public Behavior<TResultBehavior> Lift<TBehavior2, TBehavior3, TResultBehavior>(
            ITernaryFunction<TBehavior, TBehavior2, TBehavior3, TResultBehavior> behaviorFunction, 
            Behavior<TBehavior2> behavior2, 
            Behavior<TBehavior3> behavior3)
        {
            var behaviorLifter = new BehaviorLifter3<TBehavior, TBehavior2, TBehavior3, TResultBehavior>(behaviorFunction);
		    var mapFunction = Map(behaviorLifter);
            var behaviorFunction2 = Behavior<TBehavior2>.Apply(mapFunction, behavior2);
            return Behavior<TBehavior3>.Apply(behaviorFunction2, behavior3);
        }

        public Behavior<TResultBehavior> Lift<TBehavior2, TBehavior3, TResultBehavior>(
            Func<TBehavior, TBehavior2, TBehavior3, TResultBehavior> behaviorFunction,
            Behavior<TBehavior2> behavior2,
            Behavior<TBehavior3> behavior3)
        {
            return Lift(new TernaryFunction<TBehavior, TBehavior2, TBehavior3, TResultBehavior>(behaviorFunction), behavior2, behavior3);
        }

        /**
	     * Lift a ternary function into behaviors.
	     */
        public static Behavior<TResult> Lift<TBehavior2, TBehavior3, TResult>(
            ITernaryFunction<TBehavior, TBehavior2, TBehavior3, TResult> liftFunction, 
            Behavior<TBehavior> behavior1, 
            Behavior<TBehavior2> behavior2, 
            Behavior<TBehavior3> behavior3)
        {
            return behavior1.Lift(liftFunction, behavior2, behavior3);
        }

        public static Behavior<TResult> Lift<TBehavior2, TBehavior3, TResult>(
            Func<TBehavior, TBehavior2, TBehavior3, TResult> liftFunction,
            Behavior<TBehavior> behavior1,
            Behavior<TBehavior2> behavior2,
            Behavior<TBehavior3> behavior3)
        {
            return behavior1.Lift(liftFunction, behavior2, behavior3);
        }

        /**
         * Apply a value inside a behavior to a function inside a behavior. This is the
         * primitive for all function lifting.
         */
        public static Behavior<TNewBehavior> Apply<TNewBehavior>(
            Behavior<IFunction<TBehavior, TNewBehavior>> behaviorFunction, 
            Behavior<TBehavior> behavior)
        {
            var sink = new EventSink<TNewBehavior>();
            var invoker = new BehaviorPrioritizedInvoker<TBehavior, TNewBehavior>(sink, behaviorFunction, behavior);
            var listener1 = behaviorFunction.Updates().Listen(sink.Node, new ApplyBehaviorTransactionHandler<TBehavior, TNewBehavior>(invoker));
            var listener2 = behavior.Updates().Listen(sink.Node, new ApplyBehaviorTransactionHandler2<TBehavior, TNewBehavior>(invoker));
            return sink.AddCleanup(listener1).AddCleanup(listener2).Hold(behaviorFunction.Sample().Apply(behavior.Sample()));
        }

        /**
	     * Unwrap a behavior inside another behavior to give a time-varying behavior implementation.
	     */
        public static Behavior<TBehavior> SwitchB(Behavior<Behavior<TBehavior>> behaviorFunction)
        {
            var value = behaviorFunction.Sample().Sample();
            var sink = new EventSink<TBehavior>();
            var handler = new BehaviorUnwrapper<TBehavior>(sink);
            var listener = behaviorFunction.GetValue().Listen(sink.Node, handler);
            return sink.AddCleanup(listener).Hold(value);
        }

        /**
         * Unwrap an evt inside a behavior to give a time-varying evt implementation.
         */
        public static Event<TBehavior> SwitchE(Behavior<Event<TBehavior>> eventBehavior)
        {
            var code = new SwitchToEventInvoker<TBehavior>(eventBehavior);
            return Transaction.Apply(code);
        }

        public static Event<TBehavior> SwitchE(Transaction transaction, Behavior<Event<TBehavior>> eventBehavior)
        {
            var sink = new EventSink<TBehavior>();
            var handler2 = new EventSinkSender<TBehavior>(sink);
            var handler1 = new SwitchToEventTransactionHandler<TBehavior>(sink, eventBehavior, transaction, handler2);
            var listener = eventBehavior.Updates().Listen(sink.Node, transaction, handler1, false);
            return sink.AddCleanup(listener);
        }

        /**
         * Transform a behavior with a generalized state loop (a mealy machine). The function
         * is passed the input and the old state and returns the new state and output value.
         */
        public Behavior<TNewBehavior> Collect<TNewBehavior, TState>(
            TState initState, 
            IBinaryFunction<TBehavior, TState, Tuple2<TNewBehavior, TState>> melayMachineFunction)
        {
            var combiningFunction = new BinaryFunction<TBehavior, TBehavior, TBehavior>((a, b) => b);
            var evt = Updates().Coalesce(combiningFunction);
            var value = Sample();
            var zbs = melayMachineFunction.Apply(value, initState);
            var loop = new EventLoop<Tuple2<TNewBehavior, TState>>();
            var bbs = loop.Hold(zbs);
            var mapFunction1 = new Function<Tuple2<TNewBehavior, TState>, TState>((x) => x.V2);
            var bs = bbs.Map(mapFunction1);
            var ebsOut = evt.Snapshot(bs, melayMachineFunction);
            loop.Loop(ebsOut);
            var mapFunction2 = new Function<Tuple2<TNewBehavior, TState>, TNewBehavior>((x) => x.V1);
            return bbs.Map(mapFunction2);
        }

        public Behavior<TNewBehavior> Collect<TNewBehavior, TState>(
            TState initState,
            Func<TBehavior, TState, Tuple2<TNewBehavior, TState>> melayMachineFunction)
        {
            return Collect(initState, new BinaryFunction<TBehavior, TState, Tuple2<TNewBehavior, TState>>(melayMachineFunction));
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