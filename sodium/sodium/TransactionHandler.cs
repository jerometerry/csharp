namespace sodium
{
    using System;

    public class TransactionHandler<TA> : ITransactionHandler<TA>
    {
        private readonly Action<Transaction, TA> _f;

        public TransactionHandler(Action<Transaction, TA> f)
        {
            _f = f;
        }

        public void Run(Transaction trans, TA a)
        {
            _f(trans, a);
        }
    }
}
