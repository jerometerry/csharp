namespace sodium
{
    sealed class BehaviorEventListener<TBehavior> : ITransactionHandler<TBehavior>
    {
        private readonly Behavior<TBehavior> _behavior;

        public BehaviorEventListener(Behavior<TBehavior> behavior)
        {
            _behavior = behavior;
        }

        public void Run(Transaction transaction, TBehavior behavior)
        {
            if (!_behavior.ValueUpdated)
            {
                var action = new Runnable(() => _behavior.ApplyUpdate());
                transaction.Last(action);
                _behavior.ValueUpdate = behavior;
            }
        }
    }
}