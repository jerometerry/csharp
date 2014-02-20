namespace sodium
{
    using System;

    public interface TransactionHandler<A> {
        void run(Transaction trans, A a);
    }

    public class TransactionHandlerImpl<A> : TransactionHandler<A>
    {
        private readonly Action<Transaction, A> _action;

        public TransactionHandlerImpl(Action<Transaction, A> action)
        {
            _action = action;
        }

        public void run(Transaction trans, A a)
        {
            _action(trans, a);
        }
    }
}

