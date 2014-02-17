namespace sodium
{
    /// <summary>
    /// TODO - codesmell. Looks very similar to EventSinkRunner
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    class EventSinkSender<TEvent> : ITransactionHandler<TEvent>
    {
        private readonly EventSink<TEvent> _sink;

        public EventSinkSender(EventSink<TEvent> sink)
        {
            _sink = sink;
        }

        public void Run(Transaction transaction, TEvent evt)
        {
            _sink.Send(transaction, evt);
        }
    }
}