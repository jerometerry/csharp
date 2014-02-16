namespace sodium
{
    public class SnapshotTransactionHandler<TEvent, TBehavior, TSnapshot> : ITransactionHandler<TEvent>
    {
        private readonly EventSink<TSnapshot> _sink;
        private readonly IBinaryFunction<TEvent, TBehavior, TSnapshot> _snapshotFunction;
        private readonly Behavior<TBehavior> _behavior;

        public SnapshotTransactionHandler(
            EventSink<TSnapshot> sink, 
            IBinaryFunction<TEvent, TBehavior, TSnapshot> snapshotFunction, 
            Behavior<TBehavior> behavior)
        {
            _sink = sink;
            _snapshotFunction = snapshotFunction;
            _behavior = behavior;
        }

        public void Run(Transaction transaction, TEvent evt)
        {
            _sink.Send(transaction, _snapshotFunction.Apply(evt, _behavior.Sample()));
        }
    }
}
