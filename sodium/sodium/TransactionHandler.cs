namespace sodium
{
    using System;

    public class TransactionHandler<A> : ITransactionHandler<A>
    {
        private Action<Transaction, A> f;

        public TransactionHandler(Action<Transaction, A> f)
        {
            this.f = f;
        }

        public void Run(Transaction trans, A a)
        {
            this.f(trans, a);
        }
    }
}
