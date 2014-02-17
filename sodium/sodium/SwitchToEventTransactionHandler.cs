namespace sodium
{
    using System;

    sealed class SwitchToEventTransactionHandler<TBehavior> : ITransactionHandler<Event<TBehavior>>, IDisposable
    {
        private readonly EventSink<TBehavior> _sink;
        private IListener _currentListener;
        private readonly ITransactionHandler<TBehavior> _action;

        public SwitchToEventTransactionHandler(
            EventSink<TBehavior> sink, 
            Behavior<Event<TBehavior>> eventBehavior, 
            Transaction transaction, 
            ITransactionHandler<TBehavior> action)
        {
            _currentListener = eventBehavior.Sample().Listen(sink.Node, transaction, action, false);
            _action = action;
            _sink = sink;
        }

        public void Run(Transaction transaction, Event<TBehavior> evt)
        {
            transaction.Last(new Runnable(() =>
            {
                if (_currentListener != null)
                    _currentListener.Unlisten();
                _currentListener = evt.Listen(_sink.Node, transaction, _action, true);
            }));
        }

        public void Dispose()
        {
            if (_currentListener != null)
                _currentListener.Unlisten();
        }
    }
}