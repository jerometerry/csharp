namespace sodium
{
    public class GetBehaviorValueTransactionHandler<TBehavior> : ITransactionHandler<TBehavior>
    {
        private readonly EventSink<TBehavior> _sink;

        public GetBehaviorValueTransactionHandler(EventSink<TBehavior> sink)
        {
            _sink = sink;
        }

        public void Run(Transaction transaction, TBehavior behavior)
        {
            _sink.Send(transaction, behavior);
        }
    }
}