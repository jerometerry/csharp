namespace sodium
{
    class OnceSinkSender<TEvent> : ITransactionHandler<TEvent>
    {
        private readonly EventSink<TEvent> _sink;
        private readonly IListener[] _listeners;

        public OnceSinkSender(EventSink<TEvent> sink, IListener[] listeners)
        {
            _sink = sink;
            _listeners = listeners;
        }

        public void Run(Transaction transaction, TEvent evt)
        {
            _sink.Send(transaction, evt);
            if (_listeners[0] != null)
            {
                _listeners[0].Unlisten();
                _listeners[0] = null;
            }
        }
    }
}
