namespace sodium
{
    class CoalesceHandler<TEvent> : ITransactionHandler<TEvent>
    {
        private readonly IBinaryFunction<TEvent, TEvent, TEvent> _combiningFunction;
        private readonly EventSink<TEvent> _sink;
        public bool AccumulationValid = false;
        public TEvent Accumulation;

        public CoalesceHandler(IBinaryFunction<TEvent, TEvent, TEvent> combiningFunction, EventSink<TEvent> sink)
        {
            _combiningFunction = combiningFunction;
            _sink = sink;
        }

        public void Run(Transaction transaction, TEvent evt)
        {
            if (AccumulationValid)
            {
                Accumulation = _combiningFunction.Apply(Accumulation, evt);
            }
            else
            {
                transaction.Prioritized(_sink.Node, new CoalesceTransactionHandler<TEvent>(this, _sink));
                Accumulation = evt;
                AccumulationValid = true;
            }
        }
    }
}
