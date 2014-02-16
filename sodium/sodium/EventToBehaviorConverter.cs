namespace sodium
{
    public class BehaviorListenInvoker<TBehavior> : IHandler<Transaction>
    {
        private readonly Behavior<TBehavior> _behavior;
        private readonly Event<TBehavior> _event;

        public BehaviorListenInvoker(Behavior<TBehavior> behavior, Event<TBehavior> evt)
        {
            _behavior = behavior;
            _event = evt;
        }

        public void Run(Transaction transaction)
        {
            var handler = new BehaviorTransactionHandler<TBehavior>(_behavior);
            _behavior.Cleanup = _event.Listen(Node.Null, transaction, handler, false);
        }
    }
}