namespace sodium
{
    public sealed class BehaviorTransactionHandler<TBehavior> : ITransactionHandler<TBehavior>
    {
        private readonly Behavior<TBehavior> _behavior;

        public BehaviorTransactionHandler(Behavior<TBehavior> behavior)
        {
            _behavior = behavior;
        }

        public void Run(Transaction transaction, TBehavior behavior)
        {
            if (!_behavior.ValueUpdated)
            {
                transaction.Last(new Runnable(() =>
                {
                    _behavior.Reset();
                }));
                _behavior.ValueUpdate = behavior;
            }
        }
    }
}