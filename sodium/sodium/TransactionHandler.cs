namespace sodium
{
    using System;

    public class TransactionHandler<T> : ITransactionHandler<T>
    {
        private Action<Transaction, T> _handler;

        public TransactionHandler(IHandler<T> action)
            : this((t, a) => { action.Run(a); })
        {
        }

        public TransactionHandler(Action<Transaction, T> handler)
        {
            _handler = handler;
        }

        public void Run(Transaction transaction, T action)
        {
            _handler(transaction, action);
        }
    }
}
