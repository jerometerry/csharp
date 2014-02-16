namespace sodium
{
    public class SnapshotSinkSender<TEvent, TBehavior, TSnapshot> : ITransactionHandler<TEvent>
    {
        private readonly EventSink<TSnapshot> _sink;
        private readonly IBinaryFunction<TEvent, TBehavior, TSnapshot> _snapshotFunction;
        private readonly Behavior<TBehavior> _behavior;

        public SnapshotSinkSender(
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
            var snapshot = _behavior.Sample();
            var newEvt = _snapshotFunction.Apply(evt, snapshot);
            _sink.Send(transaction, newEvt);
        }
    }
}
