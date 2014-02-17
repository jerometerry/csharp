namespace sodium
{
    class MapSinkSender<TEvent, TNewEvent> : ITransactionHandler<TEvent>
    {
        private readonly EventSink<TNewEvent> _sink;
        private readonly IFunction<TEvent, TNewEvent> _mapFunction;

        public MapSinkSender(EventSink<TNewEvent> sink, IFunction<TEvent, TNewEvent> mapFunction)
        {
            _sink = sink;
            _mapFunction = mapFunction;
        }

        public void Run(Transaction transaction, TEvent evt)
        {
            _sink.Send(transaction, _mapFunction.Apply(evt));
        }
    }
}