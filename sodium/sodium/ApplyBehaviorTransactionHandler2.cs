namespace sodium
{
    /// <summary>
    /// TODO - codesmell. Class name ending with a number 
    /// </summary>
    /// <typeparam name="TBehavior"></typeparam>
    /// <typeparam name="TNewBehavior"></typeparam>
    public class ApplyBehaviorTransactionHandler2<TBehavior, TNewBehavior> : 
        ITransactionHandler<TBehavior>
    {
        private readonly IHandler<Transaction> _handler;

        public ApplyBehaviorTransactionHandler2(IHandler<Transaction> h)
        {
            _handler = h;
        }

        public void Run(Transaction transaction, TBehavior behavior)
        {
            _handler.Run(transaction);
        }
    }
}