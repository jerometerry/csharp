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
                var handler = this;
                var action = new Handler<Transaction>(t => 
                {
                    _sink.Send(t, this.Accumulation);
                    this.AccumulationValid = false;
                    this.Accumulation = default(TEvent);
                });
                transaction.Prioritized(_sink.Node, action);
                Accumulation = evt;
                AccumulationValid = true;
            }
        }
    }
}
