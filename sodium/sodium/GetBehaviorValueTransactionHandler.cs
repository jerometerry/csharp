namespace sodium
{
    public class GetBehaviorValueTransactionHandler<TA> : ITransactionHandler<TA>
    {
        private readonly EventSink<TA> _o;

        public GetBehaviorValueTransactionHandler(EventSink<TA> o)
        {
            _o = o;
        }

        public void Run(Transaction trans, TA a)
        {
            _o.Send(trans, a);
        }
    }
}