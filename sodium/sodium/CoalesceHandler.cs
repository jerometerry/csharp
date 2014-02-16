namespace sodium
{
    public class CoalesceHandler<TEvent> : ITransactionHandler<TEvent>
    {
        private readonly IBinaryFunction<TEvent, TEvent, TEvent> _combiningFunction;
        private readonly EventSink<TEvent> _sink;
        public bool AccumValid = false;
        public TEvent Accum;

        public CoalesceHandler(IBinaryFunction<TEvent, TEvent, TEvent> combiningFunction, EventSink<TEvent> sink)
        {
            _combiningFunction = combiningFunction;
            _sink = sink;
        }

        public void Run(Transaction transaction, TEvent evt)
        {
            if (AccumValid)
            {
                Accum = _combiningFunction.Apply(Accum, evt);
            }
            else
            {
                transaction.Prioritized(_sink.Node, new CoalesceTransactionHandler<TEvent>(this, _sink));
                Accum = evt;
                AccumValid = true;
            }
        }
    }
}
