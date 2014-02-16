namespace sodium
{
    public class ApplyBehaviorEventSinkSender<TBehavior, TNewBehavior> : IHandler<Transaction>
    {
        private readonly EventSink<TNewBehavior> _sink;
        private readonly Behavior<IFunction<TBehavior, TNewBehavior>> _behaviorFunction;
        private readonly Behavior<TBehavior> _behavior;
        private readonly BehaviorPrioritizedInvoker<TBehavior, TNewBehavior> _behaviorPrioritizedInvoker;

        public ApplyBehaviorEventSinkSender(
            EventSink<TNewBehavior> sink, 
            Behavior<IFunction<TBehavior, TNewBehavior>> behaviorFunction, 
            Behavior<TBehavior> behavior, 
            BehaviorPrioritizedInvoker<TBehavior, TNewBehavior> behaviorPrioritizedInvoker)
        {
            _sink = sink;
            _behaviorFunction = behaviorFunction;
            _behavior = behavior;
            _behaviorPrioritizedInvoker = behaviorPrioritizedInvoker;
        }

        public void Run(Transaction transaction)
        {
            _sink.Send(transaction, _behaviorFunction.NewValue().Apply(_behavior.NewValue()));
            _behaviorPrioritizedInvoker.Fired = false;
        }
    }
}