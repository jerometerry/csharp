namespace sodium
{
    public class TransactionHandler<T> : ITransactionHandler<T>
    {
        private readonly IHandler<T> _handler;

        public TransactionHandler(IHandler<T> handler)
        {
            _handler = handler;
        }

        public void Run(Transaction transaction, T action)
        {
            _handler.Run(action);
        }
    }
}
