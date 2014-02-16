namespace sodium
{
    public class SwitchToEventTransactionHandler2<TBehavior> : ITransactionHandler<TBehavior>
    {
        private readonly EventSink<TBehavior> _sink;

        public SwitchToEventTransactionHandler2(EventSink<TBehavior> sink)
        {
            _sink = sink;
        }

        public void Run(Transaction transaction, TBehavior behavior)
        {
            _sink.Send(transaction, behavior);
        }
    }
}