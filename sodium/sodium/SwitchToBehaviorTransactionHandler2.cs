namespace sodium
{
    public class SwitchToBehaviorTransactionHandler2<TBehavior> : ITransactionHandler<TBehavior>
    {
        private readonly EventSink<TBehavior> _sink;

        public SwitchToBehaviorTransactionHandler2(EventSink<TBehavior> sink)
        {
            _sink = sink;
        }

        public void Run(Transaction transaction, TBehavior behavior)
        {
            _sink.Send(transaction, behavior);
        }
    }
}