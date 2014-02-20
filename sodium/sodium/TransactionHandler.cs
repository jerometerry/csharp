namespace sodium
{
    using System;

    public interface TransactionHandler<A> {
        void run(Transaction trans, A a);
    }

    public class TransactionHandlerInvoker<A> : TransactionHandler<A>
    {
        private Action<Transaction, A> _action;

        public TransactionHandlerInvoker(Action<Transaction, A> action)
        {
            _action = action;
        }

        public void run(Transaction trans, A a)
        {
            _action(trans, a);
        }
    }
}

