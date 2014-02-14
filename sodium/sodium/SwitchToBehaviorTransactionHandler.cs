namespace sodium
{
    using System;

    public sealed class SwitchToBehaviorTransactionHandler<TA> : ITransactionHandler<Behavior<TA>>, IDisposable
    {
        private IListener _currentListener;
        private readonly EventSink<TA> _o;

        public SwitchToBehaviorTransactionHandler(EventSink<TA> o)
        {
            _o = o;
        }

        public void Run(Transaction trans, Behavior<TA> a)
        {
            // Note: If any switch takes place during a transaction, then the
            // value().listen will always cause a sample to be fetched from the
            // one we just switched to. The caller will be fetching our output
            // using value().listen, and value() throws away all firings except
            // for the last one. Therefore, anything from the old input behaviour
            // that might have happened during this transaction will be suppressed.
            if (_currentListener != null)
                _currentListener.Unlisten();
            _currentListener = a.GetValue(trans).Listen(_o.Node, trans, new SwitchToBehaviorTransactionHandler2<TA>(_o), false);
        }

        public void Dispose()
        {
            if (_currentListener != null)
                _currentListener.Unlisten();
        }
    }
}