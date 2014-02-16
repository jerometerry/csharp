namespace sodium
{
    public class ApplyBehaviorTransactionHandler<TBehavior, TNewBehavior> : 
        ITransactionHandler<IFunction<TBehavior, TNewBehavior>>
    {
        private readonly IHandler<Transaction> _handler;

        public ApplyBehaviorTransactionHandler(IHandler<Transaction> handler)
        {
            _handler = handler;
        }

        public void Run(Transaction transaction, IFunction<TBehavior, TNewBehavior> behaviorFunction)
        {
            _handler.Run(transaction);
        }
    }
}