namespace sodium
{
    public class SwitchToBehaviorTransactionHandler2<TA> : ITransactionHandler<TA>
    {
        private readonly EventSink<TA> _o;

        public SwitchToBehaviorTransactionHandler2(EventSink<TA> o)
        {
            _o = o;
        }

        public void Run(Transaction trans, TA a)
        {
            _o.Send(trans, a);
        }
    }
}