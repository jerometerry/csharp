namespace sodium
{
    using System;

    public sealed class SwitchToEventTransactionHandler<TA> : ITransactionHandler<Event<TA>>, IDisposable
    {
        private readonly EventSink<TA> _o;
        private IListener _currentListener;
        private readonly ITransactionHandler<TA> _h2;

        public SwitchToEventTransactionHandler(EventSink<TA> o, Behavior<Event<TA>> bea, Transaction trans, ITransactionHandler<TA> h2)
        {
            _currentListener = bea.Sample().Listen(o.Node, trans, h2, false);
            _h2 = h2;
            _o = o;
        }

        public void Run(Transaction trans, Event<TA> a)
        {
            trans.Last(new Runnable(() =>
                                        {
                                            if (_currentListener != null)
                                                _currentListener.Unlisten();
                                            _currentListener = a.Listen(_o.Node, trans, _h2, true);
                                        }));
        }

        public void Dispose()
        {
            if (_currentListener != null)
                _currentListener.Unlisten();
        }
    }
}