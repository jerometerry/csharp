namespace sodium
{
    using System;

    sealed class BehaviorUnwrapper<TBehavior> : ITransactionHandler<Behavior<TBehavior>>, IDisposable
    {
        private IListener _currentListener;
        private readonly EventSink<TBehavior> _sink;

        public BehaviorUnwrapper(EventSink<TBehavior> sink)
        {
            _sink = sink;
        }

        public void Run(Transaction transaction, Behavior<TBehavior> behavior)
        {
            // Note: If any switch takes place during a transaction, then the
            // value().listen will always cause a sample to be fetched from the
            // one we just switched to. The caller will be fetching our output
            // using value().listen, and value() throws away all firings except
            // for the last one. Therefore, anything from the old input behaviour
            // that might have happened during this transaction will be suppressed.
            if (_currentListener != null)
            { 
                _currentListener.Unlisten();
            }
            var handler = new EventSinkSender<TBehavior>(_sink);
            _currentListener = behavior.GetValue(transaction).Listen(_sink.Node, transaction, handler, false);
        }

        public void Dispose()
        {
            if (_currentListener != null)
            { 
                _currentListener.Unlisten();
            }
        }
    }
}