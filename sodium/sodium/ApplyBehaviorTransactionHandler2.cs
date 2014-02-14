namespace sodium
{
    public class ApplyBehaviorTransactionHandler2<TA, TB> : ITransactionHandler<TA>
    {
        private readonly IHandler<Transaction> _h;

        public ApplyBehaviorTransactionHandler2(IHandler<Transaction> h)
        {
            _h = h;
        }

        public void Run(Transaction trans, TA a)
        {
            _h.Run(trans);
        }
    }
}