namespace sodium
{
    public class TransactionHandler<A> : ITransactionHandler<A>
    {
        private IHandler<A> action;

        public TransactionHandler(IHandler<A> action)
        {
            this.action = action;
        }

        public void run(Transaction trans, A a)
        {
            action.run(a);
        }
    }
}
