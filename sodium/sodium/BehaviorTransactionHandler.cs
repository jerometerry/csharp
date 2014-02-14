namespace sodium
{
    public sealed class BehaviorTransactionHandler<TA> : ITransactionHandler<TA>
    {
        private readonly Behavior<TA> _b;

        public BehaviorTransactionHandler(Behavior<TA> b)
        {
            _b = b;
        }

        public void Run(Transaction trans, TA a)
        {
            if (!_b.HasValueUpdate)
            {
                trans.Last(new Runnable(() =>
                {
                    _b.Value = _b.ValueUpdate;
                    _b.ValueUpdate = default(TA);
                }));
                _b.ValueUpdate = a;
            }
        }
    }
}