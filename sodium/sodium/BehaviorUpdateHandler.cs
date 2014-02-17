namespace sodium
{
    class BehaviorUpdateHandler<TBehavior> : 
        ITransactionHandler<TBehavior>
    {
        private readonly IHandler<Transaction> _handler;

        public BehaviorUpdateHandler(IHandler<Transaction> h)
        {
            _handler = h;
        }

        public void Run(Transaction transaction, TBehavior behavior)
        {
            _handler.Run(transaction);
        }
    }
}