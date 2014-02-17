namespace sodium
{
    class BehaviorPrioritizedInvoker<TBehavior, TNewBehavior> : IHandler<Transaction>
    {
        public bool Fired = false;
        private readonly EventSink<TNewBehavior> _sink;
        private readonly Behavior<IFunction<TBehavior, TNewBehavior>> _behaviorFunction;
        private readonly Behavior<TBehavior> _behavior;

        public BehaviorPrioritizedInvoker(
            EventSink<TNewBehavior> sink, 
            Behavior<IFunction<TBehavior, TNewBehavior>> behaviorFunction, 
            Behavior<TBehavior> behavior)
        {
            _sink = sink;
            _behaviorFunction = behaviorFunction;
            _behavior = behavior;
        }

        public void Run(Transaction transaction)
        {
            if (Fired)
                return;

            Fired = true;
            var handler = new ApplyBehaviorEventSinkSender<TBehavior, TNewBehavior>(_sink, _behaviorFunction, _behavior, this);
            transaction.Prioritized(_sink.Node, handler);
        }
    }
}