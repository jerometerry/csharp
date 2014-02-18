namespace sodium
{
    using System;

    public class TransactionHandler<T> : ITransactionHandler<T>
    {
        private readonly Action<Transaction, T> _handler;

        public static TransactionHandler<T> Create<T>(IHandler<T> action)
        {
            return new TransactionHandler<T>((t, a) => action.Run(a));
        }

        public static TransactionHandler<T> Create<T>(IHandler<Transaction> h)
        {
            return new TransactionHandler<T>((t, a) => h.Run(t));
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
