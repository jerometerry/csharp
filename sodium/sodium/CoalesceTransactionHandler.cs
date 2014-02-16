namespace sodium
{
    public class CoalesceTransactionHandler<TEvent> : IHandler<Transaction>
    {
        private readonly CoalesceHandler<TEvent> _handler;
        private readonly EventSink<TEvent> _sink;

        public CoalesceTransactionHandler(CoalesceHandler<TEvent> handler, EventSink<TEvent> sink)
        {
            _handler = handler;
            _sink = sink;
        }

        public void Run(Transaction transaction)
        {
            _sink.Send(transaction, _handler.Accum);
            _handler.AccumValid = false;
            _handler.Accum = default(TEvent);
        }
    }
}
